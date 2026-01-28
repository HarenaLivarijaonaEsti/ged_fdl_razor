using ged_fdl_razor.Enums;
using System.ComponentModel.DataAnnotations;

namespace ged_fdl_razor.Models
{
    public class Document
    {
        public int DocumentID { get; set; }

        [Required]
        [StringLength(200)]
        public string Nom { get; set; } = string.Empty;

        public TypeDocument Type { get; set; }

        [Required]
        public string ContentType { get; set; } = "application/pdf";

        public long Taille { get; set; }

        [Required]
        public byte[] Contenu { get; set; } = Array.Empty<byte>();

        public int DossierFinancementId { get; set; }
        public DossierFinancement? DossierFinancement { get; set; }

        public DateTime DateUpload { get; set; } = DateTime.UtcNow;
    }
}
