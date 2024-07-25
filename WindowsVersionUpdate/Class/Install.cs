﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsVersionUpdate.Class
{
    public static class Install
    {
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
                Console.WriteLine("Instalando Atualizações... Isso pode levar alguns minutos.");


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
    }
}
