using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoterX.Core.Voters;
using VoterX.Utilities.Commands;
using VoterX.Kiosk.Methods;

namespace VoterX.Kiosk.Views.Validation
{
    // Contains basic properties and logic that all Voter Validation Pages use
    public class VerifyVoterBaseViewModel : NotifyPropertyChanged
    {
        // Dont use Observable Collections for single voter display
        // Have to use INotifyPropertyChanged when not using an Observable Collection
        public NMVoter VoterItem { get; set; }

        protected VoterSearchModel _searchItems;

        #region IdRequired
        public bool IdRequired
        {
            get
            {
                if(AppSettings.System.IdRequired == true)
                {
                    return true;
                }
                else
                {
                    return VoterItem.IdRequired().Value;
                }
            }
        }
        #endregion

        #region ValidationLogic
        // Name is valid checkbox
        private bool _nameIsSelected;
        public bool NameIsSelected
        {
            get { return _nameIsSelected; }
            set
            {
                if (_nameIsSelected == value) return;
                else
                {
                    _nameIsSelected = value;
                    if (_nameIsSelected == true) Console.WriteLine("Name is Selected");
                    ValidateVoter();
                }
            }
        }

        // Address is valid checkbox
        private bool _addressIsSelected;
        public bool AddressIsSelected
        {
            get { return _addressIsSelected; }
            set
            {
                if (_addressIsSelected == value) return;
                else
                {
                    _addressIsSelected = value;
                    if (_addressIsSelected == true) Console.WriteLine("Address is Selected");
                    ValidateVoter();
                }
            }
        }

        // Birth year is valid checkbox
        private bool _dateIsSelected;
        public bool DateIsSelected
        {
            get { return _dateIsSelected; }
            set
            {
                if (_dateIsSelected == value) return;
                else
                {
                    _dateIsSelected = value;
                    if (_dateIsSelected == true) Console.WriteLine("Date is Selected");
                    ValidateVoter();
                }
            }
        }

        // Valid ID was presented checkbox
        private bool? _idIsSelected;
        public bool? IDIsSelected
        {
            get { return _idIsSelected; }
            set
            {
                if (_idIsSelected == value) return;
                else
                {
                    _idIsSelected = value;
                    if (_idIsSelected == true) Console.WriteLine("ID Required is Selected");
                    ValidateVoter();
                }
            }
        }

        // Check if all the boxes have been clicked        
        protected void ValidateVoter()
        {
            _provisionalVoter = false;

            if ((AppSettings.System.IdRequired == true || VoterItem.Data.IDRequired == true) && !VoterItem.HasVoted())
            {
                // Sum of all boxes equals true
                _voterIsValid = (bool)(_idIsSelected == null ? false : _idIsSelected)
                                && _nameIsSelected
                                && _addressIsSelected
                                && _dateIsSelected;

                // When Id is required and not given then display Provisional button
                if (_idIsSelected == false)
                {
                    _provisionalVoter = _nameIsSelected && _addressIsSelected && _dateIsSelected;
                }
            }
            else
            {
                // Sum of all boxes equals true
                _voterIsValid = _nameIsSelected && _addressIsSelected && _dateIsSelected;
            }
            // Update property state for both buttons
            RaisePropertyChanged("VoterIsValid");
            RaisePropertyChanged("ProvisionalVoter");
        }

        // Return the state of combined checkboxes
        private bool _voterIsValid;
        public bool VoterIsValid
        {
            get
            {
                return _voterIsValid;
            }
        }

        // Return the state of combined checkboxes
        private bool _provisionalVoter;
        public bool ProvisionalVoter
        {
            get
            {
                return _provisionalVoter;
            }
        }
        #endregion


    }
}
