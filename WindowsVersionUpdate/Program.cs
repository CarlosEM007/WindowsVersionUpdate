using WindowsVersionUpdate.Class;
using WUApiLib;

class Program
{
    static void Main()
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
        {
            // Verifica se existem atualizações não instaladas
            if (Verify.NotInstalledUpdates())
            {
                Console.WriteLine("Ativando o serviço de atualização...");
                Verify.EnableUpdateServices();

                Console.WriteLine("Baixando e instalando atualizações...");
                UpdateCollection updates = Install.DownloadUpdates();
                if (updates.Count > 0)
                {
                    Install.InstallUpdates(updates);
                }
                else
                {
                    Console.WriteLine("Nenhuma atualização disponível para instalação.");
                }
            }
            else
            {
                Console.WriteLine("Nenhuma atualização disponível.");
            }
        }
        else
        {
            Console.WriteLine("O programa só é executado às quintas-feiras.");
        }

        // Aguarda 1 segundo antes de fechar
        System.Threading.Thread.Sleep(1000);
        Environment.Exit(0);
    }
}
