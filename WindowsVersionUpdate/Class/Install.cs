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

            // Accept EULA for each update if not already accepted
            for (int i = 0; i < updateSearchResult.Updates.Count; i++)
            {
                IUpdate update = updateSearchResult.Updates[i];
                if (!update.EulaAccepted)
                {
                    update.AcceptEula();
                }
                updateCollection.Add(update);
            }

            // Ensure there are updates to download
            if (updateCollection.Count > 0)
            {
                // Collection for updates that will be downloaded
                UpdateCollection downloadCollection = new UpdateCollection();

                // Downloader object
                IUpdateDownloader downloader = updateSession.CreateUpdateDownloader();

                // Add all available updates to download collection
                for (int i = 0; i < updateCollection.Count; i++)
                {
                    downloadCollection.Add(updateCollection[i]);
                }

                downloader.Updates = downloadCollection;
                Console.WriteLine("Baixando atualizações... Isso pode levar alguns minutos.");

                // Start the download process
                IDownloadResult downloadResult = downloader.Download();

                // Collection for updates that were downloaded successfully
                UpdateCollection installCollection = new UpdateCollection();
                for (int i = 0; i < downloadCollection.Count; i++)
                {
                    installCollection.Add(downloadCollection[i]);
                }

                Console.WriteLine("Download concluído.");

                Console.WriteLine($"Disponiveis: {installCollection.Count}\n");

                return installCollection;
            }
            else
            {
                Console.WriteLine("Nenhuma atualização disponível para download.");
                return new UpdateCollection();
            }
        }

        public static void InstallUpdates(UpdateCollection downloadedUpdates)
        {
            // Create a new update session
            UpdateSession updateSession = new UpdateSession();

            // Create the update installer
            IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();

            Console.WriteLine("Iniciando a instalação das atualizações...\n");

            // Start the installation process
            try
            {
                // Aceita o EULA para cada atualização se ainda não for aceito
                foreach (IUpdate update in downloadedUpdates)
                {
                    if (!update.EulaAccepted)
                    {
                        update.AcceptEula();
                        Console.WriteLine($"EULA aceito para a atualização: {update.Title}\n");
                    }
                }

                // Atribui a coleção de atualizações baixadas ao instalador
                updateInstaller.Updates = downloadedUpdates;

                Console.WriteLine("Iniciando a instalação das atualizações...");
                IInstallationResult installationResult = updateInstaller.Install();

                // Verifica se a instalação foi bem-sucedida
                if (installationResult.ResultCode == OperationResultCode.orcSucceeded)
                {
                    Console.WriteLine("Atualizações instaladas com sucesso.");

                    // Verifica se é necessário reiniciar o sistema
                    if (installationResult.RebootRequired)
                    {
                        Console.WriteLine("É necessário reiniciar o sistema para concluir a instalação das atualizações.");
                        Process.Start("ShutDown", "/r");
                    }
                }
                else
                {
                    Console.WriteLine("A instalação das atualizações falhou. Código de resultado: " + installationResult.ResultCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro durante a instalação das atualizações: " + ex.Message);
            }
        }
    }
}
