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

namespace VoterX.Kiosk.Views.Validation
{
    /// <summary>
    /// Interaction logic for VerifyInvalidVoterPage.xaml
    /// </summary>
    public partial class VerifyInvalidVoterPage : Page
    {
        private NMVoter _voter;

        VoterSearchModel _searchItems;

        // Page constructors
        public VerifyInvalidVoterPage(NMVoter voter) : this(voter, null) { }
        public VerifyInvalidVoterPage(NMVoter voter, VoterSearchModel SearchItems)
        {
            InitializeComponent();

            _voter = voter;
            _searchItems = SearchItems;
        }

        // Alternate constructor uses Voter Navigation Model
        public VerifyInvalidVoterPage(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        // Load view models
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new VerifyInvalidVoterViewModel(_voter, _searchItems);
        }
    }
}
