using WUApiLib;

class Program
{
    static void Main()
    {

        if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
        {
            if (NotInstalledUpdates())
            {
                
                EnableUpdateServices();

                //InstallUpdates(DownloadUpdates());

                Console.WriteLine("\nPrecione qualquer tecla para continuar");
                Console.ReadLine();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Nenhuma atualização Disponível");
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
        }
        else
        {
            Environment.Exit(0);
        }
    }

    public static bool NotInstalledUpdates()
    {
        //Session var
        UpdateSession UpdateSession = new UpdateSession();

        //Search Updates
        IUpdateSearcher UpdateSearchResult = UpdateSession.CreateUpdateSearcher();

        UpdateSearchResult.Online = true;

        //filter for a Not Installed and Not Hidden update        
        ISearchResult SearchResults = UpdateSearchResult.Search("IsInstalled=0 AND IsHidden=0");

        Console.WriteLine("Updates Disponíveis: \n");

        if (SearchResults.Updates.Count > 0)
        {
            foreach (IUpdate x in SearchResults.Updates)
            {
                Console.WriteLine(x.Title);
            }
            return true;
        }
        else
        {
            Console.WriteLine("Não possui Atualizações");
            return false;
        }
    }

    public static UpdateCollection DownloadUpdates()
    {

        //Search for updates in the Microsoft database

        //Session var
        UpdateSession UpdateSession = new UpdateSession();

        //Search Updates
        IUpdateSearcher SearchUpdates = UpdateSession.CreateUpdateSearcher();

        //filter for a Not Installed and Not available update
        ISearchResult UpdateSearchResult = SearchUpdates.Search("IsInstalled=0 AND IsHidden=0");

        //Store all updates to Download
        UpdateCollection UpdateCollection = new UpdateCollection();

        //Accept Eula code for each update
        for (int i = 0; i < UpdateSearchResult.Updates.Count; i++)
        {
            IUpdate Updates = UpdateSearchResult.Updates[i];
            if (Updates.EulaAccepted == false)
            {
                Updates.AcceptEula();
            }
            UpdateCollection.Add(Updates);
        }

        if (UpdateSearchResult.Updates.Count > 0)
        {
            UpdateCollection DownloadCollection = new UpdateCollection();
            UpdateDownloader Downloader = UpdateSession.CreateUpdateDownloader();

            for (int i = 0; i < UpdateCollection.Count; i++)
            {
                //Add all available to Download Collection
                DownloadCollection.Add(UpdateCollection[i]);
            }

            Downloader.Updates = DownloadCollection;
            Console.WriteLine("Downloading Updates... This may take several minutes.");


            IDownloadResult DownloadResult = Downloader.Download();
            UpdateCollection InstallCollection = new UpdateCollection();
            for (int i = 0; i < UpdateCollection.Count; i++)
            {
                if (DownloadCollection[i].IsDownloaded)
                {
                    InstallCollection.Add(DownloadCollection[i]);
                }
            }

            Console.WriteLine("Download Finished");
            return InstallCollection;
        }
        else
            return UpdateCollection;

    }

    public static void InstallUpdates(UpdateCollection downloadedUpdates)
    {
        if (downloadedUpdates.Count > 0)
        {
            // Cria uma nova sessão de atualização
            UpdateSession updateSession = new UpdateSession();
            // Cria o instalador de atualizações
            IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();

            // Atribui a coleção de atualizações baixadas ao instalador
            updateInstaller.Updates = downloadedUpdates;

            Console.WriteLine("Iniciando a instalação das atualizações...");

            // Inicia o processo de instalação
            IInstallationResult installationResult = updateInstaller.Install();

            // Verifica se a instalação foi bem-sucedida
            if (installationResult.ResultCode == OperationResultCode.orcSucceeded)
            {
                Console.WriteLine("Atualizações instaladas com sucesso.");

                // Verifica se é necessário reiniciar o sistema
                if (installationResult.RebootRequired)
                {
                    Console.WriteLine("É necessário reiniciar o sistema para concluir a instalação das atualizações.");
                }
            }
            else
            {
                Console.WriteLine("A instalação das atualizações falhou. Código de resultado: " + installationResult.ResultCode);
            }
        }
        else
        {
            Console.WriteLine("Não há atualizações para instalar.");
        }
    }

    public static void EnableUpdateServices()
    {
        IAutomaticUpdates updates = new AutomaticUpdates();
        if (!updates.ServiceEnabled)
        {
            updates.EnableService();
        }       
    }
}
