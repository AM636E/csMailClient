using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Test
{
//    class MPop2Response
    class MPop3Client
    {
        private TcpClient _server;
        private NetworkStream _serverStream;       
        private StreamReader _reader;

        public MPop3Client(string server, int port = 110)
        {
            _server = new TcpClient(server, port);
            _serverStream = _server.GetStream();
            _reader = new StreamReader(_serverStream);
        }

        public void SendCommand(string command)
        {
            if(command.EndsWith("\r\n") == false)
            {
                command += "\r\n";
            }
            byte[] tmp = Encoding.ASCII.GetBytes(command);
 
            _serverStream.Write(tmp, 0, tmp.Length);
        }

        public string GetResponse()
        {
            return _reader.ReadLine();
        }

        public void Login(string login, string pass)
        {
            try
            {
                SendCommand(String.Format("USER {0}", login));
                SendCommand("STAT");
                Console.WriteLine(GetResponse());
                Console.WriteLine(GetResponse());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
