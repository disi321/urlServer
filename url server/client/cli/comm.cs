using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.Text.Json;

namespace Client
{
    public sealed class Communicator
    {
        private TcpClient m_tcp;
        private NetworkStream m_stream;
        private const int DST_PORT = 3333;
        private const int BUFFER_SIZE = 2048;
        public Communicator()
        {
            m_tcp = new TcpClient(GetHostName(), DST_PORT);
            m_stream = m_tcp.GetStream();
        }
        ~Communicator()
        {
            m_tcp.Close();
        }
        private string GetHostName()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var address in host.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("Server IP is: " + address);
                    return address.ToString();
                }
            }

            return "127.0.0.1";
        }

        public void Talk(byte[] data = null)
        {
            //m_stream.Write(m_token.Concat(data).ToArray(), 0, data.Length + m_token.Length);

            byte[] buffer = new byte[BUFFER_SIZE];
            m_stream.Read(buffer, 0, BUFFER_SIZE);
            Console.WriteLine(Encoding.ASCII.GetString(buffer.ToArray()));

            int id = Convert.ToInt32(Console.ReadLine());
            string param;
            if (id == 1)
            {
                Console.WriteLine("enter url");
                param = Console.ReadLine();
            }
            else if (id == 2)
            {
                Console.WriteLine("enter id");
                param = Console.ReadLine();
            }
            else
                param = "";

            buffer = Encoding.Default.GetBytes(JsonSerializer.Serialize( new Request(id, param)));

            m_stream.Write(buffer, 0, buffer.Length);

            buffer = new byte[BUFFER_SIZE];
            m_stream.Read(buffer, 0, BUFFER_SIZE);
            Console.WriteLine(Encoding.ASCII.GetString(buffer.ToArray()));
        }
    }

    class Request
    {
        public int m_id { get; set; }
        public string m_param { get; set; }
        public Request(int id, string parameter)
        {
            m_id = id;
            m_param = parameter;
        }
    }
}
