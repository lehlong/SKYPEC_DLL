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
    public class SAPActualDataService : GenericService<T_SAP_ACTUAL_DATA, SAPActualDataRepo>
    {
        public SAPActualDataService() : base()
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
                    var functionSAP = new Get_ActualData_Function();
                    functionSAP.Parameters = new
                    {
                        I_AEDAT = systemConfig.ObjDetail.LAST_UPDATE_PR.HasValue ? systemConfig.ObjDetail.LAST_UPDATE_PR.Value.AddDays(-1) : new DateTime(2018, 01, 01)
                    };

                    var result = conn.ExecuteFunction(functionSAP).ToList();

                    using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString))
                    {

                        objConn.Open();
                        SqlTransaction transction = objConn.BeginTransaction();

                        string tableName = "T_SAP_ACTUAL_DATA";
                        SqlCommand cmd = new SqlCommand($"SELECT * FROM {tableName} WHERE 1 = 0", objConn);
                        cmd.Transaction = transction;
                        SqlDataAdapter daAdapter = new SqlDataAdapter(cmd);
                        daAdapter.UpdateBatchSize = 1000;
                        DataSet dataSet = new DataSet(tableName);
                        daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                        daAdapter.Fill(dataSet, tableName);
                        DataTable dataTable;
                        dataTable = dataSet.Tables[tableName];

                        var actionBy = ProfileUtilities.User?.USER_NAME;
                        var actionDate = DateTime.Now;

                        //Xóa các bản ghi có trường update != null
                        var strDelete = string.Empty;

                        if (systemConfig.ObjDetail.LAST_UPDATE_PR.HasValue)
                        {
                            foreach (var item in result)
                            {
                                strDelete += $"DELETE T_SAP_ACTUAL_DATA WHERE COMPANY_CODE = '{item.COMPANY_CODE}' AND DOCUMENT_NUMBER = '{item.DOCUMENT_NUMBER}' AND FISCAL_YEAR = {item.FISCAL_YEAR} AND LINE_NUMBER = '{item.LINE_NUMBER}';";
                            }
                        }

                        // Insert các bản ghi vào
                        foreach (var item in result)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["LINE_NUMBER"] = item.LINE_NUMBER;
                            newRow["COMPANY_CODE"] = item.COMPANY_CODE;
                            newRow["DOCUMENT_NUMBER"] = item.DOCUMENT_NUMBER;
                            newRow["FISCAL_YEAR"] = item.FISCAL_YEAR;
                            if (item.POSTING_DATE.HasValue)
                            {
                                newRow["POSTING_DATE"] = item.POSTING_DATE.Value;
                            }
                            newRow["POSTING_KEY"] = item.POSTING_KEY;
                            newRow["DEBIT_CREDIT"] = item.DEBIT_CREDIT;
                            newRow["TAX_CODE"] = item.TAX_CODE;
                            newRow["GL_ACCOUNT"] = item.GL_ACCOUNT;
                            newRow["GL_AMOUNT"] = item.GL_AMOUNT;
                            newRow["CURRENCY"] = item.CURRENCY;
                            newRow["TAX_AMOUNT"] = item.TAX_AMOUNT;
                            newRow["COST_CENTER"] = item.COST_CENTER;
                            newRow["C_ORDER"] = item.C_ORDER;
                            newRow["PROFIT_CENTER"] = item.PROFIT_CENTER;
                            newRow["FUND_CODE"] = item.FUND_CODE;
                            newRow["PAYMENT_CODE"] = item.PAYMENT_CODE;
                            newRow["EXPENSE_CODE"] = item.EXPENSE_CODE;
                            if (item.CREATE_DATE_SAP.HasValue)
                            {
                                newRow["CREATE_DATE_SAP"] = item.CREATE_DATE_SAP.Value;
                            }
                            if (item.UPDATE_DATE_SAP.HasValue)
                            {
                                newRow["UPDATE_DATE_SAP"] = item.UPDATE_DATE_SAP.Value;
                            }
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }

                        try
                        {
                            if (!string.IsNullOrEmpty(strDelete))
                            {
                                SqlCommand sqlCommand = new SqlCommand(strDelete, objConn, transction);
                                sqlCommand.ExecuteNonQuery();
                            }
                            SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                            daAdapter.InsertCommand = objCommandBuilder.GetInsertCommand();
                            daAdapter.InsertCommand.CommandTimeout = 1800;
                            daAdapter.InsertCommand.Transaction = transction;
                            daAdapter.Update(dataSet, tableName);
                            transction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transction.Rollback();
                            throw ex;
                        }
                    }
                }

                systemConfig.ObjDetail.LAST_UPDATE_PR = DateTime.Now.Date;
                systemConfig.Update();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

    }
}
