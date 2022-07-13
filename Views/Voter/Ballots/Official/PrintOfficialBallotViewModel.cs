using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Voter.Ballots
{
    public class PrintOfficialBallotViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        public NMVoter VoterItem { get; set; }

        public PrintOfficialBallotViewModel(NMVoter voter)
        {
            VoterItem = voter;

            SetDisplayMessages();

            // Check printer status
            //WaitForPrinterCheck();

            CanPrintBallot = true;

            StatusBar.PageHeader = "Print Ballot";
        }

        private async void WaitForPrinterCheck()
        {
            // Disable print button
            CanPrintBallot = false;

            // Display Status Message
            StatusBar.TextLeft = "Checking Printer";
            StatusBar.ShowLeftSpinner();

            // Check printer status
            var printerStatus = await CheckBallotPrinter();

            // Reenable print button
            CanPrintBallot = true;
            RaisePropertyChanged("CanPrintBallot");

            // Hide status message
            StatusBar.TextLeft = "";
            StatusBar.HideLeftSpinner();
        }

        #region DisplayText
        public string PrintBallotMessage { get; set; }

        private void SetDisplayMessages()
        {
            PrintBallotMessage = "PRINT OFFICIAL BALLOT FOR CURRENT VOTER";

            //PrintBallotMessage = _currentText.PrintBallotPage.PrintBallotMessage.Value;
        }
        #endregion


        #region Commands
        // Bound command for returning to the search screen
        public RelayCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(param => this.ReturnToSearchClick());
                }
                return _goBackCommand;
            }
        }

        // Force parent frame to navigate back to the search page
        public void ReturnToSearchClick()
        {
            //_parent.Navigate(new VoterSearchPage(_parent));
            //NavigationMenuMethods.VoterSearchPage();
            NavigationMenuMethods.ReturnToOrigin();
        }

        // Bound command for printing an official ballot
        public RelayCommand _printBallotCommand;
        public ICommand PrintBallotCommand
        {
            get
            {
                if (_printBallotCommand == null)
                {
                    _printBallotCommand = new RelayCommand(param => this.PrintBallotClick(), param => this.CanPrintBallot);
                }
                return _printBallotCommand;
            }
        }

        // Enable or Disable the Print Ballot Button
        private bool _canPrintBallot;
        public bool CanPrintBallot
        {
            get { return _canPrintBallot; }
            internal set
            {
                _canPrintBallot = value;
                RaisePropertyChanged("CanPrintBallot");
            }
        }

        // Print ballot and application or permit or stub
        public async void PrintBallotClick()
        {
            // Prevent spam clicking
            CanPrintBallot = false;

            UIServices.SetBusyState(true);

            bool printerStatus = false,
                 voterStatus = false,
                 ballotStatus = false;

            // Check printer status
            //printerStatus = await CheckBallotPrinter();
            printerStatus = true;

            // Check voter status and mark the record
            if (printerStatus == true)
            {
                // Skip marking voter and printing ballot while developing questionnaire
                voterStatus = true;

                voterStatus = await MarkVoter();

                // Print ballot, application, permit, and/or stub
                if (voterStatus == true)
                {
                    ballotStatus = await PrintOfficialBallotBundle();

                    // Stop spinning cursor
                    UIServices.SetBusyState(false);

                    // Print signature form if sign refused -- maybe put this inside the Ballot Bundle function
                    if (VoterItem.Data.SignRefused == true)
                    {
                        if (AppSettings.System.VCCType == VotingCenterMode.ElectionDay) // Election Day Mode
                        {
                            // Print signature form
                            //await Task.Run(() => BallotPrinting.PrintSignatureForm(VoterItem.Data, AppSettings.Global));

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

                    // Navigate to print troubleshooting page -- Navigation should not wait on the ballot to print
                    NavigationMenuMethods.OfficialPrintTroubleShootingPage(VoterItem);
                }
            }

            CanPrintBallot = true;
            UIServices.SetBusyState(false);
        }

        // Bound command for printing an official ballot
        public RelayCommand _printApplicationCommand;
        public ICommand PrintApplicationCommand
        {
            get
            {
                if (_printApplicationCommand == null)
                {
                    _printApplicationCommand = new RelayCommand(param => this.PrintApplicationClick(), null);
                }
                return _printApplicationCommand;
            }
        }

        // Print ballot and application or permit or stub
        public async void PrintApplicationClick()
        {
            // Prevent spam clicking
            CanPrintBallot = false;

            UIServices.SetBusyState(true);

            bool printerStatus = false,
                 voterStatus = false,
                 ballotStatus = false;

            // Check printer status
            printerStatus = await CheckBallotPrinter();

            // Check voter status and mark the record
            if (printerStatus == true)
            {
                // Skip marking voter and printing ballot while developing questionnaire
                voterStatus = true;

                //voterStatus = await MarkVoter(AppSettings.ConnectionStringLocal);

                // Print ballot, application, permit, and/or stub
                if (voterStatus == true)
                {
                    PrintApplication();

                    // Print signature form if sign refused -- maybe put this inside the Ballot Bundle function

                    // Navigate to print troubleshooting page -- Navigation should not wait on the ballot to print
                    //_parent.Navigate(new TroubleShooting.OfficialPrintTroubleShootingPage(_parent));
                }
            }

            CanPrintBallot = true;
            UIServices.SetBusyState(false);
        }
        #endregion

        #region PrinterStatusMethods
        // Return true if Printer is Ready
        private async Task<bool> CheckBallotPrinter()
        {
            bool result = await StatusBar.CheckPrinter(AppSettings.Printers);

            return result;
        }
        #endregion

        #region VoterMethods
        // Checks if the voter record is valid and attempts to updated it as Voted
        // Returns true when the voter has been marked
        private async Task<bool> MarkVoter()
        {
            bool result = false;

            // Check a second time to see if the voter status has changed before printing an official ballot
            var currentLogCode = await CheckCurrentLogCode();
            if (currentLogCode < 5)
            {
                string errorMessage = await MarkVotedAtPolls();

                if (errorMessage != null)
                {
                    // Display Error Message
                    StatusBar.TextCenter = errorMessage;
                    result = false;
                }
                else
                {
                    result = true;
                }
            }

            StatusBar.HideLeftSpinner();

            return result;
        }

        // Returns the voter's current log code
        // If the voter's record has been updated in the time since the record was selected on the search screen
        // this will prevent the record from being changed
        private async Task<int?> CheckCurrentLogCode()
        {
            return await Task.Run(() =>
            {
                //ElectionContextFactory contextFactory = new ElectionContextFactory(Connection);
                return VoterItem.CheckLogCode();
            });
        }

        // Attempts to update the voters record and mark them as voted
        // Returns an error message if the update fails
        private async Task<string> MarkVotedAtPolls()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Set local system values
                    VoterItem.Localize(AppSettings.Global);

                    // Get next ballot number
                    VoterItem.GetNextBallotNumber((int)AppSettings.System.SiteID);

                    // Update the voter record as Early Voting
                    VoterItem.VotedAtPolls(AppSettings.System.VCCType.ToInt());

                    return null;
                }
                catch (Exception e)
                {
                    //throw new Exception("Record could not be written");
                    e.Message.ToString();
                    return "Record could not be written";
                }
            });
        }
        #endregion

        #region BallotPrinting
        // Attempts to print the ballot and other official documents
        // Returns true when all documents have been printed
        private async Task<bool> PrintOfficialBallotBundle()
        {
            // REBUILD BALLOTPRINTING METHODS FROM VoterX.REVISED.12.0.45.0

            //var errorMessage = await BallotPrinting.PrintOfficialBallotBundleAsync(VoterItem, AppSettings.Global);
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintBallot(VoterItem.Data, AppSettings.Global));

            // Print Application for Early Voting
            if (AppSettings.Global.System.VCCType == VotingCenterMode.EarlyVoting)
            {
                //ApplicationReport application = GeneralReportFactory.GetReport("Early Voting Application") as ApplicationReport;
                //string message = application.PrintReport(VoterItem, AppSettings.Global);

                errorMessage = await Task.Run(() => BallotPrinting.ReprintApplication(VoterItem.Data, AppSettings.Global));
            }
            // Print Permit for Election Day
            else if (AppSettings.Global.System.VCCType == VotingCenterMode.ElectionDay)
            {
                //PermitReport permit = GeneralReportFactory.GetReport("Voting Permit") as PermitReport;
                //string message = permit.PrintReport(VoterItem, AppSettings.Global, true);

                errorMessage = await Task.Run(() => BallotPrinting.ReprintPermit(VoterItem.Data, AppSettings.Global));

                // Print Stub
                if (AppSettings.Global.System.BallotStub == 1)
                {
                    //BallotStubReport stub = GeneralReportFactory.GetReport("Ballot Stub") as BallotStubReport;
                    //message = stub.PrintReport(VoterItem, AppSettings.Global);

                    errorMessage = await Task.Run(() => BallotPrinting.ReprintStub(VoterItem.Data, AppSettings.Global));
                }

                // Print Signature Form
                if (VoterItem.Data.SignRefused == true)
                {
                    //SignatureFormReport signature = GeneralReportFactory.GetReport("Signature Form") as SignatureFormReport;
                    //message = signature.PrintReport(VoterItem, AppSettings.Global);

                    errorMessage = await Task.Run(() => BallotPrinting.PrintSignatureForm(VoterItem.Data, AppSettings.Global));
                }
            }

            if (errorMessage != null)
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
                return false;
            }
            else
            {
                return true;
            }
        }

        private void PrintApplication()
        {
            //AppSettings.Global.System.SiteID = 49;
            //AppSettings.Global.System.SiteName = "Testing";

            //ApplicationReport application = GeneralReportFactory.GetReport("Early Voting Application") as ApplicationReport;
            //string message = application.PreviewReport(VoterItem, AppSettings.Global);

            //PermitReport permit = GeneralReportFactory.GetReport("Voting Permit") as PermitReport;
            //string message = permit.PrintReport(VoterItem, AppSettings.Global, true);

            //BallotStubReport stub = GeneralReportFactory.GetReport("Ballot Stub") as BallotStubReport;
            //string message = stub.PrintReport(VoterItem, AppSettings.Global);

            //SignatureFormReport signature = GeneralReportFactory.GetReport("Signature Form") as SignatureFormReport;
            //string message = signature.PrintReport(VoterItem, AppSettings.Global);

            //AddressLabelReport address = GeneralReportFactory.GetReport("Address Label") as AddressLabelReport;
            //string message = address.PrintReport(VoterItem, AppSettings.Global);

            //var report = DailyReportFactory.GetReport("Daily Summary Report");
            //string message = report.PrintReport(DateTime.Parse("10/9/2018"), AppSettings.Global);

            //var report = DailyReportFactory.GetReport("End Of Day Report", AppSettings.Global);
            //string message = report.PreviewReport(DateTime.Parse("10/9/2018"), AppSettings.Global);

            //GlobalReferences.StatusBar.TextCenter = message;
        }
        #endregion
    }
}
