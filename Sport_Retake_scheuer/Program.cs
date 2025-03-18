
using Sport_Retake_scheuer.Config;

namespace Sport_Retake_scheuer
{
    class main
    {
        static void Main(string[] args)
        {
            int port = 10001;
            HttpServer meinServer = new HttpServer(port);
            meinServer.Start();
            DatabaseConnection.TestConnection();
            DatabaseConnection.SetupTables();
            Console.WriteLine("Press 'q' to stop the server...");
            while (true)
            {
               // Console.WriteLine("Waiting for a request..."); anm. war ein kurzer test 
                if (Console.ReadKey(true).KeyChar == 'q')
                {
                    meinServer.Stop();
                    DatabaseConnection.DropTables();
                    break;
                }
            }
            Console.WriteLine("Server stopped. Tschüss!");
        }
    }
}
