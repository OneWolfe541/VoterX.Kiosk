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
using VoterX.Utilities.Extensions;
using VoterX.Core.Reconciles;
using VoterX.Kiosk.Methods;
using VoterX.SystemSettings.Models;
using VoterX.Utilities.Dialogs;
using VoterX.SystemSettings.Enums;

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for ApplicationCountPage.xaml
    /// </summary>
    public partial class ApplicationCountPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        private bool _skipTextChanged = false;

        public ApplicationCountPage()
        {
            InitializeComponent();
        }

        public ApplicationCountPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new ProvisionalBallotCountPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //var test = _reconcile.CompareRegular();

            if (_reconcile.RegularMatch == true)
            {
                this.NavigateToPage(new HandTallyCountPage(_displayText, _reconcile));
            }
            else
            {
                AreYouSureDialog signatureDialog = new AreYouSureDialog("ARE YOU SURE?",
                    string.Format(
                    DisplayTextMethods.ParseReconcile(_displayText.AreYouSureWarning, _reconcile)
                    , DisplayTextMethods.ApplicationType()));
                if (signatureDialog.ShowDialog() == true)
                {
                    this.NavigateToPage(new HandTallyCountPage(_displayText, _reconcile));
                }

                //StatusBar.StatusTextLeft = "ARE YOU SURE";

                //OfficialBallotsDEM.Focus();
                //OfficialBallotsDEM.SelectAll();
            }
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageHeader + " - PRIMARY", _reconcile);
            }
            else
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageHeader, _reconcile);
            }

            try // If ProvisionalPageBoldLine1 is null ToUpper will fail
            {
                ApplicationPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            if (_reconcile.Data != null)
            {
                _skipTextChanged = true;
                DetailList.ItemsSource = _reconcile.Details.OrderBy(d => d.Party);
                OfficialBallots.Text = _reconcile.Details.Sum(d => d.Regular).ToString();
                //OfficialBallots.Text = _reconcile.Data.Regular.ToString();
                //OfficialBallotsDEM.Text = _reconcile.Data.RegularDEM.ToString();
                //OfficialBallotsREP.Text = _reconcile.Data.RegularREP.ToString();
                //OfficialBallotsLIB.Text = _reconcile.Data.RegularLIB.ToString();
                _skipTextChanged = false;

                ApplicationPageInstructions2.Text = "For comparison, VoterX shows that you have ";
                string documentType = "";
                if (AppSettings.System.VCCType == VotingCenterMode.EarlyVoting)
                {
                    documentType = "Applications";
                }
                else
                {
                    documentType = "Permits";
                }

                if (_reconcile.ComputerDetails.Count > 1)
                {
                    var last = _reconcile.ComputerDetails.Last();
                    foreach (var calcDetail in _reconcile.ComputerDetails)
                    {
                        ApplicationPageInstructions2.Text += calcDetail.Regular.ToString() + " " + calcDetail.Party + " " + documentType;
                        if (calcDetail != last)
                        {
                            ApplicationPageInstructions2.Text += ", ";
                        }
                        else
                        {
                            ApplicationPageInstructions2.Text += ".";
                        }
                    }
                }
                else
                {
                    var calcDetail = _reconcile.ComputerDetails.FirstOrDefault();
                    ApplicationPageInstructions2.Text += calcDetail.Regular.ToString() + " " + documentType + ".";

                    // Hide Total row
                    OfficialTotalGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                //OfficialBallots.Text = "0";
                //OfficialBallotsDEM.Text = "0";
                //OfficialBallotsREP.Text = "0";
                //OfficialBallotsLIB.Text = "0";
            }

            ApplicationPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageInstructions1, _reconcile);

            //ApplicationPageInstructions2.Text = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageInstructions2, _reconcile);
            //if (AppSettings.System.VCCType == VotingCenterMode.EarlyVoting)
            //{
            //    ApplicationPageInstructions2.Text = "For comparison, VoterX shows that you have " + _reconcile.Data.ComputerRegularDEM.ToString() + " DEM Applications, " + _reconcile.Data.ComputerRegularREP.ToString() + " REP Applications, " + _reconcile.Data.ComputerRegularLIB.ToString() + " LIB Applications.";
            //}
            //else
            //{
            //    ApplicationPageInstructions2.Text = "For comparison, VoterX shows that you have " + _reconcile.Data.ComputerRegularDEM.ToString() + " DEM Permits, " + _reconcile.Data.ComputerRegularREP.ToString() + " REP Permits, " + _reconcile.Data.ComputerRegularLIB.ToString() + " LIB Permits.";
            //}

            ApplicationPageHelpLink1.Text = DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageHelpLink1, _reconcile);

            //OfficialBallotsDEM.Focus();
            //OfficialBallotsDEM.SelectAll();
        }

        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.ApplicationPageHelpDialog1, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void OfficialBallots_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipTextChanged == false)
            {
                int total = 0;

                //// Check if the text entered is a valid number
                //if (Int32.TryParse(OfficialBallots.Text, out int value) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.Regular = value;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(OfficialBallotsDEM.Text, out int valueDEM) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.RegularDEM = valueDEM;
                //    total += valueDEM;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(OfficialBallotsREP.Text, out int valueREP) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.RegularREP = valueREP;
                //    total += valueREP;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(OfficialBallotsLIB.Text, out int valueLIB) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.RegularLIB = valueLIB;
                //    total += valueLIB;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                OfficialBallots.Text = total.ToString();
                //_reconcile.Data.Regular = total;
            }
        }

        private void OfficialBallots_LostFocus(object sender, RoutedEventArgs e)
        {
            OfficialBallots.Text = _reconcile.Details.Sum(d => d.Regular).ToString();
        }
    }
}
