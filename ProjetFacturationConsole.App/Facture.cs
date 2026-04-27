using System.Text;
namespace ProjetFacturationConsole.App;

public class Facture : DocumentCommercial
{
    public DateTime DateEcheance { get; set; }
    public string Statut { get; set; }

    public Facture(string numero, DateTime dateEmission, Client client, Entreprise entreprise, string statut="Brouillon")
        : base(numero, dateEmission, client, entreprise)
    {
        DateEcheance = dateEmission.AddDays(30);
        Statut = statut;
    }

    public string BuildTexte()
    {
        if (Lignes.Count < 2) throw new Exception("Une facture doit contenir au minimum deux lignes.");
        var sb = new StringBuilder();
        sb.AppendLine("FACTURE");
        sb.AppendLine($"Numéro : {Numero}");
        sb.AppendLine($"Date d'émission : {DateEmission:dd/MM/yyyy}");
        sb.AppendLine($"Date d'échéance : {DateEcheance:dd/MM/yyyy}");
        sb.AppendLine($"Statut : {Statut}");
        sb.AppendLine("Entreprise :");
        sb.AppendLine($"{Entreprise.Id} - {Entreprise.Nom} - {Entreprise.Email} - {Entreprise.Telephone} - {Entreprise.Adresse} - {Entreprise.Ville} - {Entreprise.CodePostal} - {Entreprise.Siret}");
        sb.AppendLine("Client :");
        sb.AppendLine($"{Client.Id} - {Client.Nom} - {Client.Email} - {Client.Telephone} - {Client.Adresse} - {Client.Ville} - {Client.CodePostal} - {Client.DateInscription:dd/MM/yyyy}");
        sb.AppendLine("Lignes :");
        for (int i = 0; i < Lignes.Count; i++)
        {
            var l = Lignes[i];
            sb.AppendLine($"{i + 1}. {l.Description} - Qté : {l.Quantite} - PU HT : {l.PrixUnitaireHT} - TVA : {l.TauxTVA} - Total HT : {l.CalculerTotalHT()} - Total TTC : {l.CalculerTotalTTC()}");
        }
        sb.AppendLine($"Total HT : {CalculerTotalHT()}");
        sb.AppendLine($"Total TVA : {CalculerTotalTVA()}");
        sb.AppendLine($"Total TTC : {CalculerTotalTTC()}");
        return sb.ToString();
    }

    public override void AfficherFacture() => Console.WriteLine(BuildTexte());
}
