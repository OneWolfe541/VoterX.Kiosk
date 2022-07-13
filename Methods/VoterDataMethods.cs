using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using VoterX.Core.Repositories;
using VoterX.Utilities.Extensions;
using VoterX.Core.Voters;
using System.Collections.ObjectModel;
//using VoterX.Core.Containers;

namespace VoterX.Kiosk.Methods
{
    public static class VoterMethods
    {
        /// <summary>
        /// Repository Container
        /// </summary>
        public static VoterFactory Voters
        {
            get
            {
                return ((App)Application.Current).Voters;
            }
            set
            {
                ((App)Application.Current).Voters = value;
            }
        }

        public static bool Exists
        {
            get
            {
                return ((App)Application.Current).Voters.Exists();
            }
        }

        //public static Func<ObservableCollection<NMVoter>> LimitedList(VoterSearchModel SearchItems)
        //{
        //    var list = ((App)Application.Current).Voters.LimitedList(SearchItems);

        //    return () => list;
        //}
    }
}
