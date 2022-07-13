using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VoterX.Kiosk.Views;
//using VoterX.Core.Containers;
//using VoterX.Core.Methods;
using VoterX.SystemSettings;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.BasePageDefinitions;
using VoterX.Utilities.Controls;
using VoterX.Utilities.Methods;
using VoterX.Core.Voters;
using VoterX.Core.Elections;
using VoterX.Kiosk.Timers;
using System.Windows.Threading;
using System.Windows.Controls;
using VoterX.Utilities.Views;
using VoterX.SystemSettings.Extensions;
using System.Diagnostics;
using VoterX.SystemSettings.Models;
using VoterX.Logging;

namespace VoterX.Kiosk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        // Create empty global system settings object
        public SystemSettingsController GlobalSettings = new SystemSettingsController();
        //public SystemSettingsController TestSettings;

        public string Connection;

        // Create empty Voter Factory
        public VoterFactory Voters;

        // Create empty Election Factory
        public NMElection Election;

        // This property lets the application know that it is running on my Development Computer
        public bool debugMode = false;

        public int? SearchMethod;

        public bool settingsChanged = false;

        public MasterBasePage mainpage = null;
        //public StateVoterX.Utilities.UserControls.StatusBarControl mainstatusbar = null;
        public SliderMenuFrameControl mainslidermenu = null;

        public StatusBarViewModel StatusBar;

        public MainHeaderViewModel MainHeader;

        public IdleTimer IdleTimer = null;
        //public DispatcherTimer TestTimer = null;

        public Page originpage = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            debugMode = GlobalSettings.CheckDebugFile();

            // Shutdown system if already running
            if (debugMode != true)
            {
                Process proc = Process.GetCurrentProcess();
                int count = Process.GetProcesses().Where(p =>
                    p.ProcessName == proc.ProcessName).Count();

                if (count > 1)
                {
                    MessageBox.Show("An instance of VoterX is already running");
                    App.Current.Shutdown();
                }
            }

            // Load System Settings File
            try
            {
                string server = System.Environment.MachineName;

                var DebugSettings = new DebugSettingModel();
                try
                {
                    DebugSettings = GlobalSettings.LoadDebugFile("C:\\VoterX\\Settings\\");
                }
                catch
                {
                    DebugSettings = new DebugSettingModel();                    
                }

                //DebugSettings.SettingsType = 2;
                SearchMethod = 2;

                if (DebugSettings.SettingsType == null)
                {
                    string connection = "XXXX";
                    GlobalSettings = new SystemSettingsController(StorageType.Database, ConfigurationManager.AppSettings["AppFolder"], connection, null);
                    Connection = "XXXX";
                }
                if (DebugSettings.SettingsType == 0)
                {
                    GlobalSettings = new SystemSettingsController(StorageType.Json, ConfigurationManager.AppSettings["AppFolder"], null);
                }
                else if (DebugSettings.SettingsType == 1)
                {
                    string connection = @"XXXX";
                    GlobalSettings = new SystemSettingsController(StorageType.Database, ConfigurationManager.AppSettings["AppFolder"], connection, DebugSettings.SystemId);
                    Connection = "XXXX";
                }
                else if (DebugSettings.SettingsType == 2)
                {
                    string connection = @"XXXX";
                    GlobalSettings = new SystemSettingsController(StorageType.Database, ConfigurationManager.AppSettings["AppFolder"], connection, null);
                    Connection = "XXXX";
                }
            }
            catch (Exception exception)
            {
                //var settingsLog = new VoterXLogger("VCClogs", true);
                //settingsLog.WriteLog("Settings Load Error: " + exception.InnerException);

                AlertDialog settingsFailed = new AlertDialog("SETTINGS FILE FAILED TO LOAD\r\n" + exception.Message);
                settingsFailed.ShowDialog();
                // Sample Shut down process
                // https://stackoverflow.com/questions/606043/shutting-down-a-wpf-application-from-app-xaml-cs
                Shutdown(1);
                return;
            }

            GlobalSettings.System.ReportErrorLogging = true;

            // Set VCC Mode based on the current election date
            GlobalSettings.BODVersion = "12.04.07.12";
            GlobalSettings.BODName = "VoterX VCC";            

            // Set Default PDFTools License
            if (GlobalSettings.System.PDFTools == null || GlobalSettings.System.PDFTools == "")
            {
                GlobalSettings.System.PDFTools = "XXXX";
            }

            if (GlobalSettings.Election.ElectionType == VoterX.SystemSettings.Enums.ElectionType.General)
            {
                // Clear Eligible Parties
                GlobalSettings.Election.EligibleParties = null;
            }

            // Hard Code Site Verification test ballot file name
            GlobalSettings.Ballots.TestBallot = "C:\\VoterX\\Ballots\\DOMBODtest14.pdf";

            // Set connection string from system settings file
            ConnectionSetup.SetServerConnection(GlobalSettings.Network.SQLServer, GlobalSettings.Network.LocalDatabase, GlobalSettings.Network.SQLMode);
            //ConnectionSetup.EncryptConnection();
            ConnectionSetup.RefreshConnection();

            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            //var newConnectionString = connectionStringsSection.ConnectionStrings["VoterDatabase"].ConnectionString;
            //MessageBox.Show(newConnectionString);

            // Create and Load new Unity container
            //voterContainer = new VoterContainer();

            // Create new Voter Factory
            Voters = new VoterFactory(GlobalSettings.Election.ElectionType.ToInt(), Connection);

            // Moved to LoginPage
            //// Create new Election
            //using (ElectionFactory factory = new ElectionFactory())
            //{
            //    Election = factory.Create(GlobalSettings.Election.ElectionID);
            //}

            // Moved to LoginPage
            //Election.Load(GlobalSettings.Election.ElectionID);

            // Create empty timer object
            //IdleTimer = new IdleTimer();
        }
    }
}
