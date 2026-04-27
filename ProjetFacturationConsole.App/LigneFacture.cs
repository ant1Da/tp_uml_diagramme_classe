namespace ProjetFacturationConsole.App;

public class LigneFacture
{
    public string Description { get; set; }
    public int Quantite { get; set; }
    public decimal PrixUnitaireHT { get; set; }
    public decimal TauxTVA { get; set; }

    public LigneFacture(string description, int quantite, decimal prixUnitaireHT, decimal tauxTVA)
    {
        if (quantite <= 0) throw new Exception("quantité invalide");
        if (prixUnitaireHT <= 0) throw new Exception("prix invalide");
        Description = description; Quantite = quantite; PrixUnitaireHT = prixUnitaireHT; TauxTVA = tauxTVA;
    }
    public decimal CalculerTotalHT() => Quantite * PrixUnitaireHT;
    public decimal CalculerMontantTVA() => CalculerTotalHT() * TauxTVA / 100;
    public decimal CalculerTotalTTC() => CalculerTotalHT() + CalculerMontantTVA();
}
