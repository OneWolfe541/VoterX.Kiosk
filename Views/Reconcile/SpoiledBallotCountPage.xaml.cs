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
using VoterX.SystemSettings.Models;
using VoterX.SystemSettings.Methods;
using VoterX.Core.Reconciles;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Dialogs;

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for SpoiledBallotCountPage.xaml
    /// </summary>
    public partial class SpoiledBallotCountPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        private bool _skipTextChanged = false;

        public SpoiledBallotCountPage()
        {
            InitializeComponent();
        }

        public SpoiledBallotCountPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new ReconcileStartPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reconcile.SpoiledMatch == true)
            {
                this.NavigateToPage(new ProvisionalBallotCountPage(_displayText, _reconcile));
            }
            else
            {
                AreYouSureDialog areYouSureDialog = new AreYouSureDialog("ARE YOU SURE?",
                    string.Format(
                    DisplayTextMethods.ParseReconcile(_displayText.AreYouSureWarning, _reconcile)
                    , "Spoiled Ballot"));
                if(areYouSureDialog.ShowDialog()==true)
                {
                    this.NavigateToPage(new ProvisionalBallotCountPage(_displayText, _reconcile));
                }

                //StatusBar.StatusTextLeft = "ARE YOU SURE";

                //SpoiledBallotsDEM.Focus();
                //SpoiledBallotsDEM.SelectAll();
            }
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = _displayText.SpoiledPageHeader + " - PRIMARY";
            }
            else
            {
                StatusBar.PageHeader = _displayText.SpoiledPageHeader;
            }

            try // If SpoiledPageBoldLine1 is null ToUpper will fail
            {
                SpoiledPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            if (_reconcile.Data != null && _reconcile.Details != null)
            {
                _skipTextChanged = true;
                DetailList.ItemsSource = _reconcile.Details.OrderBy(d => d.Party);
                SpoiledBallots.Text = _reconcile.Details.Sum(d => d.Spoiled).ToString();
                //SpoiledBallotsDEM.Text = _reconcile.Data.SpoiledDEM.ToString();
                //SpoiledBallotsREP.Text = _reconcile.Data.SpoiledREP.ToString();
                //SpoiledBallotsLIB.Text = _reconcile.Data.SpoiledLIB.ToString();
                //SpoiledBallotsDEM.Text = _reconcile.Details.Where(d => d.Party == "DEM").FirstOrDefault().Spoiled.ToString();
                //SpoiledBallotsREP.Text = _reconcile.Details.Where(d => d.Party == "REP").FirstOrDefault().Spoiled.ToString();
                //SpoiledBallotsLIB.Text = _reconcile.Details.Where(d => d.Party == "LIB").FirstOrDefault().Spoiled.ToString();
                _skipTextChanged = false;

                // Display VoterX totals for each party
                SpoiledPageInstructions3.Text = "For comparison, VoterX shows that you have ";
                if (_reconcile.Details.Count > 1)
                {
                    var last = _reconcile.ComputerDetails.Last();
                    foreach (var calcDetail in _reconcile.ComputerDetails)
                    {
                        SpoiledPageInstructions3.Text += calcDetail.Spoiled.ToString() + " Spoiled " + calcDetail.Party + " Ballots";
                        if(calcDetail != last)
                        {
                            SpoiledPageInstructions3.Text += ", ";
                        }
                        else
                        {
                            SpoiledPageInstructions3.Text += ".";
                        }
                    }
                }
                else
                {
                    var calcDetail = _reconcile.ComputerDetails.FirstOrDefault();
                    SpoiledPageInstructions3.Text += calcDetail.Spoiled.ToString() + " Spoiled Ballots.";

                    // Hide Total row
                    SpoiledTotalGrid.Visibility = Visibility.Collapsed;
                }
            }            

            SpoiledPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageInstructions1, _reconcile);

            SpoiledPageInstructions2.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageInstructions2, _reconcile);

            //SpoiledPageInstructions3.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageInstructions3, _reconcile);
            //SpoiledPageInstructions3.Text = "For comparison, VoterX shows that you have " + 
            //    _reconcile.Data.ComputerSpoiledDEM.ToString() + " Spoiled DEM Ballots, " + 
            //    _reconcile.Data.ComputerSpoiledREP.ToString() + " Spoiled REP Ballots, " + 
            //    _reconcile.Data.ComputerSpoiledLIB.ToString() + " Spoiled LIB Ballots.";

            SpoiledPageInstructions4.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageInstructions4, _reconcile);

            SpoiledPageHelpLink1.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpLink1, _reconcile);

            SpoiledPageHelpLink2.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpLink2, _reconcile);

            SpoiledPageHelpLink3.Text = DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpLink3, _reconcile);

            //SpoiledBallotsDEM.Focus();
            //SpoiledBallotsDEM.SelectAll();
        }

        private void SpoiledBallots_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (_skipTextChanged == false)
            //{
            //    //int total = 0;

            //    //// Check if the text entered is a valid number
            //    //if (Int32.TryParse(SpoiledBallots.Text, out int value) == true)
            //    //{
            //    //    // Save the value as the count of Spoiled Ballots
            //    //    _reconcile.Data.Spoiled = value;

            //    //    StatusBar.TextCenter = "";
            //    //}
            //    //else
            //    //{
            //    //    StatusBar.TextCenter = "Not A Number";
            //    //}

            //    ////if (Int32.TryParse(SpoiledBallotsDEM.Text, out int valueDEM) == true)
            //    ////{
            //    ////    // Save the value as the count of Spoiled Ballots
            //    ////    _reconcile.Data.SpoiledDEM = valueDEM;
            //    ////    _reconcile.Details.Where(d => d.Party == "DEM").FirstOrDefault().Spoiled = valueDEM;
            //    ////    total += valueDEM;

            //    ////    StatusBar.TextCenter = "";
            //    ////}
            //    ////else
            //    ////{
            //    ////    StatusBar.TextCenter = "Not A Number";
            //    ////}

            //    ////if (Int32.TryParse(SpoiledBallotsREP.Text, out int valueREP) == true)
            //    ////{
            //    ////    // Save the value as the count of Spoiled Ballots
            //    ////    _reconcile.Data.SpoiledREP = valueREP;
            //    ////    _reconcile.Details.Where(d => d.Party == "REP").FirstOrDefault().Spoiled = valueREP;
            //    ////    total += valueREP;

            //    ////    StatusBar.TextCenter = "";
            //    ////}
            //    ////else
            //    ////{
            //    ////    StatusBar.TextCenter = "Not A Number";
            //    ////}

            //    ////if (Int32.TryParse(SpoiledBallotsLIB.Text, out int valueLIB) == true)
            //    ////{
            //    ////    // Save the value as the count of Spoiled Ballots
            //    ////    _reconcile.Data.SpoiledLIB = valueLIB;
            //    ////    _reconcile.Details.Where(d => d.Party == "LIB").FirstOrDefault().Spoiled = valueLIB;
            //    ////    total += valueLIB;

            //    ////    StatusBar.TextCenter = "";
            //    ////}
            //    ////else
            //    ////{
            //    ////    StatusBar.TextCenter = "Not A Number";
            //    ////}

            //    //SpoiledBallots.Text = total.ToString();
            //    //_reconcile.Data.Spoiled = total;

            //    SpoiledBallots.Text = _reconcile.Details.Sum(d => d.Spoiled).ToString();
            //}
        }

        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpDialog1, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void Hyperlink2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpDialog2, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void Hyperlink3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.SpoiledPageHelpDialog3, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void SpoiledBallots_LostFocus(object sender, RoutedEventArgs e)
        {
            SpoiledBallots.Text = _reconcile.Details.Sum(d => d.Spoiled).ToString();
        }
    }
}
