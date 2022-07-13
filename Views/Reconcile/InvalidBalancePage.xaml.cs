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
    public partial class InvalidBalancePage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        public InvalidBalancePage()
        {
            InitializeComponent();
        }

        public InvalidBalancePage(ReconcileSettingsModel text, NMReconcile reconcile)
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

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            if(PollWorkerName.Text == "")
            {
                // Display message
                AlertDialog alertDialog = new AlertDialog("Please enter your name.");
                alertDialog.ShowDialog();
                return;
            }
            if (PhoneNumber.Text == "")
            {
                // Display message
                AlertDialog alertDialog = new AlertDialog("Please enter a contact number.");
                alertDialog.ShowDialog();
                return;
            }
            if (ReconcileNotes.Text == "")
            {
                // Display message
                AlertDialog alertDialog = new AlertDialog("Please explain why your numbers do not match.");
                alertDialog.ShowDialog();
                return;
            }

            // Save Reconcile and Tabulators
            _reconcile.Reconcile();

            // Go to final page and print the report
            this.NavigateToPage(new Admin.EndofDayPage(true));
        }

        private void StartOverButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new ReconcileStartPage());
        }

        private void ReconcileButton_Click(object sender, RoutedEventArgs e)
        {
            StatusBar.TextLeft = "Reconciled: " + _reconcile.IsReconciled.ToString();
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = _displayText.InvalidPageHeader + " - PRIMARY";
            }
            else
            {
                StatusBar.PageHeader = _displayText.InvalidPageHeader;
            }
            
            InvalidPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageBoldLine1, _reconcile);

            InvalidPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageInstructions1, _reconcile);

            InvalidPageInstructions2.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageInstructions2, _reconcile);

            InvalidPageInstructions3.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageInstructions3, _reconcile);

            InvalidPageInstructions4.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageInstructions4, _reconcile);

            if (_reconcile.Data != null)
            {
                SpoiledCount.Text = _reconcile.Spoiled.ToString();

                ProvisionalCount.Text = _reconcile.Provisional.ToString();

                ApplicationLabel.Text = DisplayTextMethods.ApplicationType() + "s"; ;
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

                if (_reconcile.Data.ComputerNotTabulated > 0 )
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

            InvalidPageBoldLine2.Text = DisplayTextMethods.ParseReconcile(_displayText.InvalidPageBoldLine2, _reconcile);
        }

        private void HighlightText()
        {
            // Check Spoileds
            if (_reconcile.SpoiledMatch)
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

        //private void Hyperlink1_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        HelpDialog signatureDialog = new HelpDialog(
        //            DisplayTextMethods.ParseReconcile(_displayText.BalancePageHelpDialog1, _reconcile)
        //            );
        //        signatureDialog.ShowDialog();
        //    }
        //    catch { }
        //}

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //// Notes on scroll offsets
            //// https://stackoverflow.com/questions/1033841/is-it-possible-to-implement-smooth-scroll-in-a-wpf-listview
            //// https://social.msdn.microsoft.com/Forums/en-US/3594c80a-7ccf-4cfc-9cc0-9731fd080d72/in-what-unit-is-the-scrollviewerverticaloffset?forum=winappswithcsharp

            ////double delta = (e.Delta * .26978); // Roughly half of 1 list item
            //double delta = (e.Delta * .01);
            ////double delta = (e.Delta / 120)*32; // Reduce to +1 or -1 then multiply to get exact units
            ////StatusBar.ApplicationStatusCenter("Scrolling:" + (delta).ToString());

            //ScrollViewer scv = (ScrollViewer)sender;
            //scv.ScrollToVerticalOffset(scv.VerticalOffset - (delta));
            //e.Handled = true;
        }

        private void PollWorkerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_reconcile.Data != null)
            {
                _reconcile.Data.PollWorkerName = PollWorkerName.Text;
            }
        }

        private void PhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_reconcile.Data != null)
            {
                _reconcile.Data.PollWorkerPhone = PhoneNumber.Text;
            }
        }

        private void ReconcileNotes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_reconcile.Data != null)
            {
                _reconcile.Data.Notes = ReconcileNotes.Text;
            }
        }
    }
}
