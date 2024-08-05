using WindowsVersionUpdate.Class;
using WUApiLib;

class Program
{
    static void Main()
    {
        //Check if it's Monday
        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        {
            Console.WriteLine("Buscando atualizações...\n");
            //Check for updates
            if (Verify.NotInstalledUpdates())
            {
                Console.WriteLine("Atualizações encontradas!\n");

                Verify.EnableUpdateServices();

                Console.WriteLine("Instalando atualizações... (Isso pode levar alguns minutos!)\n");
                UpdateCollection updates = InstallUpdate.DownloadUpdates();

                if (updates.Count > 0)
                {

                    InstallUpdate.InstallUpdates(updates);
                }
            }
            else
            {
                Console.WriteLine("Sem atualizações!");
                // Aguarda 1 segundo antes de fechar
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }      
        }
    }
}
