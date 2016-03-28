using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.ComponentModel;
using System.Windows.Threading;

namespace WorkTrack_Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        ShowData sd = new ShowData();
        FTP ftp = new FTP();
        string selectedDate,selectedPC,remotepath,path;
        List<string> pclist;
        BackgroundWorker bgw = new BackgroundWorker();
        BackgroundWorker getpcdate = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            createFolder();
            bgw.DoWork += worker_onSelectedDateChange;
            bgw.RunWorkerCompleted += worker_SelectedDateCompleted;
            getpcdate.DoWork += worker_onSelectedPCchange;
            getpcdate.RunWorkerCompleted += worker_SelectedPCCompleted;
            progbar.Visibility = Visibility.Hidden;
            lbprogress.Visibility = Visibility.Hidden;
 
        }

        public void createFolder()
        {
            string temppath = "Tempdata";

            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(temppath))
                {
                    return;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(temppath);      
                }              
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            } 
        
        
        }
        public static void CleanFilesAndDirectories(System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        public void worker_onSelectedDateChange(object sender, DoWorkEventArgs e)
        {
           pclist = ftp.getlist(selectedDate);          
        }
        private void worker_SelectedDateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            for (int i = 2; i < pclist.Count; i++)
            {
                pclistcombobox.Items.Add(pclist[i]);
            }
            progbar.Visibility = Visibility.Hidden;
            lbprogress.Content = "Данные загружены";
        }
        private void datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDate = datepicker.SelectedDate.Value.Day.ToString() + "." + datepicker.SelectedDate.Value.Month.ToString() + "." + datepicker.SelectedDate.Value.Year.ToString();
            pclistcombobox.Items.Clear();
            lbprogress.Content = "Загружаем список компьютеров";
            progbar.Visibility = Visibility.Visible;
            lbprogress.Visibility = Visibility.Visible;

            bgw.RunWorkerAsync();
        }



        public void worker_onSelectedPCchange(object sender, DoWorkEventArgs e)
        {            
            string[] listfile = (ftp.GetFileList(selectedDate + "/" + selectedPC));
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    for (int i = 2; i < listfile.Length; i++)
                    {
                        ftp.Download(remotepath, listfile[i]);
                    }
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    for (int i = 2; i < listfile.Length; i++)
                    {
                        ftp.Download(remotepath, listfile[i]);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
        private void worker_SelectedPCCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
            installappDatagrid.DataContext = sd.showinstalledapps(remotepath, selectedPC);
            appsrunDatagrid.DataContext = sd.showusedapps(remotepath, selectedPC);
            string[] temp = sd.showworkedtime(remotepath, selectedPC);
            lbworkedtime.Content = temp[0];
            progbar.Visibility = Visibility.Hidden;
            lbprogress.Content = "Данные о ПК загружены";
        }
        private void pclistcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPC = pclistcombobox.SelectedItem.ToString();
            remotepath = selectedDate + "/" + selectedPC + "/";
            path = "Tempdata/" + remotepath;
            lbprogress.Content = "Загружаем данных о ПК";
            progbar.Visibility = Visibility.Visible;
            lbprogress.Visibility = Visibility.Visible;
            getpcdate.RunWorkerAsync();
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            CleanFilesAndDirectories(new System.IO.DirectoryInfo("Tempdata"));
        }


    }
}
