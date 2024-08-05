using System;
using WindowsVersionUpdate.Class;
using WUApiLib;

class Program
{
    static async Task Main()
    {
        if (DateTime.Now.DayOfWeek != DayOfWeek.Monday)
        {
            // Fecha imediatamente se não for segunda-feira
            Console.WriteLine("Hoje não é segunda-feira. O programa será fechado.");
            return;
        }

        Console.WriteLine("Buscando atualizações...\n");

        if (Verify.NotInstalledUpdates())
        {
            Console.WriteLine("Atualizações encontradas!\n");

            Verify.EnableUpdateServices();

            Console.WriteLine("Instalando atualizações... (Isso pode levar alguns minutos!)\n");
            UpdateCollection updates = InstallUpdate.DownloadUpdates();

            if (updates.Count > 0)
            {
                await InstallUpdate.InstallUpdatesAsync(updates);
            }
        }
        else
        {
            Console.WriteLine("Sem atualizações!");
        }
    }
}
