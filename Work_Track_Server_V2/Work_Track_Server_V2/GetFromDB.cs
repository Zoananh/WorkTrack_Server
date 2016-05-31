using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work_Track_Server_V2
{
    class GetFromDB
    {
        string conn = ConfigurationManager.ConnectionStrings["Work_Track"].ConnectionString;
        public DataTable GetUsedApp(int pcID,string Useddate)
        {
            List<string[]> usedappList = new List<string[]>();
            SqlConnection sqlcon = new SqlConnection(conn);
            //Open the connection
            sqlcon.Open();

            using (SqlCommand sqlcmd = new SqlCommand("SELECT Applications.App_names,Used_app.Used_time,Used_app.Used_date FROM Applications,Used_app WHERE Used_app.PC_ID=@PC_id AND Applications.App_ID = Used_app.App_ID AND Used_date=@Used_date", sqlcon))
            {
                sqlcmd.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlcmd.Parameters["@PC_id"].Value = pcID;
                sqlcmd.Parameters.Add(new SqlParameter("@Used_date", SqlDbType.Date));
                sqlcmd.Parameters["@Used_date"].Value = Useddate;
                object[] obj = new object[2];
                using (var reader = sqlcmd.ExecuteReader())
                {
                    usedappList.Clear();
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        string[] tempstring = new string[2];
                        tempstring[0] = obj[0].ToString();
                        tempstring[1] = obj[1].ToString();
                        usedappList.Add(tempstring);
                    }
                }
            }
            sqlcon.Close();

            DataTable DTT = new DataTable();

            DTT.Columns.Add("Название");
            DTT.Columns.Add("Время работы");

            for (int i = 0; i < usedappList.Count; i++)
            {

                DataRow DR = DTT.NewRow();
                DTT.Rows.Add(DR);
                DR["Название"] = usedappList[i][0];
                DR["Время работы"] = usedappList[i][1];

            }
            return DTT;

        }
        public DataTable GetInstalledApp(int pcID)
        {
            List<string[]> InstalledAppList = new List<string[]>();
            SqlConnection sqlcon = new SqlConnection(conn);
            //Open the connection
            sqlcon.Open();

            using (SqlCommand sqlcmd = new SqlCommand("SELECT Applications.App_names, Applications.App_Publisher,Installedapp.Installed_date FROM Applications,Installedapp WHERE Installedapp.PC_ID=@PC_id AND Installedapp.App_ID=Applications.App_ID", sqlcon))
            {
                sqlcmd.Parameters.Add(new SqlParameter("@PC_id", SqlDbType.Int));
                sqlcmd.Parameters["@PC_id"].Value = pcID;
                object[] obj = new object[3];
                using (var reader = sqlcmd.ExecuteReader())
                {
                    InstalledAppList.Clear();
                    while (reader.Read())
                    {
                        reader.GetValues(obj);
                        string[] tempstring = new string[3];
                        tempstring[0] = obj[0].ToString();
                        tempstring[1] = obj[1].ToString();
                        tempstring[2] = obj[2].ToString();
                        InstalledAppList.Add(tempstring);
                    }
                }
            }
            sqlcon.Close();

            DataTable DTT = new DataTable();

            DTT.Columns.Add("Название");
            DTT.Columns.Add("Издатель");
            DTT.Columns.Add("Дата установки");

            for (int i = 0; i < InstalledAppList.Count; i++)
            {

                DataRow DR = DTT.NewRow();
                DTT.Rows.Add(DR);
                DR["Название"] = InstalledAppList[i][0];
                DR["Издатель"] = InstalledAppList[i][1];
                DR["Дата установки"] = InstalledAppList[i][2] = DateTime.Parse(InstalledAppList[i][2]).ToShortDateString();
            }
            return DTT;



        }
    }
}
