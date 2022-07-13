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
//using VoterX.Core.Models.ViewModels;
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Views.Voter;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.Core.Voters;
using VoterX.Logging;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.Ballots
{
    /// <summary>
    /// Interaction logic for PrintBundlePage.xaml
    /// </summary>
    public partial class PrintBundlePage : Page
    {
        private NMVoter _voter = new NMVoter();

        private int printingErrors = 0;
        private bool exitLogout = false;

        VoterXLogger _errorLogger;

        public PrintBundlePage()
        {
            InitializeComponent();
        }

        public PrintBundlePage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;

            LoadVoterFields(voter);

            //CheckPrinter();

            StatusBar.PageHeader = "Print Ballot";

            //StatusBar.CheckPrinter(AppSettings.Printers);

            StatusBar.Clear();

            CheckServer();

            _errorLogger = new VoterXLogger("VCCLogs", AppSettings.System.ReportErrorLogging);

            PrintBallot.Visibility = Visibility.Visible;
        }

        public PrintBundlePage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _voter = voter;

            exitLogout = returnStatus;

            LoadVoterFields(voter);

            //CheckPrinter();

            StatusBar.PageHeader = "Print Ballot";

            StatusBar.Clear();

            CheckServer();

            PrintBallot.Visibility = Visibility.Visible;
        }

        private async void CheckServer()
        {
            VoterDetailGrid.Visibility = Visibility.Collapsed;

            if (await StatusBar.CheckServer(ElectionDataMethods.Election) == true) 
            {
                VoterDetailGrid.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Voter.Search.VoterSearchPage(null));
        }

        // This entire function needs to be async not just the ballot printing part
        // inorder for the application to move to the next page before the ballot prints
        private async void PrintBallot_Click(object sender, RoutedEventArgs e)
        {
            //StatusBar.TextCenter = "Locking Button";

            // Change cursor
            Mouse.OverrideCursor = Cursors.Wait;

            // Prevent spam clicking
            PrintBallot.IsEnabled = false;
            BackButton.IsEnabled = false;


            //StatusBar.TextCenter = "Check Connection";
            var server = ((App)Application.Current).Voters;
            if (await Task.Run(() => VoterMethods.Exists) == true)
            {
                //StatusBar.TextCenter = "Check Printer Status";
                // Check printer status first
                var printstatus = PrinterStatus.PrinterIsReadyAsync(AppSettings.Printers.BallotPrinter);
                if (await PrinterStatus.PrinterIsReadyAsync(AppSettings.Printers.BallotPrinter) == true)
                {
                    //StatusBar.TextCenter = "Get Current Log Code";
                    // Check a second time to see if the voter status has changed before printing an official ballot
                    //var currentLogCode = await Task.Run(() => VoterManagmentMethods.CheckLogCode(_voter, VoterMethods.Container));
                    var currentLogCode = await Task.Run(() => _voter.CheckLogCode());
                    if (currentLogCode < 5)
                    {
                        try
                        {
                            //StatusBar.TextCenter = "Save Record";

                            _voter.Localize(AppSettings.Global);

                            // Clear null value from temp address
                            if (_voter.Data.TempAddress == null) _voter.Data.TempAddress = false;

                            //Mark the Voter
                            string message = VoterManagmentMethods.InsertOrUpdateVotedAtPolls(
                            _voter,
                            AppSettings.Global);

                            if (message != "")
                            {
                                _errorLogger.WriteLog("Voted Record Error: " + message);

                                //_errorLogger.WriteLog(_voter.ToString());
                            }
                        }
                        catch (Exception error)
                        {
                            StatusBar.TextCenter = error.Message;
                            _errorLogger.WriteLog("Voted Record Error: " + error.Message);
                            if (error.InnerException != null)
                            {
                                _errorLogger.WriteLog(error.InnerException.ToString());
                            }
                            return;
                        }

                        // Set printed date
                        _voter.Data.BallotPrinted = DateTime.Now;

                        //StatusBar.TextCenter = "Printing Ballot";
                        // Print ballot, application, permit, and/or stub
                        await Task.Run(() => BallotPrinting.PrintBallotBundle(_voter.Data, AppSettings.Global));

                        if (_voter.Data.SignRefused == true)
                        {
                            if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay) // Election Day Mode
                            {
                                // Print signature form
                                await Task.Run(() => BallotPrinting.PrintSignatureForm(_voter.Data, AppSettings.Global));

                                // Display message
                                AlertDialog signatureDialog = new AlertDialog("MAKE SURE THE VOTER SIGNS THE SIGNATURE FORM");
                                signatureDialog.ShowDialog();
                            }
                            else
                            {
                                // Display message
                                AlertDialog signatureDialog = new AlertDialog("MAKE SURE THE VOTER SIGNS THE PRINTED APPLICATION");
                                signatureDialog.ShowDialog();
                            }
                        }

                        // Goto the print varification page
                        //if (exitLogout == true)
                        //{
                        //    this.NavigateToPage(new Troubleshooting.PrintVerifyTroubleshootPage(_voter, true));
                        //}
                        //else
                        //{
                        //    this.NavigateToPage(new Troubleshooting.PrintVerifyTroubleshootPage(_voter));
                        //}
                        NavigationMenuMethods.OfficialPrintTroubleShootingPage(_voter);

                    }
                    else
                    {
                        // Display message
                        AlertDialog signatureDialog = new AlertDialog("THIS VOTER HAS ALREADY BEEN ASSIGNED A BALLOT");
                        signatureDialog.ShowDialog();

                        BackButton.IsEnabled = false;
                    }
                }
                else
                {
                    // Add 1 to error count
                    printingErrors++;
                    // Stop the process if they have passed this point before
                    if (printingErrors <= 1)
                    {
                        // Show error message and rest buttons
                        PrinterErrorPanel.Visibility = Visibility.Visible;
                        // Turn off No button
                        ReadyToPrintNo.IsChecked = false;
                        ready_print_fa_check_no.Visibility = Visibility.Collapsed;
                        // Turn off the Yes button
                        ReadyToPrintYes.IsChecked = false;
                        ready_print_fa_check_yes.Visibility = Visibility.Collapsed;

                        PrintBallot.IsEnabled = true;
                        BackButton.IsEnabled = true;
                    }
                    else
                    {
                        // Show return to search button
                        OptOutButton.Visibility = Visibility.Visible;
                        SeriousPrinterErrorPanel.Visibility = Visibility.Visible;
                    }

                    // Hide print button
                    PrintBallot.Visibility = Visibility.Collapsed;

                    // Display printer state and status
                    //StatusBar.PrinterStatusIcon = false;
                }

                //StatusBar.TextCenter = "Done";
            }
            else
            {
                StatusBar.TextCenter = "Database not found";
                //PrintBallot.IsEnabled = true;
                BackButton.IsEnabled = true;
            }

            // Change cursor back
            Mouse.OverrideCursor = null;
        }

        private void LoadVoterFields(NMVoter voter)
        {
            FullName.Text = voter.Data.FullName;
            BirthYear.Text = voter.Data.DOBYear;
            Address.Text = voter.Data.Address1;
            CityStateAndZip.Text = voter.Data.City + ", " + voter.Data.State + " " + voter.Data.Zip;
            BallotStyle.Text = voter.Data.BallotStyle;
        }

        private async void CheckPrinter()
        {
            PrintBallot.Visibility = Visibility.Visible;
            if (await StatusBar.CheckPrinter(AppSettings.Printers) == true)
            {
                PrintBallot.Visibility = Visibility.Visible;
            }
            else
            {
                printingErrors++;
                PrinterErrorPanel.Visibility = Visibility.Visible;
            }
        }

        private void ReadyToPrintYes_Click(object sender, RoutedEventArgs e)
        {
            // Turn on Yes button
            ReadyToPrintYes.IsChecked = true;
            ready_print_fa_check_yes.Visibility = Visibility.Visible;

            // Turn off No button
            ReadyToPrintNo.IsChecked = false;
            ready_print_fa_check_no.Visibility = Visibility.Collapsed;

            // Show Print Button
            PrintBallot.Visibility = Visibility.Visible;
            OptOutButton.Visibility = Visibility.Collapsed;

            // Hide error message
            PrinterErrorPanel.Visibility = Visibility.Collapsed;
        }

        private void ReadyToPrintNo_Click(object sender, RoutedEventArgs e)
        {
            // Turn on the No button
            ReadyToPrintNo.IsChecked = true;
            ready_print_fa_check_no.Visibility = Visibility.Visible;

            // Turn off the Yes button
            ReadyToPrintYes.IsChecked = false;
            ready_print_fa_check_yes.Visibility = Visibility.Collapsed;

            // Show return to search button
            OptOutButton.Visibility = Visibility.Visible;
            PrintBallot.Visibility = Visibility.Collapsed;
        }
    }
}
