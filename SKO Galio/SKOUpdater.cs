using System;
using LeagueSharp;
using System.ComponentModel;

namespace SKO_Galio
{

   internal class SKOUpdater
    {
        private const int localversion = 2;

        internal static bool isInitialized;

        private static void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Updater updater = new Updater("https://github.com/SKOBoL/LeagueSharp/raw/master/Version/SKOGalio/SKOGalio.version", "https://github.com/SKOBoL/LeagueSharp/raw/master/Build/SKOGalio/SKOGalio.exe", 2);
            if (!updater.NeedUpdate)
            {
                Game.PrintChat("<font color='#1d87f2'> SKOGalio: Most recent version loaded!");
            }
            else
            {
                Game.PrintChat("<font color='#1d87f2'> SKOGalio: Updating ...");
                if (updater.Update())
                {
                    Game.PrintChat("<font color='#1d87f2'> SKOGalio: Update complete, reload please.");
                    return;
                }
            }
        }

        internal static void InitializeSKOUpdate()
        {
            isInitialized = true;
            UpdateCheck();
        }

        private static void UpdateCheck()
        {
            Game.PrintChat("<font color='#1d87f2'> SKOGalio loaded!");
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(bgw_DoWork);
            backgroundWorker.RunWorkerAsync();
        }
    }
}
