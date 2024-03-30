using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUsefullThings;
using Task16.Other;

namespace Task16.ViewModel
{

    public class MainWindowVM : INotifyPropertyChangedPlus
    {
        public string _SqlConnectionString;
        public string SqlConnectionString
        {
            get => _SqlConnectionString;
            set => Set(ref _SqlConnectionString, value);
        }

        public string _SqlConnectionStatus;
        public string SqlConnectionStatus
        {
            get => _SqlConnectionStatus;
            set => Set(ref _SqlConnectionStatus, value);
        }

        public string _AccessConnectionString;
        public string AccessConnectionString
        {
            get => _AccessConnectionString;
            set => Set(ref _AccessConnectionString, value);
        }

        public string _AccessConnectionStatus;
        public string AccessConnectionStatus
        {
            get => _AccessConnectionStatus;
            set => Set(ref _AccessConnectionStatus, value);
        }

        public MainWindowVM()
        {
            _SqlConnectionString = DBConnection.GetMsSqlConnectionString();
            _SqlConnectionStatus = "Connecting...";
            _AccessConnectionString = DBConnection.GetOleDbConnectionString();
            _AccessConnectionStatus = "Connecting...";
            var status = Task.Run(GetConnectionStatus);
            Task.WaitAll(status);
        }

        public async Task GetConnectionStatus()
        {
            SqlConnectionStatus = await DBConnection.IsMSSQLConnectionAccessible() ? "Ok" : "Unaccessible";
            AccessConnectionStatus = await DBConnection.IsOleDbConnectionAccessible() ? "Ok" : "Unaccessible";
        }
    }
}
