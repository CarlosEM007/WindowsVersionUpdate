using WUApiLib;

class Program
{
    static void Main()
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
        {
            //Check for updates to make
            if (NotInstalledUpdates())
            {
                //Ace
                EnableUpdateServices();

                InstallUpdates(DownloadUpdates());
            }
            else
            { 
                Console.WriteLine("Nenhuma atualização Disponível");

                //Wait 1 second to close
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

        //Defines that the search for updates must be done over the internet as well
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

        //filter for a Not Installed and Not hidden updates
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

            //Download object
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
            // Create a new update session
            UpdateSession updateSession = new UpdateSession();

            // Create the update installer
            IUpdateInstaller updateInstaller = updateSession.CreateUpdateInstaller();

            // Assigns the collection of downloaded updates to the installer
            updateInstaller.Updates = downloadedUpdates;

            Console.WriteLine("Iniciando a instalação das atualizações...");

            // Start the installation process
            IInstallationResult installationResult = updateInstaller.Install();

            // Checks whether the installation was successful
            if (installationResult.ResultCode == OperationResultCode.orcSucceeded)
            {
                Console.WriteLine("Atualizações instaladas com sucesso.");

                // Checks if it is necessary to restart the system
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
