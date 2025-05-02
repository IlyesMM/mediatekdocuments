using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Serilog;
using Serilog.Formatting.Json;
using System.Diagnostics;

namespace MediaTekDocuments.dal
{
    public class Access
    {
        private static readonly string uriApi = "http://localhost/rest_mediatekdocuments/";
        private static readonly string connectionName = "MediaTekDocuments.Properties.Settings.mediatek86ConnectionString";
        private static Access instance = null;
        private readonly ApiRest api = null;
        private const string GET = "GET";
        private const string POST = "POST";
        private const string PUT = "PUT";
        private const string DELETE = "DELETE";

        static string GetConnectionStringByName(string name)
        {
            string value = null;
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[name];
            if (settings != null)
                value = settings.ConnectionString;
            return value;
        }

        private Access()
        {
            String authenticationString;
            try
            {
                authenticationString = GetConnectionStringByName(connectionName);

                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), "logs/log.txt",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {

                Console.Error.WriteLine(e);
                Log.Fatal("Access.Access catch connectionString={0} erreur={1}", connectionName, e.Message);
                Environment.Exit(0);
            }

        }

        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre");
            return new List<Categorie>(lesGenres);
        }

        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon");
            return new List<Categorie>(lesRayons);
        }

        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public");
            return new List<Categorie>(lesPublics);
        }

        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre");
            return lesLivres;
        }

        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd");
            return lesDvd;
        }

        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue");
            return lesRevues;
        }


        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + idDocument);
            return lesExemplaires;
        }

        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            List<Exemplaire> lesExemplairesDocument = TraitementRecup<Exemplaire>(GET, "exemplairesdocument/" + (string)idDocument);
            return lesExemplairesDocument;
        }

        public List<Suivi> GetAllSuivis()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi");
            return lesSuivis;
        }


        public List<Commande> GetCommandes(string idDocument)
        {
            List<Commande> lesCommandes = TraitementRecup<Commande>(GET, "commande/" + idDocument);
            return lesCommandes;
        }


        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            List<CommandeDocument> lesCommandesDocument = TraitementRecup<CommandeDocument>(GET, "commandedocument/" + idDocument);
            return lesCommandesDocument;
        }

        public List<Document> GetAllDocuments(string idDocument)
        {
            List<Document> lesDocuments = TraitementRecup<Document>(GET, "document/" + idDocument);
            return lesDocuments;
        }

        public List<Abonnement> GetAbonnementRevue(string idDocument)
        {
            List<Abonnement> lesAbonnementsRevue = TraitementRecup<Abonnement>(GET, "abonnement/" + idDocument);
            return lesAbonnementsRevue;
        }

        public List<Abonnement> GetAbonnementsEcheance()
        {
            List<Abonnement> lesAbonnementsAEcheance = TraitementRecup<Abonnement>(GET, "abonnementsecheance");
            return lesAbonnementsAEcheance;
        }

        public List<Etat> GetAllEtatsDocument()
        {
            List<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat");
            return lesEtats;
        }

        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire/" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerExemplaire catch jsonExemplaire={0} erreur={1} ", jsonExemplaire, ex.Message);
            }
            return false;
        }

        public bool CreerDocument(string Id, string Titre, string Image, string IdRayon, string IdPublic, string IdGenre)
        {
            String jsonCreerDocument = "{ \"id\" : \"" + Id + "\", \"titre\" : \"" + Titre + "\", \"image\" : \"" + Image + "\", \"idRayon\" : \"" + IdRayon + "\", \"idPublic\" : \"" + IdPublic + "\", \"idGenre\" : \"" + IdGenre + "\"}";
            Console.WriteLine("jsonCreerDocument" + jsonCreerDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(POST, "document/" + jsonCreerDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerDocument catch jsonCreerDocument={0} erreur={1} ", jsonCreerDocument, ex.Message);
            }
            return false;
        }

        public bool ModifierDocument(string Id, string Titre, string Image, string IdGenre, string IdPublic, string IdRayon)
        {
            String jsonModifierDocument = "{ \"id\" : \"" + Id + "\", \"titre\" : \"" + Titre + "\", \"image\" : \"" + Image + "\", \"idGenre\" : \"" + IdGenre + "\", \"idPublic\" : \"" + IdPublic + "\", \"idRayon\" : \"" + IdRayon + "\"}";
            Console.WriteLine("jsonModifierDocument" + jsonModifierDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(PUT, "document/" + Id + "/" + jsonModifierDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierDocument catch jsonModifierDocument={0} erreur={1} ", jsonModifierDocument, ex.Message);
            }
            return false;
        }

        public bool SupprimerDocument(string Id)
        {
            String jsonIdDocument = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdDocument" + jsonIdDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(DELETE, "document/" + jsonIdDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerDocument catch jsonIdDocument={0} erreur={1} ", jsonIdDocument, ex.Message);
            }
            return false;
        }

        public bool CreerLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            String jsonCreerLivre = "{ \"id\" : \"" + Id + "\", \"isbn\" : \"" + Isbn + "\", \"auteur\" : \"" + Auteur + "\", \"collection\" : \"" + Collection + "\"}";
            Console.WriteLine("jsonCreerLivre" + jsonCreerLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Document> liste = TraitementRecup<Document>(POST, "livre/" + jsonCreerLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerLivre catch jsonCreerLivre={0} erreur={1} ", jsonCreerLivre, ex.Message);
            }
            return false;
        }


        public bool ModifierLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            String jsonModifierLivre = "{ \"id\" : \"" + Id + "\", \"isbn\" : \"" + Isbn + "\", \"auteur\" : \"" + Auteur + "\", \"collection\" : \"" + Collection + "\"}";
            Console.WriteLine("jsonModifierLivre" + jsonModifierLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Livre> liste = TraitementRecup<Livre>(PUT, "livre/" + Id + "/" + jsonModifierLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierLivre catch jsonModifierLivre={0} erreur={1} ", jsonModifierLivre, ex.Message);
            }
            return false;
        }


        public bool SupprimerLivre(string Id)
        {
            String jsonIdLivre = "{ \"id\" : \"" + Id + "\"}";
            Log.Error("jsonIdLivre" + jsonIdLivre);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Livre> liste = TraitementRecup<Livre>(DELETE, "livre/" + jsonIdLivre);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerLivre catch jsonIdLivre={0} erreur={1} ", jsonIdLivre, ex.Message);
            }
            return false;
        }

        public bool CreerDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            String jsonCreerDvd = "{ \"id\" : \"" + Id + "\", \"synopsis\" : \"" + Synopsis + "\", \"realisateur\" : \"" + Realisateur + "\", \"duree\" : \"" + Duree + "\"}";
            Console.WriteLine("jsonCreerDvd" + jsonCreerDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(POST, "dvd/" + jsonCreerDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerDvd catch jsonCreerDvd={0} erreur={1} ", jsonCreerDvd, ex.Message);
            }
            return false;
        }

        public bool ModifierDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            String jsonModifierDvd = "{ \"id\" : \"" + Id + "\", \"synopsis\" : \"" + Synopsis + "\", \"realisateur\" : \"" + Realisateur + "\", \"duree\" : \"" + Duree + "\"}";
            Console.WriteLine("jsonModifierDvd" + jsonModifierDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(PUT, "dvd/" + Id + "/" + jsonModifierDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierDvd catch jsonModifierDvd={0} erreur={1} ", jsonModifierDvd, ex.Message);
            }
            return false;
        }

        public bool SupprimerDvd(string Id)
        {
            String jsonIdDvd = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdDvd" + jsonIdDvd);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Dvd> liste = TraitementRecup<Dvd>(DELETE, "dvd/" + jsonIdDvd);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerDvd catch jsonIdDvd={0} erreur={1} ", jsonIdDvd, ex.Message);
            }
            return false;
        }

        public bool CreerRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            String jsonCreerRevue = "{ \"id\" : \"" + Id + "\", \"periodicite\" : \"" + Periodicite + "\", \"delaiMiseADispo\" : \"" + DelaiMiseADispo + "\"}";
            Console.WriteLine("jsonCreerRevue" + jsonCreerRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(POST, "revue/" + jsonCreerRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerRevue catch jsonCreerRevue={0} erreur={1} ", jsonCreerRevue, ex.Message);
            }
            return false;
        }

        public bool ModifierRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            String jsonModifierRevue = "{ \"id\" : \"" + Id + "\", \"periodicite\" : \"" + Periodicite + "\", \"delaiMiseADispo\" : \"" + DelaiMiseADispo + "\"}";
            Console.WriteLine("jsonModifierRevue" + jsonModifierRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(PUT, "revue/" + Id + "/" + jsonModifierRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierRevue catch jsonModifierRevue={0} erreur={1} ", jsonModifierRevue, ex.Message);
            }
            return false;
        }

        public bool SupprimerRevue(string Id)
        {
            String jsonIdRevue = "{ \"id\" : \"" + Id + "\"}";
            Console.WriteLine("jsonIdRevue" + jsonIdRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Revue> liste = TraitementRecup<Revue>(DELETE, "revue/" + jsonIdRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerRevue catch jsonIdRevue={0} erreur={1} ", jsonIdRevue, ex.Message);
            }
            return false;
        }

        public bool CreerCommande(Commande commande)
        {
            String jsonCreerCommande = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());
            Console.WriteLine("jsonCreerCommande " + jsonCreerCommande);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Commande> liste = TraitementRecup<Commande>(POST, "commande/" + jsonCreerCommande);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerCommande catch jsonCreerCommande={0} erreur={1} ", jsonCreerCommande, ex.Message);
            }
            return false;
        }

        public bool CreerCommandeDocument(string id, int nbExemplaire, string idLivreDvd, string idSuivi)
        {
            String jsonCreerCommandeDocument = "{ \"id\" : \"" + id + "\", \"nbExemplaire\" : \"" + nbExemplaire + "\", \"idLivreDvd\" : \"" + idLivreDvd + "\", \"idSuivi\" : \"" + idSuivi + "\"}";
            Console.WriteLine("jsonCreerCommandeDocument" + jsonCreerCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(POST, "commandedocument/" + jsonCreerCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerCommandeDocument catch jsonCreerCommandeDocument={0} erreur={1} ", jsonCreerCommandeDocument, ex.Message);
            }
            return false;
        }

        public bool ModifierSuiviCommandeDocument(string id, string idSuivi)
        {
            String jsonModifierSuiviCommandeDocument = "{ \"id\" : \"" + id + "\", \"idSuivi\" : \"" + idSuivi + "\"}";
            Console.WriteLine("jsonModifierSuiviCommandeDocument" + jsonModifierSuiviCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(PUT, "commandedocument/" + id + "/" + jsonModifierSuiviCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierSuiviCommandeDocument catch jsonModifierSuiviCommandeDocument={0} erreur={1} ", jsonModifierSuiviCommandeDocument, ex.Message);

            }
            return false;
        }

        public bool SupprimerCommandeDocument(CommandeDocument commandesDocument)
        {
            String jsonSupprimerCommandeDocument = "{\"id\":\"" + commandesDocument.Id + "\"}";
            Console.WriteLine("jsonSupprimerCommandeDocument=" + jsonSupprimerCommandeDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<CommandeDocument> liste = TraitementRecup<CommandeDocument>(DELETE, "commandedocument/" + jsonSupprimerCommandeDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerCommandeDocument catch jsonSupprimerCommandeDocument={0} erreur={1} ", jsonSupprimerCommandeDocument, ex.Message);
            }
            return false;
        }

        public bool CreerAbonnementRevue(string id, DateTime dateFinAbonnement, string idRevue)
        {
            String jsonDateCommande = JsonConvert.SerializeObject(dateFinAbonnement, new CustomDateTimeConverter());
            String jsonCreerAbonnementRevue = "{\"id\":\"" + id + "\", \"dateFinAbonnement\" : " + jsonDateCommande + ", \"idRevue\" :  \"" + idRevue + "\"}";
            Console.WriteLine("jsonCreerAbonnementRevue" + jsonCreerAbonnementRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "abonnement/" + jsonCreerAbonnementRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerAbonnementRevue catch jsonCreerAbonnementRevue={0} erreur={1} ", jsonCreerAbonnementRevue, ex.Message);
            }
            return false;
        }

        public bool SupprimerAbonnementRevue(Abonnement abonnement)
        {
            String jsonSupprimerAbonnementRevue = "{\"id\":\"" + abonnement.Id + "\"}";
            Console.WriteLine("jsonSupprimerAbonnementRevue=" + jsonSupprimerAbonnementRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Abonnement> liste = TraitementRecup<Abonnement>(DELETE, "abonnement/" + jsonSupprimerAbonnementRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerAbonnementRevue catch jsonSupprimerAbonnementRevue={0} erreur={1} ", jsonSupprimerAbonnementRevue, ex.Message);
            }
            return false;
        }

        public bool ModifierEtatExemplaireDocument(Exemplaire exemplaire)
        {
            String jsonModifierEtatExemplaireDocument = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            Console.WriteLine("jsonModifierEtatExemplaireDocument" + jsonModifierEtatExemplaireDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(PUT, "exemplaire/" + exemplaire.Numero + "/" + jsonModifierEtatExemplaireDocument); // Modification de la requête


                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.ModifierEtatExemplaireDocument catch jsonModifierEtatExemplaireDocument={0} erreur={1} ", jsonModifierEtatExemplaireDocument, ex.Message);
            }
            return false;
        }

        public bool SupprimerExemplaireDocument(Exemplaire exemplaire)
        {
            String jsonSupprimerExemplaireDocument = "{\"id\":\"" + exemplaire.Id + "\",\"numero\":\"" + exemplaire.Numero + "\"}";
            Console.WriteLine("jsonSupprimerExemplaireDocument" + jsonSupprimerExemplaireDocument);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(DELETE, "exemplaire/" + jsonSupprimerExemplaireDocument);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.SupprimerExemplaireDocument catch jsonSupprimerExemplaireDocument={0} erreur={1} ", jsonSupprimerExemplaireDocument, ex.Message);
            }
            return false;
        }

        public bool CreerExemplaireRevue(string id, int numero, DateTime dateAchat, string photo, string idEtat)
        {
            String jsonDateAchat = JsonConvert.SerializeObject(dateAchat, new CustomDateTimeConverter());
            String jsonCreerExemplaireRevue = "{\"id\":\"" + id + "\", \"numero\":\"" + numero + "\", \"dateAchat\" : " + jsonDateAchat + ", \"photo\" :  \"" + photo + "\" , \"idEtat\" :  \"" + idEtat + "\"}";
            Console.WriteLine("jsonCreerExemplaireRevue" + jsonCreerExemplaireRevue);
            try
            {
                // récupération soit d'une liste vide (requête ok) soit de null (erreur)
                List<Abonnement> liste = TraitementRecup<Abonnement>(POST, "exemplaire/" + jsonCreerExemplaireRevue);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.CreerExemplaireRevue catch jsonCreerExemplaireRevue={0} erreur={1} ", jsonCreerExemplaireRevue, ex.Message);
            }
            return false;
        }

        public Utilisateur GetUtilisateur(string login, string password)
        {
            try
            {

                List<Utilisateur> liste = TraitementRecup<Utilisateur>(GET, "utilisateur/" + login);
                Console.WriteLine(login + " " + password);
                if (liste == null || liste.Count == 0)
                {
                    return null;
                }
                Utilisateur utilisateur = liste[0];
                if (utilisateur.Password != password)
                {
                    return null;
                }
                return utilisateur;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Error("Access.GetUtilisateur catch login={0} erreur={1}", login, ex.Message);
            }
            return null;
        }

        private List<T> TraitementRecup<T>(String methode, String message)
        {

            Console.WriteLine("message:");
            Console.WriteLine(message);


            List<T> liste = new List<T>();
            try
            {
                Console.WriteLine(message);
                JObject retour = api.RecupDistant(methode, message);
                // extraction du code retourné
                String code = (String)retour["code"];


                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + ", message = " + (String)retour["message"]);
                    Log.Error("Access.TraitementRecup catch code={0} erreur={1} ", code);
                }
            }
            catch (Exception e)
            {
                Log.Error("Access.TraitementRecup catch liste={0} erreur={1}", liste, e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
