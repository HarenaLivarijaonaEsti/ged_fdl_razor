using ged_fdl_razor.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ged_fdl_razor.Models
{
    public class DossierFinancement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime? DateSoumission { get; set; }

        public StatutDossier Statut { get; set; } = StatutDossier.Brouillon;

        // Relation Commune (Many-to-One)
        public int CommuneId { get; set; }
        public Commune? Commune { get; set; }

        public DateTime? DateValidation { get; set; }

        // Collections de navigation
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Remarque> Remarques { get; set; } = new List<Remarque>();
    }
}
