﻿using VoterX.Core.Voters;
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

namespace VoterX.Kiosk.Views.Validation
{
    /// <summary>
    /// Interaction logic for VerifySpoiledVoterPage.xaml
    /// </summary>
    public partial class VerifySpoiledVoterPage : Page
    {
        private NMVoter _voter;

        VoterSearchModel _searchItems;

        // Page constructors
        public VerifySpoiledVoterPage(NMVoter voter) : this(voter, null) { }
        public VerifySpoiledVoterPage(NMVoter voter, VoterSearchModel SearchItems)
        {
            InitializeComponent();

            _voter = voter;
            _searchItems = SearchItems;
        }

        // Alternate constructor uses Voter Navigation Model
        public VerifySpoiledVoterPage(VoterNavModel voterNav) : this(voterNav.Voter, voterNav.Search) { }

        // Load view models
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = new VerifySpoiledVoterViewModel(_voter, _searchItems);
        }
    }
}
