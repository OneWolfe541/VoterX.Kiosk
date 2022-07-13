using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Utilities.Controls;
using VoterX.Utilities.BasePageDefinitions;
using System.Windows.Controls;
using VoterX.Utilities.Models;

namespace VoterX.Kiosk.Methods
{
    public static class MainMenuMethods
    {
        //private static MasterBasePage MAINWINDOW = ((App)Application.Current).mainpage;
        //private static SliderMenuControl MAINMENUSLIDER = ((App)Application.Current).mainslidermenu;

        public static void ShowTopContent()
        {
            //((App)Application.Current).mainslidermenu.ShowTopContent();
        }

        public static void HideTopContent()
        {
            //((App)Application.Current).mainslidermenu.HideTopContent();
        }

        public static void ShowMiddleContent()
        {
            //((App)Application.Current).mainslidermenu.ShowMiddleContent();
        }

        public static void HideMiddleContent()
        {
            //((App)Application.Current).mainslidermenu.HideMiddleContent();
        }

        public static void ShowBottomContent()
        {
            //((App)Application.Current).mainslidermenu.ShowBottomContent();
        }

        public static void HideBottomContent()
        {
            //((App)Application.Current).mainslidermenu.HideBottomContent();
        }

        //public static void LoadMenu(object content)
        //{
        //    ((App)Application.Current).mainslidermenu.MenuFrame.Navigate(content);
        //}

        public static void LoadMenu(Page content, MenuCollapseMode mode)
        {
            ((App)Application.Current).mainslidermenu.SetMenu(content, mode);
        }

        public static void ShowExitButton()
        {
            ((App)Application.Current).mainpage.CloseButtonVisibility = Visibility.Visible;
            ((App)Application.Current).mainpage.MenuBarsVisibility = Visibility.Collapsed;
            ((App)Application.Current).mainpage.HamburgerButtonVisibility = Visibility.Collapsed;
        }

        public static void HideExitButton()
        {
            ((App)Application.Current).mainpage.CloseButtonVisibility = Visibility.Collapsed;
            ((App)Application.Current).mainpage.MenuBarsVisibility = Visibility.Visible;
            ((App)Application.Current).mainpage.HamburgerButtonVisibility = Visibility.Collapsed;
        }

        public static void RemoveExitButton()
        {
            ((App)Application.Current).mainpage.CloseButtonVisibility = Visibility.Collapsed;
        }

        public static void ShowHamburger()
        {
            ((App)Application.Current).mainpage.HamburgerButtonVisibility = Visibility.Visible;
        }

        public static void HideHamburger()
        {
            ((App)Application.Current).mainpage.HamburgerButtonVisibility = Visibility.Collapsed;
        }

        public static void RemoveHamburger()
        {
            ((App)Application.Current).mainpage.RemoveHamburger();
        }

        public static void OpenMenu()
        {
            ((App)Application.Current).mainslidermenu.Open();
        }

        public static void CloseMenu()
        {
            ((App)Application.Current).mainslidermenu.Close();
        }

        public static void ToggleMenu()
        {
            ((App)Application.Current).mainslidermenu.Toggle();
        }

        public static void HideMenu()
        {
            ((App)Application.Current).mainslidermenu.Hide();
        }

        public static void SetMode(MenuCollapseMode mode)
        {
            ((App)Application.Current).mainslidermenu.CollapseMode = mode;
        }

        public static MenuCollapseMode GetMode()
        {
            return ((App)Application.Current).mainslidermenu.CollapseMode;
        }

        public static bool ClickEnabled
        {
            get
            {
                return ((App)Application.Current).mainslidermenu.IsClickEnabled;
            }
            set
            {
                ((App)Application.Current).mainslidermenu.IsClickEnabled = value;
            }
        }
    }
}
