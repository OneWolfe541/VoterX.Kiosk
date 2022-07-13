using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoterX.Core.Reconciles;
using VoterX.SystemSettings.Enums;
using VoterX.Utilities.Extensions;

namespace VoterX.Kiosk.Methods
{
    public static class DisplayTextMethods
    {
        public static string ParseReconcile(string text)
        {
            return ParseReconcile(text, null);
        }

        public static string ParseReconcile(string text, NMReconcile reconcile)
        {
            if (text != null)
            {
                // Check for tags. Dont try to parse the text if no tags are found.
                if (Regex.IsMatch(text, "\\[(.*?)\\]"))
                {
                    text = text.Replace("[N]", "\n");

                    text = text.Replace("[APP]", ApplicationType());

                    // The [VAL] tag can have many forms
                    Match match = Regex.Match(text, "\\[VAL\\d\\]");
                    if (match.Success)
                    {
                        int index = match.Index;
                        string tag = text.Substring(index, 6);
                        string number = tag.Substring(4, 1);
                        text = text.Replace(tag, "{" + number + "}");
                    }
                    else
                    {
                        text = text.Replace("[VAL]", "{0}");
                    }

                    // Check if the reconcile object has any data
                    if (reconcile != null || reconcile.Data != null)
                    {
                        text = text.Replace("[REG]", reconcile.Regular.ToString());
                        text = text.Replace("[COMREG]", reconcile.Data.ComputerRegular.ToString());

                        text = text.Replace("[SPOIL]", reconcile.Spoiled.ToString());
                        text = text.Replace("[COMSPOIL]", reconcile.Data.ComputerSpoiled.ToString());

                        text = text.Replace("[PROV]", reconcile.Provisional.ToString());
                        text = text.Replace("[COMPROV]", reconcile.Data.ComputerProvisional.ToString());

                        //text = text.Replace("[COMWRONG]", reconcile.Data.ComputerWrong.ToString());
                        //text = text.Replace("[COMFLED]", reconcile.Data.ComputerFled.ToString());

                        text = text.Replace("[COMNOTTAB]", reconcile.Data.ComputerNotTabulated.ToString());

                        text = text.Replace("[HAND]", reconcile.HandTally.ToString());

                        text = text.Replace("[TAB]", reconcile.TabulatorTotal.ToString());
                    }
                }
            }
            else
            {
                // Replace null with ""
                text = "";
            }

            return text;
        }

        public static string ApplicationType()
        {
            if(AppSettings.System.VCCType == VotingCenterMode.EarlyVoting)
            {
                return "Application";
            }
            else
            {
                return "Permit";
            }
        }
    }
}
