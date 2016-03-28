using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WorkTrack_Server
{
    class ShowData
    {
        public DataTable showinstalledapps(string remotepath,string pcname)
        {
            //string text = "0 2 0 3 0 0\r\n0 0 1 0 4 0\r\n0 0 0 0 0 5\r\n0 0 0 0 2 0\r\n0 0 0 0 0 1\r\n0 0 0 0 0 0";

            //string[] lines = text.Replace("\r", "").Split(new char[] { '\n' });

            //if (lines.Length == 0) return;

            //int RowCount = lines.Length;
            //int ColumnCount = lines[0].Split(new char[] { ' ' }).Length;

            //string[] massive = text.Replace("\r", "").Split(new char[] { '\n', ' ' });
            //FileStream fs = File.OpenRead(System.Environment.MachineName + "_InstalledApp.txt");
           // StreamReader Sr = new StreamReader(System.Environment.MachineName + "_InstalledApp.txt");
            string[] SourceText=null;
            SourceText = System.IO.File.ReadAllLines("Tempdata/" +remotepath+ pcname+"_InstalledApp.txt");
            int RowCount = SourceText.Length;
            List<string[]> ResMassive = new List<string[]>();
            foreach(string str in SourceText)
            {
                string[] Text = str.Split('/');
                ResMassive.Add(Text);
            }
            
            //int ColumnCount = 3;

            DataTable DTT = new DataTable();

            DTT.Columns.Add("Название");
            DTT.Columns.Add("Издатель");
            DTT.Columns.Add("Дата установки");

            for (int i = 0; i < RowCount; i++)
            {

                DataRow DR = DTT.NewRow();
                DTT.Rows.Add(DR);

                DR["Название"] = ResMassive[i][0];
                DR["Издатель"] = ResMassive[i][1];
                DR["Дата установки"] = ResMassive[i][2];
            }
            return DTT;
            //datagrid.DataContext = DTT;
        
        
        
        }

        public DataTable showusedapps(string remotepath,string pcname)
        {
            string[] SourceText=null;
            SourceText = System.IO.File.ReadAllLines("Tempdata/" + remotepath + pcname + "_ProcessTime.txt");
            int RowCount = SourceText.Length;
            List<string[]> ResMassive1 = new List<string[]>();
            foreach (string str in SourceText)
            {
                string[] Text = str.Split('/');
                ResMassive1.Add(Text);
            }

            //int ColumnCount = 3;

            DataTable DTT1 = new DataTable();

            DTT1.Columns.Add("Название");
            DTT1.Columns.Add("Время");

            for (int i = 0; i < RowCount; i++)
            {

                DataRow DR1 = DTT1.NewRow();
                DTT1.Rows.Add(DR1);

                DR1["Название"] = ResMassive1[i][0];
                DR1["Время"] = ResMassive1[i][1];
            }
            return DTT1;
            //datagrid.DataContext = DTT;



        }
        public string[] showworkedtime(string remotepath, string pcname)
        {
            string[] workedtime;
            workedtime = System.IO.File.ReadAllLines("Tempdata/" + remotepath + pcname + "_WorkedTime.txt");
            return workedtime;
        }
    }
}
