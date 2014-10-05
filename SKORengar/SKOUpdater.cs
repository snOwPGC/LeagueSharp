using System;
using LeagueSharp;
using System.ComponentModel;

namespace SKORengar
{

   internal class SKOUpdater
    {

        internal static bool isInitialized;

        private static void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Updater updater = new Updater("https://github.com/SKOBoL/LeagueSharp/raw/master/Version/SKORengar/SKORengar.version", "https://github.com/SKOBoL/LeagueSharp/raw/master/Build/SKORengar/SKORengar.exe", 7);
            if (!updater.NeedUpdate)
            {
                Game.PrintChat("<font color='#1d87f2'> SKORengar: Most recent version loaded!");
            }
            else
            {
                Game.PrintChat("<font color='#1d87f2'> SKORengar: Updating ...");
                if (updater.Update())
                {
                    Game.PrintChat("<font color='#1d87f2'> SKORengar: Update complete, reload please.");
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
            Game.PrintChat("<font color='#1d87f2'> SKORengar loaded!");
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(bgw_DoWork);
            backgroundWorker.RunWorkerAsync();
        }
    }
}
