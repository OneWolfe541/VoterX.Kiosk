using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoterX.Kiosk.Methods;
using VoterX.Utilities.Methods;
using VoterX.Utilities.Dialogs;
using VoterX.Utilities.Extensions;
using VoterX.Kiosk.Views.Voter;
using VoterX.Core.Voters;

namespace VoterX.Kiosk.Views.Signature
{
    /// <summary>
    /// Interaction logic for SignatureCapturePage.xaml
    /// </summary>
    public partial class SignatureCapturePage : Page
    {
        private NMVoter _voter = new NMVoter();

        private bool exitLogout = false;

        public SignatureCapturePage(NMVoter voter)
        {
            InitializeComponent(); 

            _voter = voter;

            // Display voter details on the page
            LoadVoterFields(voter);

            // Display any existing signature on the page
            LoadSignatureImage(voter);            

            StatusBar.PageHeader = "Signature Capture";

            //StatusBar.CheckSignaturePad();

            StatusBar.Clear();            
        }

        public SignatureCapturePage(NMVoter voter, bool returnStatus)
        {
            InitializeComponent();

            _voter = voter;

            exitLogout = returnStatus;
            if (returnStatus == true)
            {
                BackReturnLabel.Visibility = Visibility.Collapsed;
                BackLogoutLabel.Visibility = Visibility.Visible;
            }

            // Display voter details on the page
            LoadVoterFields(voter);

            // Display any existing signature on the page
            LoadSignatureImage(voter);

            StatusBar.PageHeader = "Signature Capture";

            //StatusBar.SignaturePadStatus = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new SearchPage());
        }

        // Load the voter details into TextBlocks on the page
        // I should consider making the voter details into a separate user control (Same with the signature pad)
        private void LoadVoterFields(NMVoter voter)
        {
            FullName.Text = voter.Data.FullName;
            BirthYear.Text = voter.Data.DOBYear;
            Address.Text = voter.Data.Address1;
            CityStateAndZip.Text = voter.Data.City + ", " + voter.Data.State + " " + voter.Data.Zip;
        }

        // Load a new file into the image control
        private void LoadSignatureImage(NMVoter voter)
        {
            //VoterSignature.Source = null;
            //VoterSignature.Source = SignatureMethods.LoadSignatureFromFile(voter, AppSettings.System.SignatureFolder);

            //if (VoterSignature.Source == null)
            //{
            //    //StatusBar.ApplicationStatus("No Signature File Found");
            //}
            //else
            //{
            //    //StatusBar.ApplicationStatus("Signature File Found");
            //    PrintBallot.Visibility = Visibility.Visible;
            //}

            SignatureCaptureControl.Voter = voter.Data;
            SignatureCaptureControl.Folder = AppSettings.System.SignatureFolder;
            SignatureCaptureControl.Settings = AppSettings.System;
            if(SignatureCaptureControl.LoadSignature() == true)
            {
                //StatusBar.StatusTextLeft = "Signature File Found";
                PrintBallot.Visibility = Visibility.Visible;

                SignRefused.IsEnabled = false; 
            }
            else
            {
                //StatusBar.StatusTextLeft = "No Signature File Found";
            }
        }

        // Clear the image and delete the existing file
        private void DeleteExistingFile(NMVoter voter)
        {
            //// Clear the image control
            //VoterSignature.Source = null;

            //// Display which file is being deleted
            //StatusBar.StatusTextLeft = "Searching For: " + voter.VoterID.ToString();

            //if (SignatureMethods.DeleteVoterSignature(voter, AppSettings.System.SignatureFolder))
            //{
            //    StatusBar.StatusTextLeft = "File Deleted: " + voter.VoterID.ToString();
            //}
            //else
            //{
            //    StatusBar.StatusTextLeft = "File not found: " + voter.VoterID.ToString();
            //}            
        }

        // Turn on the signature pad and wait for voter to sign or cancel
        private void EnablePadButton_Click(object sender, RoutedEventArgs e)
        {
            // Delete existing signature file
            DeleteExistingFile(_voter);

            // Set affirmation and voter strings
            string affText = "I, " + _voter.Data.FirstName + " " + _voter.Data.LastName + " confirm that I am a Regsitered Voter and to my knowledge have not cast a ballot in this election.";
            string userText = _voter.Data.FirstName + " " + _voter.Data.LastName + " Party: " + _voter.Data.Party + " Birth Year: " + _voter.Data.DOBYear;

            // Save voter signature from sigPad then display image on the page
            if (SignatureMethods.SaveSignatureFromPad(
                _voter.Data, 
                AppSettings.System.SignatureFolder,
                (int)AppSettings.System.SiteID, 
                AppSettings.System.SiteName, 
                affText, 
                userText))
                LoadSignatureImage(_voter);
        }

        // Clear the image and delete the existing file
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if file exists
            if (SignatureMethods.VoterSignatureExists(_voter.Data, AppSettings.System.SignatureFolder) == true)
            {
                ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
                if (passwordDialog.ShowDialog() == true)
                {
                    PrintBallot.Visibility = Visibility.Collapsed;
                    DeleteExistingFile(_voter);
                }
                else
                {
                    // Display error message
                    AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED\nTHE CURRENT SIGNATURE WILL NOT BE DELETED");
                    wrongPassword.ShowDialog();
                }
            }
            else
            {
                // Display error message
                AlertDialog wrongPassword = new AlertDialog("THERE IS NO SIGNATURE TO DELETE");
                wrongPassword.ShowDialog();
            }
        }

