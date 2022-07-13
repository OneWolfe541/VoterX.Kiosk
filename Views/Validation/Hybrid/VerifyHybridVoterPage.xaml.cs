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
    /// Interaction logic for VerifyHybridVoterPage.xaml
    /// </summary>
    public partial class VerifyHybridVoterPage : Page
    {
        private NMVoter _voter;

        VoterSearchModel _searchItems;

        // Page constructors
        public VerifyHybridVoterPage(NMVoter voter) : this(voter, null) { }
        public VerifyHybridVoterPage(NMVoter voter, VoterSearchModel SearchItems)
        {
            InitializeComponent();

            _voter = voter;
            _searchItems = SearchItems;
        }

        // Alternate constructor uses Voter Navigation Model
        public VerifyHybridVoterPage(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        // Load view models
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new VerifyHybridVoterViewModel(_voter, _searchItems);
        }
    }
}
