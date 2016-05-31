using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
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
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;

namespace Work_Track_Server_V2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
        string SelectedDate,SelectedPC;
        int pc_ID;
        DateTime startdate;
        List<string> pclist = new List<string>();
        BackgroundWorker bgw = new BackgroundWorker();
        BackgroundWorker getpcdate = new BackgroundWorker();
        GetFromDB getbd = new GetFromDB();
        public MainWindow()
        {
            InitializeComponent();
            SqlConnection sqlconnection = new SqlConnection(conn);
            //Open the connection
            sqlconnection.Open();
            using (SqlCommand sqlcmd = new SqlCommand("SELECT First_date FROM FirstTime", sqlconnection))
            {
                startdate = DateTime.Parse(sqlcmd.ExecuteScalar().ToString());
            }
            sqlconnection.Close();
            Progbar.Visibility = Visibility.Hidden;
            StatusLb.Visibility = Visibility.Hidden;
            bgw.DoWork += worker_onSelectedDateChange;
            bgw.RunWorkerCompleted += worker_SelectedDateCompleted;
            getpcdate.DoWork += worker_onSelectedPCchange;
            getpcdate.RunWorkerCompleted += worker_SelectedPCCompleted;
        }
        public void worker_onSelectedPCchange(object sender, DoWorkEventArgs e)
        {
            SqlConnection sqlconnection3 = new SqlConnection(conn);
            //Open the connection
            sqlconnection3.Open();

            using (var sqlcmd = new SqlCommand("SELECT PC_ID FROM PC WHERE PC_name=@pcname", sqlconnection3))
            {
                sqlcmd.Parameters.Add("@pcname", SqlDbType.VarChar);

                sqlcmd.Parameters["@pcname"].Value = SelectedPC;
                pc_ID = (int)sqlcmd.ExecuteScalar();

            }
          
            sqlconnection3.Close();

        }
        private void worker_SelectedPCCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGrid1.DataContext = getbd.GetUsedApp(pc_ID, SelectedDate);
            dataGrid2.DataContext = getbd.GetInstalledApp(pc_ID);
            Progbar.Visibility = Visibility.Hidden;
            StatusLb.Visibility = Visibility.Hidden;
            StatusLb.Content = "";
        }
        public void worker_onSelectedDateChange(object sender, DoWorkEventArgs e)
        {
            
            //===========================================================================================================================
            SqlConnection sqlconnection2 = new SqlConnection(conn);
            //Open the connection
            sqlconnection2.Open();
            pclist.Clear();
            using (SqlCommand sqlcmd = new SqlCommand("SELECT PC.PC_name FROM PC,Worked_time WHERE Worked_time.Worked_Date=@Worked_date AND PC.PC_ID=Worked_time.PC_ID", sqlconnection2))
            {
                sqlcmd.Parameters.Add(new SqlParameter("@Worked_date", SqlDbType.Date));
                sqlcmd.Parameters["@Worked_date"].Value = SelectedDate;
                object[] obj = new object[1];
                using (var reader = sqlcmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        pclist.Add(obj[0].ToString());
                    }
                }
            }
            sqlconnection2.Close();

        }
        private void worker_SelectedDateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string str in pclist)
            {
                pclistcmbox.Items.Add(str);

            }
            Progbar.Visibility = Visibility.Hidden;
            StatusLb.Visibility = Visibility.Hidden;
            StatusLb.Content = "";

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            datepicker1.DisplayDateStart = startdate;
            datepicker1.DisplayDateEnd = DateTime.Now;

        }

        private void datepicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Progbar.Visibility = Visibility.Visible;
            StatusLb.Visibility = Visibility.Visible;
            StatusLb.Content = "Receiving data";
            SelectedDate = datepicker1.SelectedDate.Value.ToShortDateString();
            bgw.RunWorkerAsync();
        }

        private void pclistcmbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Progbar.Visibility = Visibility.Visible;
            StatusLb.Visibility = Visibility.Visible;
            StatusLb.Content = "Receiving data";
            SelectedPC = pclistcmbox.SelectedItem.ToString();
            getpcdate.RunWorkerAsync();
        }
    }
}
