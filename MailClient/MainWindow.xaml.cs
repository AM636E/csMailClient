using System;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net.Mime;
using System.Net.Mail;

using OpenPop.Common;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace MailClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserDetails _user = null;
        ObservableCollection<MailMessage> _messages = new ObservableCollection<MailMessage>();

        public static readonly DependencyProperty MessagesProperty = 
        DependencyProperty.Register("Messages", typeof(ObservableCollection<MailMessage>), typeof(MainWindow));

        public ObservableCollection<MailMessage> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                //SetValue(MessagesProperty, value);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            if (!Directory.Exists("./Mail"))
            {
                Directory.CreateDirectory("./Mail");
            }
        }

        public MainWindow(UserDetails user)
            :this()
        {
            _user = user;
            this.DataContext = this;
            Messages = new ObservableCollection<MailMessage>(GetUserMessages(user));
            foreach(var message in Messages)
            {
                _dgMessages.Items.Add(message.Subject);
            }
        }

        IEnumerable<MailMessage> GetUserMessages(UserDetails user)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(user.PopServer, 110, false);
                client.Authenticate(user.Name, user.Passwod);
                int messageCount = client.GetMessageCount();

                List<MailMessage> allMessages = new List<MailMessage>(messageCount);
                Message message = null;
                for (var i = messageCount; i > 0; i--)
                {
                    var headers = client.GetMessageHeaders(i);
                    // Create unique name for message.
                    // Unique name consist of message unique(in server) id and pop server adress.
                    string hash = (headers.MessageId + user.PopServer).GetHashCode().ToString();
                    string filename = "./Mail/" + hash + ".emf";
                    // If message isn't in file
                    // Load it from server
                    if (!File.Exists(filename))
                    {
                        message = client.GetMessage(i);
                        message.Save(File.Create(filename));
                    }
                    else
                    {
                        message = Message.Load(new FileInfo(filename));
                    }
                    allMessages.Add(message.ToMailMessage());
                }

                return allMessages;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
