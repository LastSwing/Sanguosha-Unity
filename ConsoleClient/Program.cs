using CommonClassLibrary;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;

namespace ConsoleClient
{
    class Program
    {
        static byte[] bytes;
        static void Main(string[] args)
        {

            var myData = new MyData
            {
                Description = PacketDescription.Client2Hall,
                Protocol = protocol.Login ,
                Body = new List<string> { "damnyou", "damnyou1" }
            };

            bytes =  PacketTranslator.data2byte(myData, PacketTranslator.GetTypeString(TransferType.TypeLogin));
            Console.WriteLine(System.Text.Encoding.Default.GetString(bytes));


            OnConnect2Server("127.0.0.1", 8234);
        }

        private static async void OnConnect2Server(string ip, int port)
        {
            EasyClient client = new EasyClient();
            client.Initialize(new MsgFixedHeaderReceiveFilter(), OnReceiveHandler);         //收到信息

            bool result = await  client.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));

            client.Send(bytes);

            Console.ReadKey();
        }

        private static void OnReceiveHandler(MsgBinaryRequestInfo obj)
        {
        }
    }
}
