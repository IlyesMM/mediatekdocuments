using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System;

namespace MediaTekDocuments.controller
{
    public class FrmMediatekController
    {
        private readonly Access access;

        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            return access.GetExemplairesRevue(idDocument);
        }

        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }


        public List<Commande> GetCommandes(string idDocument)
        {
            return access.GetCommandes(idDocument);
        }

        public List<CommandeDocument> GetCommandesDocument(string idDocument)
        {
            return access.GetCommandesDocument(idDocument);
        }

        public List<Abonnement> GetAbonnementRevue(string idDocument)
        {
            return access.GetAbonnementRevue(idDocument);
        }
        public List<Abonnement> GetAbonnementsEcheance()
        {
            return access.GetAbonnementsEcheance();
        }

        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            return access.GetExemplairesDocument(idDocument);
        }

        public List<Etat> GetAllEtatsDocument()
        {
            return access.GetAllEtatsDocument();
        }

        public List<Document> GetAllDocuments(string idDocument)
        {
            return access.GetAllDocuments(idDocument);
        }

        public bool CreerExemplaireRevue(string id, int numero, DateTime dateAchat, string photo, string idEtat)
        {
            return access.CreerExemplaireRevue(id, numero, dateAchat, photo, idEtat);
        }

        public bool CreerDocument(string Id, string Titre, string Image, string IdRayon, string IdPublic, string IdGenre)
        {
            return access.CreerDocument(Id, Titre, Image, IdRayon, IdPublic, IdGenre);
        }

        public bool ModifierDocument(string Id, string Titre, string Image, string IdGenre, string IdPublic, string IdRayon)
        {
            return access.ModifierDocument(Id, Titre, Image, IdGenre, IdPublic, IdRayon);
        }

        public bool SupprimerDocument(string Id)
        {
            return access.SupprimerDocument(Id);
        }

        public bool CreerLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            return access.CreerLivre(Id, Isbn, Auteur, Collection);
        }

        public bool ModifierLivre(string Id, string Isbn, string Auteur, string Collection)
        {
            return access.ModifierLivre(Id, Isbn, Auteur, Collection);
        }

        public bool SupprimerLivre(string Id)
        {
            return access.SupprimerLivre(Id);
        }

        public bool CreerDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            return access.CreerDvd(Id, Synopsis, Realisateur, Duree);
        }

        public bool ModifierDvd(string Id, string Synopsis, string Realisateur, int Duree)
        {
            return access.ModifierDvd(Id, Synopsis, Realisateur, Duree);
        }

        public bool SupprimerDvd(string Id)
        {
            return access.SupprimerDvd(Id);
        }

        public bool CreerRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            return access.CreerRevue(Id, Periodicite, DelaiMiseADispo);
        }

        public bool ModifierRevue(string Id, string Periodicite, int DelaiMiseADispo)
        {
            return access.ModifierRevue(Id, Periodicite, DelaiMiseADispo);
        }

        public bool SupprimerRevue(string Id)
        {
            return access.SupprimerRevue(Id);
        }

        public bool CreerCommande(Commande commande)
        {
            return access.CreerCommande(commande);
        }

        public bool CreerCommandeDocument(string id, int nbExemplaire, string idLivreDvd, string idSuivi)
        {
            return access.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
        }

        internal bool ModifierSuiviCommandeDocument(string id, string idSuivi)
        {
            return access.ModifierSuiviCommandeDocument(id, idSuivi);
        }

        public bool SupprimerCommandeDocument(CommandeDocument commandesDocument)
        {
            return access.SupprimerCommandeDocument(commandesDocument);
        }

        public bool CreerAbonnementRevue(string id, DateTime dateFinAbonnement, string idRevue)
        {
            return access.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
        }

        public bool SupprimerAbonnementRevue(Abonnement abonnement)
        {
            return access.SupprimerAbonnementRevue(abonnement);
        }

        public bool ModifierEtatExemplaireDocument(Exemplaire exemplaire)
        {
            return access.ModifierEtatExemplaireDocument(exemplaire);
        }

        public bool SupprimerExemplaireDocument(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaireDocument(exemplaire);
        }


    }
}
