using System.Text.Json;
namespace ProjetFacturationConsole.App;

public class GestionFacturation
{
    private List<Client> clients = new();
    private List<Entreprise> entreprises = new();
    private Dictionary<int, Client> dictionnaireClients = new();
    private Dictionary<int, Entreprise> dictionnaireEntreprises = new();

    public void ImporterClientsDepuisCsv(string path="clients.csv")
    {
        if (!File.Exists(path)) { Console.WriteLine("Fichier introuvable."); return; }
        foreach (var l in File.ReadAllLines(path).Skip(1))
        {
            try
            {
                var c = l.Split(';');
                if (!int.TryParse(c[0], out int id)) throw new Exception("id invalide");
                if (!DateTime.TryParse(c[7], out DateTime date)) throw new Exception("date invalide");
                var client = new Client(id, c[1], c[2], c[3], c[4], c[5], c[6], date);
                clients.Add(client); dictionnaireClients[client.Id] = client;
            }
            catch (Exception ex) { Console.WriteLine($"Erreur ligne client : {ex.Message}"); }
        }
        File.WriteAllText("clients.json", JsonSerializer.Serialize(clients));
        Console.WriteLine("Import clients terminé.");
    }

    public void ImporterEntreprisesDepuisCsv(string path="entreprises.csv")
    {
        if (!File.Exists(path)) { Console.WriteLine("Fichier introuvable."); return; }
        foreach (var l in File.ReadAllLines(path).Skip(1))
        {
            try
            {
                var c = l.Split(';');
                if (!int.TryParse(c[0], out int id)) throw new Exception("id invalide");
                var e = new Entreprise(id, c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
                entreprises.Add(e); dictionnaireEntreprises[e.Id] = e;
            }
            catch (Exception ex) { Console.WriteLine($"Erreur ligne entreprise : {ex.Message}"); }
        }
        File.WriteAllText("entreprises.json", JsonSerializer.Serialize(entreprises));
        Console.WriteLine("Import entreprises terminé.");
    }

    public void ChargerClientsDepuisJson() {
        if (!File.Exists("clients.json")) return;
        clients = JsonSerializer.Deserialize<List<Client>>(File.ReadAllText("clients.json")) ?? new();
        dictionnaireClients = clients.ToDictionary(c => c.Id, c => c);
    }

    public void ChargerEntreprisesDepuisJson() {
        if (!File.Exists("entreprises.json")) return;
        entreprises = JsonSerializer.Deserialize<List<Entreprise>>(File.ReadAllText("entreprises.json")) ?? new();
        dictionnaireEntreprises = entreprises.ToDictionary(e => e.Id, e => e);
    }

    public void AfficherClients() { if (clients.Count == 0) ChargerClientsDepuisJson(); clients.ForEach(c => Console.WriteLine($"{c.Id} - {c.Nom}")); }
    public void AfficherEntreprises() { if (entreprises.Count == 0) ChargerEntreprisesDepuisJson(); entreprises.ForEach(e => Console.WriteLine($"{e.Id} - {e.Nom}")); }

    public void AfficherCarnetContacts()
    {
        if (clients.Count == 0) ChargerClientsDepuisJson();
        if (entreprises.Count == 0) ChargerEntreprisesDepuisJson();
        List<Personne> carnet = new(); carnet.AddRange(clients); carnet.AddRange(entreprises);
        carnet.ForEach(p => p.AfficherInfos());
    }

    public void CreerFacture()
    {
        if (clients.Count == 0) ChargerClientsDepuisJson();
        if (entreprises.Count == 0) ChargerEntreprisesDepuisJson();

        AfficherEntreprises();
        Console.Write("Identifiant de l'entreprise : ");
        if (!int.TryParse(Console.ReadLine(), out int idEnt) || !dictionnaireEntreprises.ContainsKey(idEnt))
        { Console.WriteLine("Entreprise introuvable."); return; }
        var entreprise = dictionnaireEntreprises[idEnt];

        AfficherClients();
        Console.Write("Identifiant du client : ");
        if (!int.TryParse(Console.ReadLine(), out int idCli) || !dictionnaireClients.ContainsKey(idCli))
        { Console.WriteLine("Client introuvable."); return; }
        var client = dictionnaireClients[idCli];

        Console.Write("Date d'émission (dd/MM/yyyy) : ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateEmission))
        { Console.WriteLine("Date invalide."); return; }

        Console.Write("Numéro de facture : ");
        var numero = Console.ReadLine() ?? "F-001";

        var facture = new Facture(numero, dateEmission, client, entreprise);

        string reponse = "oui";
        do
        {
            Console.Write("Description : "); var desc = Console.ReadLine() ?? "";
            Console.Write("Quantité : ");
            if (!int.TryParse(Console.ReadLine(), out int qte)) { Console.WriteLine("Quantité invalide."); continue; }
            Console.Write("Prix unitaire HT : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal pu)) { Console.WriteLine("Prix invalide."); continue; }
            Console.Write("Taux TVA (%) : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal tva)) { Console.WriteLine("Taux invalide."); continue; }
            try { facture.AjouterLigne(new LigneFacture(desc, qte, pu, tva)); }
            catch (Exception ex) { Console.WriteLine($"Erreur : {ex.Message}"); }
            Console.Write("Voulez-vous ajouter une autre ligne ? (oui/non) : ");
            reponse = Console.ReadLine() ?? "non";
        } while (reponse.Trim().ToLower() == "oui");

        try
        {
            facture.AfficherFacture();
            Console.Write("Confirmer la génération du fichier texte ? (oui/non) : ");
            if ((Console.ReadLine() ?? "").Trim().ToLower() == "oui")
                GenererFichierTexteFacture(facture);
        }
        catch (Exception ex) { Console.WriteLine($"Erreur : {ex.Message}"); }
    }

    public void GenererFichierTexteFacture(Facture facture)
    {
        string nomFichier = $"facture_{facture.Numero}.txt";
        File.WriteAllText(nomFichier, facture.BuildTexte());
        Console.WriteLine($"Fichier généré : {nomFichier}");
    }

    public void AfficherMenu()
    {
        int choix;
        do
        {
            Console.WriteLine("1 - Importer les clients depuis le CSV");
            Console.WriteLine("2 - Importer les entreprises depuis le CSV");
            Console.WriteLine("3 - Afficher les clients");
            Console.WriteLine("4 - Afficher les entreprises");
            Console.WriteLine("5 - Créer une facture");
            Console.WriteLine("6 - Afficher le carnet de contacts");
            Console.WriteLine("0 - Quitter");
            int.TryParse(Console.ReadLine(), out choix);
            switch(choix)
            {
                case 1: ImporterClientsDepuisCsv(); break;
                case 2: ImporterEntreprisesDepuisCsv(); break;
                case 3: AfficherClients(); break;
                case 4: AfficherEntreprises(); break;
                case 5: CreerFacture(); break;
                case 6: AfficherCarnetContacts(); break;
            }
        } while (choix != 0);
    }
}
