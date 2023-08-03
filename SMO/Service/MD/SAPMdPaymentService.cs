using SharpSapRfc;
using SharpSapRfc.Plain;

using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using SMO.Service.AD;

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class SAPMdPaymentService : GenericService<T_SAP_MD_PAYMENT, SAPMdPaymentRepo>
    {
        public SAPMdPaymentService() : base()
        {

        }

        public void Synchronize()
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var result = conn.ExecuteFunction(new SynMD_Payment_Function()).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_SAP_MD_PAYMENT";
                    SqlDataAdapter daAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, objConn);
                    daAdapter.UpdateBatchSize = 0;
                    DataSet dataSet = new DataSet(tableName);
                    daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                    daAdapter.Fill(dataSet, tableName);
                    DataTable dataTable;
                    dataTable = dataSet.Tables[tableName];

                    var actionBy = ProfileUtilities.User?.USER_NAME;
                    var actionDate = DateTime.Now;

                    //Insert, update các nhóm tài khoản trước
                    var lstNhomTaiKhoan = result.Select(x => new { x.GROUP_NAME, x.PARENT_CODE }).Distinct().ToList();
                    foreach (var item in lstNhomTaiKhoan)
                    {
                        var row = dataTable.Rows.Find(item.PARENT_CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["CODE"] = item.PARENT_CODE;
                            newRow["NAME"] = item.GROUP_NAME;
                            newRow["PARENT_CODE"] = "";
                            newRow["ACTIVE"] = "Y";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["NAME"].ToString() != item.GROUP_NAME)
                        {
                            row.BeginEdit();
                            row["NAME"] = item.GROUP_NAME;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    foreach (var item in result)
                    {
                        var row = dataTable.Rows.Find(item.CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["CODE"] = item.CODE;
                            newRow["NAME"] = item.NAME;
                            newRow["PARENT_CODE"] = item.PARENT_CODE;
                            newRow["ACTIVE"] = "Y";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["NAME"].ToString() != item.NAME || row["PARENT_CODE"].ToString() != item.PARENT_CODE)
                        {
                            row.BeginEdit();
                            row["NAME"] = item.NAME;
                            row["PARENT_CODE"] = item.PARENT_CODE;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                    daAdapter.Update(dataSet, tableName);
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

    }
}
