using CasaVPN_install_GUI;
using System;
using System.DirectoryServices.AccountManagement;
using System.Windows.Forms;

namespace UserCredentialsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (ValidateCredentials(username, password))
            {
                errorLabel.Text = "";  // Clear any previous error message
                Program.ExecuteComputerNameChanger(username, password);
                Program.AlivateLocalAdmin(username, password);
                this.Close();
            }
            else
            {
                errorLabel.Text = "Invalid credentials. Please try again.";
            }
        }

        private bool ValidateCredentials(string username, string password)
        {
            string domain = "casa.net";

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain, domain, username, password))
                {
                    return context.ValidateCredentials(username, password);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void errorLabel_Click(object sender, EventArgs e)
        {

        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBoxBackground_Click(object sender, EventArgs e)
        {

        }
    }
}
