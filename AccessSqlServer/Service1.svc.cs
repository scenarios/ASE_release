using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace AccessSqlServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
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
        public DataSet querySql(out bool queryParam, string userid, string password, bool CHECK_USERID_ONLY)
        {
            try
            {
                /*if (CHECK_USERID_ONLY)
                {
                    sqlCon.Open();
                    string strSql = "select userid from info where userid='" + userid + "'";
                    DataSet ds = new DataSet();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                    sqlDa.Fill(ds);
                    queryParam = true;
                    return ds;
                }
                else
                {
                    sqlCon.Open();
                    string strSql = "select * from wordbook where userid='" + userid + "' and password='" + password + "'";
                    DataSet ds = new DataSet();
                    SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                    sqlDa.Fill(ds);
                    queryParam = true;
                    return ds;

                                sqlCon.Open();
                string strSql = "select userid,wordlist from wordbook where userid='" + userid + "'";
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDa = new SqlDataAdapter(strSql, sqlCon);
                sqlDa.Fill(ds);
                queryParam = true;
                return ds;
                }*/
                sqlCon.Open();
                string strSql = "select * from wordbook where userid='" + userid + "'";
                DataSet ds = new DataSet();
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

    }
}
