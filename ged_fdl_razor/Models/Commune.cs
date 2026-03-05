using System.ComponentModel.DataAnnotations;

namespace ged_fdl_razor.Models
{
    public class Commune
    {
        public int CommuneID { get; set; }

        [Required]
        [StringLength(150)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool MustChangePassword { get; set; } = true;

        [Required]
        public string District { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Commune"; // "Admin" pour l'administrateur

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // -------------------------------
        // Nouveau flag pour notification
        // -------------------------------
        public bool HasNewDocuments { get; set; } = false;

        public ICollection<DossierFinancement> Dossiers { get; set; }
            = new List<DossierFinancement>();
    }
}