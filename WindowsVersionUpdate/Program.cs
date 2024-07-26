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
                Console.WriteLine("Ativando o serviço de atualização...");
                Verify.EnableUpdateServices();

                Console.WriteLine("Baixando e instalando atualizações...");
                UpdateCollection updates = InstallUpdate.DownloadUpdates();

                if (updates.Count > 0)
                {
                    InstallUpdate.InstallUpdates(updates);
                }
                else
                {
                    Console.WriteLine("Nenhuma atualização disponível para instalação.");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma atualização disponível.");
                // Aguarda 1 segundo antes de fechar
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }
    }
}
