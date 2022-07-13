using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoterX.Core.Voters;
using VoterX.SystemSettings.Models.TroubleShooting;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Controls;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Troubleshooting
{
    public class ProvisionalPrintTroubleShootingViewModel : NotifyPropertyChanged
    {
        private NMVoter _voter;

        public ProvisionalPrintTroubleShootingViewModel(NMVoter voter)
        {
            _voter = voter;

            // Display Header
            StatusBar.PageHeader = "Provisional Print Verification";
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

            //MainMenuMethods.OpenMenu();
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

            Console.WriteLine("Ballot Reprinted");

            // Reprint the Ballot
            var errorMessage = await Task.Run(() => BallotPrinting.PrintProvisionalBallot(_voter.Data, AppSettings.Global));

            if (errorMessage != null)
            {
                // Display Error Message
                StatusBar.TextCenter = errorMessage;
            }

            // Reset the Questionnaire
            ResetBallotPrintedQuestionnaire();

            CanReprintBallot = true;
            RaisePropertyChanged("CanReprintBallot");
        }

        public bool ReprintBallotVisibility { get; set; }
        private void SetReprintBallotVisibility(bool vis)
        {
            ReprintBallotVisibility = vis;
            RaisePropertyChanged("ReprintBallotVisibility");
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
                        ReportMessage = "THE PROVISIONAL BALLOT HAS BEEN SENT TO THE PRINTER",
                        ReprintMessage = "A NEW PROVISIONAL BALLOT HAS BEEN SENT TO THE PRINTER",
                        ReportQuestion = "Did the provisional ballot print properly?",
                        PrinterMessage = "GO THROUGH THE BASIC PRINTER TROUBLESHOOTING STEPS",
                        PrinterQuestion = "After troubleshooting the printer did the ballot print properly?",
                        OptionsMessage = "CHOOSE ONE OF THE FOLLOWING OPTIONS TO PROCEED",
                        ReprintChoiceMessage = "I want to attempt to print the provisional ballot again.",
                        ExitChoiceMessage = "I want to process the voter on another computer and return to the search screen.",
                        FinalMessage = "GOODBYE"
                    };

                    // Create and populate questionnaire
                    _ballotPrintedQuestionnaire = new PrintVerificationQuestionnaireViewModel(BallotTroubleshootingQuestionnaire, false);

                    // Bind onchanged method
                    _ballotPrintedQuestionnaire.PropertyChanged += OnBallotPrintedQuestionnairePropertyChanged;
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
                    SetGoBackVisibility(true);
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
                    CanReprintBallot = true;
                    RaisePropertyChanged("CanReprintBallot");

                    SetReprintBallotVisibility(true); 
                }
            }
        }

        private void ResetBallotPrintedQuestionnaire()
        {
            _ballotPrintedQuestionnaire.Reset();
        }
        #endregion
    }
}
