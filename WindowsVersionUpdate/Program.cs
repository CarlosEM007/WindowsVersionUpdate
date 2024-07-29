using WindowsVersionUpdate.Class;
using WUApiLib;

class Program
{
    static void Main()
    {
        //Check if it's Monday
        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        {
            //Check for updates
            if (Verify.NotInstalledUpdates())
            {
                Console.WriteLine("Possui atualizações!");
                Console.Write("Aperte qualquer tecla para continuar...");
                Console.ReadLine();

                Verify.EnableUpdateServices();

                UpdateCollection updates = InstallUpdate.DownloadUpdates();

                if (updates.Count > 0)
                {
                    InstallUpdate.InstallUpdates(updates);
                }
            }
            else
            {
                Console.WriteLine("Não possui atualizações!");
                Console.Write("Aperte qualquer tecla para continuar...");
                Console.ReadLine();

                // Aguarda 1 segundo antes de fechar
                System.Threading.Thread.Sleep(10000);
                Environment.Exit(0);
            }
        }
    }
}
