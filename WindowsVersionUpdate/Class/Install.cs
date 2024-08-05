using WUApiLib;
using System.Diagnostics;

namespace WindowsVersionUpdate.Class
{
    public static class InstallUpdate
    {
        public static UpdateCollection DownloadUpdates()
        {
            // Search for updates in the Microsoft database
            UpdateSession updateSession = new UpdateSession();

            // Search Updates
            IUpdateSearcher searchUpdates = updateSession.CreateUpdateSearcher();

            // Filter for not installed and not hidden updates
            ISearchResult updateSearchResult = searchUpdates.Search("IsInstalled=0 AND IsHidden=0");

            // Store all updates to download
            UpdateCollection updateCollection = new UpdateCollection();
            int updateCount = updateSearchResult.Updates.Count;

            // Accept EULA for each update if not already accepted and not a specific driver
            Parallel.For(0, updateCount, i =>
            {
                IUpdate update = updateSearchResult.Updates[i];
                if (!IsSpecificDriver(update))
                {
                    if (!update.EulaAccepted)
                    {
                        update.AcceptEula();
                    }
                    lock (updateCollection)
                    {
                        updateCollection.Add(update);
                    }
                }
            });

            // Ensure there are updates to download
            if (updateCollection.Count > 0)
            {
                // Downloader object
                IUpdateDownloader downloader = updateSession.CreateUpdateDownloader();
                downloader.Updates = updateCollection;

                // Start the download process
                IDownloadResult downloadResult = downloader.Download();

                // Collection for updates that were downloaded successfully
                UpdateCollection installCollection = new UpdateCollection();
                int downloadCount = updateCollection.Count;

                for (int i = 0; i < downloadCount; i++)
                {
                    installCollection.Add(updateCollection[i]);
                }

                return installCollection;
            }
            else
            {
                return new UpdateCollection();
            }
        }

    }

    public static void InstallUpdates(UpdateCollection downloadedUpdates)
        {
            // Create a new update session
            UpdateSession updateSession = new UpdateSession();

            // Create the update installer
            IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();

            // Start the installation process
            try
            {
                // Aceita o EULA para cada atualização se ainda não for aceito
                foreach (IUpdate update in downloadedUpdates)
                {
                    if (!update.EulaAccepted)
                    {
                        update.AcceptEula();
                    }
                }

                // Atribui a coleção de atualizações baixadas ao instalador
                updateInstaller.Updates = downloadedUpdates;

                IInstallationResult installationResult = updateInstaller.Install();

                // Verifica se a instalação foi bem-sucedida
                if (installationResult.ResultCode == OperationResultCode.orcSucceeded)
                {
                    // Verifica se é necessário reiniciar o sistema
                    if (installationResult.RebootRequired)
                    {
                        Console.WriteLine("Reinicialização necessária...");
                        Process.Start("ShutDown", "/r");
                    }
                }
                else
                {
                    Console.WriteLine("A instalação das atualizações falhou. Código de resultado: " + installationResult.ResultCode);
                    System.Threading.Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro durante a instalação das atualizações: " + ex.Message);
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }
        static bool IsSpecificDriver(IUpdate update)
        {
            // Verifica se o título ou descrição contém palavras-chave que identificam drivers específicos
            string[] keywords = { "driver", "nvidia", "intel", "graphic", "amd" }; // Adicione as palavras-chave conforme necessário
            foreach (string keyword in keywords)
            {
                if (update.Title.ToLower().Contains(keyword) || update.Description.ToLower().Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
