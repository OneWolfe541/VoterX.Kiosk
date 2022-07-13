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
    /// Interaction logic for ReconcileBalancePage.xaml
    /// </summary>
    public partial class ReconcileBalancePage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        public ReconcileBalancePage()
        {
            InitializeComponent();
        }

        public ReconcileBalancePage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new TabulatorCountPage(_displayText, _reconcile));
        }

        private void ReconcileButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reconcile.IsReconciled)
            {
                // Clear name and phone for valid reconcile records
                _reconcile.Data.PollWorkerName = "";
                _reconcile.Data.PollWorkerPhone = "";

                // Update notes
                _reconcile.Data.Notes = "RECONCILED";

                // Save Reconcile and Tabulators
                _reconcile.Reconcile();

                if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.General)
                {
                    // Go to final page and print the report
                    this.NavigateToPage(new Admin.EndofDayPage(true));
                }
                else
                {
                    // Go to final page and print the report
                    this.NavigateToPage(new Admin.EndofDayPage(true));
                }
            }
            else
            {
                //StatusBar.StatusTextLeft = "Reconciled: " + _reconcile.IsReconciled.ToString();

                // Go to Invalid Balance page
                this.NavigateToPage(new InvalidBalancePage(_displayText, _reconcile));
            }
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = _displayText.BalancePageHeader + " - PRIMARY";
            }
            else
            {
                StatusBar.PageHeader = _displayText.BalancePageHeader;
            }

            try // If BalancePageBoldLine1 is null ToUpper will fail
            {
                BalancePageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.BalancePageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            BalancePageBoldLine2.Text = DisplayTextMethods.ParseReconcile(_displayText.BalancePageBoldLine2, _reconcile);

            BalancePageHelpLink1.Text = DisplayTextMethods.ParseReconcile(_displayText.BalancePageHelpLink1, _reconcile);

            if (_reconcile.Data != null)
            {
                SpoiledCount.Text = _reconcile.Spoiled.ToString();

                ProvisionalCount.Text = _reconcile.Provisional.ToString();

                ApplicationLabel.Text = DisplayTextMethods.ApplicationType() + "s:"; ;
                ApplicationCount.Text = _reconcile.Regular.ToString();

                TabulatorCount.Text = (_reconcile.TabulatorTotal + _reconcile.HandTally).ToString();

                VoterXSpoiledDisplay.Text = 
                    string.Format("{0}", 
                    _reconcile.Data.ComputerSpoiled, 
                    _reconcile.Data.ComputerNotTabulated);

                //VoterXFledWrongDisplay.Text =
                //    string.Format("(Including {0} fled voter(s) and {1} wrong voter(s))",
                //    _reconcile.Data.ComputerFled,
                //    _reconcile.Data.ComputerWrong);

                VoterXProvisionalDisplay.Text =
                    string.Format("{0}", _reconcile.Data.ComputerProvisional);

                VoterXApplicationDisplay.Text =
                    string.Format("{0}", _reconcile.Data.ComputerRegular);

                // Tabulator Counts minus Fled and Wrong counts
                VoterXTabulatorDisplay.Text =
                    string.Format("{0}", _reconcile.Data.ComputerRegular -
                    _reconcile.Data.ComputerNotTabulated);

                if (_reconcile.Data.ComputerNotTabulated > 0)
                {
                    TabulatorCalculationsDisplay.Text = DisplayTextMethods.ParseReconcile(_displayText.BalancePageCalculation, _reconcile);
                    TabulatorCalculationsDisplay.Visibility = Visibility.Visible;
                    TabulatorAndHandTalliesDisplay.Visibility = Visibility.Collapsed;
                }
                else
                {
                    TabulatorAndHandTalliesDisplay.Text =
                        string.Format("(Tabulator Total: {0}; Hand Tallies: {1})",
                        _reconcile.TabulatorTotal,
                        _reconcile.HandTally);
                    TabulatorAndHandTalliesDisplay.Visibility = Visibility.Visible;
                    TabulatorCalculationsDisplay.Visibility = Visibility.Collapsed;
                }

                HighlightText();
            }
        }

        private void HighlightText()
        {
            // Check Spoileds
            if(_reconcile.SpoiledMatch)
            {
                SpoiledCount.Foreground = Brushes.Green;
                VoterXSpoiledDisplay.Foreground = Brushes.Green;
            }
            else
            {
                SpoiledCount.Foreground = Brushes.Red;
                VoterXSpoiledDisplay.Foreground = Brushes.Red;
            }

            // Check Provisionals
            if (_reconcile.ProvisionalMatch)
            {
                ProvisionalCount.Foreground = Brushes.Green;
                VoterXProvisionalDisplay.Foreground = Brushes.Green;
            }
            else
            {
                ProvisionalCount.Foreground = Brushes.Red;
                VoterXProvisionalDisplay.Foreground = Brushes.Red;
            }

            // Check Regular
            if (_reconcile.RegularMatch)
            {
                ApplicationCount.Foreground = Brushes.Green;
                VoterXApplicationDisplay.Foreground = Brushes.Green;
            }
            else
            {
                ApplicationCount.Foreground = Brushes.Red;
                VoterXApplicationDisplay.Foreground = Brushes.Red;
            }

            // Check Tabulators
            if (_reconcile.TabulatorMatch)
            {
                TabulatorCount.Foreground = Brushes.Green;
                VoterXTabulatorDisplay.Foreground = Brushes.Green;
            }
            else
            {
                TabulatorCount.Foreground = Brushes.Red;
                VoterXTabulatorDisplay.Foreground = Brushes.Red;
            }
        }

        private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HelpDialog signatureDialog = new HelpDialog(
                    DisplayTextMethods.ParseReconcile(_displayText.BalancePageHelpDialog1, _reconcile)
                    );
                signatureDialog.ShowDialog();
            }
            catch { }
        }
    }
}
