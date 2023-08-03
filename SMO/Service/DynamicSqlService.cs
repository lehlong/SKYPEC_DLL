using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SMO.Service
{
    public class T_AD_DATATABLE_NAME
    {
        public string Name { get; set; }
        public string TextSql { get; set; }
    }

    public class DynamicSqlService : BaseService
    {
        public List<T_AD_DATATABLE_NAME> ObjList { get; set; }
        public T_AD_DATATABLE_NAME ObjDetail { get; set; }
        public DataTable ObjResult { get; set; }
        public DataTableCollection ObjListResult { get; set; }

        public DynamicSqlService()
        {
            ObjList = new List<T_AD_DATATABLE_NAME>();
            ObjDetail = new T_AD_DATATABLE_NAME();
            ObjResult = new DataTable();
        }

        public void GetAllTableName()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString
            };

            string strDbName = builder.InitialCatalog;
            string strSql = @"
                                SELECT TABLE_NAME AS NAME
                                FROM INFORMATION_SCHEMA .TABLES
                                WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG= '{0}'
                            ";

            strSql = string.Format(strSql, strDbName);

            SqlCommand cm = new SqlCommand
            {
                Connection = new SqlConnection(builder.ConnectionString)
            };
            cm.Connection.Open();

            cm.CommandType = CommandType.Text;
            cm.CommandText = strSql;

            var result = new DataTable();
            SqlDataReader rc = cm.ExecuteReader();
            result.Load(rc);
            rc.Close();
            cm.Connection.Close();

            foreach (DataRow item in result.Rows)
            {
                var table = new T_AD_DATATABLE_NAME
                {
                    Name = item["NAME"].ToString()
                };
                ObjList.Add(table);
            }
        }

        public bool RunSql(string codeSql)
        {
            bool blnResult = true;
            string strSql = @"";
            string strcon = ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString;
            string strConnection = strcon;

            strSql += codeSql;

            try
            {
                using (SqlConnection cn = new SqlConnection(strConnection))
                {
                    cn.Open();

                    if (strSql.ToLower().Contains("select"))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(strSql, cn))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            ObjListResult = ds.Tables;
                        }
                    }
                    else
                    {
                        SqlCommand command = new SqlCommand(strSql)
                        {
                            Connection = cn
                        };
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                State = false;
                ErrorMessage = ex.ToString();
                Exception = ex;
            }

            return blnResult;
        }
    }
}