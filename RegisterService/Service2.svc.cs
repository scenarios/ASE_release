using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.IO;



namespace RegisterService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService2
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        /*
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        */
        SqlConnection sqlCon = new SqlConnection("Data Source=B9RG63X\\ASEDATABASE;Initial Catalog=LOGIN;uid=sa;pwd=ASEserver5");



        public DataSet querySql(out bool queryParam, string userid, string wordlist, bool IsInsert)
        {
            if (IsInsert)
            {
                string word = "";
                DataSet ds = new DataSet();
                for (int i = 0; i < wordlist.Length; i += 100)
                {
                    if (i + 100 >= wordlist.Length)
                    {
                        word = wordlist.Substring(i, wordlist.Length - i);
                    }
                    else
                    {
                        word = wordlist.Substring(i, 100);
                    }

                    try
                    {
                        //read register information


                        string strSql = "insert into wordbook(userid,wordlist) values ('" + userid + "',N'" + word + "')";
                        sqlCon.Open();
                        /*SqlCommand SqlCmd = new SqlCommand(strSql, sqlCon);
                        SqlCmd.ExecuteNonQuery();
                         DataSet ds = new DataSet();*/
                        SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                        sqlDa.Fill(ds);

                    }
                    catch
                    {
                        queryParam = false;
                        return null;
                    }
                    finally
                    {
                        sqlCon.Close();
                    }

                }
                queryParam = true;
                return ds;

            }
            else
            {
                try
                {
                    //read register information

                    sqlCon.Open();
                    string strSql = "delete from wordbook where userid='" + userid + "'";
                    DataSet ds = new DataSet();
                    /*SqlCommand SqlCmd = new SqlCommand(strSql, sqlCon);
                    SqlCmd.ExecuteNonQuery();
                     DataSet ds = new DataSet();*/
                    SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                    sqlDa.Fill(ds);
                    queryParam = true;
                    return ds;
                }
                catch
                {
                    queryParam = false;
                    return null;
                }
                finally
                {
                    sqlCon.Close();
                }

            }

            /* try
             {
                 //read register information

                 sqlCon.Open();
                 //string strSql = "drop table wordbook";
                  string strSql = "create table wordbook([userid][nvarchar](10) NOT NULL,[wordlist][nvarchar](max) NULL,)";
                 DataSet ds = new DataSet();
                 SqlCommand SqlCmd = new SqlCommand(strSql, sqlCon);
                 SqlCmd.ExecuteNonQuery();
                 queryParam = true;
                 return ds;
             }
             catch
             {
                 queryParam = false;
                 return null;
             }
             finally
             {
                 sqlCon.Close();
             }
             */

        }

    }
}
