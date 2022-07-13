using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Kiosk.Views;
using VoterX.Utilities.BasePageDefinitions;
using VoterX.Utilities.Methods;
using VoterX.Utilities.UserControls;
using VoterX.SystemSettings.Models;
using VoterX.Core.Elections;
//using VoterX.Core.Containers;

namespace VoterX.Kiosk.Methods
{
    public static class StatusBar
    {
        //private static MasterBasePage MAINWINDOW = ((App)Application.Current).mainpage;
        //private static StatusBarControl MAINSTATUSBAR = ((App)Application.Current).mainstatusbar;

        public static string PageHeader
        {
            get { return ((App)Application.Current).mainpage.GetPageHeader(); }
            set { ((App)Application.Current).mainpage.SetPageHeader(value); }
        }

        public static string TextLeft
        {
            //get { return ((App)Application.Current).mainstatusbar.StatusBarLeft; }
            //set { ((App)Application.Current).mainstatusbar.StatusBarLeft = value; }

            get { return ((App)Application.Current).StatusBar.TextLeft; }
            set { ((App)Application.Current).StatusBar.TextLeft = value; }
        }

        public static string TextRight
        {
            //get { return ((App)Application.Current).mainpage.StatusRight; }
            //set { ((App)Application.Current).mainpage.StatusRight = value; }

            get { return ((App)Application.Current).StatusBar.TextRight; }
            set { ((App)Application.Current).StatusBar.TextRight = value; }
        }

        public static string TextCenter
        {
            //get { return ((App)Application.Current).mainstatusbar.StatusBarCenter; }
            //set { ((App)Application.Current).mainstatusbar.StatusBarCenter = value; }

            get { return ((App)Application.Current).StatusBar.TextCenter; }
            set { ((App)Application.Current).StatusBar.TextCenter = value; }
        }

        public static async Task<bool> CheckPrinter(PrinterSettingsModel printers)
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //return ((App)Application.Current).mainstatusbar.CheckPrinter(printers);

                return await ((App)Application.Current).StatusBar.CheckPrinterStatusAsync(printers.BallotPrinter);
            }
            else return false;
        }

        //public static void HidePrinterStatusIcon()
        //{
        //    ((App)Application.Current).mainstatusbar.HidePrinterStatus();
        //}

        //public static bool PrinterStatusIcon
        //{
        //    get { return ((App)Application.Current).mainstatusbar.GetPrinterStatus(); }
        //    set { ((App)Application.Current).mainstatusbar.SetPrinterStatus(value); }
        //}

        public static async Task<bool> CheckSignaturePad()
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //((App)Application.Current).mainstatusbar.CheckSignaturePad();

                return await ((App)Application.Current).StatusBar.CheckSignaturePadStatusAsync();
            }
            else return false;
        }

        //public static void HideSignaturePadStatusIcon()
        //{
        //    ((App)Application.Current).mainstatusbar.HideSignaturePadStatus();
        //}

        //public static bool SignaturePadStatus
        //{
        //    get { return ((App)Application.Current).mainstatusbar.GetPrinterStatus(); }
        //    set { ((App)Application.Current).mainstatusbar.SetPrinterStatus(value); }
        //}

        public static void ShowLeftSpinner()
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //((App)Application.Current).mainstatusbar.ShowLeftSpinner();
                ((App)Application.Current).StatusBar.SpinnerVisibility = true;
            }
        }

        public static void HideLeftSpinner()
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //((App)Application.Current).mainstatusbar.HideLeftSpinner();
                ((App)Application.Current).StatusBar.SpinnerVisibility = false;
            }
            
        }

        public static void Clear()
        {
            if (((App)Application.Current).StatusBar != null)
            {
                HideLeftSpinner();
                //((App)Application.Current).mainstatusbar.StatusBarLeft = "";
                //((App)Application.Current).mainstatusbar.StatusBarCenter = "";
                ((App)Application.Current).mainpage.StatusRight = "";

                ((App)Application.Current).StatusBar.TextLeft = "";
                ((App)Application.Current).StatusBar.TextCenter = "";
            }
        }

        public static async Task<bool> CheckServer(ElectionFactory election)
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //return await ((App)Application.Current).mainstatusbar.CheckServer(election);
                if (((App)Application.Current).Connection != null)
                {
                    return await ((App)Application.Current).StatusBar.CheckDatabaseStatusAsync(0, ((App)Application.Current).Connection);
                }
                else
                {
                    return await ((App)Application.Current).StatusBar.CheckDatabaseStatusAsync(0);
                }
            }
            else return false;
        }

        public static async Task<bool> CheckServer(NMElection election)
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //return await ((App)Application.Current).mainstatusbar.CheckServer(election);
                if (((App)Application.Current).Connection != null)
                {
                    return await ((App)Application.Current).StatusBar.CheckDatabaseStatusAsync(0, ((App)Application.Current).Connection);
                }
                else
                {
                    return await ((App)Application.Current).StatusBar.CheckDatabaseStatusAsync(0);
                }
            }
            else return false;
        }

        public static void DisplayMode(SystemSettingsModel system)
        {
            if (((App)Application.Current).StatusBar != null)
            {
                //((App)Application.Current).mainstatusbar.DisplaySystemMode(system);
                ((App)Application.Current).StatusBar.DisplaySystemMode((int)system.VCCType);
            }
        }
    }
}
