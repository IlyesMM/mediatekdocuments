using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class VueAlerteAbonnement : Form
    {
        private readonly BindingSource bdgAbonnementsAEcheance = new BindingSource();
        private readonly List<Abonnement> lesAbonnementsAEcheance = new List<Abonnement>();

        public VueAlerteAbonnement(FrmMediatekController controller)
        {
            InitializeComponent();
            lesAbonnementsAEcheance = controller.GetAbonnementsEcheance();
            RemplirAbonnementsAEcheance(lesAbonnementsAEcheance);
        }

        private void RemplirAbonnementsAEcheance(List<Abonnement> lesAbonnementsAEcheance)
        {
            bdgAbonnementsAEcheance.DataSource = lesAbonnementsAEcheance;
            dgvAbonnementsAEcheance.DataSource = bdgAbonnementsAEcheance;
            dgvAbonnementsAEcheance.Columns["dateCommande"].Visible = false;
            dgvAbonnementsAEcheance.Columns["montant"].Visible = false;
            dgvAbonnementsAEcheance.Columns["idRevue"].Visible = false;
            dgvAbonnementsAEcheance.Columns["id"].Visible = false;
            dgvAbonnementsAEcheance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvAbonnementsAEcheance.Columns[0].HeaderCell.Value = "Date de fin d'abonnement";
            dgvAbonnementsAEcheance.Columns[1].HeaderCell.Value = "Titre";
        }

        private void dgvAbonnementsAEcheance_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dgvAbonnementsAEcheance.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();
            switch (titreColonne)
            {
                case "Titre":
                    sortedList = lesAbonnementsAEcheance.OrderBy(o => o.Titre).ToList();
                    break;
                case "Date de fin d'abonnement":
                    sortedList = lesAbonnementsAEcheance.OrderBy(o => o.DateFinAbonnement).Reverse().ToList();
                    break;
            }
            RemplirAbonnementsAEcheance(sortedList);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvAbonnementsAEcheance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
