using System;
using WUApiLib;

namespace WindowsVersionUpdate.Class
{
    public sealed class Verify
    {
        public static bool NotInstalledUpdates()
        {
            // Create a session and search for updates
            IUpdateSearcher updateSearcher = new UpdateSession().CreateUpdateSearcher();
            updateSearcher.Online = true; // Ensure search is performed online

            // Search for updates that are not installed and not hidden
            ISearchResult searchResults = updateSearcher.Search("IsInstalled=0 AND IsHidden=0");

            // Return true if updates are found, otherwise false
            return searchResults.Updates.Count > 0;
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
}
