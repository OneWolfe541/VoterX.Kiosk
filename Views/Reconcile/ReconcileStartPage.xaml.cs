using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.SystemSettings.Models;
using VoterX.SystemSettings.Methods;
using VoterX.Core.Reconciles;
using VoterX.Utilities.Dialogs;
using VoterX.Logging;

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for ReconcileStartPage.xaml
    /// </summary>
    public partial class ReconcileStartPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        public ReconcileStartPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "Reconcile";
        }

        public ReconcileStartPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            DisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _reconcile = null;

            MainMenuMethods.OpenMenu();
            this.NavigateToPage(new Admin.AdministrationPage());
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SpoiledBallotCountPage(_displayText, _reconcile));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_reconcile == null)
            {
                LoadReconcile();
            }

            if (_displayText == null)
            {
                LoadDisplayText();
            }
        }

        private void LoadReconcile()
        {
            try
            {
                ReconcileFactory factory = new ReconcileFactory(((App)Application.Current).Connection);

                // Calculate the reconcile data for this site today
                _reconcile = factory.Create(
                    (int)AppSettings.System.SiteID,
                    DateTime.Now,
                    (int)AppSettings.Election.ElectionID,
                    AppSettings.Election.CountyCode,
                    AppSettings.Election.EligibleParties);
            }
            catch (Exception e)
            {
                // Log error
                VoterXLogger settingsLog = new VoterXLogger("VCClogs", true);
                settingsLog.WriteLog("RECONCILE DATA FAILED TO LOAD: " + e.Message);

                if(e.InnerException != null)
                {
                    settingsLog.WriteLog("INNER EXCEPTION: " + e.InnerException);

                    if (e.InnerException.InnerException != null)
                    {
                        settingsLog.WriteLog("INNER EXCEPTION: " + e.InnerException.InnerException);
                    }
                }

                AlertDialog settingsFailed = new AlertDialog("RECONCILE DATA FAILED TO LOAD\r\n" + e.Message);
                if (settingsFailed.ShowDialog() == true)
                {
                    // Return to admin page
                    _reconcile = null;

                    MainMenuMethods.OpenMenu();
                    this.NavigateToPage(new Admin.AdministrationPage());
                }
            }
        }

        private void LoadDisplayText()
        {
            // Create blank file
            //ReconcileMethods.CreateJsonFile(AppSettings.Global.GetFilePath());

            try
            {
                _displayText = ReconcileMethods.LoadJsonFile(AppSettings.Global.GetFilePath());

                if (_displayText != null)
                {
                    DisplayText();
                }
            }
            catch (Exception e)
            {
                // Log error
                VoterXLogger settingsLog = new VoterXLogger("VCClogs", true);
                settingsLog.WriteLog("RECONCILE FILE FAILED TO LOAD: " + e.Message);                

                // Display error message
                StatusBar.TextCenter = "File could not be loaded: " + e.Message;

                // Hide page
                PageGrid.Visibility = Visibility.Collapsed;

                AlertDialog settingsFailed = new AlertDialog("SETTINGS FILE FAILED TO LOAD\r\n" + e.Message);
                if (settingsFailed.ShowDialog() == true)
                {
                    // Return to admin page
                    _reconcile = null;

                    MainMenuMethods.OpenMenu();
                    this.NavigateToPage(new Admin.AdministrationPage());
                }
            }            
        }    
        
        private void DisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = _displayText.StartPageHeader + " - PRIMARY";
            }
            else
            {
                StatusBar.PageHeader = _displayText.StartPageHeader;
            }

            StartPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.StartPageBoldLine1, _reconcile).ToUpper();

            StartPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.StartPageInstructions1, _reconcile);

            StartPageItemList.Text = DisplayTextMethods.ParseReconcile(_displayText.StartPageItemList, _reconcile);
        }
    }
}
