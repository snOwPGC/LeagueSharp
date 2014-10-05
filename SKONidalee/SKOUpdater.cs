using System;
using LeagueSharp;
using System.ComponentModel;

namespace SKONidalee
{

   internal class SKOUpdater
    {

        internal static bool isInitialized;

        private static void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Updater updater = new Updater("https://github.com/SKOBoL/LeagueSharp/raw/master/Version/SKONidalee/SKONidalee.version", "https://github.com/SKOBoL/LeagueSharp/raw/master/Build/SKONidalee/SKONidalee.exe", 7);
            if (!updater.NeedUpdate)
            {
                Game.PrintChat("<font color='#1d87f2'> SKONidalee: Most recent version ({0}) loaded!", new object[] { 7 });
            }
            else
            {
                Game.PrintChat("<font color='#1d87f2'> SKONidalee: Updating ...");
                if (updater.Update())
                {
                    Game.PrintChat("<font color='#1d87f2'> SKONidalee: Update complete, reload please.");
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
            Game.PrintChat("<font color='#1d87f2'> SKONidalee loaded!");
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(bgw_DoWork);
            backgroundWorker.RunWorkerAsync();
        }
    }
}
