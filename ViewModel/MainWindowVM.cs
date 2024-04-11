using WPFUsefullThings;
using Task16.Other;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Windows.Input;
using Task16.View;
using System.Windows;
using System.ComponentModel;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace Task16.ViewModel
{

    public class MainWindowVM : INotifyPropertyChangedPlus
    {
        public SqlDataAdapter SqlDataAdapter => DataAdapterInitializer.Instance.SqlDataAdapter;
        public OleDbDataAdapter OleDbDataAdapter => DataAdapterInitializer.Instance.OleDbDataAdapter;

        private DataTable _Clients;
        public DataTable Clients
        {
            get => _Clients;
            set => Set(ref _Clients, value);
        }
        public DataTable Orders { get; set; }

        public DataView _OrdersView;

        public DataView OrdersView
        {
            get => _OrdersView;
            set => Set(ref _OrdersView, value);
        }


        private DataRowView? _SelectedClient;
        public DataRowView? SelectedClient
        {
            get => _SelectedClient;
            set => Set(ref _SelectedClient, value);
        }

        private DataRowView? _SelectedOrder;
        public DataRowView? SelectedOrder
        {
            get => _SelectedOrder;
            set => Set(ref _SelectedOrder, value);
        }

        private bool _IsAllOrdersVisible = true;
        public bool IsAllOrdersVisible
        {
            get => _IsAllOrdersVisible;
            set => Set(ref _IsAllOrdersVisible, value);
        }

        public ICommand AddNewClientCommand { get; }
        public ICommand ChangeClientCommand { get; }
        public ICommand DeleteClientCommand { get; }

        public ICommand AddNewOrderCommand { get; }
        public ICommand ChangeOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public MainWindowVM()
        {
            SubscribeToPropertyChanged(MainWindowVM_PropertyChanged);

            Clients = new DataTable();
            Orders = new DataTable();
            SqlDataAdapter.Fill(Clients);
            OleDbDataAdapter.Fill(Orders);
            Clients.PrimaryKey = [Clients.Columns["Id"]];
            Orders.PrimaryKey = [Orders.Columns["Id"]];

            AddNewClientCommand = new RelayCommand(obj => ExecuteChangeClient());
            ChangeClientCommand = new RelayCommand(obj => ExecuteChangeClient(SelectedClient.Row), obj => SelectedClient != null);
            DeleteClientCommand = new RelayCommand(obj => ExecuteDeleteClient(SelectedClient.Row), obj => SelectedClient != null);

            AddNewOrderCommand = new RelayCommand(obj => ExecuteChangeOrder(SelectedClient != null ? SelectedClient.Row["Email"].ToString() : null));
            ChangeOrderCommand = new RelayCommand(obj => ExecuteChangeOrder(null, SelectedOrder.Row), obj => SelectedOrder != null);
            DeleteOrderCommand = new RelayCommand(obj => ExecuteDeleteOrder(SelectedOrder.Row), obj => SelectedOrder != null);
        }

        public void RefreshOrders()
        {
            using (var connection = DBConnections.GetOleDbConnection())
            {
                connection.Open();
                using (var command = new OleDbCommand("SELECT * FROM Orders", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        Orders.Load(reader);
                    }
                }
            }

            OrdersView = new DataView(Orders);

            if (IsAllOrdersVisible)
            {
                return;
            }
            if (SelectedClient == null)
            {
                OrdersView.RowFilter = "Email = ''";
                return;
            }
            else
            {
                var email = SelectedClient["Email"].ToString();
                OrdersView.RowFilter = $"Email = '{email.Replace("'", "''")}'";
            }

            //var selection = Orders.AsEnumerable()
            //    .Where(row => row.Field<string>("Email") == email);
            //if (selection.Any())
            //{
            //    OrdersView = selection.CopyToDataTable();
            //}
            //else
            //{
            //    OrdersView = Orders.Copy();
            //    OrdersView.Rows.Clear();
            //}

        }

        public void RefreshView(object sender, EventArgs e)
        {
            RefreshView();
        }

        public void RefreshView()
        {
            using (var connection = DBConnections.Instance.SqlConnection)
            {
                connection.Open();
                using (var reader = SqlDataAdapter.SelectCommand.ExecuteReader())
                {
                    Clients.Load(reader);

                }
                connection.Close();
            }

            //SelectedClient = null;
            //var newClients = new DataTable();
            //SqlDataAdapter.Fill(newClients);
            //Clients = newClients;
            RefreshOrders();
        }

        private void ExecuteChangeClient(DataRow? clientRow = null)
        {
            var clientView = new ClientView(Clients, SqlDataAdapter, clientRow);
            clientView.ShowDialog();
        }

        private void ExecuteDeleteClient(DataRow clientRow)
        {
            if (Orders.Rows.IsAnyOrder(clientRow))
            {
                MessageBox.Show("Нельзя удалить клиента у которого есть заказы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            clientRow.Delete();
            SqlDataAdapter.Update(Clients);
        }

        private void ExecuteChangeOrder(string? clientEmail, DataRow? orderRow = null)
        {
            var clientsList = Clients.GetClientsEmailDictionary();

            var orderView = new OrderView(OleDbDataAdapter, Orders, clientsList, clientEmail, orderRow);
            orderView.ShowDialog();
        }

        private void ExecuteDeleteOrder(DataRow orderRow)
        {
            orderRow.Delete();
            OleDbDataAdapter.Update(Orders);
        }

        private void MainWindowVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedClient) || e.PropertyName == nameof(IsAllOrdersVisible))
            {
                RefreshOrders();
            }
        }
    }
}
