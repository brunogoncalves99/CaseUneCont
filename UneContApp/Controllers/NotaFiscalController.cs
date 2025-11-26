using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UneContApp.Entidades;
using UneContApp.Service;

namespace UneContApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotaFiscalController : Controller
    {
        private readonly NotaFiscalService _notaFiscalService;

        public NotaFiscalController(NotaFiscalService notaFiscalService)
        {
            _notaFiscalService = notaFiscalService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotaFiscal>>> GetTotalNotas()
        {
            try
            {
                var Notasfiscais = await _notaFiscalService.ObterTodasNotas();
                return Ok(Notasfiscais);
            }
            catch (Exception ex)
            {
                return BadRequest( "Nenhuma nota fiscal encontrado" + ex.Message);
            }
        }

        [HttpPost("ProcessarNotaFiscal")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProcessaXML(IFormFile formFile)
        {
            if(formFile == null)
            {
                return BadRequest("Nenhum arquivo foi inserido");
            }
            if(!formFile.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("O arquivo inserido deve ser do formato XML");
            }

            try
            {
                var caminho = Path.Combine(Path.GetTempPath(), formFile.FileName);
                using (var reader = new FileStream(caminho, FileMode.Create))
                {
                    await formFile.CopyToAsync(reader);
                }

                var (sucesso, mensagem, nota) = await _notaFiscalService.ProcessarXML(caminho);

                if (System.IO.File.Exists(caminho))
                {
                    System.IO.File.Delete(caminho);
                }

                if (sucesso)
                {
                    return Ok(new { mensagem, nota});
                }
                else
                {
                    return BadRequest(new { mensagem });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao processar o arquivo selecionado:", ex.Message });
            }
        }
    }
}
