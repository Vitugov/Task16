using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUsefullThings;

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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ClientVM(DataTable dataTable, SqlDataAdapter dataAdapter, DataRow? clientRow = null)
        {
            DataAdapter = dataAdapter;
            Clients = dataTable;
            IsNew = (clientRow == null);
            CurrentClientRow = clientRow ?? Clients.NewRow();

            Surname = CurrentClientRow["Surname"].ToString() ?? "";
            FirstName = CurrentClientRow["FirstName"].ToString() ?? "";
            Patronymic = CurrentClientRow["Patronymic"].ToString() ?? "";
            TelephoneNumber = CurrentClientRow["TelephoneNumber"].ToString() ?? "";
            Email = CurrentClientRow["Email"].ToString() ?? "";

            SaveCommand = new RelayCommand(obj => { ExecuteSaveCommand(); CloseWindow(obj); });
            CancelCommand = new RelayCommand(obj => CloseWindow(obj));
        }

        private void ExecuteSaveCommand()
        {
            CurrentClientRow["Surname"] = Surname;
            CurrentClientRow["FirstName"] = FirstName;
            CurrentClientRow["Patronymic"] = Patronymic;
            CurrentClientRow["TelephoneNumber"] = TelephoneNumber;
            CurrentClientRow["Email"] = Email;
            
            if (IsNew)
            {
                Clients.Rows.Add(CurrentClientRow);
            }

            DataAdapter.Update(Clients);
        }

        internal void CloseWindow(object obj) => (obj as Window).Close();

    }
}
