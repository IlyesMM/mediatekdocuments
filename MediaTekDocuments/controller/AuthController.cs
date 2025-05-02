using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments.authcontroller
{

    public class AuthentificationController
    {
        private readonly Access access;

        public AuthentificationController()
        {
            access = Access.GetInstance();
        }

        public static string GetSha256Hex(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); 
                }
                return sb.ToString();
            }
        }

        public Service GetUtilisateur(string login, string password)
        {

            password = GetSha256Hex(password);

            //string hashedPassword = GetSha256Hex("paul");
            Utilisateur utilisateur = access.GetUtilisateur(login, password );

            if(utilisateur == null)
            {
                MessageBox.Show("Compte non existant.");
            }
            else{

            if (utilisateur != null && utilisateur.Password.Equals(password))
            {
                
                return new Service(utilisateur.IdService, utilisateur.Libelle);
                   
            }

            }
            return null;
        }
    }
}
