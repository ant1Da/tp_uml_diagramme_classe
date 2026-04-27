using ProjetFacturationConsole.App;

namespace ProjetFacturationConsole.Tests;

[TestClass]
public class FacturationTests
{
    [TestMethod]
    public void Test_CalculerTotalHT()
    {
        var l = new LigneFacture("test", 2, 100, 20);
        Assert.AreEqual(200, l.CalculerTotalHT());
    }

    [TestMethod]
    public void Test_CalculerTotalTTC()
    {
        var l = new LigneFacture("test", 2, 100, 20);
        Assert.AreEqual(240, l.CalculerTotalTTC());
    }

    [TestMethod]
    public void Test_TotalTTC_Facture()
    {
        var c = new Client(1,"A","a@a.fr","0","adr","ville","00000",DateTime.Today);
        var e = new Entreprise(1,"E","e@e.fr","0","adr","ville","00000","123");
        var f = new Facture("F1", DateTime.Today, c, e);
        f.AjouterLigne(new LigneFacture("a",1,100,20));
        f.AjouterLigne(new LigneFacture("b",1,50,20));
        Assert.AreEqual(180, f.CalculerTotalTTC());
    }
}
