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

namespace VoterX.Kiosk.Views.Troubleshooting
{
    /// <summary>
    /// Interaction logic for ProvisionalPrintTroubleShootingPage.xaml
    /// </summary>
    public partial class ProvisionalPrintTroubleShootingPage : Page
    {
        private NMVoter _voter;

        public ProvisionalPrintTroubleShootingPage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new ProvisionalPrintTroubleShootingViewModel(_voter);
        }
    }
}
