﻿using WUApiLib;
using WindowsVersionUpdate.Class;

class Program
{
    static void Main()
    {
        if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
        {
            //Check for updates to make
            if (Verify.NotInstalledUpdates())
            {
                Verify.EnableUpdateServices();

                Install.InstallUpdates(Install.DownloadUpdates());
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
}
