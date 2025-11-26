using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using UneContApp.Data;
using UneContApp.Entidades;

namespace UneContApp.Service
{
    public class NotaFiscalService
    {
        private readonly NotaFiscalDbContext _context;

        public NotaFiscalService(NotaFiscalDbContext context)
        {
            _context = context;
        }

        public async Task<(bool sucesso, string texto, NotaFiscal? NotaFiscal)> ProcessarXML(string dadoXML)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(dadoXML);
                XElement? notaElement = xmlDoc.Element("NotaFiscal");

                if (notaElement == null)
                    return (false, "XML não contém o elemento raiz NotaFiscal", null);


                var notaFiscal = ExtrairDadosXML(notaElement);

                var (valido, erros) = ValidarNotaFiscal(notaFiscal);

                if (!valido)
                {
                    return (false, $"Erros de validação: {string.Join(", ", erros)}", null);
                }

                var notaExistente = await _context.NotaFiscais.FirstOrDefaultAsync(n => n.Numero == notaFiscal.Numero && n.CNPJPrestador == notaFiscal.CNPJPrestador && n.DataEmissao == notaFiscal.DataEmissao);

                if (notaExistente != null)
                {
                    return (false, "Nota fiscal já cadastrada no sistema", notaExistente);
                }

                _context.NotaFiscais.Add(notaFiscal);
                await _context.SaveChangesAsync();

                return (true, "Nota fiscal processada com sucesso", notaFiscal);
            }
            catch (Exception ex)
            {
                return (false, $"Erro ao processar arquivo: {ex.Message}", null);
            }
        }
        

        private NotaFiscal ExtrairDadosXML(XElement notaElement)
        {
            var nota = new NotaFiscal
            {
                Numero = notaElement.Element("Numero")?.Value ?? string.Empty,
                CNPJPrestador = notaElement.Element("Prestador")?.Element("CNPJ")?.Value ?? string.Empty,
                CNPJTomador = notaElement.Element("Tomador")?.Element("CNPJ")?.Value ?? string.Empty,
                DescricaoServico = notaElement.Element("Servico")?.Element("Descricao")?.Value ?? string.Empty,
            };

            if (DateTime.TryParse(notaElement.Element("DataEmissao")?.Value, out DateTime dataEmissao))
            {
                nota.DataEmissao = dataEmissao;
            }

            if (decimal.TryParse(notaElement.Element("Servico")?.Element("Valor")?.Value,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal valor))
            {
                nota.ValorTotal = valor;
            }

            return nota;
        }
        private (bool Valido, List<string> Erros) ValidarNotaFiscal(NotaFiscal nota)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(nota.Numero))
                erros.Add("Número da nota é obrigatório");

            if (string.IsNullOrWhiteSpace(nota.CNPJPrestador))
                erros.Add("CNPJ do prestador é obrigatório");

            if (string.IsNullOrWhiteSpace(nota.CNPJTomador))
                erros.Add("CNPJ do tomador é obrigatório");

            if (string.IsNullOrWhiteSpace(nota.DescricaoServico))
                erros.Add("Descrição do serviço é obrigatória");

            if (!string.IsNullOrWhiteSpace(nota.CNPJPrestador))
            {
                string cnpjLimpo = new string(nota.CNPJPrestador.Where(char.IsDigit).ToArray());

                if (!ValidaCNPJ.ValicaoCNPJ(cnpjLimpo))
                    erros.Add("CNPJ do prestador é inválido");
            }

            if (!string.IsNullOrWhiteSpace(nota.CNPJTomador))
            {
                string cnpjLimpo = new string(nota.CNPJTomador.Where(char.IsDigit).ToArray());

                if (!ValidaCNPJ.ValicaoCNPJ(cnpjLimpo))
                    erros.Add("CNPJ do tomador é inválido");
            }

            if (nota.ValorTotal <= 0)
                erros.Add("Valor total deve ser maior que zero");

            if (nota.DataEmissao == DateTime.MinValue)
                erros.Add("Data de emissão é inválida ");

            if (nota.DataEmissao > DateTime.Now)
                erros.Add("Data de emissão não pode ser futura");

            return (erros.Count == 0, erros);
        }
        public async Task<List<NotaFiscal>> ObterTodasNotas()
        {
            return await _context.NotaFiscais.OrderBy(n => n.DataCadastro).ToListAsync();
        }
    }
}