using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VoterX.Logging;
using VoterX.Core.Voters;
using VoterX.Utilities.Commands;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Utilities.Methods;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Voter.Signature
{
    public class SignatureCaptureViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        public NMVoter VoterItem { get; set; }

        protected string _folder = "C:\\VoterX\\Signatures";

        VoterXLogger _signatureLog;

        public SignatureCaptureViewModel(NMVoter voter)
        {
            VoterItem = voter;

            //CheckSignaturePad();

            SetDisplayMessages();

            //CanEnablePad = true;
            //CanDeleteSignature = false;

            // Display page header
            StatusBar.PageHeader = "Signature Capture";

            if (AppSettings.System.MachineID != 1)
            {
                _folder = AppSettings.System.SignatureFolder;
            }

            _signatureLog = new VoterXLogger("VCClogs", true);

            LoadSignatureFileAsync();
        }

        #region DisplayText
        public string SignatureMessage { get; set; }
        public string SignRefusedMessage { get; set; }
        public string SignRefusedCheck { get; set; }

        private void SetDisplayMessages()
        {
            SignatureMessage = "CAPTURE SIGNATURE FOR CURRENT VOTER";
            SignRefusedMessage = "If Voter Refuses to sign the electronic signature pad, or the signature pad has malfunctioned, check the refused to sign box:";
            SignRefusedCheck = "Voter Refuses to Sign or Signature Pad Malfunctioned";
        }

        private string _errorMessage;
        private bool _errorStatus;
        //private string ErrorMessage
        //{
        //    get
        //    {
        //        return _errorMessage;
        //    }

        //    set
        //    {
        //        _errorMessage = value;
        //        StatusBar.TextCenter = _errorMessage;
        //        RaisePropertyChanged("ErrorMessage");
        //    }
        //}
        #endregion


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
                //_signatureFile = null;

                //string path = _folder + "\\" + VoterItem.Data.VoterID.ToString() + ".jpg";

                //_signatureLog.WriteLog("Loading Signature: " + path);

                //try
                //{
                //    // Try to load the voter's signature
                //    //_signatureFile = SignatureMethods.LoadSignatureFromFile(VoterItem.Data, _folder);

                //    using (var stream = File.OpenRead(path))
                //    //using (var stream = await FileIOExtensions.OpenReadAsync(path))
                //    {
                //        var image = new BitmapImage();
                //        image.BeginInit();
                //        image.CacheOption = BitmapCacheOption.OnLoad;
                //        image.StreamSource = stream;
                //        image.EndInit();
                //        _signatureFile = image;
                //    }

                //    //_signatureFile = new BitmapImage(new Uri(path));
                //}
                //catch (Exception e)
                //{
                //    _signatureLog.WriteLog(e);
                //}

                //// When no signature is found enable the Capture Button
                //if (_signatureFile != null)
                //{
                //    CanEnablePad = false;                       // Cannot capture another signature
                //    CanDeleteSignature = true;                  // Can delete a the signature
                //    CanRefuseSignature = false;                 // Cannot refuse the signature
                //    RaisePropertyChanged("IsSignatureLoaded");
                //    RaisePropertyChanged("SignatureCaptured");

                //    _signatureLog.WriteLog("Signature Loaded");
                //}
                //else
                //{
                //    CanEnablePad = true;                        // Can capture a signature
                //    CanDeleteSignature = false;                 // Cannot delete a signature
                //    CanRefuseSignature = true;                  // Can refuse the signature

                //    _signatureLog.WriteLog("Signature Not Found");
                //}

                return _signatureFile;
            }
        }

        private async void LoadSignatureFileAsync()
        {
            _signatureFile = null;

            StatusBar.TextLeft = "Loading Signature";
            StatusBar.ShowLeftSpinner();

            if (await CheckDirectoryAsync(_folder))
            {
            }
            else
            {
                // If network folder not found switch to local
                _folder = "C:\\VoterX\\Signatures";
            }
            //else
            //{
            //    if (_folder.Substring(0, 2) == "C:")
            //    {
            //        // Server system 1 message
            //        StatusBar.TextCenter = "The local path was not found: " + _folder;
            //    }
            //    else
            //    {
            //        // Client system messages
            //        StatusBar.TextCenter = "The network path was not found: " + _folder;
            //    }

            //    // Allow refuse to sign on error
            //    CanRefuseSignature = true;
            //    RaisePropertyChanged("IsSignatureLoaded");
            //}
            string path = _folder + "\\" + VoterItem.Data.VoterID.ToString() + ".jpg";

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
            else if (_errorStatus != true)
            {
                CanEnablePad = true;                        // Can capture a signature
                CanDeleteSignature = false;                 // Cannot delete a signature
                CanRefuseSignature = true;                  // Can refuse the signature

                _signatureLog.WriteLog("Signature Not Found");

                RaisePropertyChanged("SignatureFile");
            }
            else
            {
                // Allow refuse to sign on error
                CanRefuseSignature = true;
                RaisePropertyChanged("IsSignatureLoaded");
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
                _errorStatus = true;
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
                _errorMessage = e.Message.Replace("\r", "").Replace("\n", "");
                _errorStatus = true;
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

            if((await Task.Run(() => StatusBar.CheckSignaturePad())) == true)
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

            // Disable Sign Refused button
            CanRefuseSignature = false;

            // Set affirmation and voter strings
            string affText = "I, " + VoterItem.Data.FirstName + "  " + VoterItem.Data.LastName + " confirm that I am a Registered Voter and to my knowledge have not cast a ballot in this election.";
            string userText = VoterItem.Data.FirstName + "  " + VoterItem.Data.LastName + " Party: " + VoterItem.Data.Party + " Birth Year: " + VoterItem.Data.DOBYear;

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
                catch(Exception error)
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

            var voterMeta = AmbirSignatureMethods.SignatureMetadata(VoterItem, AppSettings.System.SiteID.ToString(), AppSettings.System.SiteName);

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
                SignatureMethods.SaveSignatureFromPad(VoterItem.Data, _folder, (int)AppSettings.System.SiteID, AppSettings.System.SiteName, affText, userText);
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
                ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
                if (passwordDialog.ShowDialog() == true)
                {
                    // Delete the file
                    SignatureMethods.DeleteVoterSignature(VoterItem.Data, _folder);
                    _signatureFile = null;
                    CanEnablePad = true;
                    CanDeleteSignature = false;
                    CanRefuseSignature = true;
                    RaisePropertyChanged("IsSignatureLoaded");
                    RaisePropertyChanged("SignatureFile");
                    RaisePropertyChanged("SignatureCaptured");
                }
                else
                {
                    // Display error message
                    AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED\nTHE CURRENT SIGNATURE WILL NOT BE DELETED");
                    wrongPassword.ShowDialog();

                    // Enable delete button
                    CanDeleteSignature = true;
                }
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
            VoterItem.Data.SignRefused = SignRefused;
            RaisePropertyChanged("SignRefused");
            RaisePropertyChanged("SignatureCaptured");
        }

        public RelayCommand _printBallotCommand;
        public ICommand PrintBallotCommand
        {
            get
            {
                if (_printBallotCommand == null)
                {
                    _printBallotCommand = new RelayCommand(param => this.PrintBallotClick());
                }
                return _printBallotCommand;
            }
        }

        private void PrintBallotClick()
        {
            //_parent.Navigate(new PrintOfficialBallotPage(_parent, (NMVoter)VoterItem));
            NavigationMenuMethods.PrintOfficialBallotPage(VoterItem);
        }
        #endregion

    }
}
