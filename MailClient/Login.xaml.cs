using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using OpenPop;
using OpenPop.Pop3;

namespace MailClient
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var login = _userName.Text;
            var pass = _password.Password;
            var pop = _popServer.Text;
            var smtp = _smtpServer.Text;

            UserDetails user = new UserDetails() { Name = login, Passwod = pass, PopServer = pop, SmtpServer = smtp };
            try
            {

                    using (Pop3Client client = new Pop3Client())
                    {
                        client.Connect(pop, 110, false);
                    }

new MainWindow(user).Show();
            Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void EndLogin()
        {
            
        }
    }
}
