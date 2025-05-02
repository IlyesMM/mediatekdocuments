namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string IdService { get; set; }

        public string Libelle { get; set; }

        public Utilisateur(string login, string password, string idService, string libelle)
        {
            this.Login = login;
            this.Password = password;
            this.IdService = idService;
            this.Libelle = libelle;
        }
    }
}