using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsVersionUpdate.Class
{
    public static class Verify
    {
        public static bool NotInstalledUpdates()
        {
            //Session var
            UpdateSession UpdateSession = new UpdateSession();

            //Search Updates
            IUpdateSearcher UpdateSearchResult = UpdateSession.CreateUpdateSearcher();

            //Defines that the search for updates must be done over the internet as well
            UpdateSearchResult.Online = false; //true

            //filter for a Not Installed and Not Hidden update        
            ISearchResult SearchResults = UpdateSearchResult.Search("IsInstalled=0 AND IsHidden=0");

            Console.WriteLine("Updates Disponíveis: \n");

            if (SearchResults.Updates.Count > 0)
            {
                foreach (IUpdate x in SearchResults.Updates)
                {
                    Console.WriteLine("----------------------------------");
                    Console.WriteLine($"Titulo: {x.Title}");
                    Console.WriteLine($"Descrição: {x.Description}");
                    Console.WriteLine($"Instalada: {x.IsInstalled}");
                    Console.WriteLine($"Obrigatório: {x.IsMandatory}\n");
                }
                return true;
            }
            else
            {

                Console.WriteLine("Não possui Atualizações");
                Console.ReadKey();
                return false;
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
}
