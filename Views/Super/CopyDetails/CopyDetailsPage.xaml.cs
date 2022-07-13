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

namespace VoterX.Kiosk.Views.Super.CopyDetails
{
    /// <summary>
    /// Interaction logic for VoterDetailsPage.xaml
    /// </summary>
    public partial class CopyDetailsPage : Page
    {
        //private NMVoter _voter = new NMVoter();

        public CopyDetailsPage()
        {
            InitializeComponent();            

            StatusBar.PageHeader = "Move Voter Credit";

            StatusBar.Clear();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get view model for the page
            var viewModel = new CopyDetailsViewModel();

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Menu.CopyVoterMenuViewModel(viewModel)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

            // Load the view model
            this.DataContext = viewModel;
        }
    }
}