        // Turns On or Off the print button
        // Sets the Sign Refused field in the voter record
        private void SignRefused_Click(object sender, RoutedEventArgs e)
        {
            // If no image exists turn on or off the print button
            if (SignatureCaptureControl.IsSignatureLoaded == false)
            {
                if (SignRefused.IsChecked == true)
                {
                    PrintBallot.Visibility = Visibility.Visible;
                    SignatureCaptureControl.RefuseToSign = true;
                }
                else
                {
                    PrintBallot.Visibility = Visibility.Collapsed;
                    SignatureCaptureControl.RefuseToSign = false;
                }
            }
            else
            {
                // Display warning message
                RefuseVerificationPanel.Visibility = Visibility.Visible;
                PrintBallot.Visibility = Visibility.Collapsed;
            }

            // Regardless if an image exists or not then set the sign refused field
            if (SignRefused.IsChecked == true)
            {
                _voter.Data.SignRefused = true;
            }
            else
            {
                _voter.Data.SignRefused = false;
            }

        }

        private void RefusedSignatureYes_Click(object sender, RoutedEventArgs e)
        {
            // Turn on Yes button
            RefusedSignatureYes.IsChecked = true;
            refuse_fa_check_yes.Visibility = Visibility.Visible;

            // Turn off the No button
            refuse_fa_check_no.Visibility = Visibility.Collapsed;
            RefusedSignatureNo.IsChecked = false;

            // Set sign refused to true
            SignRefused.IsChecked = true;
            _voter.Data.SignRefused = true;

            // Delete image file
            ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
            if (passwordDialog.ShowDialog() == true)
            {
                PrintBallot.Visibility = Visibility.Visible;
                //DeleteExistingFile(_voter);
                SignatureCaptureControl.DeleteExistingFile();
            }
            else
            {
                // Display error message
                AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED\nTHE CURRENT SIGNATURE WILL STILL BE ACCEPTED");
                wrongPassword.ShowDialog();

                // Clear check box and sign refused field
                SignRefused.IsChecked = false;
                _voter.Data.SignRefused = false;
                refuse_fa_check_yes.Visibility = Visibility.Collapsed;
                RefuseVerificationPanel.Visibility = Visibility.Collapsed;

                PrintBallot.Visibility = Visibility.Visible;
            }
        }

        private void RefusedSignatureNo_Click(object sender, RoutedEventArgs e)
        {
            // Turn on No button
            RefusedSignatureNo.IsChecked = true;
            refuse_fa_check_no.Visibility = Visibility.Visible;

            // Turn off the Yes button
            refuse_fa_check_yes.Visibility = Visibility.Collapsed;
            RefusedSignatureYes.IsChecked = false;

            PrintBallot.Visibility = Visibility.Visible;

            // Set sign refused to false
            SignRefused.IsChecked = false;
            _voter.Data.SignRefused = false;

            // Hide "Are you sure" question
            RefuseVerificationPanel.Visibility = Visibility.Collapsed;
            // Turn off the No button
            refuse_fa_check_no.Visibility = Visibility.Collapsed;
            RefusedSignatureNo.IsChecked = false;
        }

        private void PrintBallot_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToPage(new Ballots.PrintBundlePage(_voter));
        }

        private void Signature_EnablePadClick(object sender, RoutedEventArgs e)
        {
            if (SignatureCaptureControl.IsSignatureLoaded == true)
            {
                PrintBallot.Visibility = Visibility.Visible;

                SignRefused.IsEnabled = false;
            }
            else
            {
                PrintBallot.Visibility = Visibility.Collapsed;
                //StatusBar.StatusTextCenter = SignatureCaptureControl.Error;

                SignRefused.IsEnabled = true;
            }
        }

        private void Signature_DeleteClick(object sender, RoutedEventArgs e)
        {
            // Check if file exists
            if (SignatureMethods.VoterSignatureExists(_voter.Data, AppSettings.System.SignatureFolder) == true)
            {
                ValidationDialog passwordDialog = new ValidationDialog(AppSettings.User, "Manager");
                if (passwordDialog.ShowDialog() == true)
                {
                    PrintBallot.Visibility = Visibility.Collapsed;
                    //DeleteExistingFile(_voter);
                    SignatureCaptureControl.DeleteExistingFile();

                    SignRefused.IsEnabled = true;
                    SignatureCaptureControl.CanEnablePad = true;
                }
                else
                {
                    // Display error message
                    AlertDialog wrongPassword = new AlertDialog("THE WRONG PASSWORD WAS ENTERED\nTHE CURRENT SIGNATURE WILL NOT BE DELETED");
                    wrongPassword.ShowDialog();

                    SignatureCaptureControl.CanEnablePad = false;
                }
            }
            else
            {
                // Display error message
                AlertDialog wrongPassword = new AlertDialog("THERE IS NO SIGNATURE TO DELETE");
                wrongPassword.ShowDialog();
            }

            //// Hide Print Ballot Button
            //PrintBallot.Visibility = Visibility.Collapsed; 

            ////StatusBar.StatusTextCenter = SignatureCaptureControl.Error;

            //// Hide warning message
            //RefuseVerificationPanel.Visibility = Visibility.Collapsed;
        }

        private void Signature_EnablePadClickPreview(object sender, RoutedEventArgs e)
        {
            //// Hide Print Ballot Button
            //PrintBallot.Visibility = Visibility.Collapsed;

            //SignRefused.IsEnabled = false;

            if (SignatureCaptureControl.IsSignatureLoaded == true)
            {
                PrintBallot.Visibility = Visibility.Visible;

                SignRefused.IsEnabled = false;
            }
            else
            {
                PrintBallot.Visibility = Visibility.Collapsed;
                //StatusBar.StatusTextCenter = SignatureCaptureControl.Error;

                SignRefused.IsEnabled = true;
            }
        }
    }
}
