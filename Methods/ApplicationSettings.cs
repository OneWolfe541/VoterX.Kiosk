using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VoterX.SystemSettings.Models;
using VoterX.SystemSettings;
using VoterX.Logging;

namespace VoterX.Kiosk.Methods
{
    /* Static methods should not have any instance fields 
     * but only work with calculations or retreiving and storing data*/
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members

    // The AppSettings class simply passes values to and from the GlobalSettings object stored in App.xaml
    // The only purpose is to replace "((App)Application.Current).GlobalSettings" with "AppSettings"
    // An old fashion c++ CONST could perfrom the same function

    // Parameter assigments seem to be cleaner than function calls in this case
    //
    // the only difference is the removal of the parentheses.
    // Because no other parameters need to be passed this seems a much simpler syntax.
    // Function Call Example: AppSettings.SystemConfigs().SiteID = "1"; 
    // vs
    // Parameter Assignment Example: AppSettings.System.SiteId = "1"; 

    /// <summary>
    /// Static methods for easily referencing all the Global Application Settings
    /// </summary>
    public static class AppSettings
    {
        // Static reference to the Global Settings object from App.xaml.cs
        //private static SystemSettings globalSettings = ((App)Application.Current).GlobalSettings;

        public static bool DebugMode
        {
            get { return ((App)Application.Current).debugMode; }
            set { ((App)Application.Current).debugMode = value; }
        }

        //public static bool AbsenteeMode
        //{
        //    get { return ((App)Application.Current).absenteeMode; }
        //    set { ((App)Application.Current).absenteeMode = value; }
        //}

        public static int? SearchMethod
        {
            get { return ((App)Application.Current).SearchMethod; }
            set { ((App)Application.Current).SearchMethod = value; }
        }

        public static bool SettingsChanged
        {
            get { return ((App)Application.Current).settingsChanged; }
            set { ((App)Application.Current).settingsChanged = value; }
        }

        /// <summary>
        /// System Configuration Settings 
        /// </summary>
        /// <returns>SystemSettingsModel</returns>
        public static SystemSettingsController Global
        {
            get { return ((App)Application.Current).GlobalSettings; }
            private set { ((App)Application.Current).GlobalSettings = value; }
        }

        /// <summary>
        /// System Configuration Settings 
        /// </summary>
        /// <returns>SystemSettingsModel</returns>
        public static SystemSettingsModel System
        {
            get { return ((App)Application.Current).GlobalSettings.System; }
            set { ((App)Application.Current).GlobalSettings.System = value; }
        }

        /// <summary>
        /// Network Configuration Settings
        /// </summary>
        /// <returns>NetworkSettingsModel</returns>
        public static NetworkSettingsModel Network
        {
            get { return ((App)Application.Current).GlobalSettings.Network; }
            set { ((App)Application.Current).GlobalSettings.Network = value; }
        }

        /// <summary>
        /// Current User Settings
        /// </summary>
        /// <returns>UserSettingsModel</returns>
        public static UserSettingsModel User
        {
            //return globalSettings.User;
            get { return ((App)Application.Current).GlobalSettings.User; }
            set { ((App)Application.Current).GlobalSettings.User = value; }
        }

        /// <summary>
        /// Printer Configuration Settings
        /// </summary>
        /// <returns>PrinterSettingsModel</returns>
        public static PrinterSettingsModel Printers
        {
            //return globalSettings.Printers;
            get { return ((App)Application.Current).GlobalSettings.Printers; }
            set { ((App)Application.Current).GlobalSettings.Printers = value; }
        }

        /// <summary>
        /// Election Configuration Settings
        /// </summary>
        /// <returns>ElectionSettingsModel</returns>
        public static ElectionSettingsModel Election
        {
            //return globalSettings.Election;
            get { return ((App)Application.Current).GlobalSettings.Election; }
            set { ((App)Application.Current).GlobalSettings.Election = value; }
        }

        /// <summary>
        /// Ballot Configuration Settings
        /// </summary>
        /// <returns>BallotSettingsModel</returns>
        public static BallotSettingsModel Ballots
        {
            //return globalSettings.Ballots;
            get { return ((App)Application.Current).GlobalSettings.Ballots; }
            set { ((App)Application.Current).GlobalSettings.Ballots = value; }
        }

        /// <summary>
        /// Report Configuration Settings
        /// </summary>
        /// <returns>ReportSettingsModel</returns>
        public static ReportSettingsModel ReportConfigs
        {
            //return globalSettings.Reports;
            get { return ((App)Application.Current).GlobalSettings.Reports; }
            set { ((App)Application.Current).GlobalSettings.Reports = value; }
        }

        /// <summary>
        /// Save Settings to predefined files
        /// </summary>
        public static void SaveChanges()
        {
            ((App)Application.Current).GlobalSettings.SaveSettings();
        }

        /// <summary>
        /// Checks current time and sets VCC mode based on the current election date
        /// </summary>
        public static void SetElectionMode()
        {
            ((App)Application.Current).GlobalSettings.SetElectionMode();
        }

        /// <summary>
        /// Load latest settings
        /// </summary>
        public static bool CheckUpdates()
        {
            // Check if settings record has updated
            bool update = ((App) Application.Current).GlobalSettings.CheckForUpdate();

            // Update status bar
            if (update == true) StatusBar.DisplayMode(AppSettings.System);

            // Return if updated
            return update;
        }

        public static void CheckUpdates(bool? log)
        {
            if (CheckUpdates() == true && log == true)
            {
                // Log update status
                var settingsLog = new VoterXLogger("VCClogs", true);
                settingsLog.WriteLog("Updates have been loaded.");
            }
        }

        #region CommonValues
        public static int SiteId
        {
            get { return (int)((App)Application.Current).GlobalSettings.System.SiteID; }
        }

        public static string SiteName
        {
            get { return ((App)Application.Current).GlobalSettings.System.SiteName; }
        }
        #endregion
    }
}
