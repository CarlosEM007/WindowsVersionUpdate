using System;
using WUApiLib;

class Program
{
    static void Main()
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
        {
            if (NotInstalledUpdates())
            {
                EnableUpdateServices();

            }
        }
        else
        {
            Environment.Exit(0);
        }
        
    }

    //Checks for all required updates
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

        if (SearchResults != null)
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
    
    public static void InstallUpdates()
    {
        UpdateSession updateSession = new UpdateSession();

        UpdateInstaller UpdateInstaller = updateSession.CreateUpdateInstaller() as UpdateInstaller;

        UpdateInstaller.Updates = Downloaded
    }

    public static UpdateCollection DownloadUpdates()
    {

        //Search for updates in the Microsoft database

        //Session var
        UpdateSession UpdateSession = new UpdateSession();

        //Search Updates
        IUpdateSearcher SearchUpdates = UpdateSession.CreateUpdateSearcher();

        //filter for a Not Installed and Not available update
        ISearchResult UpdateSearchResult = SearchUpdates.Search("IsInstalled=0 and IsPresent=0");

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

    public static void EnableUpdateServices()
    {
        IAutomaticUpdates updates = new AutomaticUpdates();
        if (!updates.ServiceEnabled)
        {
            updates.EnableService();
        }
    }


}
