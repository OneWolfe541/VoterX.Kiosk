using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Enums;
using VoterX.SystemSettings.Models.TroubleShooting;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Troubleshooting
{
    public class OfficialPrintTroubleShootingViewModel : NotifyPropertyChanged
    {
        private NMVoter _voter;

        private bool ElectionDay { get; set; }
        private bool StubAllowed { get; set; }
        //private bool SignRefused { get; set; }

        public OfficialPrintTroubleShootingViewModel(NMVoter voter)
        {
            _voter = voter;

            ElectionDay = AppSettings.System.VCCType == VotingCenterMode.ElectionDay ? true : false;
            StubAllowed = AppSettings.System.BallotStub == 1 ? true : false;

            // ASK IF WE ACTUALLY WANT TO TROUBLE SHOOT THE SIGNATURE FORM
            // ANSWER: DONT CHECK IF THE SIGNATURE FORM PRINTED
            //SignRefused = voter.Data.SignRefused;

            GoBackVisibility = false;
            ReprintBallotVisibility = false;

            // Display Header
            StatusBar.PageHeader = "Print Verification";
        }

        #region DisplayText
        public string BallotStyleName
        {
            get
            {
                return _voter.Data.BallotStyle;
            }
        }
        public string BallotNumber
        {
            get
            {
                if (_voter.Data.BallotNumber != null)
                {
                    return _voter.Data.BallotNumber.ToString();
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region ButtonCommands
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
            //_parent.Navigate(new Voter.VoterSearchPage(_parent));
            //NavigationMenuMethods.VoterSearchPage();
            NavigationMenuMethods.ReturnToOrigin();
        }

        public bool GoBackVisibility { get; set; }
        private void SetGoBackVisibility(bool vis)
        {
            GoBackVisibility = vis;
            RaisePropertyChanged("GoBackVisibility");
        }

        // Bound command for reprinting the ballot
        public RelayCommand _reprintBallotCommand;
        public ICommand ReprintBallotCommand
        {
            get
            {
                if (_reprintBallotCommand == null)
                {
                    _reprintBallotCommand = new RelayCommand(param => this.ReprintBallotClick(), param => this.CanReprintBallot);
                }
                return _reprintBallotCommand;
            }
        }

        public bool CanReprintBallot { get; set; }

        // Reprint the ballot
        public async void ReprintBallotClick() 
        {
            CanReprintBallot = false;
            RaisePropertyChanged("CanReprintBallot");

            OkCancelDialog spoilDialog = new OkCancelDialog("Spoil The Ballot", "A NEW BALLOT WILL BE SENT TO THE PRINTER\r\nAND THE PREVIOUS BALLOT WILL BE SPOILED.\r\n\r\nMAKE SURE TO FOLLOW THE PROPER PROCEDURES\r\nTO SPOIL THE OLD BALLOT.");
            if (spoilDialog.ShowDialog() == true)
            {
                // Ensure that local values are set (This should already happen on the previous page)
                _voter.Localize(AppSettings.Global);

                // Spoil the ballot
                _voter.SpoilBallot(1);

                // REMOVED BY JOHN 12/29/2020 "Spoiled Ballots should not increment the ballot numbers"
                // Update Ballot Number
                //_voter.GetNextBallotNumber((int)AppSettings.System.SiteID);
                //_voter.UpdateBallotNumber();
                //RaisePropertyChanged("BallotNumber");

                Console.WriteLine("Ballot Reprinted");

                // Reprint the Ballot
                var errorMessage = await Task.Run(() => BallotPrinting.ReprintBallot(_voter.Data, AppSettings.Global));

                if (errorMessage != null)
                {
                    // Display Error Message
                    StatusBar.TextCenter = errorMessage;
                }

                // Reset the Questionnaire
                ResetBallotPrintedQuestionnaire();
            }
            else
            {
                ReprintBallotVisibility = false;
                RaisePropertyChanged("ReprintBallotVisibility");
                GoBackVisibility = true;
                RaisePropertyChanged("GoBackVisibility");
            }
        }

        public bool ReprintBallotVisibility { get; set; }
        private void SetReprintBallotVisibility(bool vis)
        {
            ReprintBallotVisibility = vis;
            RaisePropertyChanged("ReprintBallotVisibility");
        }

        // Bound command for reprinting the application
        public RelayCommand _reprintApplicationCommand;
        public ICommand ReprintApplicationCommand
        {
            get
            {
                if (_reprintApplicationCommand == null)
                {
                    _reprintApplicationCommand = new RelayCommand(param => this.ReprintApplicationClick());
                }
                return _reprintApplicationCommand;
            }
        }

        // Reprint the application
        public async void ReprintApplicationClick()
        {
            Console.WriteLine("Application Reprinted");

            // Reprint the Application
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintApplication(_voter.Data, AppSettings.Global));

            if (errorMessage != null)
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Reset the Questionnaire
            ResetApplicationPrintedQuestionnaire();
        }

        public bool ReprintApplicationVisibility { get; set; }
        private void SetReprintApplicationVisibility(bool vis)
        {
            ReprintApplicationVisibility = vis;
            RaisePropertyChanged("ReprintApplicationVisibility");
        }

        // Bound command for reprinting the permit
        public RelayCommand _reprintPermitCommand;
        public ICommand ReprintPermitCommand
        {
            get
            {
                if (_reprintPermitCommand == null)
                {
                    _reprintPermitCommand = new RelayCommand(param => this.ReprintPermitClick());
                }
                return _reprintPermitCommand;
            }
        }

        // Reprint the application
        public async void ReprintPermitClick()
        {
            Console.WriteLine("Permit Reprinted");

            // Reprint the Permit
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintPermit(_voter.Data, AppSettings.Global));

            if (errorMessage != null)
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Reset the Questionnaire
            ResetPermitPrintedQuestionnaire();
        }

        public bool ReprintPermitVisibility { get; set; }
        private void SetReprintPermitVisibility(bool vis)
        {
            ReprintPermitVisibility = vis;
            RaisePropertyChanged("ReprintPermitVisibility");
        }

        // Bound command for reprinting the stub
        public RelayCommand _reprintStubCommand;
        public ICommand ReprintStubCommand
        {
            get
            {
                if (_reprintStubCommand == null)
                {
                    _reprintStubCommand = new RelayCommand(param => this.ReprintStubClick());
                }
                return _reprintStubCommand;
            }
        }

        // Reprint the stub
        public async void ReprintStubClick()
        {
            Console.WriteLine("Stub Reprinted");

            // Reprint the Stub
            var errorMessage = await Task.Run(() => BallotPrinting.ReprintStub(_voter.Data, AppSettings.Global));

            if (errorMessage != null)
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Reset the Questionnaire
            ResetStubPrintedQuestionnaire();
        }

        public bool ReprintStubVisibility { get; set; }
        private void SetReprintStubVisibility(bool vis)
        {
            ReprintStubVisibility = vis;
            RaisePropertyChanged("ReprintStubVisibility");
        }
        #endregion

        #region BallotPrintedQuestionnaire
        private PrintVerificationQuestionnaireViewModel _ballotPrintedQuestionnaire;
        public PrintVerificationQuestionnaireViewModel BallotPrintedQuestionnaire
        {
            get
            {
                if (_ballotPrintedQuestionnaire == null)
                {
                    BallotTroubleshootingQuestionnaireText BallotTroubleshootingQuestionnaire = new BallotTroubleshootingQuestionnaireText
                    {
                        ReportMessage = "THE BALLOT HAS BEEN SENT TO THE PRINTER",
                        ReprintMessage = "A NEW BALLOT HAS BEEN SENT TO THE PRINTER",
                        ReportQuestion = "Did the ballot print properly?",
                        PrinterMessage = "GO THROUGH THE BASIC PRINTER TROUBLESHOOTING STEPS",
                        PrinterQuestion = "After troubleshooting the printer did the ballot print properly?",
                        OptionsMessage = "CHOOSE ONE OF THE FOLLOWING OPTIONS TO PROCEED",
                        ReprintChoiceMessage = "I want to attempt to print the ballot again.",
                        ExitChoiceMessage = "I want to process the voter on another computer and return to the search screen.",
                        FinalMessage = "GOODBYE"
                    };

                    // Create and populate questionnaire
                    _ballotPrintedQuestionnaire = new PrintVerificationQuestionnaireViewModel(BallotTroubleshootingQuestionnaire, false);

                    // Bind onchanged method
                    _ballotPrintedQuestionnaire.PropertyChanged += OnBallotPrintedQuestionnairePropertyChanged;

                    CanReprintBallot = true;
                    RaisePropertyChanged("CanReprintBallot");
                }
                return _ballotPrintedQuestionnaire;
            }
        }

        private void OnBallotPrintedQuestionnairePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get questionnaire response (YES/NO)
            if (e.PropertyName == "Response")
            {
                // When any property is changed reset all the buttons
                // then determine which buttons to activate based on the responses
                SetGoBackVisibility(false);

                // When YES is clicked then display the next questionnaire
                if (((PrintVerificationQuestionnaireViewModel)sender).Response == true)
                {
                    if (ElectionDay == true)
                    {
                        // Create a new questionnaire for the permit
                        CreatePermitPrintedQuestionnaire();
                        RaisePropertyChanged("PermitPrintedQuestionnaire");
                    }
                    else
                    {
                        // Create a new questionnaire for the application
                        CreateApplicationPrintedQuestionnaire();
                        RaisePropertyChanged("ApplicationPrintedQuestionnaire");
                    }
                }
                else if (((PrintVerificationQuestionnaireViewModel)sender).Response == false)
                {
                    SetGoBackVisibility(true);
                }
            }

            // Get questionnaire response to reprint the ballot
            if (e.PropertyName == "Reprint")
            {
                SetReprintBallotVisibility(false);

                if (((PrintVerificationQuestionnaireViewModel)sender).Reprint == true)
                {
                    SetReprintBallotVisibility(true);
                }
            }
        }
        
        private void ResetBallotPrintedQuestionnaire()
        {
            _ballotPrintedQuestionnaire.Reset();

            CanReprintBallot = true;
            RaisePropertyChanged("CanReprintBallot");
        }
        #endregion

        #region ApplicationPrintedQuestionnaire
        private PrintVerificationQuestionnaireViewModel _applicationPrintedQuestionnaire;
        public PrintVerificationQuestionnaireViewModel ApplicationPrintedQuestionnaire
        {
            get
            {
                return _applicationPrintedQuestionnaire;
            }
        }

        private void CreateApplicationPrintedQuestionnaire()
        {
            if (_applicationPrintedQuestionnaire == null)
            {
                ApplicationTroubleshootingQuestionnaireText ApplicationTroubleshootingQuestionnaire = new ApplicationTroubleshootingQuestionnaireText
                {
                    ReportMessage = "THE APPLICATION HAS BEEN SENT TO THE PRINTER",
                    ReprintMessage = "A NEW APPLICATION HAS BEEN SENT TO THE PRINTER",
                    ReportQuestion = "Did the application print properly?",
                    PrinterMessage = "GO THROUGH THE BASIC PRINTER TROUBLESHOOTING STEPS",
                    PrinterQuestion = "After troubleshooting the printer did the application print properly?",
                    OptionsMessage = "CHOOSE ONE OF THE FOLLOWING OPTIONS TO PROCEED",
                    ReprintChoiceMessage = "I want to attempt to print the application again.",
                    ExitChoiceMessage = "I want to process the voter on another computer and return to the search screen.",
                    FinalMessage = "GOODBYE"
                };

                // Create and populate questionnaire
                _applicationPrintedQuestionnaire = new PrintVerificationQuestionnaireViewModel(ApplicationTroubleshootingQuestionnaire, true);

                // Disable the second question
                _applicationPrintedQuestionnaire.TroubleshootQuestionEnabled = false;

                // Bind onchanged method
                _applicationPrintedQuestionnaire.PropertyChanged += OnApplicationPrintedQuestionnairePropertyChanged;
            }
        }

        private void OnApplicationPrintedQuestionnairePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get questionnaire response (YES/NO)
            if (e.PropertyName == "Response")
            {
                // When any property is changed reset all the buttons
                // then determine which buttons to activate based on the responses
                SetGoBackVisibility(false);

                // When YES is clicked then display the next questionnaire
                if (((PrintVerificationQuestionnaireViewModel)sender).Response == true)
                {
                    if (StubAllowed == true)
                    {
                        // Create and populate stub questionnaire
                        CreateStubPrintedQuestionnaire();
                        RaisePropertyChanged("StubPrintedQuestionnaire");
                    }
                    else
                    {
                        // Finish
                        SetGoBackVisibility(true);
                    }
                }
                else if (((PrintVerificationQuestionnaireViewModel)sender).Response == false)
                {
                    SetGoBackVisibility(true);
                }
            }

            // Get questionnaire response to reprint the application
            if (e.PropertyName == "Reprint")
            {
                SetReprintApplicationVisibility(false);

                if (((PrintVerificationQuestionnaireViewModel)sender).Reprint == true)
                {
                    SetReprintApplicationVisibility(true);
                }
            }
        }

        private void ResetApplicationPrintedQuestionnaire()
        {
            _applicationPrintedQuestionnaire.Reset();
        }
        #endregion

        #region PermitPrintedQuestionnaire
        private PrintVerificationQuestionnaireViewModel _permitPrintedQuestionnaire;
        public PrintVerificationQuestionnaireViewModel PermitPrintedQuestionnaire
        {
            get
            {
                return _permitPrintedQuestionnaire;
            }
        }

        private void CreatePermitPrintedQuestionnaire()
        {
            if (_permitPrintedQuestionnaire == null)
            {
                PermitTroubleshootingQuestionnaireText PermitTroubleshootingQuestionnaire = new PermitTroubleshootingQuestionnaireText
                {
                    ReportMessage = "THE PERMIT HAS BEEN SENT TO THE PRINTER",
                    ReprintMessage = "A NEW PERMIT HAS BEEN SENT TO THE PRINTER",
                    ReportQuestion = "Did the permit print properly?",
                    PrinterMessage = "GO THROUGH THE BASIC PRINTER TROUBLESHOOTING STEPS",
                    PrinterQuestion = "After troubleshooting the printer did the permit print properly?",
                    OptionsMessage = "CHOOSE ONE OF THE FOLLOWING OPTIONS TO PROCEED",
                    ReprintChoiceMessage = "I want to attempt to print the permit again.",
                    ExitChoiceMessage = "I want to process the voter on another computer and return to the search screen.",
                    FinalMessage = "GOODBYE"
                };

                // Create and populate questionnaire
                _permitPrintedQuestionnaire = new PrintVerificationQuestionnaireViewModel(PermitTroubleshootingQuestionnaire, true);

                // Disable the second question
                _permitPrintedQuestionnaire.TroubleshootQuestionEnabled = false;

                // Bind onchanged method
                _permitPrintedQuestionnaire.PropertyChanged += OnPermitPrintedQuestionnairePropertyChanged;
            }
        }

        private void OnPermitPrintedQuestionnairePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get questionnaire response (YES/NO)
            if (e.PropertyName == "Response")
            {
                // When any property is changed reset all the buttons
                // then determine which buttons to activate based on the responses
                SetGoBackVisibility(false);

                // When YES is clicked then display the next questionnaire
                if (((PrintVerificationQuestionnaireViewModel)sender).Response == true)
                {
                    if (StubAllowed == true)
                    {
                        // Create and populate stub questionnaire
                        CreateStubPrintedQuestionnaire();
                        RaisePropertyChanged("StubPrintedQuestionnaire");
                    }
                    else
                    {
                        // Finish
                        SetGoBackVisibility(true);
                    }
                }
                else if (((PrintVerificationQuestionnaireViewModel)sender).Response == false)
                {
                    SetGoBackVisibility(true);
                }
            }

            // Get questionnaire response to reprint the permit
            if (e.PropertyName == "Reprint")
            {
                SetReprintPermitVisibility(false);

                if (((PrintVerificationQuestionnaireViewModel)sender).Reprint == true)
                {
                    SetReprintPermitVisibility(true);
                }
            }
        }

        private void ResetPermitPrintedQuestionnaire()
        {
            _permitPrintedQuestionnaire.Reset();
        }
        #endregion

        #region StubPrintedQuestionnaire
        private PrintVerificationQuestionnaireViewModel _stubPrintedQuestionnaire;
        public PrintVerificationQuestionnaireViewModel StubPrintedQuestionnaire
        {
            get
            {
                return _stubPrintedQuestionnaire;
            }
        }

        private void CreateStubPrintedQuestionnaire()
        {
            if (_stubPrintedQuestionnaire == null)
            {
                StubTroubleshootingQuestionnaireText StubTroubleshootingQuestionnaire = new StubTroubleshootingQuestionnaireText
                {
                    ReportMessage = "THE STUB HAS BEEN SENT TO THE PRINTER",
                    ReprintMessage = "A NEW STUB HAS BEEN SENT TO THE PRINTER",
                    ReportQuestion = "Did the stub print properly?",
                    PrinterMessage = "GO THROUGH THE BASIC PRINTER TROUBLESHOOTING STEPS",
                    PrinterQuestion = "After troubleshooting the printer did the stub print properly?",
                    OptionsMessage = "CHOOSE ONE OF THE FOLLOWING OPTIONS TO PROCEED",
                    ReprintChoiceMessage = "I want to attempt to print the stub again.",
                    ExitChoiceMessage = "I want to process the voter on another computer and return to the search screen.",
                    FinalMessage = "GOODBYE"
                };

                // Create and populate questionnaire
                _stubPrintedQuestionnaire = new PrintVerificationQuestionnaireViewModel(StubTroubleshootingQuestionnaire, true);

                // Disable the second question
                _stubPrintedQuestionnaire.TroubleshootQuestionEnabled = false;

                // Bind onchanged method
                _stubPrintedQuestionnaire.PropertyChanged += OnStubPrintedQuestionnairePropertyChanged;
            }
        }

        private void OnStubPrintedQuestionnairePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get questionnaire response (YES/NO)
            if (e.PropertyName == "Response")
            {
                // When any property is changed reset all the buttons
                // then determine which buttons to activate based on the responses
                SetGoBackVisibility(false);

                // When YES is clicked then display the next questionnaire
                if (((PrintVerificationQuestionnaireViewModel)sender).Response == true)
                {
                    // Finish
                    SetGoBackVisibility(true);
                }
                else if (((PrintVerificationQuestionnaireViewModel)sender).Response == false)
                {
                    SetGoBackVisibility(true);
                }
            }

            // Get questionnaire response to reprint the stub
            if (e.PropertyName == "Reprint")
            {
                SetReprintStubVisibility(false);

                if (((PrintVerificationQuestionnaireViewModel)sender).Reprint == true)
                {
                    SetReprintStubVisibility(true);
                }
            }
        }

        private void ResetStubPrintedQuestionnaire()
        {
            _stubPrintedQuestionnaire.Reset();
        }
        #endregion
    }
}
