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
using VoterX.Utilities.Views.VoterDetails;

namespace VoterX.Kiosk.Views.VoidedBallots
{
    /// <summary>
    /// Interaction logic for SignatureCapturePage.xaml
    /// </summary>
    public partial class VoidSignatureCapturePage : Page
    {
        private NMVoter _voter;

        public VoidSignatureCapturePage(NMVoter voter)
        {
            InitializeComponent();

            _voter = voter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VoidSignatureCaptureViewModel signatureViewModel = new VoidSignatureCaptureViewModel(_voter);
            this.DataContext = signatureViewModel;

            VoterDetails.DataContext = new VoterDetailsViewModel(_voter);

            //SignaturePadControl.DataContext = new SignatureControlViewModel(_voter, AppSettings.System.SignatureFolder, AppSettings.Global.System);
            SignaturePadControl.DataContext = signatureViewModel;
        }
    }
}
