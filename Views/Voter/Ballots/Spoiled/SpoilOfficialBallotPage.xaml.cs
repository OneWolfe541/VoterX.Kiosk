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

namespace VoterX.Kiosk.Views.Voter.Ballots
{
    /// <summary>
    /// Interaction logic for SpoilOfficialBallotPage.xaml
    /// </summary>
    public partial class SpoilOfficialBallotPage : Page
    {
        private NMVoter _voter;

        public SpoilOfficialBallotPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new SpoilOfficialBallotViewModel(_voter);
        }
    }
}
