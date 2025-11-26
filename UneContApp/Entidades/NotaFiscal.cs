using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UneContApp.Entidades
{
    public class NotaFiscal
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O número da nota é obrigatório")]
        public string Numero { get; set; } 

        [Required(ErrorMessage = "O CNPJ do prestador é obrigatório")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ do prestador deve conter 14 dígitos")]
        public string CNPJPrestador { get; set; } 

        [Required(ErrorMessage = "O CNPJ do tomador é obrigatório")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ do tomador deve conter 14 dígitos")]
        public string CNPJTomador { get; set; } 

        [Required(ErrorMessage = "A data de emissão é obrigatória")]
        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "A descrição do serviço é obrigatória")]
        [StringLength(500)]
        public string DescricaoServico { get; set; }

        [Required(ErrorMessage = "O valor total é obrigatório")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero")]
        public decimal ValorTotal { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
