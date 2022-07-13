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
    /// Interaction logic for TabulatorStartPage.xaml
    /// </summary>
    public partial class TabulatorStartPage : Page
    {
        private NMReconcile _reconcile;

        private ReconcileSettingsModel _displayText;

        public TabulatorStartPage()
        {
            InitializeComponent();
        }

        public TabulatorStartPage(ReconcileSettingsModel text, NMReconcile reconcile)
        {
            InitializeComponent();

            _reconcile = reconcile;

            _displayText = text;

            LoadDisplayText();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new HandTallyCountPage(_displayText, _reconcile));
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new TabulatorCountPage(_displayText, _reconcile));
        }

        private void LoadDisplayText()
        {
            if (AppSettings.Election.ElectionType == StateVoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.TabulatorStartPageHeader + " - PRIMARY", _reconcile);
            }
            else
            {
                StatusBar.PageHeader = DisplayTextMethods.ParseReconcile(_displayText.TabulatorStartPageHeader, _reconcile);
            }

            try // If ProvisionalPageBoldLine1 is null ToUpper will fail
            {
                TabulatorStartPageBoldLine1.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorStartPageBoldLine1, _reconcile).ToUpper();
            }
            catch { }

            TabulatorStartPageInstructions1.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorStartPageInstructions1, _reconcile);

            TabulatorPageItemList.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorPageItemList, _reconcile);

            TabulatorStartPageInstructions2.Text = DisplayTextMethods.ParseReconcile(_displayText.TabulatorStartPageInstructions2, _reconcile);
        }
    }
}
