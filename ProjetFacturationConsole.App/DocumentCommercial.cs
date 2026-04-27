using System.Text;
namespace ProjetFacturationConsole.App;

public abstract class DocumentCommercial
{
    public string Numero { get; set; }
    public DateTime DateEmission { get; set; }
    public Client Client { get; set; }
    public Entreprise Entreprise { get; set; }
    public List<LigneFacture> Lignes { get; set; } = new();

    protected DocumentCommercial(string numero, DateTime dateEmission, Client client, Entreprise entreprise)
    { Numero = numero; DateEmission = dateEmission; Client = client; Entreprise = entreprise; }

    public void AjouterLigne(LigneFacture ligne) => Lignes.Add(ligne);
    public decimal CalculerTotalHT() => Lignes.Sum(l => l.CalculerTotalHT());
    public decimal CalculerTotalTVA() => Lignes.Sum(l => l.CalculerMontantTVA());
    public decimal CalculerTotalTTC() => Lignes.Sum(l => l.CalculerTotalTTC());

    public abstract void AfficherFacture();
}
