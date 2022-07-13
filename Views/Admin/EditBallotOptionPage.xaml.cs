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
//using VoterX.Core.Models.Utilities;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Extensions;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Admin
{
    /// <summary>
    /// Interaction logic for EditBallotOptionPage.xaml
    /// </summary>
    public partial class EditBallotOptionPage : Page
    {
        private VoterNavModel _voterNav = new VoterNavModel();

        //public EditBallotOptionPage(VoterNavModel voterFromNav)
        public EditBallotOptionPage()
        {
            InitializeComponent();

            //_voterNav = voterFromNav;

            StatusBar.PageHeader = "Edit Ballot Style Options";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Admin.EditBallotSearchPage(_voterNav.Search));
        }

        private void EditProvisional_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Admin.EditBallotStylePage(_voterNav, false));
        }

        private void EditOfficial_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Admin.EditBallotStylePage(_voterNav, true));
        }

    }
}
