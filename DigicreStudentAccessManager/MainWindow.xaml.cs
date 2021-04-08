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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using FelicaLib;
using System.IO;

namespace DigicreStudentAccessManager
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            currentDate = DateTime.Now;
            targetFilePath = $"Log_{ DateTime.Now.ToShortDateString()}.csv";
            InitializeComponent();
        }
        public class AccessLog
        {
            public string studentId { get; set; }
            public DateTime inTime { get; set; }
        }
        private List<AccessLog> accessLogs = new List<AccessLog>();
        private string targetFilePath = "logs.csv";
        private DateTime currentDate;
        private bool felicaCheckFlag = false;
        private void OnTimer(object sender,EventArgs e)
        {
            ClockLabel.Content = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            try
            {
                using (Felica f = new Felica())
                {
                    string studentId = ReadStudentID(f);
                    StudentIDText.Content = studentId +" さん";
                    if (felicaCheckFlag) return;
                    //検索処理
                    if (accessLogs.Count(al => al.studentId == studentId) == 0)
                    {
                        //見つからない場合
                        accessLogs.Add(new AccessLog() { studentId = studentId, inTime = DateTime.Now });
                        OutputLog(accessLogs[accessLogs.Count - 1], InOutKind.入室);
                        StatusText.Content = "入室";
                    }
                    else
                    {
                        //見つかった場合
                        var target = accessLogs.Find(al => al.studentId == studentId);
                        var passTime = DateTime.Now - target.inTime;
                        if (passTime < new TimeSpan(0, 0, 10)) return;
                        OutputLog(target, InOutKind.退室);
                        accessLogs.Remove(target);
                        StatusText.Content = "退室";
                    }
                    felicaCheckFlag = true;
                    
                }
            }
            catch (Exception ex)
            {
                AllViewClear();
                felicaCheckFlag = false;
                Console.WriteLine(ex.Message);
            }
        }
        enum InOutKind
        {
            入室,退室,異常終了
        }
        private void OutputLog(AccessLog accessLog,InOutKind inOutKind)
        {
            var currentTime = DateTime.Now;
            if(currentDate.Day != currentTime.Day)
            {
                //path更新
                targetFilePath = $"Log_{ currentTime.ToShortDateString()}.csv";
            }
            File.AppendAllText(targetFilePath, $"{accessLog.studentId},{currentTime.ToShortTimeString()},{inOutKind}\n");

        }
        private void AllViewClear()
        {
            StudentIDText.Content = "";
            StatusText.Content = "";
        }
        private string ReadStudentID(Felica f)
        {
            string studentID = "";
            f.Polling(0x8277);
            byte[] data = f.ReadWithoutEncryption(0x010b, 0);
            if (data == null)
            {
                throw new Exception("学生証が読み取れません");
            }
            for (int i = 3; i < 10; i++)
            {
                studentID += (char)data[i];
            }
            return studentID;
        }
        private DispatcherTimer timer;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(1000000);//0.1秒
            timer.Tick += new EventHandler(OnTimer);
            timer.Start();
        }

        private void ShowInAccessLogListButton_Click(object sender, RoutedEventArgs e)
        {
            var accessLogListView = new AccessLogListView(accessLogs);
            accessLogListView.ShowDialog();
        }
    }
}
