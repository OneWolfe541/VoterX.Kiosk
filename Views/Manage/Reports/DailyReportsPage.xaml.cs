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

namespace VoterX.Kiosk.Views.Manage
{
    /// <summary>
    /// Interaction logic for DailyReportsPage.xaml
    /// </summary>
    public partial class DailyReportsPage : Page
    {
        public DailyReportsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new DailyReportsViewModel();
        }
    }
}
