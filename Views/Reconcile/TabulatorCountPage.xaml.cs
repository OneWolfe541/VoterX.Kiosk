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
using VoterX.SystemSettings.Models;
using VoterX.Utilities.Dialogs;
using VoterX.Kiosk.Methods;
using VoterX.Logging;
using System.Collections.ObjectModel;

namespace VoterX.Kiosk.Views.ReconcilePrimary
{
    /// <summary>
    /// Interaction logic for TabulatorCountPage.xaml
    /// </summary>
    public partial class TabulatorCountPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        public TabulatorCountPage()
        {
            InitializeComponent();
        }

        public TabulatorCountPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();

            LoadTabulators();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new TabulatorStartPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Check for blank tabulator            
            if(_reconcile.CheckBlankTabulators())
            {
                if (_reconcile.CheckInvalidTabulators())
                {
                    // Ask before deleting
                    AreYouSureDialog areYouSureDialog = new AreYouSureDialog("ARE YOU SURE?",
                    DisplayTextMethods.ParseReconcile(_displayText.TabulatorListDeleteWarning, _reconcile));

                    if (areYouSureDialog.ShowDialog() == true)
                    {
                        _reconcile.RemoveBlankTabulators();

                        //_reconcile.LoadTabulators();

                        LoadTabulators();
                    }
                }
                else
                {
                    // Just delete them
                    _reconcile.RemoveBlankTabulators();
                    this.NavigateToPage(new ReconcileBalancePage(_displayText, _reconcile));
                }
            }
            else
            {
                this.NavigateToPage(new ReconcileBalancePage(_displayText, _reconcile));
            }
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.TabulatorListPageHeader + " - PRIMARY", _reconcile);
            }
            else
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.TabulatorListPageHeader, _reconcile);
            }

            TabulatorListPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorListPageBoldLine1, _reconcile);

            TabulatorListPageBoldLine2.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorListPageBoldLine2, _reconcile);

            TabulatorListPageBoldLine3.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorListPageBoldLine3, _reconcile);
        }

        private void LoadTabulators()
        {
            if(_reconcile.Tabulators != null && _reconcile.Tabulators.Count() > 0)
            {
                TabulatorList.ItemsSource = new ObservableCollection<ReconcileTabulatorModel>(_reconcile.Tabulators).OrderBy(t =>t.TabulatorName);
            }
        }

        private void AddTabulatorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _reconcile.NewTabulator();
            }
            catch (Exception error)
            {
                // Log error
                VoterXLogger reconcileLog = new VoterXLogger("VCClogs", true);
                reconcileLog.WriteLog("RECONCILE FAILED ADD TABULATOR: " + error.Message);
            }

            if (_reconcile.Tabulators != null && _reconcile.Tabulators.Count() > 0)
            {
                TabulatorList.ItemsSource = new ObservableCollection<ReconcileTabulatorModel>(_reconcile.Tabulators);
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Notes on scroll offsets
            // https://stackoverflow.com/questions/1033841/is-it-possible-to-implement-smooth-scroll-in-a-wpf-listview
            // https://social.msdn.microsoft.com/Forums/en-US/3594c80a-7ccf-4cfc-9cc0-9731fd080d72/in-what-unit-is-the-scrollviewerverticaloffset?forum=winappswithcsharp

            //double delta = (e.Delta * .26978); // Roughly half of 1 list item
            double delta = (e.Delta * .36);
            //double delta = (e.Delta / 120)*32; // Reduce to +1 or -1 then multiply to get exact units
            //StatusBar.ApplicationStatusCenter("Scrolling:" + (delta).ToString());

            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - (delta));
            e.Handled = true;
        }
    }
}
