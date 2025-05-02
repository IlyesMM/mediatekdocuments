
namespace MediaTekDocuments.model
{
    public class Categorie
    {
        public string Id { get; set; }

        public string Libelle { get; set; }

        public Categorie(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        public override string ToString()
        {
            return this.Libelle;
        }

    }
}
