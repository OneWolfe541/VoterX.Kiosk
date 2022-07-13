using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using VoterX.Core.Models.Database;
using VoterX.SystemSettings.Extensions;
using VoterX.Utilities.Extensions;
using VoterX.Logging;

namespace VoterX.Kiosk.Methods
{
    public static class PollSummaryMethods
    {
        public static async Task<bool> SaveLogin(string user)
        {
            VoterXLogger _reportLogger = new VoterXLogger("VCCLogs", AppSettings.System.ReportErrorLogging);
            
            bool saved = await ((App)Application.Current).Election
                .SaveLogin(
                (int)AppSettings.System.SiteID,
                AppSettings.System.MachineID
                );

            if(saved == true)
            {
                _reportLogger.WriteLog("Logged In As: " + user + " SAVED");
            }
            else
            {
                _reportLogger.WriteLog("Logged In As: " + user + " NOT SAVED");
            }

            return saved;
        }

        public static bool SaveLoginSync(string user)
        {
            VoterXLogger _reportLogger = new VoterXLogger("VCCLogs", AppSettings.System.ReportErrorLogging);

            bool saved = ((App)Application.Current).Election
                .SaveLoginSync(
                (int)AppSettings.System.SiteID,
                AppSettings.System.MachineID
                );

            if (saved == true)
            {
                _reportLogger.WriteLog("Logged In As: " + user + " SAVED");
            }
            else
            {
                _reportLogger.WriteLog("Logged In As: " + user + " NOT SAVED");
            }

            return saved;
        }

        public static async Task<bool> SaveLogout()
        {
            return await SaveLogout(null);
        }

        public static async Task<bool> SaveLogout(string elapsedTime)
        {
            VoterXLogger _reportLogger = new VoterXLogger("VCCLogs", AppSettings.System.ReportErrorLogging);

            bool saved = false;
            if (((App)Application.Current).Election != null)
            {
                saved = await ((App)Application.Current).Election
                    .SaveLogout(
                    (int)AppSettings.System.SiteID,
                    AppSettings.System.MachineID
                    );
            }

            if (saved == true)
            {
                _reportLogger.WriteLog("Logged Out: SAVED {TIMER " + elapsedTime + "}");
            }
            else
            {
                _reportLogger.WriteLog("Logged Out: NOT SAVED {TIMER " + elapsedTime + "}");
            }

            return saved;
        }
    }
}
