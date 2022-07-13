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
using VoterX.Core.Reconciles;
using VoterX.SystemSettings.Models;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for HandTallyCountPage.xaml
    /// </summary>
    public partial class HandTallyCountPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        private bool _skipTextChanged = false;

        public HandTallyCountPage()
        {
            InitializeComponent();
        }

        public HandTallyCountPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new ApplicationCountPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new TabulatorStartPage(_displayText, _reconcile));
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageHeader + " - PRIMARY", _reconcile);
            }
            else
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageHeader, _reconcile);
            }

            try // If ProvisionalPageBoldLine1 is null ToUpper will fail
            {
                HandTallyPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            if (_reconcile.Data != null)
            {
                _skipTextChanged = true;
                DetailList.ItemsSource = _reconcile.Details.OrderBy(d => d.Party);
                HandTallies.Text = _reconcile.Details.Sum(d => d.HandTally).ToString();
                //HandTallies.Text = _reconcile.Data.HandTally.ToString();
                //HandTalliesDEM.Text = _reconcile.Data.HandTallyDEM.ToString();
                //HandTalliesREP.Text = _reconcile.Data.HandTallyREP.ToString();
                //HandTalliesLIB.Text = _reconcile.Data.HandTallyLIB.ToString();
                _skipTextChanged = false;

                if (_reconcile.ComputerDetails.Count == 1)
                {
                    // Hide Total row
                    HandTallyTotalGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                //HandTallies.Text = "0";
                //HandTalliesDEM.Text = "0";
                //HandTalliesREP.Text = "0";
                //HandTalliesLIB.Text = "0";
            }

            HandTallyPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageInstructions1, _reconcile);

            HandTallyPageHelpLink1.Text = DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageHelpLink1, _reconcile);

            //HandTalliesDEM.Focus();
            //HandTalliesDEM.SelectAll();
        }

        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.HandTallyPageHelpDialog1, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }

        private void HandTallies_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipTextChanged == false)
            {
                int total = 0;

                //// Check if the text entered is a valid number
                //if (Int32.TryParse(HandTallies.Text, out int value) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.HandTally = value;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(HandTalliesDEM.Text, out int valueDEM) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.HandTallyDEM = valueDEM;
                //    total += valueDEM;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(HandTalliesREP.Text, out int valueREP) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.HandTallyREP = valueREP;
                //    total += valueREP;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                //if (Int32.TryParse(HandTalliesLIB.Text, out int valueLIB) == true)
                //{
                //    // Save the value as the count of Spoiled Ballots
                //    _reconcile.Data.HandTallyLIB = valueLIB;
                //    total += valueLIB;

                //    StatusBar.TextCenter = "";
                //}
                //else
                //{
                //    StatusBar.TextCenter = "Not A Number";
                //}

                HandTallies.Text = total.ToString();
                //_reconcile.Data.HandTally = total;
            }
        }

        private void HandTallies_LostFocus(object sender, RoutedEventArgs e)
        {
            HandTallies.Text = _reconcile.Details.Sum(d => d.HandTally).ToString();
        }
    }
}
