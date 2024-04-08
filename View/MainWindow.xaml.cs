using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Task16.Other;
using Task16.ViewModel;

namespace Task16.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> columnNames = new Dictionary<string, string>
        {
            {"Id", "Id"},
            {"FirstName", "Имя                   "},
            {"Surname", "Фамилия             "},
            {"Patronymic", "Отчество         "},
            {"Email", "Электронная почта                            "},
            {"TelephoneNumber", "Номер телефона         "},
            {"ProductId", "Id товара        "},
            {"ProductName", "Название товара       "}
        };
    public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowVM();
            this.Activated += (DataContext as MainWindowVM).RefreshView;
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
            string headerName = e.Column.Header.ToString();
            if (columnNames.ContainsKey(headerName))
            {
                e.Column.Header = columnNames[headerName];
            }
        }

    }
}