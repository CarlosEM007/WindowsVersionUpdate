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
                Verify.EnableUpdateServices();

                UpdateCollection updates = InstallUpdate.DownloadUpdates();

                if (updates.Count > 0)
                {
                    InstallUpdate.InstallUpdates(updates);
                }
            }
            else
            {
                // Aguarda 1 segundo antes de fechar
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }
    }
}
