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
using System.Windows.Shapes;

namespace DigicreStudentAccessManager
{
    /// <summary>
    /// AccessLogListView.xaml の相互作用ロジック
    /// </summary>
    public partial class AccessLogListView : Window
    {
        private List<MainWindow.AccessLog> AccessLogs { get; set; }
        public AccessLogListView(List<MainWindow.AccessLog> accessLogs)
        {
            AccessLogs = accessLogs;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            foreach(var al in AccessLogs)
            {
                AccessLogListLabel.Content += $"{al.studentId} : {al.inTime.ToShortTimeString()}入室\n";
            }
        }
    }
}
