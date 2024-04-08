using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUsefullThings;
using Task16.Other;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Management.Smo.Agent;
using System.Data.OleDb;
using System.Windows.Input;
using Task16.View;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Data.Common;

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

        public DataTable _OrdersView;
        
        public DataTable OrdersView
        {
            get => _OrdersView;
            set => Set(ref _OrdersView, value);
        }

        
        private DataRowView? _SelectedClient;
        public DataRowView? SelectedClient
        {
            get => _SelectedClient;
            set => SetWithAction(ref _SelectedClient, value, RefreshOrders);
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
            set => SetWithAction(ref _IsAllOrdersVisible, value, RefreshOrders);
        }

        public ICommand AddNewClientCommand { get; }
        public ICommand ChangeClientCommand { get; }
        public ICommand DeleteClientCommand { get; }

        public ICommand AddNewOrderCommand { get; }
        public ICommand ChangeOrderCommand { get; }
        public ICommand DeleteOrderCommand { get; }

        public MainWindowVM()
        {
            Clients = new DataTable();
            Orders = new DataTable();
            SqlDataAdapter.Fill(Clients);
            OleDbDataAdapter.Fill(Orders);
            RefreshOrders();
            AddNewClientCommand = new RelayCommand(obj => ExecuteChangeClient());
            ChangeClientCommand = new RelayCommand(obj => ExecuteChangeClient(SelectedClient.Row), obj => SelectedClient != null );
            DeleteClientCommand = new RelayCommand(obj => ExecuteDeleteClient(SelectedClient.Row), obj => SelectedClient != null );
        }

        public void RefreshOrders()
        {
            DataTable newOrders = new DataTable();
            OleDbDataAdapter.Fill(newOrders);
            Orders = newOrders;
            OrdersView = newOrders;
            if (IsAllOrdersVisible)
            {
                return;
            }
            if (SelectedClient == null)
            {
                //OrdersView = Orders;
                OrdersView = Orders.Copy();
                OrdersView.Rows.Clear();
                return;
            }

            var email = SelectedClient["Email"].ToString();
            Orders = new DataTable();
            OleDbDataAdapter.Fill(Orders);
            var selection = Orders.AsEnumerable()
                .Where(row => row.Field<string>("Email") == email);
            if (selection.Any())
            {
                OrdersView = selection.CopyToDataTable();
            }
            else
            {
                OrdersView = Orders.Copy();
                OrdersView.Rows.Clear();
            }

        }

        public void RefreshView(object sender, EventArgs e)
        {
            RefreshView();
        }

        public void RefreshView()
        {
            SelectedClient = null;
            var newClients = new DataTable();
            SqlDataAdapter.Fill(newClients);
            Clients = newClients;
            RefreshOrders();
        }

        private void ExecuteChangeClient(DataRow? clientRow = null)
        {
            var clientView = new ClientView(Clients, SqlDataAdapter, clientRow);
            clientView.ShowDialog();
        }

        private void ExecuteDeleteClient(DataRow clientRow)
        {
            clientRow.Delete();
            SqlDataAdapter.Update(Clients);
        }
    }
}
