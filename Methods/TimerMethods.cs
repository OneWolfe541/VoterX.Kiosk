using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VoterX.Kiosk.Methods
{
    public static class TimerMethods
    {
        public static void StartLogTimer(double Duration, Frame Parent)
        {
            ((App)Application.Current).IdleTimer.StartLogOutTimer(Duration, Parent);
        }

        public static async Task<string> GetElapsedTime()
        {
            return await Task.Run(() => ((App)Application.Current).IdleTimer.GetElapsedTime());
        }

        public static void RestartTimer()
        {
            ((App)Application.Current).IdleTimer.RestartTimer();
        }

        public static void KillTimer()
        {
            //((App)Application.Current).IdleTimer.KillTimer();
        }
    }
}
