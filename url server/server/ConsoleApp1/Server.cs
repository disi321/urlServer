using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Text.Encodings;
using System.Text.Json;

namespace server
{
    public class Server
    {
        private const int PORT = 3333;
        public const int BUFFER_SIZE = 2048;

        Database db;
        private TcpListener m_tcpListener;
        public Server()
        {
            m_tcpListener = new TcpListener(IPAddress.Any, PORT);
            db = new Database();
        }

        public void run()
        {
            m_tcpListener.Start();
            Console.WriteLine("Listening on port: " + PORT);

            while (true)
            {
                TcpClient client = m_tcpListener.AcceptTcpClient();
                Console.WriteLine("Tcp Client accepted");
                try
                {
                    Thread thread = new Thread(() => handleClient(ref client));
                    thread.Start();
                }
                catch (Exception)
                {

                    Console.WriteLine("client crushed");
                }
            }
        }
        void handleClient(ref TcpClient client)
        {
            NetworkStream ns = client.GetStream();

            byte[] buffer = new byte[BUFFER_SIZE];
            buffer = Encoding.Default.GetBytes("enter 1 to map new url\n" +
                                                "2 to get specific url by id\n" +
                                                "3 to get all mapping urls" +
                                                "enter option and then param");

            ns.Write(buffer, 0, buffer.Length);

            byte[] msg = new byte[BUFFER_SIZE];
            ns.Read(msg, 0, msg.Length);
            string s = Encoding.ASCII.GetString(msg.ToArray()).Trim('\0');
            Console.WriteLine("msg is: " + s);
                
            Request req = JsonSerializer.Deserialize<Request>(s);


            buffer = Encoding.Default.GetBytes(handleRequest(req));
            ns.Write(buffer, 0, buffer.Length);
        }
        string handleRequest(Request req)
        {
            switch (req.m_id)
            {
                case 1:
                    return db.add(req.m_param);
                case 2:
                    return db.getOne(req.m_param);
                case 3:
                    return db.getAll();
                default:
                    return "wrong input";
            }
        }
    }
    class Request
    {
        public int m_id { get; set; }
        public string m_param { get; set; }
        //public Request(int id, string parameter)
        //{
        //    m_id = id;
        //    m_param = parameter;
        //}
    }
}
