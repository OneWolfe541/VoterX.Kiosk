using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Factories;
using VoterX.Kiosk.Methods;
using VoterX.Kiosk.Reporting.Methods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for EndofDayPage.xaml
    /// </summary>
    public partial class EndofDayPage : Page
    {
        public EndofDayPage()
        {
            InitializeComponent();

            StatusBar.PageHeader = "End of Day";

            StatusBar.Clear();

            //if (((App)Application.Current).debugMode == false)
            //{
            //    PrintDailyReports();
            //}
        }

        public EndofDayPage(bool reconciled)
        {
            InitializeComponent();

            StatusBar.PageHeader = "End of Day";

            StatusBar.Clear();

            if (reconciled)
            {
                PrintReconcile();
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var log = await PollSummaryMethods.SaveLogout();
            this.NavigateToPage(new Login.LoginPage());
        }

        private async void PrintDailyReports()
        {
            var reports = VCCReportingFactory.GetReports(AppSettings.Election.ElectionType.ToInt(), AppSettings.System.VCCType.ToInt());

            int index = 0;
            do
            {
                // Print todays reports
                //StatusBar.TextCenter = await Task.Run(() => DailySummaryMethods.PrintDailySummaryReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global));
                StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "Daily Summary").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));

                // Print todays spoiled
                //StatusBar.TextCenter = await Task.Run(() => SpoiledReportMethods.PrintSpoiledReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, DateTime.Now, AppSettings.Global));
                StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "Spoiled Ballots").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));

                // Print todays provisional
                //StatusBar.TextCenter = await Task.Run(() => ProvisionalReportMethods.PrintProvisionalReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, DateTime.Now, AppSettings.Global));
                StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "Provisional Ballots").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));

                // Print todays voter details
                //StatusBar.TextCenter = await Task.Run(() => DailyRosterBSMethods.PrintDailyRosterBSReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global));
                StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "Daily Details").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));

                index++;
            } while (index < AppSettings.ReportConfigs.ReconcileCopies); 


            ////  Print Second Set

            //// Print todays reports
            //StatusBar.StatusTextCenter = await Task.Run(() => DailySummaryMethods.PrintDailySummaryReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global));

            //// Print todays spoiled
            //StatusBar.StatusTextCenter = await Task.Run(() => SpoiledReportMethods.PrintSpoiledReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, DateTime.Now, AppSettings.Global));

            //// Print todays provisional
            //StatusBar.StatusTextCenter = await Task.Run(() => ProvisionalReportMethods.PrintProvisionalReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, DateTime.Now, AppSettings.Global));

            //// Print todays voter details
            //StatusBar.StatusTextCenter = await Task.Run(() => DailyRosterBSMethods.PrintDailyRosterBSReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global));
        }

        private void DailyReportPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (DailyReportPrintQuestion.GetAnswer() == true)
            {
                // Show spoiled report question grid
                SpoiledReportTroubleshootingGrid.Show();
            }
            else
            {
                DailyReportPrinterCheckPanel.Show();
            }
        }

        private void DailyReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if(DailyReportPrinterCheckQuestion.GetAnswer() == true)
            {
                DailyReportPrintQuestion.ChangeAnswer(true);

                // Hide Report Check Panel
                DailyReportPrinterCheckPanel.Hide();

                // Show spoiled report question grid
                SpoiledReportTroubleshootingGrid.Show();
            }
            else
            {
                DailyReportOptionsPanel.Show();
            }
        }

        private void TransferClerkCheck_Click(object sender, RoutedEventArgs e)
        {
            LogoutButton.Visibility = Visibility.Visible;
        }

        private void SpoiledReportPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if(SpoiledReportPrintQuestion.GetAnswer() == true)
            {
                // Show provisional report question
                ProvisionalReportTroubleshootingGrid.Show();
            }
            else
            {
                DailyReportTroubleshootingGrid.Hide();

                // Show trouble shooting question
                SpoiledReportPrinterCheckPanel.Show();
            }
        }

        private void SpoiledReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (SpoiledReportPrinterCheckQuestion.GetAnswer() == true)
            {
                SpoiledReportPrintQuestion.ChangeAnswer(true);

                // Hide Report Check Panel
                SpoiledReportPrinterCheckPanel.Hide();

                DailyReportTroubleshootingGrid.Hide();

                // Show provisional report question grid
                ProvisionalReportTroubleshootingGrid.Show();
            }
            else
            {
                SpoiledReportOptionsPanel.Show();
            }
        }

        private void ProvisionalReportPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (ProvisionalReportPrintQuestion.GetAnswer() == true)
            {
                // Show daily report question
                DetailReportTroubleshootingGrid.Visibility = Visibility.Visible;
            }
            else
            {
                DailyReportTroubleshootingGrid.Hide();
                SpoiledReportTroubleshootingGrid.Hide();

                // Show trouble shooting question
                ProvisionalReportPrinterCheckPanel.Show(); 
            }
        }

        private void ProvisionalReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (ProvisionalReportPrinterCheckQuestion.GetAnswer() == true)
            {
                ProvisionalReportPrintQuestion.ChangeAnswer(true);

                // Hide Report Check Panel
                ProvisionalReportPrinterCheckPanel.Hide();

                // Show reconcile button
                //ReconcileButton.Visibility = Visibility.Visible;

                // Hide the other questions
                //DailyReportTroubleshootingGrid.Hide();
                //SpoiledReportTroubleshootingGrid.Hide();
                //ProvisionalReportTroubleshootingGrid.Hide();

                DetailReportTroubleshootingGrid.Show();
            }
            else
            {
                // Hide the other questions
                DailyReportTroubleshootingGrid.Hide();
                SpoiledReportTroubleshootingGrid.Hide();

                ProvisionalReportOptionsPanel.Show();
            }
        }

        private void ReconcileButton_Click(object sender, RoutedEventArgs e)
        {
            MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            // Go to final page and print the report
            this.NavigateToPage(new ReconcilePrimary.ReconcileStartPage());

            //// Hide the other questions
            //DailyReportTroubleshootingGrid.Hide();
            //SpoiledReportTroubleshootingGrid.Hide();
            //ProvisionalReportTroubleshootingGrid.Hide();
            //DetailReportTroubleshootingGrid.Hide();

            //// Activate Reconsile Application
            //// https://stackoverflow.com/questions/1585354/get-return-value-from-process

            //int result = await Task.Run(() =>
            //{
            //    Process P = Process.Start("C:\\Program Files\\VoterX\\VoterXReconcile.exe");
            //    P.WaitForExit();
            //    return P.ExitCode;
            //});

            //if (result == 0)
            //{
            //    // Print End of Day report twice
            //    StatusBar.StatusTextCenter = ReportPrintingMethods.PrintEndOfDayReport(DateTime.Now, AppSettings.Global);
            //    // Bonnie requested a second copy be printed 7/20/2018
            //    StatusBar.StatusTextCenter = ReportPrintingMethods.PrintEndOfDayReport(DateTime.Now, AppSettings.Global);

            //    // Show end of day print check
            //    EODReportTroubleshootingGrid.Show();
            //}
            //else
            //{
            //    //StatusBar.StatusTextCenter = "Reconcile was not complete: Exit Code(" + result.ToString() + ")";
            //}
        }

        private async void PrintReconcile()
        {
            // Hide the other questions
            DailyReportTroubleshootingGrid.Hide();
            SpoiledReportTroubleshootingGrid.Hide();
            ProvisionalReportTroubleshootingGrid.Hide();
            DetailReportTroubleshootingGrid.Hide();

            var reports = VCCReportingFactory.GetReports(AppSettings.Election.ElectionType.ToInt(), AppSettings.System.VCCType.ToInt());

            int index = 0;
            do
            {
                // Print End of Day report twice
                //StatusBar.TextCenter = EndOfDayReportMethods.PrintEndOfDayReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global);
                StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "End Of Day").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));

                index++;
            } while (index < AppSettings.ReportConfigs.ReconcileCopies);

            //// Bonnie requested a second copy be printed 7/20/2018
            //StatusBar.StatusTextCenter = EndOfDayReportMethods.PrintEndOfDayReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global);

            // Show end of day print check
            EODReportTroubleshootingGrid.Show();
        }

        private void EODReportPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (EODReportPrintQuestion.GetAnswer() == true)
            {
                // Finish Reconcile and log out
                // popup "Have a nice day"
                FinishEndOfDay();
            }
            else
            {
                // Show trouble shooting question
                EODReportPrinterCheckPanel.Show();
            }
        }

        private void EODReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (EODReportPrinterCheckQuestion.GetAnswer() == true)
            {
                EODReportPrintQuestion.ChangeAnswer(true);

                // Hide Report Check Panel
                EODReportPrinterCheckPanel.Hide();

                // Finish Reconcile and log out
                // popup "Have a nice day"
                FinishEndOfDay(); 
            }
            else
            {
                EODReportOptionsPanel.Show();
            }
        }

        private void FinishEndOfDay()
        {
            // Display message
            AlertDialog signatureDialog = new AlertDialog("YOU HAVE SUCCESSFULLY COMPLETED THE END OF DAY PROCESS.");
            if(signatureDialog.ShowDialog() == true)
            {
                //PollSummaryMethods.SaveLogout();
                //this.NavigateToPage(new Login.LoginPage());

                PrintEODReport.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Visible;
            }
        }

        private void DetailReportPrintQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (DetailReportPrintQuestion.GetAnswer() == true)
            {
                // Hide the other questions
                DailyReportTroubleshootingGrid.Hide();
                SpoiledReportTroubleshootingGrid.Hide();
                ProvisionalReportTroubleshootingGrid.Hide();
                DetailReportTroubleshootingGrid.Hide();

                // Show daily report question
                ReconcileButton.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide the other questions
                DailyReportTroubleshootingGrid.Hide();
                SpoiledReportTroubleshootingGrid.Hide();
                ProvisionalReportTroubleshootingGrid.Hide();

                // Show trouble shooting question
                DetailReportPrinterCheckPanel.Show();
            }
        }

        private void DetailReportPrinterCheckQuestion_AnswerClick(object sender, RoutedEventArgs e)
        {
            if (DetailReportPrinterCheckQuestion.GetAnswer() == true)
            {
                DetailReportPrintQuestion.ChangeAnswer(true);

                // Hide Report Check Panel
                DetailReportTroubleshootingGrid.Hide();

                // Show reconcile button
                ReconcileButton.Visibility = Visibility.Visible;
            }
            else
            {
                DetailReportOptionsPanel.Show();
            }
        }

        private async void StartEndOfDayButton_Click(object sender, RoutedEventArgs e)
        {
            YesNoDialog signatureDialog = new YesNoDialog("End of Day", "ARE YOU SURE YOU WANT TO START THE\r\nEND OF DAY PROCESS?");
            if (signatureDialog.ShowDialog() == true)
            {
                StartEndOfDayButton.Visibility = Visibility.Collapsed;

                DailyReportPrintingStatus.Visibility = Visibility.Visible;
                DailyReportPrintQuestion.Visibility = Visibility.Visible;

                PrintDailyReports();
            }
            else
            {
                var log = await PollSummaryMethods.SaveLogout();
                this.NavigateToPage(new Login.LoginPage());
            }
        }

        private async void PrintEODReport_Click(object sender, RoutedEventArgs e)
        {
            var reports = VCCReportingFactory.GetReports(AppSettings.Election.ElectionType.ToInt(), AppSettings.System.VCCType.ToInt());

            //StatusBar.TextCenter = EndOfDayReportMethods.PrintEndOfDayReport(AppSettings.Election.ElectionID, (int)AppSettings.System.SiteID, AppSettings.Global);

            StatusBar.TextCenter = await Task.Run(() => reports.Where(r => r.Name == "End Of Day").FirstOrDefault().PrintReport(AppSettings.Global, AppSettings.System.SiteID, DateTime.Now, false));
        }
    }
}
