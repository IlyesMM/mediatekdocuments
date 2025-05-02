using System;
using MediaTekDocuments.model;
using System.Windows.Forms;
using MediaTekDocuments.authcontroller;


namespace MediaTekDocuments.view
{
    public partial class VueLogin : Form
    {
        private readonly AuthentificationController controller;

        public VueLogin()
        {
            InitializeComponent();
            this.controller = new AuthentificationController();


            this.txbPassword.KeyPress += txbPasswordEnter;
        }

        private void txbPasswordEnter(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {
                btnConnexion.PerformClick();
            }
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            string login = txbLogin.Text;
            string password = txbPassword.Text;

            if (!txbLogin.Text.Equals("") && !txbPassword.Text.Equals(""))
            {
                Service service = controller.GetUtilisateur(login, password);

                if (service == null)
                {
                    MessageBox.Show("Erreur d'authentification", "Alerte");
                    txbPassword.Text = "";
                }
                else if(service.Libelle == "culture")
                {
                    MessageBox.Show("Droits d'accès manquants.", "Alerte");
                    Application.Exit();
                }
                else
                {
                    MessageBox.Show("Connecté", "Information");
                    this.Hide();
                    VuePrincipale frmMediatek = new VuePrincipale(service);
                    frmMediatek.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Les 2 champs sont obligatoires");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FrmAuthentification_Load(object sender, EventArgs e)
        {

        }

        private void lblUtilisateur_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
