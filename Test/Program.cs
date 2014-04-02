using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using System.Net.Security;
using System.Reflection;

using OpenPop.Pop3;
using OpenPop.Mime;

class UserDetails
{
    public string Name { get; set; }
    public string Passwod { get; set; }
    public string PopServer { get; set; }
    public string SmtpServer { get; set; }
}

namespace Test
{
    class Program
    {
        public string CRLF = "\r\n";

        static void Main(string[] args)
        {
            TcpClient c = new TcpClient("pop.yandex.ru", 110);
            NetworkStream s = c.GetStream();
            StreamReader r = new StreamReader(s);

            string tmp = null;
            byte[] data = null;
            
            while(true)
            {
                tmp = Console.ReadLine() + "\r\n";
                data = Encoding.ASCII.GetBytes(tmp);
                s.Write(data, 0, data.Length);
                Console.WriteLine(r.ReadLine());
            }
          
          /*  SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.yandex.ru";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("nazarevic@yandex.ru", "Sharafin_12");

            MailMessage mm = new MailMessage("nazarevic@yandex.ru", "bnazariy@gmail.com", "Hi", "How are you?");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);    */
            if (!Directory.Exists("./Mail"))
            {
                Directory.CreateDirectory("./Mail");
            }
            UserDetails user = new UserDetails() { Name = "nazarevic", Passwod = "Sharafin_12", PopServer = "pop.yandex.ru" };
            GetUserMessages(user);
           
        }
        static IEnumerable<Message> GetUserMessages(UserDetails user)
        {
            using(Pop3Client client = new Pop3Client())
            {
                client.Connect(user.PopServer, 110, false);
                client.Authenticate(user.Name, user.Passwod);
                int messageCount = client.GetMessageCount();

                List<Message> allMessages = new List<Message>(messageCount);
                Message message = null;
                for (var i = messageCount; i > 0; i--)
                {
                    var headers = client.GetMessageHeaders(i);
                    string hash = (headers.MessageId + user.PopServer).GetHashCode().ToString();
                    Console.WriteLine(hash);
                    string filename = "./Mail/" + hash + ".emf";
                    if(!File.Exists(filename))
                    {
                        Console.WriteLine("Loading");
                        message = client.GetMessage(i);
                        message.Save(File.Create(filename));
                    }
                    else
                    {
                        message = Message.Load(new FileInfo(filename));
                    }
                }

                    return allMessages;
            }
        }
        public static void ShowProperties(object o, string indent = "-")
        {
            if (o == null) { return; }
            Type t = o.GetType();
            PropertyInfo[] prs = t.GetProperties();
            Assembly a = t.Assembly;
            object tmp = null;
            foreach (PropertyInfo p in prs)
            {
                tmp = p.GetValue(o);

                if (p.PropertyType.Assembly == t.Assembly)
                {
                    Console.WriteLine("{0}{1} {2}", indent, p.Name, tmp);
                    ShowProperties(tmp, indent + '-');
                }
                else
                {
                    Console.WriteLine("{0}{1} {2}", indent, p.Name, tmp);
                }
            }
        }

    }
}
