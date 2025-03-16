using System;
using System.Threading;

namespace Sport_Retake_scheuer
{
    class main
    {
        static void Main(string[] args)
        {
            int port = 10001;
            HttpServer meinServer = new HttpServer(port);
            meinServer.Start();
            Console.WriteLine("Press 'q' to stop the server...");
            while (true)
            {
                if (Console.ReadKey(true).KeyChar == 'q')
                {
                    meinServer.Stop();
                    break;
                }
            }
            Console.WriteLine("Server stopped. Tschüss!");
        }
    }
}
