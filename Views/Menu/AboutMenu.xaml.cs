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
using VoterX.Utilities.BasePageDefinitions;

namespace VoterX.Kiosk.Views.Menu
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutMenu : MenuBasePage
    {
        public AboutMenu()
        {
            InitializeComponent();

            SetElectionTitle();
        }

        public override string GetMenu()
        {
            return "About Menu";
        }

        public void SetElectionTitle()
        {
            ElectionEntity.Text = ((App)Application.Current).GlobalSettings.Election.ElectionEntity;
            ElectionName.Text = ((App)Application.Current).GlobalSettings.Election.ElectionTitle;
            PollLocationName.Text = ((App)Application.Current).GlobalSettings.System.SiteName;
            ComputerNumber.Text = ((App)Application.Current).GlobalSettings.System.MachineID.ToString();

            CopyrightBlock.Text = "AUTOMATED ELECTION SERVICES";
            CopyrightDate.Text = "Copyright © 2020";
        }
    }
}
