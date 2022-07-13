using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VoterX.Core.Voters;
using VoterX.Utilities.Views;
using VoterX.Kiosk.Views.Menu;

namespace VoterX.Kiosk.Methods
{
    public static class NavigationMenuMethods
    {
        public static void NavigateToPage(Page Destination)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(Destination);
        }

        public static void SetOrigin(Page page)
        {
            ((App)Application.Current).originpage = page;
        }

        public static void ReturnToOrigin()
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(((App)Application.Current).originpage);
            MainMenuMethods.ClickEnabled = true;
        }

        public static void LoginPage()
        {
            // CLEAR MENU PAGE
            // SET MENU FRAME TO NULL

            //GlobalReferences.Header.HamburgerMenuVisibility = false;

            //GlobalReferences.MenuSlider.Hide();

            //((App)Application.Current).Navigation = null;

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Login.LoginPage());

            ((App)Application.Current).mainpage.SaveLogout();

            //MainMenuMethods.SetMode(StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.LoadMenu(null, VoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Login.LoginPage());
        }

        public static void Logout()
        {
            //GlobalReferences.Header.HamburgerMenuVisibility = false;

            //GlobalReferences.MenuSlider.Hide();

            //((App)Application.Current).Navigation = null;

            //// SAVE LOGOUT ??? (maybe does not belong in the navigation methods)

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Login.LoginPage());
        }

        #region VotingPages
        public static void VoterSearchPage()
        {
            VoterSearchPage(null);
        }
        public static void VoterSearchPage(VoterSearchModel SearchItems)
        {
            //GlobalReferences.MenuSlider.Close();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Search.VoterSearchPage(SearchItems));
        }

        public static void SignatureCapturePage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Signature.SignatureCapturePage(voter));
        }

        public static void VoidSignatureCapturePage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.VoidedBallots.VoidSignatureCapturePage(voter));
        }

        public static void PrintOfficialBallotPage(NMVoter voter)
        {
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Ballots.PrintOfficialBallotPage(voter));
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Ballots.PrintBundlePage(voter));
        }

        public static void PrintVoidBallotPage(NMVoter voter)
        {
            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Ballots.PrintOfficialBallotPage(voter));
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.VoidedBallots.VoidBundlePage(voter));
        }

        public static void OfficialPrintTroubleShootingPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Troubleshooting.OfficialPrintTroubleShootingPage(voter));
        }

        public static void VoidPrintTroubleShootingPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.VoidedBallots.VoidPrintTroubleShootingPage(voter));
        }

        public static void SpoilOfficialBallotPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Ballots.SpoilOfficialBallotPage(voter));
        }

        public static void SpoiledPrintTroubleShootingPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Troubleshooting.SpoiledPrintTroubleShootingPage(voter));
        }

        public static void ProvisionalBallotPage(NMVoter voter)
        {
            ProvisionalBallotPage(voter, false);
        }
        public static void ProvisionalBallotPage(NMVoter voter, bool editStyle)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Voter.Ballots.ProvisionalBallotPage(voter, editStyle));
        }

        public static void ProvisionalPrintTroubleShootingPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Troubleshooting.ProvisionalPrintTroubleShootingPage(voter));
        }

        public static void VoidAbsenteeBallotPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.VoidedBallots.VoidAbsenteeBallotPage(voter));
        }
        #endregion

        #region ManagementPages
        public static void ManagePage()
        {
            //GlobalReferences.MenuSlider.SetMenu(new Menus.ManageMenuView(
            //    ((App)Application.Current).mainpage.MainFrame,
            //    GlobalReferences.MenuSlider),
            //    MenuCollapseMode.ShowIcons);

            //GlobalReferences.Header.HamburgerMenuVisibility = true;

            //GlobalReferences.MenuSlider.Open();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.ManagePage());

            //MainMenuMethods.LoadMenu(new ManageMenuView(), StateVoterX.Utilities.Models.MenuCollapseMode.ThreeState);
            //MainMenuMethods.OpenMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Admin.AdministrationPage());

            MainMenuMethods.OpenMenu();
        }

        public static void AddProvisionalPage()
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.AddProvisionalPage());
        }

        public static void SearchProvisionalPage()
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.ProvisionalSearchPage());
        }

        public static void EditBallotStylePage()
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.EditBallotStylesPage());
        }

        public static void EditBallotPage(NMVoter voter)
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.EditBallotPage(voter, true));
        }

        public static void DailyReportsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.DailyReportsPage());
        }

        public static void EmergencyBallotsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            MainMenuMethods.CloseMenu();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.EmergencyBallotsPage());
        }

        public static void EndOfDayPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.EmergencyBallotsPage());
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Admin.EndofDayPage());
        }

        public static void EndOfDayPage(StateVoterX.SystemSettings.Enums.ElectionType type)
        {
            //GlobalReferences.MenuSlider.Close();

            //MainMenuMethods.LoadMenu(null, StateVoterX.Utilities.Models.MenuCollapseMode.None);
            MainMenuMethods.CloseMenu();

            if (type == VoterX.SystemSettings.Enums.ElectionType.General)
            {
                //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Manage.EmergencyBallotsPage());
                ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Admin.EndofDayPage());
            }
            else if(type == VoterX.SystemSettings.Enums.ElectionType.Primary)
            {
                ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Admin.EndofDayPage());
            }
        }

        #endregion

        #region SettingsPages
        public static void SettingsPage()
        {
            var settingsPage = new Views.Settings.SettingsPage();

            MainMenuMethods.LoadMenu(new DynamicMenuView(new Views.Menu.SettingsMenuViewModel(settingsPage)), StateVoterX.Utilities.Models.MenuCollapseMode.ShowIcons);

            //GlobalReferences.MenuSlider.SetMenu(new Menus.SettingsMenuView(
            //    ((App)Application.Current).mainpage.MainFrame,
            //    GlobalReferences.MenuSlider),
            //    MenuCollapseMode.ShowIcons);

            //GlobalReferences.MenuSlider.Open();

            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.SettingsPage());
        }

        public static void SystemSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.SystemSettingsPage());
        }

        public static void NetworkSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.NetworkSettingsPage());
        }

        public static void PrinterSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.PrinterSettingsPage());
        }

        public static void ElectionSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.ElectionSettingsPage());
        }

        public static void BallotSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.BallotSettingsPage());
        }

        public static void TabulatorSettingsPage()
        {
            //GlobalReferences.MenuSlider.Close();

            //((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Settings.TabulatorSettingsPage());
        }
        #endregion

        #region SuperUserPages
        public static void SuperSearchPage()
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Super.Search.SuperSearchPage(null));
        }

        public static void SuperVoterDetailsPage(NMVoter voter)
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Super.VoterDetails.VoterDetailsPage(voter));
        }

        public static void CopyVoterDetailsPage()
        {
            ((App)Application.Current).mainpage.MainFrame.Navigate(new Views.Super.CopyDetails.CopyDetailsPage());
        }
        #endregion
    }
}
