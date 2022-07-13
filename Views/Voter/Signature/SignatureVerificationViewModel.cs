using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VoterX.Logging;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Voter.Signature
{
    public class SignatureVerificationViewModel : NotifyPropertyChanged
    {
        protected string _folder = "C:\\VoterX\\Signatures";

        protected string _fileName = "SiteVerification";

        VoterXLogger _signatureLog;

        public SignatureVerificationViewModel()
        {
            if (AppSettings.System.MachineID != 1)
            {
                _folder = AppSettings.System.SignatureFolder;
            }

            string datestamp = DateTime.Now.ToShortDateString().Replace("\\","").Replace("/", "");
            string timestamp = DateTime.Now.ToString("HHmmss");
            _fileName = "SiteCheck_" + AppSettings.System.SiteID.ToString() + "_" + AppSettings.System.MachineID.ToString() + "_" + datestamp + "_" + timestamp;

            _signatureLog = new VoterXLogger("VCClogs", true);

            CanEnablePad = true;
        }

        private bool? _siteVerification;
        public bool SiteVerification
        {
            get
            {
                return _siteVerification ?? false;
            }

            set
            {
                _siteVerification = value;
                RaisePropertyChanged("SiteVerification");
            }
        }

        #region CheckBoxParameters
        private bool _signRefused;
        public bool SignRefused
        {
            get { return _signRefused; }
            set
            {
                if (_signRefused == value) return;
                else
                {
                    _signRefused = value;
                }
            }
        }

        public bool SignatureCaptured
        {
            get
            {
                return SignRefused || IsSignatureLoaded;
            }
        }
        #endregion

        #region ImageData
        private BitmapImage _signatureFile;
        public BitmapImage SignatureFile
        {
            get
            {
                return _signatureFile;
            }
        }

        private async void LoadSignatureFileAsync()
        {
            _signatureFile = null;

            if (await CheckDirectoryAsync(_folder))
            {
            }
            else
            {
                // If network folder not found switch to local
                //_folder = "C:\\VoterX\\Signatures";
            }

            string path = _folder + "\\" + _fileName + ".jpg";

            _signatureLog.WriteLog("Loading Signature: " + path);

            try
            {
                // Try to load the voter's signature
                //_signatureFile = SignatureMethods.LoadSignatureFromFile(VoterItem.Data, _folder);

                using (var stream = File.OpenRead(path))
                //using (var stream = await OpenReadAsync(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    _signatureFile = image;
                }

                //_signatureFile = new BitmapImage(new Uri(path));
            }
            catch (Exception e)
            {
                _signatureLog.WriteLog(e);

                //StatusBar.TextCenter = _errorMessage;
            }

            // When no signature is found enable the Capture Button
            if (_signatureFile != null)
            {
                CanEnablePad = false;                       // Cannot capture another signature
                CanDeleteSignature = true;                  // Can delete a the signature
                CanRefuseSignature = false;                 // Cannot refuse the signature
                RaisePropertyChanged("IsSignatureLoaded");
                RaisePropertyChanged("SignatureCaptured");

                _signatureLog.WriteLog("Signature Loaded");

                RaisePropertyChanged("SignatureFile");
            }
            else
            {
                CanEnablePad = true;                        // Can capture a signature
                CanDeleteSignature = false;                 // Cannot delete a signature
                CanRefuseSignature = true;                  // Can refuse the signature

                _signatureLog.WriteLog("Signature Not Found");

                RaisePropertyChanged("SignatureFile");
            }
            


            StatusBar.TextLeft = "";
            StatusBar.HideLeftSpinner();
        }

        public async Task<bool> CheckDirectoryAsync(string folder)
        {
            return await Task.Run(() => Directory.Exists(folder));
        }

        public async Task<FileStream> OpenReadAsync(string path)
        {
            try
            {
                return await Task.Run(() => OpenRead(path));
            }
            catch (Exception e)
            {
                _signatureLog.WriteLog(e);
                return null;
            }
        }

        private FileStream OpenRead(string path)
        {
            try
            {
                return File.OpenRead(path);
            }
            catch (Exception e)
            {
                _signatureLog.WriteLog(e);
                // CALLING THE STATUS BAR HERE CAUSES IT TO LOCK UP PREVENTING ANY FURTHER STATUS CHANGES
                //StatusBar.TextCenter = e.Message.Replace("\r","").Replace("\n", "");
                return null;
            }
        }

        public bool IsSignatureLoaded
        {
            get
            {
                if (_signatureFile != null) return true;
                return false;
            }
        }
        #endregion

        #region Commands
        // Bound command for returning to the search screen
        public RelayCommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(param => this.ReturnToSearchClick());
                }
                return _goBackCommand;
            }
        }

        // Force parent frame to navigate back to the search page
        public void ReturnToSearchClick()
        {
            //_parent.Navigate(new VoterSearchPage(_parent));
            //NavigationMenuMethods.VoterSearchPage();
            NavigationMenuMethods.ReturnToOrigin();
        }

        private async void CheckSignaturePad()
        {
            CanEnablePad = false;
            CanDeleteSignature = false;

            if ((await Task.Run(() => StatusBar.CheckSignaturePad())) == true)
            {
                CanEnablePad = true;
                CanDeleteSignature = false;
            }
        }

        // Bound command to initiate the Signature Pad
        public RelayCommand _enablePadCommand;
        public ICommand EnablePadCommand
        {
            get
            {
                if (_enablePadCommand == null)
                {
                    _enablePadCommand = new RelayCommand(param => this.EnablePadClick(), param => this.CanEnablePad);
                }
                return _enablePadCommand;
            }
        }

        // Enable or Disable the Capture Signature Button
        public bool CanEnablePad { get; set; }

        // Capture the Voters Signature
        private void EnablePadClick()
        {

            // Disable the Capture Signature Button
            CanEnablePad = false;

            // Set affirmation and voter strings
            string affText = "I, " + "Site" + "  " + "Tester" + " confirm that I am a Registered Voter and to my knowledge have not cast a ballot in this election.";
            string userText = "Site" + "  " + "Tester" + " Party: " + "DTS" + " Birth Year: " + "2020";

            try
            {
                // Check Which Signature Pad To Use
                var ambirPad = AmbirSignatureMethods.Create();

                if (ambirPad.DeviceCount() > 0)
                {
                    EnableAmbirPad(ambirPad, affText, userText);
                }
                else
                {
                    EnableVisionPad(affText, userText);
                }
            }
            catch (Exception e)
            {
                _signatureLog.WriteLog("Enable Signature Pad Failed: " + e.Message);
                if (e.InnerException != null)
                {
                    _signatureLog.WriteLog(e.InnerException.Message);
                }

                try
                {
                    EnableVisionPad(affText, userText);
                }
                catch (Exception error)
                {
                    _signatureLog.WriteLog("Enable Signature Pad Failed: " + error.Message);
                    if (error.InnerException != null)
                    {
                        _signatureLog.WriteLog(e.InnerException.Message);
                    }
                }
            }
        }

        private void EnableAmbirPad(AmbirSigPad.AmbirSignaturePad ambirPad, string affText, string userText)
        {
            _signatureLog.WriteLog("Ambir Signature Pad Loaded");

            var voterMeta = AmbirSignatureMethods.SignatureMetadata(_fileName, AppSettings.System.SiteID.ToString(), AppSettings.System.SiteName);

            string messageText = affText + "\r\n\r\n" + userText;

            string resources = "C:\\Program Files\\VoterX\\Images\\Ambir\\";

            //ambirPad.SetFont("Arial", 12, System.Drawing.FontStyle.Regular);

            //var padFont = new System.Drawing.Font("Times Roman", 12, System.Drawing.FontStyle.Regular);

            if (ambirPad.OpenSignaturePad(_folder, messageText, resources, voterMeta) == true)
            {
                LoadSignatureFileAsync();
                //RaisePropertyChanged("SignatureFile");
            }
            else
            {
                // Reset enable button
                RaisePropertyChanged("SignatureFile");
            }
        }

        private void EnableVisionPad(string affText, string userText)
        {
            _signatureLog.WriteLog("ePad Signature Pad Loaded");

            try
            {
                SignatureMethods.SaveSignatureFromPad(_fileName, _folder, (int)AppSettings.System.SiteID, AppSettings.System.SiteName, affText, userText);
                LoadSignatureFileAsync();
                //RaisePropertyChanged("SignatureFile");
            }
            catch (Exception e)
            {
                _signatureLog.WriteLog("Load Signature Pad Failed: " + e.Message);
                if (e.InnerException != null)
                {
                    _signatureLog.WriteLog(e.InnerException.Message);
                }
            }
        }

        // Bound command to delete the existing file
        public RelayCommand _deleteSignatureCommand;
        public ICommand DeleteSignatureCommand
        {
            get
            {
                if (_deleteSignatureCommand == null)
                {
                    _deleteSignatureCommand = new RelayCommand(param => this.DeleteSignatureClick(), param => this.CanDeleteSignature);
                }
                return _deleteSignatureCommand;
            }
        }

        // Enable or Disable the Delete Signature Button
        public bool CanDeleteSignature { get; set; }

        // Delete the Voters Signature
        private void DeleteSignatureClick()
        {
            // Prevent spamming the delete button
            CanDeleteSignature = false;

            // Check if signature exists
            if (IsSignatureLoaded == true)
            {
                // Prompt for password
                //ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
                //if (passwordDialog.ShowDialog() == true)
                //{
                //    // Delete the file
                //    //SignatureMethods.DeleteVoterSignature(VoterItem.Data, _folder);
                //    RaisePropertyChanged("SignatureFile");
                //    RaisePropertyChanged("SignatureCaptured");
                //}
                //else
                //{
                //    // Display error message
                //    //AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED\nTHE CURRENT SIGNATURE WILL NOT BE DELETED");
                //    wrongPassword.ShowDialog();

                //    // Enable delete button
                //    CanDeleteSignature = true;
                //}
            }
        }

        // Bound command to refuse the existing file
        public RelayCommand _signRefusedCommand;
        public ICommand SignRefusedCommand
        {
            get
            {
                if (_signRefusedCommand == null)
                {
                    _signRefusedCommand = new RelayCommand(param => this.RefuseSignatureClick(), param => this.CanRefuseSignature);
                }
                return _signRefusedCommand;
            }
        }

        // Enable or Disable the Refuse Signature Button
        public bool CanRefuseSignature { get; set; }

        // Refuse the Voters Signature
        private void RefuseSignatureClick()
        {
            // Cannot capture another signature while Sign Refused is checked
            if (SignRefused == true)
            {
                CanEnablePad = false;
            }
            else
            {
                CanEnablePad = true;
            }
            //VoterItem.Data.SignRefused = SignRefused;
            RaisePropertyChanged("SignRefused");
            RaisePropertyChanged("SignatureCaptured");
        }
        #endregion
    }
}
