using System.ComponentModel.DataAnnotations;

namespace ged_fdl_razor.Models
{
    public class Remarque
    {
        public int RemarqueID { get; set; }

        [Required]
        [StringLength(500)]
        public string Texte { get; set; } = string.Empty;

        // Relation DossierFinancement (Many-to-One)
        public int DossierFinancementId { get; set; }
        public DossierFinancement? DossierFinancement { get; set; }

        // Optionnel : qui a écrit la remarque
        public int? CommuneId { get; set; }
        public Commune? Commune { get; set; }

        // Audit
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
