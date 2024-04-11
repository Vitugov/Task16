using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;
using WPFUsefullThings;
using WPFUsefullThings.Validation;

namespace Task16.ViewModel
{
    public class OrderVM : INotifyPropertyChangedPlus
    {
        public OleDbDataAdapter DataAdapter { get; }
        public DataTable Orders { get; }
        public DataRow CurrentOrdersRow { get; }
        
        private Dictionary<string, string> _ClientsEmailDic;
        public Dictionary<string, string> ClientsEmailDic
        {
            get => _ClientsEmailDic;
            set => Set(ref _ClientsEmailDic, value);
        }

        public bool IsNew { get; }

        private string _Email;
        public string Email
        {
            get => _Email;
            set => Set(ref _Email, value);
        }
        
        private bool _EmailValid;
        public bool EmailValid
        {
            get => _EmailValid;
            set => Set(ref _EmailValid, value);
        }

        private int _ProductId;
        public int ProductId
        {
            get => _ProductId;
            set => Set(ref _ProductId, value);
        }
        
        public bool _ProductIdValid;
        public bool ProductIdValid
        {
            get => _ProductIdValid;
            set => Set(ref _ProductIdValid, value);
        }

        private string _ProductName;
        public string ProductName
        {
            get => _ProductName;
            set => Set(ref _ProductName, value);
        }
        
        private bool _ProductNameValid;
        public bool ProductNameValid
        {
            get => _ProductNameValid;
            set => Set(ref _ProductNameValid, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public OrderVM(OleDbDataAdapter dataAdapter, DataTable orders, Dictionary<string, string> clientsEmailDic, string? clientEmail = null, DataRow? currentOrdersRow = null)
        {
            SubscribeToPropertyChanged(OrderVM_PropertyChanged);
            
            DataAdapter = dataAdapter;
            Orders = orders;
            ClientsEmailDic = clientsEmailDic;
            IsNew = (currentOrdersRow == null);

            if (IsNew)
            {
                CurrentOrdersRow = Orders.NewRow();
                Email = clientEmail;
                ProductId = 0;
                ProductName = "";
            }
            else
            {
                CurrentOrdersRow = currentOrdersRow;
                Email = CurrentOrdersRow["Email"].ToString();
                ProductId = (int)(CurrentOrdersRow["ProductId"]);
                ProductName = CurrentOrdersRow["ProductName"].ToString();
            }

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
            AddRowToTableIfNew();
            CheckRowBelongsToTable();
            UpdateOleDB();
            CloseWindow(obj);
        }

        private void SaveDataToRow()
        {
            CurrentOrdersRow["Email"] = Email;
            CurrentOrdersRow["ProductId"] = ProductId;
            CurrentOrdersRow["ProductName"] = ProductName;
        }

        private void AddRowToTableIfNew()
        {
            if (IsNew)
            {
                Orders.Rows.Add(CurrentOrdersRow);
            }
        }

        private void CheckRowBelongsToTable()
        {
            if (CurrentOrdersRow.RowState == DataRowState.Detached || CurrentOrdersRow.Table != Orders)
            {
                throw new InvalidOperationException();
            }
        }

        private void UpdateOleDB()
        {
            try
            {
                DataAdapter.Update(Orders);
                Orders.AcceptChanges();
            }
            catch (DBConcurrencyException ex)
            {
                MessageBox.Show(ex.Row.ToString());
            }
        }

        private void CloseWindow(object obj) => (obj as Window).Close();

        private bool Validate()
        {
            var validationArray = new bool[]
            {
                EmailValid,
                ProductIdValid,
                ProductNameValid
            };

            return validationArray.IsValid();
        }

        private void OrderVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Email))
            {
                EmailValid = Email.ValidateEmail();
            }
            if (e.PropertyName == nameof(ProductId))
            {
                ProductIdValid = ProductId.ValidateMoreThanZero();
            }
            if (e.PropertyName == nameof(ProductName))
            {
                ProductNameValid = ProductName.ValidateNotEmptyString();
            }
        }
    }
}
