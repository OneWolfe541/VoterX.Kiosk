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

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for ProvisionalBallotCountPage.xaml
    /// </summary>
    public partial class ProvisionalBallotCountPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        private bool _skipTextChanged = false;

        public ProvisionalBallotCountPage()
        {
            InitializeComponent();
        }

        public ProvisionalBallotCountPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SpoiledBallotCountPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reconcile.ProvisionalMatch == true)
            {
                this.NavigateToPage(new ApplicationCountPage(_displayText, _reconcile));
            }
            else
            {
                AreYouSureDialog signatureDialog = new AreYouSureDialog("ARE YOU SURE?",
                    string.Format(
                    DisplayTextMethods.ParseReconcile(_displayText.AreYouSureWarning, _reconcile)
                    , "Provisional Ballot"));
                if (signatureDialog.ShowDialog() == true)
                {
                    this.NavigateToPage(new ApplicationCountPage(_displayText, _reconcile));
                }

                //StatusBar.StatusTextLeft = "ARE YOU SURE";

                //ProvisionalBallotsDEM.Focus();
                //ProvisionalBallotsDEM.SelectAll();
            }
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = _displayText.ProvisionalPageHeader + " - PRIMARY";
            }
            else
            {
                StatusBar.PageHeader = _displayText.ProvisionalPageHeader;
            }

            try // If ProvisionalPageBoldLine1 is null ToUpper will fail
            {
                ProvisionalPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.ProvisionalPageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            if (_reconcile.Data != null)
            {
                _skipTextChanged = true;
                DetailList.ItemsSource = _reconcile.Details.OrderBy(d => d.Party);
                ProvisionalBallots.Text = _reconcile.Details.Sum(d => d.Provisional).ToString();
                //ProvisionalBallots.Text = _reconcile.Data.Provisional.ToString();
                //ProvisionalBallotsDEM.Text = _reconcile.Data.ProvisionalDEM.ToString();
                //ProvisionalBallotsREP.Text = _reconcile.Data.ProvisionalREP.ToString();
                //ProvisionalBallotsLIB.Text = _reconcile.Data.ProvisionalLIB.ToString();
                _skipTextChanged = false;

                // Display VoterX totals for each party
                ProvisionalPageInstructions2.Text = "For comparison, VoterX shows that you have ";
                if (_reconcile.Details.Count > 1)
                {
                    var last = _reconcile.ComputerDetails.Last();
                    foreach (var calcDetail in _reconcile.ComputerDetails)
                    {
                        ProvisionalPageInstructions2.Text += calcDetail.Provisional.ToString() + " Provisional " + calcDetail.Party + " Ballots";
                        if (calcDetail != last)
                        {
                            ProvisionalPageInstructions2.Text += ", ";
                        }
                        else
                        {
                            ProvisionalPageInstructions2.Text += ".";
                        }
                    }
                }
                else
                {
                    var calcDetail = _reconcile.ComputerDetails.FirstOrDefault();
                    ProvisionalPageInstructions2.Text += calcDetail.Provisional.ToString() + " Provisional Ballots.";
                    
                    // Hide Total row
                    ProvisionalTotalGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                //ProvisionalBallots.Text = "0";
                //ProvisionalBallotsDEM.Text = "0";
                //ProvisionalBallotsREP.Text = "0";
                //ProvisionalBallotsLIB.Text = "0";
            }

            ProvisionalPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.ProvisionalPageInstructions1, _reconcile);

            //ProvisionalPageInstructions2.Text = DisplayTextMethods.ParseReconcile(_displayText.ProvisionalPageInstructions2, _reconcile);
            //ProvisionalPageInstructions2.Text = "For comparison, VoterX shows that you have " + _reconcile.Data.ComputerProvisionalDEM.ToString() + " Provisional DEM Ballots, " + _reconcile.Data.ComputerProvisionalREP.ToString() + " Provisional REP Ballots, " + _reconcile.Data.ComputerProvisionalLIB.ToString() + " Provisional LIB Ballots.";

            ProvisionalPageHelpLink1.Text = DisplayTextMethods.ParseReconcile(_displayText.ProvisionalPageHelpLink1, _reconcile);

            //ProvisionalBallotsDEM.Focus();
            //ProvisionalBallotsDEM.SelectAll();
        }

        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.ProvisionalPageHelpDialog1, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void ProvisionalBallots_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipTextChanged == false)
            {
                int total = 0;

                // Check if the text entered is a valid number
                //if (Int32.TryParse(ProvisionalBallots.Text, out int value) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.Provisional = value;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(ProvisionalBallotsDEM.Text, out int valueDEM) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.ProvisionalDEM = valueDEM;
                //    total += valueDEM;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(ProvisionalBallotsREP.Text, out int valueREP) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.ProvisionalREP = valueREP;
                //    total += valueREP;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(ProvisionalBallotsLIB.Text, out int valueLIB) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.ProvisionalLIB = valueLIB;
                //    total += valueLIB;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                ProvisionalBallots.Text = total.ToString();
                //_reconcile.Data.Provisional = total;
            }
        }

        private void ProvisionalBallots_LostFocus(object sender, RoutedEventArgs e)
        {
            ProvisionalBallots.Text = _reconcile.Details.Sum(d => d.Provisional).ToString();
        }
    }
}
