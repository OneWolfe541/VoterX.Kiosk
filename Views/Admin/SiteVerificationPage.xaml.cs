using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VoterX.Core.Elections;
using VoterX.SystemSettings.Enums;
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Factories;
using VoterX.Kiosk.Methods;
using VoterX.Kiosk.Views.Voter.Signature;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for SiteVerificationPage.xaml
    /// </summary>
    public partial class SiteVerificationPage : Page
    {
        public SiteVerificationPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "Site Verification";

            StatusBar.Clear();

            CheckServer();

            SiteName.Text = AppSettings.System.SiteName;
            ComputerNumber.Text = AppSettings.System.MachineID.ToString();

            //SiteNameTestPanel.Visibility = Visibility.Collapsed;
            //SignatureTestPanel.Visibility = Visibility.Visible;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SignatureVerificationViewModel sigPad = new SignatureVerificationViewModel();
            sigPad.PropertyChanged += OnSignatureSelectedPropertyChanged;
            SignaturePadControl.DataContext = sigPad;
        }

        private void OnSignatureSelectedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SignatureFile")
            {
                var file = ((SignatureVerificationViewModel)SignaturePadControl.DataContext).SignatureFile;
                if(file != null)
                {
                    AppSettings.System.SiteVerified = true;

                    // Write system settings to the file
                    AppSettings.SaveChanges();

                    // Finish Site Verification
                    SignatureTestPanel.Visibility = Visibility.Collapsed;
                    VerifiedSitePanel.Visibility = Visibility.Visible;
                    IntroTextBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    AlertDialog message = new AlertDialog("VoterX could not save the signature file.\r\nThe site cannot be verified at this time.");
                    if (message.ShowDialog() == true)
                    {
                        this.NavigateToPage(new Login.LoginPage());
                    }
                }
            }
        }

        private async void CheckServer()
        {
            VerifySitePanel.Visibility = Visibility.Collapsed;

            if (await StatusBar.CheckServer(new ElectionFactory()) == true)
            {
                VerifySitePanel.Visibility = Visibility.Visible;
            }
            else
            {
                AlertDialog message = new AlertDialog("VoterX could not establish a connection to the database.\r\nThe site cannot be verified at this time.");
                if (message.ShowDialog() == true)
                {
                    this.NavigateToPage(new Login.LoginPage());
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Login.LoginPage());
        }

        private async void ZeroReportButton_Click(object sender, RoutedEventArgs e)
        {
            ZeroReportButton.Visibility = Visibility.Collapsed;
            ZeroReportPrinterCheckPanel.Visibility = Visibility.Visible;
            if (AppSettings.System.VCCType == VotingCenterMode.EarlyVoting) // Early Voting
            {
                //StatusBar.TextCenter = (await Task.Run(() => ReportPrintingMethods.PrintZeroEarlyVotingBSReport(AppSettings.Global)));
                StatusBar.TextCenter = (await Task.Run(() =>
                    VCCReportingFactory.ZeroReport(AppSettings.System.VCCType.ToInt())
                    .PrintReport(AppSettings.Global, AppSettings.System.SiteID, false)
                ));
            }
            else // Election Day
            {
                //StatusBar.TextCenter = (await Task.Run(() => ReportPrintingMethods.PrintZeroElectionDayReport(AppSettings.Global)));
                StatusBar.TextCenter = (await Task.Run(() =>
                    VCCReportingFactory.ZeroReport(AppSettings.System.VCCType.ToInt())
                    .PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false)
                ));
            }
        }

        private void ZeroReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (ZeroReportPrinterCheckQuestion.GetAnswer() == true)
            {
                BallotTestPanel.Visibility = Visibility.Visible;
                ZeroReportTestPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Display Not Verified Message
                AlertDialog message = new AlertDialog("Make sure the printer is connected and setup correctly.\r\n\r\nFor further assistance call technical support.");
                if (message.ShowDialog() == true)
                {
                    this.NavigateToPage(new Login.LoginPage());
                }
            }
        }

        private async void TestBallotButton_Click(object sender, RoutedEventArgs e)
        {
            TestBallotButton.Visibility = Visibility.Collapsed;
            TestBallotPrinterCheckPanel.Visibility = Visibility.Visible;
            StatusBar.TextCenter = (await Task.Run(() => BallotPrinting.PrintTestBallot(AppSettings.Global)));
        }

        private void TestBallotPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (TestBallotPrinterCheckQuestion.GetAnswer() == true)
            {
                //AppSettings.System.SiteVerified = true;

                //// Write system settings to the file
                //AppSettings.SaveChanges();

                BallotTestPanel.Visibility = Visibility.Collapsed;
                VerifiedSitePanel.Visibility = Visibility.Collapsed; 
                //IntroTextBlock.Visibility = Visibility.Collapsed;

                SignatureTestPanel.Visibility = Visibility.Visible;

            }
            else
            {
                try
                {
                    // Display Not Verified Message
                    AlertDialog message = new AlertDialog("Make sure the printer is connected and setup correctly.\r\n\r\nFor further assistance call technical support.");
                    if (message.ShowDialog() == true)
                    {
                        this.NavigateToPage(new Login.LoginPage());
                    }
                }
                catch (Exception error)
                {
                    StatusBar.TextCenter = error.Message;
                }
            }
        }

        private void SiteNameCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (SiteNameCheckQuestion.GetAnswer() == true)
            {
                ZeroReportTestPanel.Visibility = Visibility.Visible;
                SiteNameTestPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AlertDialog message = new AlertDialog("If this site is incorrect please contact technical support.");
                if (message.ShowDialog() == true)
                {
                    this.NavigateToPage(new Login.LoginPage());
                }
            }
        }
    }
}
