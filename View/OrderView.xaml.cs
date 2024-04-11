using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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
using System.Windows.Shapes;
using Task16.ViewModel;

namespace Task16.View
{
    /// <summary>
    /// Логика взаимодействия для OrderView.xaml
    /// </summary>
    public partial class OrderView : Window
    {
        public OrderView(OleDbDataAdapter dataAdapter, DataTable orders, Dictionary<string, string> clientsList, string? clientEmail = null, DataRow ? orderRow = null )
        {
            InitializeComponent();
            DataContext = new OrderVM(dataAdapter, orders, clientsList, clientEmail, orderRow);
        }
    }
}
