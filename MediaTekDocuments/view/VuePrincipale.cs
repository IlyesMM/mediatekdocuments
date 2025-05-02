using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    public partial class VuePrincipale : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        private readonly Service service = null;

        public string INDEXLIVRE;
        public string INDEXDVD;
        public string INDEXREVUES;

        private void fermetureForcee(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        public VuePrincipale(Service service)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.service = service;
            this.FormClosing += fermetureForcee;
        }

        private void FrmAlerteFinAbonnement_Shown(object sender, EventArgs e)
        {
            if (service.Libelle == "administratif" || service.Libelle == "administrateur")
            {
                VueAlerteAbonnement frmAlerteFinAbonnement = new VueAlerteAbonnement(controller);
                frmAlerteFinAbonnement.ShowDialog();
            }
            AutorisationsAcces(service);
        }

        private void AutorisationsAcces(Service service)
        {
            if (service.Libelle == "prêts")
            {
                tabOngletsApplication.TabPages.Remove(tabCommandesLivres);
                tabOngletsApplication.TabPages.Remove(tabCommandesDvd);
                tabOngletsApplication.TabPages.Remove(tabCommandesRevues);


                grpLivresInfos.Enabled = false;
                txbExemplaireLivresNumero.Enabled = false;
                dtpDateAchatExemplaireLivre.Enabled = false;
                cbxEtatLibelleExemplaireLivre.Enabled = false;
                btnEtatExemplaireLivreModifier.Enabled = false;
                btnExemplaireLivreSupprimer.Enabled = false;
                grpDvdInfos.Enabled = false;
                txbExemplaireDvdNumero.Enabled = false;
                dtpDateAchatExemplaireDvd.Enabled = false;



                cbxEtatLibelleExemplaireDvd.Enabled = false;
                btnEtatExemplaireDvdModifier.Enabled = false;
                btnExemplaireDvdSupprimer.Enabled = false;

                grpRevuesInfos.Enabled = false;

                txbReceptionExemplaireNumero.Enabled = false;
                dtpReceptionExemplaireDate.Enabled = false;
                txbReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireImage.Enabled = false;
                btnReceptionExemplaireValider.Enabled = false;
                dtpDateAchatExemplaireRevue.Enabled = false;
                btnExemplaireRevueSupprimer.Enabled = false;
                cbxEtatLibelleExemplaireRevue.Enabled = false;
                btnEtatExemplaireRevueModifier.Enabled = false;
            }
        }

        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();

        private void tabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
            RemplirCbxAjoutModifGenreLivre();
            RemplirCbxAjoutModifPublicLivre();
            RemplirCbxAjoutModifRayonLivre();
            RemplirLivresListeComplete();
            gbxExemplairesLivre.Enabled = false;
            gbxEtatExemplaireLivre.Enabled = false;


            //dgvLivresListe.CellClick += affichageExemplairesLivreClick;
            dgvLivresListe.SelectionChanged += affichageExemplairesLivreSelection;
 
        }

        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
            affichageExemplairesLivreClick();


        }

        private void affichageExemplairesLivreClick(object sender = null, DataGridViewCellEventArgs e = null)
        {
            int rowIndex;

            if (e != null)
            {
                rowIndex = e.RowIndex;
            }
            else if(dgvLivresListe.Rows.Count > 0)
            {
                rowIndex = 0;
            }
            else {

                return;
            }


            if (rowIndex >= 0)
            {
                // Récupérer la valeur de la "troisième" colonne (colonne ID)
                string id = (string)dgvLivresListe.Rows[rowIndex].Cells[3].Value;

                if (id != null)
                {
                    this.INDEXLIVRE = id;
                    AfficheExemplairesLivres(id);
                }
            }

        }

        private void affichageExemplairesLivreSelection(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentRow != null) // Vérifier si une ligne est sélectionnée
            {
                int rowIndex = dgvLivresListe.CurrentRow.Index; // Récupérer l'index de la ligne sélectionnée

                // Vérifier si l'index est valide et la cellule non vide
                object cellValue = dgvLivresListe.Rows[rowIndex].Cells[3].Value;
                if (cellValue != null)
                {
                    string id = cellValue.ToString();
                    this.INDEXLIVRE = id;
                    AfficheExemplairesLivres(id);
                    gbxExemplairesLivre.Enabled = true;
                    gbxEtatExemplaireLivre.Enabled = true;

                }
            }
        }

        private void btnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                VideExemplaireLivre();
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                    gbxExemplairesLivre.Enabled = true;
                    AfficheExemplairesLivres(); 
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        private void txbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresTitre.Text = livre.Titre;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
            cbxAjoutModifGenreLivre.Text = "";
            cbxAjoutModifPublicLivre.Text = "";
            cbxAjoutModifRayonLivre.Text = "";
        }

        private void btnInfosLivreVider_Click(object sender, EventArgs e)
        {
            VideLivresInfos();
        }

        private void cbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }


        private void cbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        private void cbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        private void dgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                    VideComboAjoutModifLivres();
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        private void btnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        private void btnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        private void btnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        private void dgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        private string RemplirCbxAjoutModifGenreLivre()
        {
            List<Categorie> lesGenresLivres = controller.GetAllGenres();
            foreach (Categorie genre in lesGenresLivres)
            {
                cbxAjoutModifGenreLivre.Items.Add(genre.Libelle);
            }

            if (cbxAjoutModifGenreLivre.Items.Count > 0)
            {
                cbxAjoutModifGenreLivre.SelectedIndex = 0;
            }
            return cbxAjoutModifGenreLivre.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifPublicLivre()
        {
            List<Categorie> lesPublicsLivres = controller.GetAllPublics();
            foreach (Categorie lePublic in lesPublicsLivres)
            {
                cbxAjoutModifPublicLivre.Items.Add(lePublic.Libelle);
            }

            if (cbxAjoutModifPublicLivre.Items.Count > 0)
            {
                cbxAjoutModifPublicLivre.SelectedIndex = 0;
            }
            return cbxAjoutModifPublicLivre.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifRayonLivre()
        {
            List<Categorie> lesRayonsLivres = controller.GetAllRayons();
            foreach (Categorie rayon in lesRayonsLivres)
            {
                cbxAjoutModifRayonLivre.Items.Add(rayon.Libelle);
            }

            if (cbxAjoutModifRayonLivre.Items.Count > 0)
            {
                cbxAjoutModifRayonLivre.SelectedIndex = 0;
            }
            return cbxAjoutModifRayonLivre.SelectedItem?.ToString();
        }

        private string GetIdGenreDocument(string genre)
        {
            List<Categorie> lesGenresDocument = controller.GetAllGenres();
            foreach (Categorie c in lesGenresDocument)
            {
                if (c.Libelle == genre)
                {
                    return c.Id;
                }
            }
            return null;
        }

        private string GetIdPublicDocument(string lePublic)
        {
            List<Categorie> lesPublicsDocument = controller.GetAllPublics();
            foreach (Categorie c in lesPublicsDocument)
            {
                if (c.Libelle == lePublic)
                {
                    return c.Id;
                }
            }
            return null;
        }

        private string GetIdRayonDocument(string rayon)
        {
            List<Categorie> lesRayonsDocument = controller.GetAllRayons();
            foreach (Categorie c in lesRayonsDocument)
            {
                if (c.Libelle == rayon)
                {
                    return c.Id;
                }
            }
            return null;
        }

        private void VideComboAjoutModifLivres()
        {
            cbxAjoutModifGenreLivre.Text = "";
            cbxAjoutModifPublicLivre.Text = "";
            cbxAjoutModifRayonLivre.Text = "";
        }

        private void btnReceptionLivreAjouter_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumero.Text.Equals("") && !txbLivresTitre.Text.Equals("")  && !cbxAjoutModifGenreLivre.Text.Equals("") && !cbxAjoutModifPublicLivre.Text.Equals("") && !cbxAjoutModifRayonLivre.Text.Equals(""))
            {
                try
                {
                    string id = txbLivresNumero.Text;
                    string titre = txbLivresTitre.Text;
                    string image = txbLivresImage.Text;
                    string isbn = txbLivresIsbn.Text;
                    string auteur = txbLivresAuteur.Text;
                    string collection = txbLivresCollection.Text;
                    string idGenre = GetIdGenreDocument(cbxAjoutModifGenreLivre.Text);
                    string genre = txbLivresGenre.Text;
                    string Public = txbLivresPublic.Text;
                    string idPublic = GetIdPublicDocument(cbxAjoutModifPublicLivre.Text);
                    string rayon = txbLivresRayon.Text;
                    string idRayon = GetIdRayonDocument(cbxAjoutModifRayonLivre.Text);
                    
                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, Public, idRayon, rayon);
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, Public, idRayon, rayon);

                    var idLivreExistant = controller.GetAllDocuments(id);
                    var idLivreNonExistant = !idLivreExistant.Any();

                    if (idLivreNonExistant)
                    {
                        if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection))
                        {
                            lesLivres = controller.GetAllLivres();
                            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                            RemplirLivresListeComplete();
                            MessageBox.Show("Le livre " + titre + " a correctement été ajouté.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Le numéro de document existe déjà, veuillez saisir un autre numéro.", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Une erreur s'est produite", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }


        private void btnReceptionLivreModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresListe.SelectedRows.Count > 0)
            {
                Livre selectedLivre = (Livre)dgvLivresListe.SelectedRows[0].DataBoundItem;

                string id = selectedLivre.Id;
                string titre = txbLivresTitre.Text;
                string image = txbLivresImage.Text;
                string isbn = txbLivresIsbn.Text;
                string auteur = txbLivresAuteur.Text;
                string collection = txbLivresCollection.Text;
                string idGenre = GetIdGenreDocument(cbxAjoutModifGenreLivre.Text);
                string idPublic = GetIdPublicDocument(cbxAjoutModifPublicLivre.Text);
                string idRayon = GetIdRayonDocument(cbxAjoutModifRayonLivre.Text);

                if (!txbLivresNumero.Text.Equals("") && !txbLivresTitre.Text.Equals("") && !cbxAjoutModifGenreLivre.Text.Equals("") && !cbxAjoutModifPublicLivre.Text.Equals("") && !cbxAjoutModifRayonLivre.Text.Equals(""))
                {
                    if (controller.ModifierLivre(id, isbn, auteur, collection) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show("Le livre " + titre + " a correctement été modifié.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du livre", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Tous les champs sont obligatoires.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        private void btnSupprimerLivre_Click(object sender, EventArgs e)
        {
            Livre livre = (Livre)bdgLivresListe.Current;
            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var exemplairesLivre = controller.GetExemplairesDocument(livre.Id);
                var aucunExemplaire = !exemplairesLivre.Any();

                var commandesLivre = controller.GetCommandesDocument(livre.Id);
                var aucuneCommande = !commandesLivre.Any();

                if (aucunExemplaire && aucuneCommande)
                {
                    if (controller.SupprimerLivre(livre.Id))
                    {
                        lesLivres = controller.GetAllLivres();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirLivresListeComplete();
                        MessageBox.Show($"Le livre {livre.Titre} a correctement été supprimé.");
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show($"Impossible de supprimer le livre car il possède {(aucunExemplaire ? "une ou plusieurs commande(s)" : "un ou plusieurs exemplaire(s)")}.", "Erreur");
                }
            }

        }

        private readonly BindingSource bdgExemplairesLivre = new BindingSource();
        private List<Exemplaire> lesExemplairesDocument = new List<Exemplaire>();

        private void RemplirExemplairesLivre(List<Exemplaire> lesExemplaires)
        {
            if (lesExemplaires != null)
            {

                Console.WriteLine("_________________________________________________________________");

                bdgExemplairesLivre.DataSource = lesExemplaires;
                dgvExemplairesLivre.DataSource = bdgExemplairesLivre;
                dgvExemplairesLivre.Columns["photo"].Visible = false;
                dgvExemplairesLivre.Columns["idEtat"].Visible = false;
                dgvExemplairesLivre.Columns["id"].Visible = false;
                dgvExemplairesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvExemplairesLivre.Columns[0].HeaderCell.Value = "Numéro";
                dgvExemplairesLivre.Columns[2].HeaderCell.Value = "Date d'achat";
                dgvExemplairesLivre.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                dgvExemplairesLivre.DataSource = null;
            }
        }

        private void AfficheExemplairesLivres(string idDocument = null)
        {
            if (idDocument == null)
            {
                idDocument = txbLivresNumRecherche.Text;
            }
            //Encore null..
            if (idDocument.Length <=0 || idDocument == null)
            {
                idDocument = this.INDEXLIVRE;
            }


            Console.WriteLine(this.INDEXLIVRE);
            lesExemplairesDocument = controller.GetExemplairesDocument(idDocument);


            RemplirExemplairesLivre(lesExemplairesDocument);
        }

        private void dgvExemplairesLivre_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvExemplairesLivre.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Date d'achat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Numéro":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Numero).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirExemplairesLivre(sortedList);
        }

        private void RemplirCbxEtatLibelleExemplaireLivre(string etatExemplaireLivre)
        {
            cbxEtatLibelleExemplaireLivre.Items.Clear();
            if (etatExemplaireLivre == "neuf")
            {
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }

            else if (etatExemplaireLivre == "usagé")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "détérioré")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("inutilisable");
            }
            else if (etatExemplaireLivre == "inutilisable")
            {
                cbxEtatLibelleExemplaireLivre.Text = "";
                cbxEtatLibelleExemplaireLivre.Items.Add("neuf");
                cbxEtatLibelleExemplaireLivre.Items.Add("usagé");
                cbxEtatLibelleExemplaireLivre.Items.Add("détérioré");
            }
        }

        private void lblEtatExemplaireLivre_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireLivre = lblEtatExemplaireLivre.Text;
            RemplirCbxEtatLibelleExemplaireLivre(etatExemplaireLivre);
        }

        private string GetIdEtat(string libelle)
        {
            List<Etat> lesEtats = controller.GetAllEtatsDocument();
            foreach (Etat unEtat in lesEtats)
            {
                if (unEtat.Libelle == libelle)
                {
                    return unEtat.Id;
                }
            }
            return null;
        }

        private void VideExemplaireLivre()
        {
            txbExemplaireLivresNumero.Text = "";
            dtpDateAchatExemplaireLivre.Value = DateTime.Now;
            lblEtatExemplaireLivre.Text = "";
            gbxEtatExemplaireLivre.Enabled = false;
        }

        private void dgvExemplairesLivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvExemplairesLivre.Rows[e.RowIndex];

            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbExemplaireLivresNumero.Text = numero;
            dtpDateAchatExemplaireLivre.Value = dateAchat;
            lblEtatExemplaireLivre.Text = libelle;
            gbxEtatExemplaireLivre.Enabled = true;
        }

        private void btnEtatExemplaireLivreModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbLivresNumRecherche.Text;

            if ( idDocument == null)
            {
                if (dgvLivresListe.CurrentRow != null)
                {
                    object cellValue = dgvLivresListe.CurrentRow.Cells[3].Value;

                    idDocument = cellValue.ToString();
                }
                else
                {
                    return;
                }
            }

            idDocument = this.INDEXLIVRE;

            int numero = int.Parse(txbExemplaireLivresNumero.Text);
            DateTime dateAchat = dtpDateAchatExemplaireLivre.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireLivre.Text);

            
            Console.WriteLine(idDocument);
            Console.WriteLine("===================================");

            try
            {
                string libelle = cbxEtatLibelleExemplaireLivre.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheExemplairesLivres();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        private void btnExemplaireLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesLivre.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesLivre.List[bdgExemplairesLivre.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " du livre " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    //AfficheExemplairesLivres();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();

        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            gbxEtatExemplaireDvd.Enabled = false;
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
            RemplirCbxAjoutModifGenreDvd();
            RemplirCbxAjoutModifPublicDvd();
            RemplirCbxAjoutModifRayonDvd();
            RemplirDvdListeComplete();

            dgvDvdListe.SelectionChanged += affichageExemplairesDvdSelection;

        }


        private void affichageExemplairesDvdSelection(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentRow != null) // Vérifier si une ligne est sélectionnée
            {

                int rowIndex = dgvDvdListe.CurrentRow.Index; // Récupérer l'index de la ligne sélectionnée

                // Vérifier si l'index est valide et la cellule non vide
                object cellValue = dgvDvdListe.Rows[rowIndex].Cells[3].Value;
                if (cellValue != null)
                {
                    string id = cellValue.ToString();
                    this.INDEXDVD = id;
                    AfficheExemplairesDvd();
                    gbxExemplairesDvd.Enabled = true;
                    gbxEtatExemplaireDvd.Enabled = true;

                }
            }
        }


        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                VideExemplaireDvd();
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                    AfficheExemplairesDvd();
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }

        }

        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
            cbxAjoutModifGenreDvd.Text = "";
            cbxAjoutModifPublicDvd.Text = "";
            cbxAjoutModifRayonDvd.Text = "";
        }

        private void btnInfosDvdVider_Click(object sender, EventArgs e)
        {
            VideDvdInfos();
        }

        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                    VideComboAjoutModifDvd();
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        private string RemplirCbxAjoutModifGenreDvd()
        {
            List<Categorie> lesGenresDvd = controller.GetAllGenres();
            foreach (Categorie genre in lesGenresDvd)
            {
                cbxAjoutModifGenreDvd.Items.Add(genre.Libelle);
            }

            if (cbxAjoutModifGenreDvd.Items.Count > 0)
            {
                cbxAjoutModifGenreDvd.SelectedIndex = 0;
            }
            return cbxAjoutModifGenreDvd.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifPublicDvd()
        {
            List<Categorie> lesPublicsDvd = controller.GetAllPublics();
            foreach (Categorie lePublic in lesPublicsDvd)
            {
                cbxAjoutModifPublicDvd.Items.Add(lePublic.Libelle);
            }

            if (cbxAjoutModifPublicDvd.Items.Count > 0)
            {
                cbxAjoutModifPublicDvd.SelectedIndex = 0;
            }
            return cbxAjoutModifPublicDvd.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifRayonDvd()
        {
            List<Categorie> lesRayonsDvd = controller.GetAllRayons();
            foreach (Categorie rayon in lesRayonsDvd)
            {
                if (rayon.Libelle == "DVD films")
                {
                    cbxAjoutModifRayonDvd.Items.Add(rayon.Libelle);
                }
            }

            if (cbxAjoutModifRayonDvd.Items.Count > 0)
            {
                cbxAjoutModifRayonDvd.SelectedIndex = 0;
            }
            return cbxAjoutModifRayonDvd.SelectedItem?.ToString();
        }

        private void VideComboAjoutModifDvd()
        {
            cbxAjoutModifGenreDvd.Text = "";
            cbxAjoutModifPublicDvd.Text = "";
            cbxAjoutModifRayonDvd.Text = "";
        }

        private void btnReceptionDvdValider_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumero.Text.Equals("") && !txbDvdTitre.Text.Equals("") && !txbDvdRealisateur.Text.Equals("") && !cbxAjoutModifGenreDvd.Text.Equals("") && !cbxAjoutModifPublicDvd.Text.Equals("") && !cbxAjoutModifRayonDvd.Text.Equals(""))
            {
                try
                {
                    string id = txbDvdNumero.Text;
                    string titre = txbDvdTitre.Text;
                    string image = txbDvdImage.Text;
                    int duree = int.Parse(txbDvdDuree.Text);
                    string realisateur = txbDvdRealisateur.Text;
                    string synopsis = txbDvdSynopsis.Text;
                    string idGenre = GetIdGenreDocument(cbxAjoutModifGenreDvd.Text);
                    string genre = txbDvdGenre.Text;
                    string idPublic = GetIdPublicDocument(cbxAjoutModifPublicDvd.Text);
                    string lePublic = txbDvdPublic.Text;
                    string idRayon = GetIdRayonDocument(cbxAjoutModifRayonDvd.Text);
                    string rayon = txbDvdRayon.Text;

                    Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);

                    var idDvdExistant = controller.GetAllDocuments(id);
                    var idDvdNonExistant = !idDvdExistant.Any();

                    if (idDvdNonExistant)
                    {

                        if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree))
                        {
                            lesDvd = controller.GetAllDvd();
                            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                            RemplirDvdListeComplete();
                            MessageBox.Show("Le dvd " + titre + " a correctement été ajouté.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Le numéro de document existe déjà.", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("Une erreur s'est produite", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        private void btnReceptionDvdModifier_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumero.Text.Equals("") && !txbDvdTitre.Text.Equals("") && !txbDvdRealisateur.Text.Equals("") && !cbxAjoutModifGenreDvd.Text.Equals("") && !cbxAjoutModifPublicDvd.Text.Equals("") && !cbxAjoutModifRayonDvd.Text.Equals(""))
            {

                if (dgvDvdListe.SelectedRows.Count > 0)
                {
                    Dvd selectedDvd = (Dvd)dgvDvdListe.SelectedRows[0].DataBoundItem;
                    
                    string id = selectedDvd.Id;
                    string synopsis = txbDvdSynopsis.Text;
                    string realisateur = txbDvdRealisateur.Text;
                    int duree = int.Parse(txbDvdDuree.Text);
                    string titre = txbDvdTitre.Text;
                    string image = txbDvdImage.Text;
                    string idGenre = GetIdGenreDocument(cbxAjoutModifGenreDvd.Text);
                    string idPublic = GetIdPublicDocument(cbxAjoutModifPublicDvd.Text);
                    string idRayon = GetIdRayonDocument(cbxAjoutModifRayonDvd.Text);

                    if (controller.ModifierDvd(id, synopsis, realisateur, duree) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + titre + " a correctement été modifié.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification du DVD", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Une ligne doit être selectionnée", "Information");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        private void btnSupprimerDvd_Click(object sender, EventArgs e)
        {
            Dvd dvd = (Dvd)bdgDvdListe.Current;

            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var exemplairesDvd = controller.GetExemplairesDocument(dvd.Id);
                var aucunExemplaireDvd = !exemplairesDvd.Any();

                var commandesDvd = controller.GetCommandesDocument(dvd.Id);
                var aucuneCommandeDvd = !commandesDvd.Any();

                if (aucunExemplaireDvd && aucuneCommandeDvd)
                {
                    if (controller.SupprimerDvd(dvd.Id))
                    {
                        lesDvd = controller.GetAllDvd();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);
                        RemplirDvdListeComplete();
                        MessageBox.Show("Le dvd " + dvd.Titre + " a correctement été supprimé.");
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show($"Impossible de supprimer le dvd car il possède {(aucunExemplaireDvd ? "une ou plusieurs commande(s)" : "un ou plusieurs exemplaire(s)")}.", "Erreur");
                }
            }
        }

        private readonly BindingSource bdgExemplairesDvd = new BindingSource();

        private void RemplirExemplairesDvd(List<Exemplaire> lesExemplaires)
        {
            if (lesExemplaires != null)
            {
                bdgExemplairesDvd.DataSource = lesExemplaires;
                dgvExemplairesDvd.DataSource = bdgExemplairesDvd;
                dgvExemplairesDvd.Columns["photo"].Visible = false;
                dgvExemplairesDvd.Columns["idEtat"].Visible = false;
                dgvExemplairesDvd.Columns["id"].Visible = false;
                dgvExemplairesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvExemplairesDvd.Columns[0].HeaderCell.Value = "Numéro";
                dgvExemplairesDvd.Columns[2].HeaderCell.Value = "Date d'achat";
                dgvExemplairesDvd.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                dgvExemplairesDvd.DataSource = null;
            }
        }

        private void AfficheExemplairesDvd()
        {
            string idDocument = txbDvdNumRecherche.Text;

            if(idDocument.Length <= 0)
            {
                idDocument = this.INDEXDVD;
            }

            lesExemplairesDocument = controller.GetExemplairesDocument(idDocument);
            RemplirExemplairesDvd(lesExemplairesDocument);
        }

        private void dgvExemplairesDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvExemplairesDvd.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Date d'achat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Numéro":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Numero).ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplairesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirExemplairesDvd(sortedList);
        }

        private void RemplirCbxEtatLibelleExemplaireDvd(string etatExemplaireDvd)
        {
            cbxEtatLibelleExemplaireDvd.Items.Clear();
            if (etatExemplaireDvd == "neuf")
            {
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }

            else if (etatExemplaireDvd == "usagé")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }
            else if (etatExemplaireDvd == "détérioré")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("inutilisable");
            }
            else if (etatExemplaireDvd == "inutilisable")
            {
                cbxEtatLibelleExemplaireDvd.Text = "";
                cbxEtatLibelleExemplaireDvd.Items.Add("neuf");
                cbxEtatLibelleExemplaireDvd.Items.Add("usagé");
                cbxEtatLibelleExemplaireDvd.Items.Add("détérioré");
            }
        }

        private void lblEtatExemplaireDvd_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireDvd = lblEtatExemplaireDvd.Text;
            RemplirCbxEtatLibelleExemplaireDvd(etatExemplaireDvd);
        }

        private void dgvExemplairesDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvExemplairesDvd.Rows[e.RowIndex];

            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbExemplaireDvdNumero.Text = numero;
            dtpDateAchatExemplaireDvd.Value = dateAchat;
            lblEtatExemplaireDvd.Text = libelle;
            gbxEtatExemplaireDvd.Enabled = true;
        }

        private void VideExemplaireDvd()
        {
            txbExemplaireDvdNumero.Text = "";
            dtpDateAchatExemplaireDvd.Value = DateTime.Now;
            lblEtatExemplaireDvd.Text = "";
            gbxEtatExemplaireDvd.Enabled = false;
        }

        private void btnEtatExemplaireDvdModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbDvdNumRecherche.Text;

            if(idDocument.Length <= 0)
            {
                idDocument = this.INDEXDVD;
            }

            int numero = int.Parse(txbExemplaireDvdNumero.Text);
            DateTime dateAchat = dtpDateAchatExemplaireDvd.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireDvd.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireDvd.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheExemplairesDvd();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        private void btnExemplaireDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvExemplairesDvd.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesDvd.List[bdgExemplairesDvd.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " du dvd " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheExemplairesDvd();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirCbxAjoutModifGenreRevue();
            RemplirCbxAjoutModifPublicRevue();
            RemplirCbxAjoutModifRayonRevue();
            RemplirRevuesListeComplete();
        }

        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        private void btnInfosRevuesVider_Click(object sender, EventArgs e)
        {
            VideRevuesInfos();
        }

        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                    VideComboAjoutModifRevue();
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }

        private string RemplirCbxAjoutModifGenreRevue()
        {
            List<Categorie> lesGenresRevue = controller.GetAllGenres();
            foreach (Categorie genre in lesGenresRevue)
            {
                cbxAjoutModifGenreRevue.Items.Add(genre.Libelle);
            }

            if (cbxAjoutModifGenreRevue.Items.Count > 0)
            {
                cbxAjoutModifGenreRevue.SelectedIndex = 0;
            }
            return cbxAjoutModifGenreRevue.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifPublicRevue()
        {
            List<Categorie> lesPublicsRevue = controller.GetAllPublics();
            foreach (Categorie lePublic in lesPublicsRevue)
            {
                cbxAjoutModifPublicRevue.Items.Add(lePublic.Libelle);
            }

            if (cbxAjoutModifPublicRevue.Items.Count > 0)
            {
                cbxAjoutModifPublicRevue.SelectedIndex = 0;
            }
            return cbxAjoutModifPublicRevue.SelectedItem?.ToString();
        }

        private string RemplirCbxAjoutModifRayonRevue()
        {
            List<Categorie> lesRayonsRevue = controller.GetAllRayons();
            foreach (Categorie rayon in lesRayonsRevue)
            {
                cbxAjoutModifRayonRevue.Items.Add(rayon.Libelle);
            }

            if (cbxAjoutModifRayonRevue.Items.Count > 0)
            {
                cbxAjoutModifRayonRevue.SelectedIndex = 0;
            }
            return cbxAjoutModifRayonRevue.SelectedItem?.ToString();
        }

        private void VideComboAjoutModifRevue()
        {
            cbxAjoutModifGenreRevue.Text = "";
            cbxAjoutModifPublicRevue.Text = "";
            cbxAjoutModifRayonRevue.Text = "";
        }

        private void btnReceptionRevueValider_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumero.Text.Equals("") && !txbRevuesTitre.Text.Equals("") && !txbRevuesPeriodicite.Text.Equals("") && !txbRevuesDateMiseADispo.Text.Equals("") && !cbxAjoutModifGenreRevue.Text.Equals("") && !cbxAjoutModifPublicRevue.Text.Equals("") && !cbxAjoutModifRayonRevue.Text.Equals(""))
            {
                string id = txbRevuesNumero.Text;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = GetIdGenreDocument(cbxAjoutModifGenreRevue.Text);
                string genre = txbRevuesGenre.Text;
                string idPublic = GetIdPublicDocument(cbxAjoutModifPublicRevue.Text);
                string lePublic = txbRevuesPublic.Text;
                string idRayon = GetIdRayonDocument(cbxAjoutModifRayonRevue.Text);
                string rayon = txbRevuesRayon.Text;
                string periodicite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);

                Document document = new Document(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

                var idRevueExistante = controller.GetAllDocuments(id);
                var idRevueNonExistante = !idRevueExistante.Any();

                if (idRevueNonExistante)
                {
                    if (controller.CreerDocument(document.Id, document.Titre, document.Image, document.IdRayon, document.IdPublic, document.IdGenre) && controller.CreerRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + titre + " a correctement été ajoutée.");
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de document existe déjà.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        private void btnReceptionRevueModifier_Click(object sender, EventArgs e)
        {
            if (dgvRevuesListe.SelectedRows.Count > 0)
            {
                Revue selectedRevue = (Revue)dgvRevuesListe.SelectedRows[0].DataBoundItem;

                string id = selectedRevue.Id;
                string titre = txbRevuesTitre.Text;
                string image = txbRevuesImage.Text;
                string idGenre = GetIdGenreDocument(cbxAjoutModifGenreRevue.Text);
                string idPublic = GetIdPublicDocument(cbxAjoutModifPublicRevue.Text);
                string idRayon = GetIdRayonDocument(cbxAjoutModifRayonRevue.Text);
                string periodicite = txbRevuesPeriodicite.Text;
                int delaiMiseADispo = int.Parse(txbRevuesDateMiseADispo.Text);

                if (!txbRevuesNumero.Text.Equals("") && !txbRevuesTitre.Text.Equals("") && !txbRevuesPeriodicite.Text.Equals("") && !txbRevuesDateMiseADispo.Text.Equals("") && !cbxAjoutModifGenreRevue.Text.Equals("") && !cbxAjoutModifPublicRevue.Text.Equals("") && !cbxAjoutModifRayonRevue.Text.Equals(""))
                {
                    if (controller.ModifierRevue(id, periodicite, delaiMiseADispo) && controller.ModifierDocument(id, titre, image, idGenre, idPublic, idRayon))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + titre + " a correctement été modifiée.");
                    }
                    else
                    {
                        MessageBox.Show("Erreur lors de la modification de la revue", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Tous les champs sont obligatoires.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée", "Information");
            }
        }

        private void btnSupprimerRevue_Click(object sender, EventArgs e)
        {
            Revue revue = (Revue)bdgRevuesListe.Current;
            if (MessageBox.Show("Souhaitez-vous confirmer la suppression?", "Confirmation de suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                List<Exemplaire> lesExemplairesRevue = controller.GetExemplairesRevue(revue.Id);
                bool aucunExemplaire = true;
                foreach (Exemplaire exemplaire in lesExemplairesRevue)
                {
                    if (exemplaire.Id == revue.Id)
                    {
                        aucunExemplaire = false;
                        break;
                    }
                }
                if (aucunExemplaire)
                {
                    if (controller.SupprimerRevue(revue.Id))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
                        RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
                        RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
                        RemplirRevuesListeComplete();
                        MessageBox.Show("La revue " + revue.Titre + " a correctement été supprimée.");
                    }
                    else
                    {
                        MessageBox.Show("Une erreur s'est produite.", "Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de supprimer la revue car elle a un ou plusieurs exemplaires associés.", "Erreur");
                }
            }
        }

        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            gbxEtatExemplaireRevue.Enabled = false;
            dtpDateAchatExemplaireRevue.Value = DateTime.Now;
        }

        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                dgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                dgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                dgvReceptionExemplairesListe.Columns["id"].Visible = false;
                dgvReceptionExemplairesListe.Columns["photo"].Visible = false;
                dgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvReceptionExemplairesListe.Columns[1].HeaderCell.Value = "Numéro";
                dgvReceptionExemplairesListe.Columns[3].HeaderCell.Value = "Date d'achat";
                dgvReceptionExemplairesListe.Columns[5].HeaderCell.Value = "Etat";
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        private void AfficheReceptionExemplairesRevue()
        {
            string idDocument = txbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesDocument(idDocument);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = txbReceptionRevueNumero.Text;
                    string libelle = "";
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                    if (controller.CreerExemplaireRevue(idDocument, numero, dateAchat, photo, idEtat))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Etat":
                    sortedList = lesExemplaires.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    pcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    pcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                pcbReceptionExemplaireRevueImage.Image = null;
            }
        }

        private void RemplirCbxEtatLibelleExemplaireRevue(string etatExemplaireRevue)
        {
            cbxEtatLibelleExemplaireRevue.Items.Clear();
            if (etatExemplaireRevue == "neuf")
            {
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }

            else if (etatExemplaireRevue == "usagé")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }
            else if (etatExemplaireRevue == "détérioré")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("inutilisable");
            }
            else if (etatExemplaireRevue == "inutilisable")
            {
                cbxEtatLibelleExemplaireRevue.Text = "";
                cbxEtatLibelleExemplaireRevue.Items.Add("neuf");
                cbxEtatLibelleExemplaireRevue.Items.Add("usagé");
                cbxEtatLibelleExemplaireRevue.Items.Add("détérioré");
            }
        }

        private void lblEtatExemplaireRevue_TextChanged(object sender, EventArgs e)
        {
            string etatExemplaireRevue = lblEtatExemplaireRevue.Text;
            RemplirCbxEtatLibelleExemplaireRevue(etatExemplaireRevue);
        }

        private void dgvReceptionExemplairesListe_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvReceptionExemplairesListe.Rows[e.RowIndex];

            string numero = row.Cells["Numero"].Value.ToString();
            DateTime dateAchat = (DateTime)row.Cells["dateAchat"].Value;
            string libelle = row.Cells["Libelle"].Value.ToString();
            gbxEtatExemplaireRevue.Enabled = true;

            lblNumeroExemplaireRevue.Text = numero;
            dtpDateAchatExemplaireRevue.Value = dateAchat;
            lblEtatExemplaireRevue.Text = libelle;
        }

        private void btnEtatExemplaireRevueModifier_Click(object sender, EventArgs e)
        {
            string idDocument = txbReceptionRevueNumero.Text;
            int numero = int.Parse(lblNumeroExemplaireRevue.Text);
            DateTime dateAchat = dtpDateAchatExemplaireRevue.Value;
            string photo = "";
            string idEtat = GetIdEtat(cbxEtatLibelleExemplaireRevue.Text);
            try
            {
                string libelle = cbxEtatLibelleExemplaireRevue.SelectedItem.ToString();

                Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument, libelle);
                if (MessageBox.Show("Voulez-vous modifier l'état de l'exemplaire " + exemplaire.Numero + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierEtatExemplaireDocument(exemplaire);
                    MessageBox.Show("L'état de l'exemplaire " + exemplaire.Numero + " a bien été modifié.", "Information");
                    AfficheReceptionExemplairesRevue();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Le nouvel état de l'exemplaire doit être sélectionné.", "Information");
            }
        }

        private void btnExemplaireRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.SelectedRows.Count > 0)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                if (MessageBox.Show("Voulez-vous supprimer l'exemplaire " + exemplaire.Numero + " de la revue " + exemplaire.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.SupprimerExemplaireDocument(exemplaire);
                    MessageBox.Show("L'exemplaire " + exemplaire.Numero + " a bien été supprimé.", "Information");
                    AfficheReceptionExemplairesRevue();
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        #endregion

        #region Onglet CommandesLivres
        private readonly BindingSource bdgCommandesLivre = new BindingSource();
        private List<CommandeDocument> lesCommandesDocument = new List<CommandeDocument>();
        private List<Suivi> lesSuivis = new List<Suivi>();

        private void tabCommandesLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            lesSuivis = controller.GetAllSuivis();
            gbxInfosCommandeLivre.Enabled = false;
            gbxEtapeSuivi.Enabled = false;
        }

        private void gbxInfosCommandeLivre_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
        }

        private void btnInfosCommandeLivreAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = true;
            gbxInfosCommandeLivre.Enabled = false;
        }

        private void gbxEtapeSuivi_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeLivre.Enabled = false;
            txbCommandesLivresNumRecherche.Enabled = false;
        }

        private void btnEtapeSuiviAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuivi.Enabled = false;
            gbxInfosCommandeLivre.Enabled = true;
            txbCommandesLivresNumRecherche.Enabled = true;
        }

        private void RemplirCommandesLivresListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesLivre.DataSource = lesCommandesDocument;
                dgvCommandesLivre.DataSource = bdgCommandesLivre;
                dgvCommandesLivre.Columns["id"].Visible = false;
                dgvCommandesLivre.Columns["idLivreDvd"].Visible = false;
                dgvCommandesLivre.Columns["idSuivi"].Visible = false;
                dgvCommandesLivre.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesLivre.Columns["dateCommande"].DisplayIndex = 4;
                dgvCommandesLivre.Columns["montant"].DisplayIndex = 1;
                dgvCommandesLivre.Columns[5].HeaderCell.Value = "Date de commande";
                dgvCommandesLivre.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvCommandesLivre.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesLivre.DataSource = null;
            }
        }

        private void AfficheReceptionCommandesLivre()
        {
            string idDocument = txbCommandesLivresNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesLivresListe(lesCommandesDocument);
        }

        private void btnCommandesLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesLivresNumRecherche.Text.Equals(""))
            {
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbCommandesLivresNumRecherche.Text));
                if (livre != null)
                {
                    AfficheReceptionCommandesLivre();
                    gbxInfosCommandeLivre.Enabled = true;
                    AfficheReceptionCommandesLivreInfos(livre);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        private void AfficheReceptionCommandesLivreInfos(Livre livre)
        {
            txbCommandeLivresTitre.Text = livre.Titre;
            txbCommandeLivresAuteur.Text = livre.Auteur;
            txbCommandeLivresIsbn.Text = livre.Isbn;
            txbCommandeLivresCollection.Text = livre.Collection;
            txbCommandeLivresGenre.Text = livre.Genre;
            txbCommandeLivresPublic.Text = livre.Public;
            txbCommandeLivresRayon.Text = livre.Rayon;
            txbCommandeLivresCheminImage.Text = livre.Image;
            string image = livre.Image;
            try
            {
                pictureBox1.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox1.Image = null;
            }
            AfficheReceptionCommandesLivre();
        }

        private void lblEtapeSuivi_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuivi.Text;
            RemplirCbxCommandeLivreLibelleSuivi(etapeSuivi);
        }

        private void RemplirCbxCommandeLivreLibelleSuivi(string etapeSuivi)
        {
            cbxCommandeLivresLibelleSuivi.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("relancée");
                cbxCommandeLivresLibelleSuivi.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                cbxCommandeLivresLibelleSuivi.Text = "";
                cbxCommandeLivresLibelleSuivi.Items.Add("en cours");
                cbxCommandeLivresLibelleSuivi.Items.Add("livrée");
            }
        }

        private string GetIdSuivi(string libelle)
        {
            List<Suivi> lesSuivisCommande = controller.GetAllSuivis();
            foreach (Suivi unSuivi in lesSuivisCommande)
            {
                if (unSuivi.Libelle == libelle)
                {
                    return unSuivi.Id;
                }
            }
            return null;
        }

        private void dgvCommandeslivre_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvCommandesLivre.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbCommandeLivreNumero.Text = id;
            txbCommandeLivreNbExemplaires.Text = nbExemplaire.ToString();
            txbCommandeLivreMontant.Text = montant.ToString();
            dtpCommandeLivre.Value = dateCommande;
            lblEtapeSuivi.Text = libelle;

            if (GetIdSuivi(libelle) == "00003")
            {
                cbxCommandeLivresLibelleSuivi.Enabled = false;
                btnReceptionCommandeLivresModifierSuivi.Enabled = false;
            }
            else
            {
                cbxCommandeLivresLibelleSuivi.Enabled = true;
                btnReceptionCommandeLivresModifierSuivi.Enabled = true;
            }
          
        }

        private void dgvCommandesLivre_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesLivre.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesLivresListe(sortedList);
        }

        private void btnReceptionCommandeLivreValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeLivreNumero.Text.Equals("") && !txbCommandeLivreNbExemplaires.Text.Equals("") && !txbCommandeLivreMontant.Text.Equals(""))
            {
                string id = txbCommandeLivreNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeLivreNbExemplaires.Text);
                double montant = double.Parse(txbCommandeLivreMontant.Text);
                DateTime dateCommande = dtpCommandeLivre.Value;
                string idLivreDvd = txbCommandesLivresNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;

                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeLivreExistante = controller.GetCommandes(id);
                var idCommandeLivreNonExistante = !idCommandeLivreExistante.Any();

                if (idCommandeLivreNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        private void btnModifierSuiviCommandeLivre_Click(object sender, EventArgs e)
        {
            string id = txbCommandeLivreNumero.Text;
            int nbExemplaire = int.Parse(txbCommandeLivreNbExemplaires.Text);
            double montant = double.Parse(txbCommandeLivreMontant.Text);
            DateTime dateCommande = dtpCommandeLivre.Value;
            string idLivreDvd = txbCommandesLivresNumRecherche.Text;
            string idSuivi = GetIdSuivi(cbxCommandeLivresLibelleSuivi.Text);

            try
            {
                string libelle = cbxCommandeLivresLibelleSuivi.SelectedItem.ToString();

                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.IdSuivi);
                    MessageBox.Show("L'étape de suivi de la commande " + id + " a bien été modifiée.", "Information");
                    AfficheReceptionCommandesLivre();     
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }
        
        private void btnCommandeLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesLivre.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesLivre.List[bdgCommandesLivre.Position];
                if (commandeDocument.Libelle == "en cours" || commandeDocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandeDocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandeDocument);
                        AfficheReceptionCommandesLivre();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée, elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }

        #endregion

        #region Onglet CommandesDvd
        private readonly BindingSource bdgCommandesDvd = new BindingSource();

        private void tabCommandesDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            lesSuivis = controller.GetAllSuivis();
            dtpCommandeDvd.Value = DateTime.Now;
            gbxInfosCommandeDvd.Enabled = false;
            gbxEtapeSuiviDvd.Enabled = false;
        }

        private void gbxInfosCommandeDvd_Enter(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
        }

        private void btnInfosCommandeDvdAnnuler_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = true;
            gbxInfosCommandeDvd.Enabled = false;
        }

        private void gbxEtapeSuiviDvd_Enter(object sender, EventArgs e)
        {
            gbxInfosCommandeDvd.Enabled = false;
            txbCommandesDvdNumRecherche.Enabled = false;
        }

        private void btnEtapeSuiviAnnulerDvd_Click(object sender, EventArgs e)
        {
            gbxEtapeSuiviDvd.Enabled = false;
            gbxInfosCommandeDvd.Enabled = true;
            txbCommandesDvdNumRecherche.Enabled = true;
        }


        private void RemplirCommandesDvdListe(List<CommandeDocument> lesCommandesDocument)
        {
            if (lesCommandesDocument != null)
            {
                bdgCommandesDvd.DataSource = lesCommandesDocument;
                dgvCommandesDvd.DataSource = bdgCommandesDvd;
                dgvCommandesDvd.Columns["id"].Visible = false;
                dgvCommandesDvd.Columns["idLivreDvd"].Visible = false;
                dgvCommandesDvd.Columns["idSuivi"].Visible = false;
                dgvCommandesDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandesDvd.Columns["dateCommande"].DisplayIndex = 0;
                dgvCommandesDvd.Columns["montant"].DisplayIndex = 1;
                dgvCommandesDvd.Columns[5].HeaderCell.Value = "Date de commande";
                dgvCommandesDvd.Columns[0].HeaderCell.Value = "Nombre d'exemplaires";
                dgvCommandesDvd.Columns[3].HeaderCell.Value = "Suivi";
            }
            else
            {
                bdgCommandesDvd.DataSource = null;
            }
        }

        private void AfficheReceptionCommandesDvd()
        {
            string idDocument = txbCommandesDvdNumRecherche.Text;
            lesCommandesDocument = controller.GetCommandesDocument(idDocument);
            RemplirCommandesDvdListe(lesCommandesDocument);
        }

        private void btnCommandesDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesDvdNumRecherche.Text.Equals(""))
            {
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbCommandesDvdNumRecherche.Text));
                if (dvd != null)
                {
                    AfficheReceptionCommandesDvd();
                    gbxInfosCommandeDvd.Enabled = true;
                    AfficheReceptionCommandesDvdInfos(dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de document est obligatoire", "Information");
            }
        }

        private void AfficheReceptionCommandesDvdInfos(Dvd dvd)
        {
            txbCommandeDvdTitre.Text = dvd.Titre;
            txbCommandeDvdRealisateur.Text = dvd.Realisateur;
            txbCommandeDvdSynopsis.Text = dvd.Synopsis;
            txbCommandeDvdDuree.Text = dvd.Duree.ToString();
            txbCommandeDvdGenre.Text = dvd.Genre;
            txbCommandeDvdPublic.Text = dvd.Public;
            txbCommandeDvdRayon.Text = dvd.Rayon;
            txbCommandeDvdCheminImage.Text = dvd.Image;
            string image = dvd.Image;
            try
            {
                pictureBox2.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox2.Image = null;
            }
            AfficheReceptionCommandesDvd();
        }

        private void lblSuiviEtapeDvd_TextChanged(object sender, EventArgs e)
        {
            string etapeSuivi = lblEtapeSuiviDvd.Text;
            RemplirCbxCommandeDvdLibelleSuivi(etapeSuivi);
        }

        private void RemplirCbxCommandeDvdLibelleSuivi(string etapeSuivi)
        {
            cbxCommandeDvdLibelleSuivi.Items.Clear();
            if (etapeSuivi == "livrée")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("réglée");
            }
            else if (etapeSuivi == "en cours")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("relancée");
                cbxCommandeDvdLibelleSuivi.Items.Add("livrée");
            }
            else if (etapeSuivi == "relancée")
            {
                cbxCommandeDvdLibelleSuivi.Text = "";
                cbxCommandeDvdLibelleSuivi.Items.Add("en cours");
                cbxCommandeDvdLibelleSuivi.Items.Add("livrée");
            }
        }

        private void dgvCommandeDvd_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvCommandesDvd.Rows[e.RowIndex];

            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            int nbExemplaire = int.Parse(row.Cells["NbExemplaire"].Value.ToString());
            string libelle = row.Cells["Libelle"].Value.ToString();

            txbCommandeDvdNumero.Text = id;
            txbCommandeDvdNbExemplaires.Text = nbExemplaire.ToString();
            txbCommandeDvdMontant.Text = montant.ToString();
            dtpCommandeDvd.Value = dateCommande;

            lblEtapeSuiviDvd.Text = libelle;
            if (GetIdSuivi(libelle) == "00003")
            {
                cbxCommandeDvdLibelleSuivi.Enabled = false;
                btnReceptionCommandeDvdModifierSuivi.Enabled = false;
            }
            else
            {
                cbxCommandeDvdLibelleSuivi.Enabled = true;
                btnReceptionCommandeDvdModifierSuivi.Enabled = true;
            }
        }

        private void dgvCommandesDvd_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvCommandesDvd.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesCommandesDocument.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Montant).ToList();
                    break;
                case "Nombre d'exemplaires":
                    sortedList = lesCommandesDocument.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Suivi":
                    sortedList = lesCommandesDocument.OrderBy(o => o.Libelle).ToList();
                    break;
            }
            RemplirCommandesDvdListe(sortedList);
        }

        private void btnReceptionCommandeDvdValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeDvdNumero.Text.Equals("") && !txbCommandeDvdNbExemplaires.Text.Equals("") && !txbCommandeDvdMontant.Text.Equals(""))
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = lesSuivis[0].Id;
                string id = txbCommandeDvdNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeDvdNbExemplaires.Text);
                double montant = double.Parse(txbCommandeDvdMontant.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeExistante = controller.GetCommandes(id);
                var idCommandeNonExistante = !idCommandeExistante.Any();

                if (idCommandeNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerCommandeDocument(id, nbExemplaire, idLivreDvd, idSuivi);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires.", "Information");
            }
        }

        private void btnReceptionCommandeDvdModifierSuivi_Click(object sender, EventArgs e)
        {
            try
            {
                string idLivreDvd = txbCommandesDvdNumRecherche.Text;
                string idSuivi = GetIdSuivi(cbxCommandeDvdLibelleSuivi.Text);
                string libelle = cbxCommandeDvdLibelleSuivi.SelectedItem.ToString();
                string id = txbCommandeDvdNumero.Text;
                int nbExemplaire = int.Parse(txbCommandeDvdNbExemplaires.Text);
                double montant = double.Parse(txbCommandeDvdMontant.Text);
                DateTime dateCommande = dtpCommandeDvd.Value;

                CommandeDocument commandedocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, libelle);
                if (MessageBox.Show("Voulez-vous modifier le suivi de la commande " + commandedocument.Id + " en " + libelle + " ?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    controller.ModifierSuiviCommandeDocument(commandedocument.Id, commandedocument.IdSuivi);
                    AfficheReceptionCommandesDvd();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("La nouvelle étape de suivi de la commande doit être sélectionnée.", "Information");
            }
        }

        private void btnCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvCommandesDvd.SelectedRows.Count > 0)
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgCommandesDvd.List[bdgCommandesDvd.Position];
                if (commandeDocument.Libelle == "en cours" || commandeDocument.Libelle == "relancée")
                {
                    if (MessageBox.Show("Voulez-vous vraiment supprimer la commande " + commandeDocument.Id + " ?", "Confirmation de suppression", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        controller.SupprimerCommandeDocument(commandeDocument);
                        AfficheReceptionCommandesDvd();
                    }
                }
                else
                {
                    MessageBox.Show("La commande sélectionnée a été livrée elle ne peut pas être supprimée.", "Information");
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion

        #region Onglet CommandesRevues

        private readonly BindingSource bdgAbonnementsRevue = new BindingSource();
        private List<Abonnement> lesAbonnementsRevue = new List<Abonnement>();

        private void tabCommandesRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            gbxInfosCommandeRevue.Enabled = false;
            dtpCommandeDvd.Value = DateTime.Now;
        }

        private void RemplirAbonnementsRevueListe(List<Abonnement> lesAbonnementsRevue)
        {
            if (lesAbonnementsRevue != null)
            {
                bdgAbonnementsRevue.DataSource = lesAbonnementsRevue;
                dgvAbonnementsRevue.DataSource = bdgAbonnementsRevue;
                dgvAbonnementsRevue.Columns["id"].Visible = false;
                dgvAbonnementsRevue.Columns["idRevue"].Visible = false;
                dgvAbonnementsRevue.Columns["titre"].Visible = false;
                dgvAbonnementsRevue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvAbonnementsRevue.Columns["dateCommande"].DisplayIndex = 0;
                dgvAbonnementsRevue.Columns["montant"].DisplayIndex = 1;
                dgvAbonnementsRevue.Columns[4].HeaderCell.Value = "Date de commande";
                dgvAbonnementsRevue.Columns[0].HeaderCell.Value = "Date de fin d'abonnement";
            }
            else
            {
                bdgAbonnementsRevue.DataSource = null;
            }
        }

        private void AfficheReceptionAbonnementsRevue()
        {
            string idDocument = txbCommandesRevueNumRecherche.Text;
            lesAbonnementsRevue = controller.GetAbonnementRevue(idDocument);
            RemplirAbonnementsRevueListe(lesAbonnementsRevue);
        }

        private void btnCommandesRevueNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbCommandesRevueNumRecherche.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbCommandesRevueNumRecherche.Text));
                if (revue != null)
                {
                    AfficheReceptionAbonnementsRevue();
                    gbxInfosCommandeRevue.Enabled = true;
                    AfficheReceptionAbonnementsRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("Ce numéro de revue n'existe pas.");
                }
            }
            else
            {
                MessageBox.Show("Le numéro de revue est obligatoire.");
            }
        }

        private void AfficheReceptionAbonnementsRevueInfos(Revue revue)
        {
            txbCommandeRevueTitre.Text = revue.Titre;
            txbCommandeRevuePeriodicite.Text = revue.Periodicite;
            txbCommandeRevueDelai.Text = revue.DelaiMiseADispo.ToString();
            txbCommandeRevueGenre.Text = revue.Genre;
            txbCommandeRevuePublic.Text = revue.Public;
            txbCommandeRevueRayon.Text = revue.Rayon;
            txbCommandeRevueCheminImage.Text = revue.Image; 
            string image = revue.Image;
            try
            {
                pictureBox4.Image = Image.FromFile(image);
            }
            catch
            {
                pictureBox4.Image = null;
            }
            AfficheReceptionAbonnementsRevue();
        }

        private void dgvAbonnementsRevue_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgvAbonnementsRevue.Rows[e.RowIndex];
            string id = row.Cells["Id"].Value.ToString();
            DateTime dateCommande = (DateTime)row.Cells["dateCommande"].Value;
            double montant = double.Parse(row.Cells["Montant"].Value.ToString());
            DateTime dateFinAbonnement = (DateTime)row.Cells["DateFinAbonnement"].Value;

            txbCommandeRevueNumero.Text = id;
            txbCommandeRevueMontant.Text = montant.ToString();
            dtpCommandeRevue.Value = dateCommande;
            dtpCommandeRevueAbonnementFin.Value = dateFinAbonnement;  
        }

        private void dgvAbonnementsRevue_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnementsRevue.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Date de commande":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateCommande).Reverse().ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.Montant).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = lesAbonnementsRevue.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementsRevueListe(sortedList);
        }

        private void btnReceptionAbonnementRevueValider_Click(object sender, EventArgs e)
        {
            if (!txbCommandeRevueNumero.Text.Equals("") && !txbCommandeRevueMontant.Text.Equals(""))
            {
                string idRevue = txbCommandesRevueNumRecherche.Text;
                string id = txbCommandeRevueNumero.Text;
                double montant = double.Parse(txbCommandeRevueMontant.Text);
                DateTime dateCommande = dtpCommandeRevue.Value;
                DateTime dateFinAbonnement = dtpCommandeRevueAbonnementFin.Value;
                
                Commande commande = new Commande(id, dateCommande, montant);

                var idCommandeRevueExistante = controller.GetCommandes(id);
                var idCommandeRevueNonExistante = !idCommandeRevueExistante.Any();

                if (idCommandeRevueNonExistante)
                {
                    if (controller.CreerCommande(commande))
                    {
                        controller.CreerAbonnementRevue(id, dateFinAbonnement, idRevue);
                        MessageBox.Show("La commande " + id + " a bien été enregistrée.", "Information");
                        AfficheReceptionAbonnementsRevue();
                    }
                }
                else
                {
                    MessageBox.Show("Le numéro de la commande existe déjà, veuillez saisir un nouveau numéro.", "Erreur");
                }
            }
            else
            {
                MessageBox.Show("Tous les champs sont obligatoires", "Information");
            }
        }

        public bool ParutionDansAbonnement(DateTime dateCommande, DateTime dateFinAbonnement, DateTime dateParution)
        {
            return (DateTime.Compare(dateCommande, dateParution) < 0 && DateTime.Compare(dateParution, dateFinAbonnement) < 0);
        }

        public bool VerificationExemplaire(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplairesAbonnement = controller.GetExemplairesRevue(abonnement.IdRevue);
            bool datedeparution = false;
            foreach (Exemplaire exemplaire in lesExemplairesAbonnement.Where(exemplaires => ParutionDansAbonnement(abonnement.DateCommande, abonnement.DateFinAbonnement, exemplaires.DateAchat)))
            {
                datedeparution = true;

            }
            return !datedeparution;
        }

        private void btnAbonnementRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (dgvAbonnementsRevue.SelectedRows.Count > 0)
            {
                Abonnement abonnement = (Abonnement)bdgAbonnementsRevue.Current;
                if (MessageBox.Show("Souhaitez-vous confirmer la suppression de l'abonnement " + abonnement.Id + " ?", "Confirmation de la suppression", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {

                    if (VerificationExemplaire(abonnement))
                    {
                        if (controller.SupprimerAbonnementRevue(abonnement))
                        {
                            AfficheReceptionAbonnementsRevue();
                        }
                        else
                        {
                            MessageBox.Show("Une erreur s'est produite.", "Erreur");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cet abonnement contient un ou plusieurs exemplaires, il ne peut donc pas être supprimé.", "Information");
                    }
                }
            }
            else
            {
                MessageBox.Show("Une ligne doit être sélectionnée.", "Information");
            }
        }


        #endregion

        private void dgvExemplairesLivre_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pcbLivresImage_Click(object sender, EventArgs e)
        {

        }

        private void tabOngletsApplication_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabLivres_Click(object sender, EventArgs e)
        {

        }

        private void gbxEtatExemplaireLivre_Enter(object sender, EventArgs e)
        {

        }

        private void label112_Click(object sender, EventArgs e)
        {

        }

        private void dtpDateAchatExemplaireLivre_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label111_Click(object sender, EventArgs e)
        {

        }

        private void txbExemplaireLivresNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblEtatExemplaireLivre_Click(object sender, EventArgs e)
        {

        }

        private void label104_Click(object sender, EventArgs e)
        {

        }

        private void cbxEtatLibelleExemplaireLivre_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void gbxExemplairesLivre_Enter(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifRayonLivre_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifPublicLivre_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifGenreLivre_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblIdRayon_Click(object sender, EventArgs e)
        {

        }

        private void lblIdPublic_Click(object sender, EventArgs e)
        {

        }

        private void lblIdGenre_Click(object sender, EventArgs e)
        {

        }

        private void label59_Click(object sender, EventArgs e)
        {

        }

        private void txbLivresIsbn_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresPublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresCollection_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresAuteur_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbLivresNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void grpLivresInfos_Enter(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void grpLivresRecherche_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txbLivresNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void dgvLivresListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void tabDvd_Click(object sender, EventArgs e)
        {

        }

        private void gbxEtatExemplaireDvd_Enter(object sender, EventArgs e)
        {

        }

        private void label113_Click(object sender, EventArgs e)
        {

        }

        private void dtpDateAchatExemplaireDvd_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label114_Click(object sender, EventArgs e)
        {

        }

        private void txbExemplaireDvdNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblEtatExemplaireDvd_Click(object sender, EventArgs e)
        {

        }

        private void label116_Click(object sender, EventArgs e)
        {

        }

        private void cbxEtatLibelleExemplaireDvd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void gbxExemplairesDvd_Enter(object sender, EventArgs e)
        {

        }

        private void dgvExemplairesDvd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void grpDvdInfos_Enter(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifRayonDvd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifPublicDvd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifGenreDvd_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label62_Click(object sender, EventArgs e)
        {

        }

        private void label61_Click(object sender, EventArgs e)
        {

        }

        private void label60_Click(object sender, EventArgs e)
        {

        }

        private void label58_Click(object sender, EventArgs e)
        {

        }

        private void txbDvdDuree_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdPublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdSynopsis_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdRealisateur_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbDvdNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void pcbDvdImage_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void grpDvdRecherche_Enter(object sender, EventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {

        }

        private void txbDvdNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void label39_Click(object sender, EventArgs e)
        {

        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }

        private void dgvDvdListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {

        }

        private void tabRevues_Click(object sender, EventArgs e)
        {

        }

        private void grpRevuesInfos_Enter(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifRayonRevue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifPublicRevue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbxAjoutModifGenreRevue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label65_Click(object sender, EventArgs e)
        {

        }

        private void label64_Click(object sender, EventArgs e)
        {

        }

        private void label63_Click(object sender, EventArgs e)
        {

        }

        private void label57_Click(object sender, EventArgs e)
        {

        }

        private void txbRevuesImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesPublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesDateMiseADispo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesPeriodicite_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbRevuesNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label35_Click(object sender, EventArgs e)
        {

        }

        private void pcbRevuesImage_Click(object sender, EventArgs e)
        {

        }

        private void label36_Click(object sender, EventArgs e)
        {

        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void label43_Click(object sender, EventArgs e)
        {

        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void label45_Click(object sender, EventArgs e)
        {

        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void label47_Click(object sender, EventArgs e)
        {

        }

        private void grpRevuesRecherche_Enter(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txbRevuesNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void label33_Click(object sender, EventArgs e)
        {

        }

        private void dgvRevuesListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void tabReceptionRevue_Click(object sender, EventArgs e)
        {

        }

        private void gbxEtatExemplaireRevue_Enter(object sender, EventArgs e)
        {

        }

        private void dtpDateAchatExemplaireRevue_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label118_Click(object sender, EventArgs e)
        {

        }

        private void lblNumeroExemplaireRevue_Click(object sender, EventArgs e)
        {

        }

        private void label117_Click(object sender, EventArgs e)
        {

        }

        private void label115_Click(object sender, EventArgs e)
        {

        }

        private void lblEtatExemplaireRevue_Click(object sender, EventArgs e)
        {

        }

        private void cbxEtatLibelleExemplaireRevue_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void grpReceptionExemplaire_Enter(object sender, EventArgs e)
        {

        }

        private void txbReceptionExemplaireImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void txbReceptionExemplaireNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void dtpReceptionExemplaireDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void grpReceptionRevue_Enter(object sender, EventArgs e)
        {

        }

        private void label48_Click(object sender, EventArgs e)
        {

        }

        private void label56_Click(object sender, EventArgs e)
        {

        }

        private void pcbReceptionExemplaireRevueImage_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void dgvReceptionExemplairesListe_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txbReceptionRevueImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevueRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevuePublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevueGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevueDelaiMiseADispo_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevuePeriodicite_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbReceptionRevueTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pcbReceptionRevueImage_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label49_Click(object sender, EventArgs e)
        {

        }

        private void label50_Click(object sender, EventArgs e)
        {

        }

        private void label51_Click(object sender, EventArgs e)
        {

        }

        private void label52_Click(object sender, EventArgs e)
        {

        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void pcbReceptionExemplaireImage_Click(object sender, EventArgs e)
        {

        }

        private void tabCommandesLivres_Click(object sender, EventArgs e)
        {

        }

        private void lblEtapeSuivi_Click(object sender, EventArgs e)
        {

        }

        private void label80_Click(object sender, EventArgs e)
        {

        }

        private void cbxCommandeLivresLibelleSuivi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpCommandeLivre_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivreMontant_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivreNbExemplaires_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivreNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label79_Click(object sender, EventArgs e)
        {

        }

        private void label68_Click(object sender, EventArgs e)
        {

        }

        private void label67_Click(object sender, EventArgs e)
        {

        }

        private void label66_Click(object sender, EventArgs e)
        {

        }

        private void dgvCommandesLivre_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label69_Click(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresIsbn_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresCheminImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresPublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresCollection_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresAuteur_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeLivresTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label70_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label71_Click(object sender, EventArgs e)
        {

        }

        private void label72_Click(object sender, EventArgs e)
        {

        }

        private void label73_Click(object sender, EventArgs e)
        {

        }

        private void label75_Click(object sender, EventArgs e)
        {

        }

        private void label76_Click(object sender, EventArgs e)
        {

        }

        private void label77_Click(object sender, EventArgs e)
        {

        }

        private void label78_Click(object sender, EventArgs e)
        {

        }

        private void label74_Click(object sender, EventArgs e)
        {

        }

        private void txbCommandesLivresNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabCommandesDvd_Click(object sender, EventArgs e)
        {

        }

        private void lblEtapeSuiviDvd_Click(object sender, EventArgs e)
        {

        }

        private void label96_Click(object sender, EventArgs e)
        {

        }

        private void cbxCommandeDvdLibelleSuivi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dtpCommandeDvd_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdMontant_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdNbExemplaires_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label91_Click(object sender, EventArgs e)
        {

        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void label93_Click(object sender, EventArgs e)
        {

        }

        private void label94_Click(object sender, EventArgs e)
        {

        }

        private void dgvCommandesDvd_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdSynopsis_TextChanged(object sender, EventArgs e)
        {

        }

        private void label82_Click(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdDuree_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdCheminImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdPublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdRealisateur_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeDvdTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label83_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label84_Click(object sender, EventArgs e)
        {

        }

        private void label85_Click(object sender, EventArgs e)
        {

        }

        private void label86_Click(object sender, EventArgs e)
        {

        }

        private void label87_Click(object sender, EventArgs e)
        {

        }

        private void label88_Click(object sender, EventArgs e)
        {

        }

        private void label89_Click(object sender, EventArgs e)
        {

        }

        private void label90_Click(object sender, EventArgs e)
        {

        }

        private void txbCommandesDvdNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void label81_Click(object sender, EventArgs e)
        {

        }

        private void tabCommandesRevues_Click(object sender, EventArgs e)
        {

        }

        private void gbxInfosCommandeRevue_Enter(object sender, EventArgs e)
        {

        }

        private void dtpCommandeRevueAbonnementFin_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dtpCommandeRevue_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueMontant_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueNumero_TextChanged(object sender, EventArgs e)
        {

        }

        private void label107_Click(object sender, EventArgs e)
        {

        }

        private void label108_Click(object sender, EventArgs e)
        {

        }

        private void label109_Click(object sender, EventArgs e)
        {

        }

        private void label110_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void label106_Click(object sender, EventArgs e)
        {

        }

        private void dgvAbonnementsRevue_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txbCommandesRevueNumRecherche_TextChanged(object sender, EventArgs e)
        {

        }

        private void label105_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label95_Click(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueCheminImage_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueRayon_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevuePublic_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueGenre_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueDelai_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevuePeriodicite_TextChanged(object sender, EventArgs e)
        {

        }

        private void txbCommandeRevueTitre_TextChanged(object sender, EventArgs e)
        {

        }

        private void label97_Click(object sender, EventArgs e)
        {

        }

        private void label98_Click(object sender, EventArgs e)
        {

        }

        private void label99_Click(object sender, EventArgs e)
        {

        }

        private void label100_Click(object sender, EventArgs e)
        {

        }

        private void label101_Click(object sender, EventArgs e)
        {

        }

        private void label102_Click(object sender, EventArgs e)
        {

        }

        private void label103_Click(object sender, EventArgs e)
        {

        }
    }
}
