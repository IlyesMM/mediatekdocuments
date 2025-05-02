
namespace MediaTekDocuments.model
{
    public class Etat
    {
        public string Id { get; set; }

        public string Libelle { get; set; }

        public Etat(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

    }
}
