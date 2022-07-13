using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.Elections;

namespace VoterX.Kiosk.Methods
{
    public static class ElectionDataMethods
    {
        /// <summary>
        /// Repository Container
        /// </summary>
        public static NMElection Election
        {
            get
            {
                return ((App)Application.Current).Election;
            }
        }

        public static bool Exists
        {
            get
            {
                return ((App)Application.Current).Voters.Exists();
            }
        }

        public static List<ApplicationRejectedReasonModel> ApplicationRejectedReasons
        {
            get
            {
                return ((App)Application.Current).Election.Lists.ApplicationRejectedReasons;
            }
        }

        public static List<BallotStyleModel> BallotStyles
        {
            get
            {
                return ((App)Application.Current).Election.Lists.BallotStyles;
            }
        }

        public static List<BallotStyleModel> DistinctBallots
        {
            get
            {
                return ((App)Application.Current).Election.Lists.BallotStyles.DistinctBallots();
            }
        }

        public static List<JurisdictionModel> Jurisdictions
        {
            get
            {
                return ((App)Application.Current).Election.Lists.Jurisdictions;
            }
        }

        public static List<LocationModel> Locations
        {
            get
            {
                return ((App)Application.Current).Election.Lists.Locations;
            }
        }

        public static List<LogCodeModel> LogCodes
        {
            get
            {
                return ((App)Application.Current).Election.Lists.LogCodes;
            }
        }

        public static List<PartyModel> Partys
        {
            get
            {
                return ((App)Application.Current).Election.Lists.Partys;
            }
        }

        public static List<PollWorkerModel> PollWorkers
        {
            get
            {
                return ((App)Application.Current).Election.Lists.PollWorkers;
            }
        }

        public static List<PrecinctModel> Precincts
        {
            get
            {
                return ((App)Application.Current).Election.Lists.Precincts;
            }
        }

        public static List<ProvisionalReasonModel> ProvisionalReasons
        {
            get
            {
                return ((App)Application.Current).Election.Lists.ProvisionalReasons;
            }
        }

        public static List<SpoiledReasonModel> SpoiledReasons
        {
            get
            {
                return ((App)Application.Current).Election.Lists.SpoiledReasons;
            }
        }
    }
}
