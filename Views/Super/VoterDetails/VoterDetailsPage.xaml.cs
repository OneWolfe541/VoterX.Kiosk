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
using VoterX.Core.Voters;
using VoterX.Utilities.Views;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Super.VoterDetails
{
    /// <summary>
    /// Interaction logic for VoterDetailsPage.xaml
    /// </summary>
    public partial class VoterDetailsPage : Page
    {
        private NMVoter _voter = new NMVoter();

        public VoterDetailsPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;

            StatusBar.PageHeader = "Edit Voter Details";

            StatusBar.Clear();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get view model for the page
            var viewModel = new VoterDetailsViewModel(_voter);

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.EditVoterMenuViewModel(viewModel)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

            // Load the view model
            this.DataContext = viewModel;
        }
    }
}
