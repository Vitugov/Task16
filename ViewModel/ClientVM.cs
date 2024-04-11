using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUsefullThings;
using WPFUsefullThings.Validation;

namespace Task16.ViewModel
{
    public class ClientVM : INotifyPropertyChangedPlus
    {
        public SqlDataAdapter DataAdapter { get; }
        public DataTable Clients { get; }
        
        public DataRow CurrentClientRow { get; }

        public bool IsNew { get; }

        private string _Surname;
        public string Surname
        {
            get => _Surname;
            set => Set(ref _Surname, value);
        }

        private string _FirstName;
        public string FirstName
        {
            get => _FirstName;
            set => Set(ref _FirstName, value);
        }

        private string _Patronymic;
        public string Patronymic
        {
            get => _Patronymic;
            set => Set(ref _Patronymic, value);
        }

        private string _TelephoneNumber;
        public string TelephoneNumber
        {
            get => _TelephoneNumber;
            set => Set(ref _TelephoneNumber, value);
        }

        private string _Email;
        public string Email
        {
            get => _Email;
            set => Set(ref _Email, value);
        }

        private bool _SurnameValid;
        public bool SurnameValid
        {
            get => _SurnameValid;
            set => Set(ref _SurnameValid, value);
        }

        private bool _FirstNameValid;
        public bool FirstNameValid
        {
            get => _FirstNameValid;
            set => Set(ref _FirstNameValid, value);
        }

        private bool _PatronymicValid;
        public bool PatronymicValid
        {
            get => _PatronymicValid;
            set => Set(ref _PatronymicValid, value);
        }

        private bool _TelephoneNumberValid;
        public bool TelephoneNumberValid
        {
            get => _TelephoneNumberValid;
            set => Set(ref _TelephoneNumberValid, value);
        }

        private bool _EmailValid;
        public bool EmailValid
        {
            get => _EmailValid;
            set => Set(ref _EmailValid, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ClientVM(DataTable dataTable, SqlDataAdapter dataAdapter, DataRow? clientRow = null)
        {
            SubscribeToPropertyChanged(ClientVM_PropertyChanged);

            DataAdapter = dataAdapter;
            Clients = dataTable;
            IsNew = (clientRow == null);
            CurrentClientRow = clientRow ?? Clients.NewRow();

            Surname = CurrentClientRow["Surname"].ToString() ?? "";
            FirstName = CurrentClientRow["FirstName"].ToString() ?? "";
            Patronymic = CurrentClientRow["Patronymic"].ToString() ?? "";
            TelephoneNumber = CurrentClientRow["TelephoneNumber"].ToString() ?? "";
            Email = CurrentClientRow["Email"].ToString() ?? "";

            SaveCommand = new RelayCommand(obj => ExecuteSaveCommand(obj));
            CancelCommand = new RelayCommand(obj => CloseWindow(obj));
        }

        private void ExecuteSaveCommand(object obj)
        {
            if (!Validate())
            {
                ValidationRules.ShowErrorMessage();
                return;
            }

            SaveDataToRow();

            if (IsNew)
            {
                Clients.Rows.Add(CurrentClientRow);
            }

            DataAdapter.Update(Clients);
            Clients.AcceptChanges();
            CloseWindow(obj);
        }

        private void SaveDataToRow()
        {
            CurrentClientRow["Surname"] = Surname;
            CurrentClientRow["FirstName"] = FirstName;
            CurrentClientRow["Patronymic"] = Patronymic;
            CurrentClientRow["TelephoneNumber"] = TelephoneNumber;
            CurrentClientRow["Email"] = Email;
            
        }

        internal void CloseWindow(object obj) => (obj as Window).Close();

        private bool Validate()
        {
            var validationArray = new bool[]
            {
                SurnameValid,
                FirstNameValid,
                PatronymicValid,
                TelephoneNumberValid,
                EmailValid,
            };

            return validationArray.IsValid();
        }

        private void ClientVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Surname))
            {
                SurnameValid = Surname.ValidateNotEmptyString();
            }
            if (e.PropertyName == nameof(FirstName))
            {
                FirstNameValid = FirstName.ValidateNotEmptyString();
            }
            if (e.PropertyName == nameof(Patronymic))
            {
                PatronymicValid = Patronymic.ValidateNotEmptyString();
            }
            if (e.PropertyName == nameof(TelephoneNumber))
            {
                TelephoneNumberValid = true;
            }
            if (e.PropertyName == nameof(Email))
            {
                EmailValid = Email.ValidateEmail();
            }
        }


    }
}
