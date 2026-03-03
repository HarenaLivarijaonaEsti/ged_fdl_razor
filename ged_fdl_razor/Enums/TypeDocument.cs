using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ged_fdl_razor.Enums
{
    public enum TypeDocument
    {
        // =========================
        // DOSSIER ADMINISTRATIF
        // =========================

        [Display(Name = "Compte administratif de l’année N-1 (avis favorable du Chef District)")]
        CompteAdministratifN1,

        [Display(Name = "Budget Primitif / Budget Programme de l’année N (extrait de délibération du Conseil Communal)")]
        BudgetPrimitifOuProgramme,

        [Display(Name = "Arrêté de nomination du Trésorier Communal (Commune rurale de 2ᵉ catégorie)")]
        ArreteNominationTresorier,

        // =========================
        // DOSSIER TECHNIQUE
        // =========================

        [Display(Name = "Fiche d’Identification du Projet (FIP) dûment remplie")]
        FicheIdentificationProjet,

        [Display(Name = "Croquis / Plan de l’infrastructure")]
        CroquisPlanInfrastructure,

        [Display(Name = "Devis quantitatif du coût de l’infrastructure")]
        DevisQuantitatif,

        // =========================
        // DOSSIER COMPLÉMENTAIRE
        // =========================

        [Display(Name = "Extrait du PDL II")]
        ExtraitPdlII,

        [Display(Name = "Délibération du Conseil Communal")]
        DeliberationConseilCommunal,

        [Display(Name = "Situation foncière")]
        SituationFonciere,

        [Display(Name = "Autorisation STD")]
        AutorisationStd,

        [Display(Name = "Rapport final du projet antérieur financé par le FDL")]
        RapportFinalProjetAnterieur
    }
    // ✅ Méthode d'extension pour l'affichage DisplayName
    public static class TypeDocumentExtensions
    {
        public static string GetDisplayName(this TypeDocument doc)
        {
            return doc
                .GetType()
                .GetMember(doc.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName()
                ?? doc.ToString();
        }
    }
}
