﻿using SMO.Service.AD;
using SMO.Service.Class;
using SMO.Service.CM;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;

namespace SMO.Service.Common
{
    public class FileStreamService
    {
        public static void DeleteFile(string lstFile)
        {
            var systemConfigService = new SystemConfigService();
            systemConfigService.GetConfig();

            if (systemConfigService.ObjDetail.Connection == null || string.IsNullOrWhiteSpace(systemConfigService.ObjDetail.Connection.DIRECTORY))
            {
                throw new Exception("Chưa cấu hình đầy đủ thông tin lưu trữ file!");
            }

            if (string.IsNullOrWhiteSpace(lstFile))
            {
                return;
            }

            //TODO: Xóa file vào thư mục cache

            //Xóa file ở database file
            var strConnection = $"Server={systemConfigService.ObjDetail.Connection.ADDRESS};Database={systemConfigService.ObjDetail.CURRENT_DATABASE_NAME};user id={systemConfigService.ObjDetail.Connection.USERNAME};password={systemConfigService.ObjDetail.Connection.PASSWORD};";
            try
            {


                var strInsert = @"DELETE TB_DATA_FILE WHERE PKID = @PKID";

                using (TransactionScope ts = new TransactionScope())
                {
                    using (SqlConnection conn = new SqlConnection(strConnection))
                    {

                        conn.Open();
                        foreach (var file in lstFile.Split('|'))
                        {
                            using (SqlCommand cmd = new SqlCommand(strInsert, conn))
                            {
                                cmd.CommandTimeout = 1800;
                                cmd.Parameters.Add("@PKID", SqlDbType.NVarChar).Value = file;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Xóa file ở database file không thành công!", ex);
            }

            var fileUploadService = new FileUploadService();
            fileUploadService.Delete(lstFile);

            if (fileUploadService.State == false)
            {
                throw fileUploadService.Exception;
            }
        }

        public static void InsertFile(FILE_STREAM file)
        {
            InsertFile(new List<FILE_STREAM> { file });
        }

        public static void InsertFile(List<FILE_STREAM> lstFile)
        {
            foreach (var file in lstFile)
            {
                file.FILE_OLD_NAME = file.FILESTREAM.FileName;
                file.FILE_NAME = file.PKID + Path.GetExtension(file.FILESTREAM.FileName);
                file.FILE_EXT = Path.GetExtension(file.FILESTREAM.FileName);
                file.FILE_SIZE = file.FILESTREAM.ContentLength;
            }

            var systemConfigService = new SystemConfigService();
            systemConfigService.GetConfig();

            if (systemConfigService.ObjDetail.Connection == null || string.IsNullOrWhiteSpace(systemConfigService.ObjDetail.Connection.DIRECTORY))
            {
                throw new Exception("Chưa cấu hình đầy đủ thông tin lưu trữ file!");
            }


            if (lstFile == null || lstFile.Count == 0)
            {
                return;
            }

            var path = CreatePath(systemConfigService.ObjDetail.Connection.DIRECTORY);

            //Lưu file vào thư mục 
            if (!string.IsNullOrWhiteSpace(path))
            {
                var now = DateTime.Now;
                var pathLogic = $"\\{now.Year}\\{now.Month}\\{now.Day}\\";
                try
                {
                    foreach (var file in lstFile)
                    {
                        file.DIRECTORY_PATH = pathLogic;
                        file.FULL_PATH = Path.Combine(path, file.FILE_NAME);
                        file.FILESTREAM.SaveAs(file.FULL_PATH);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lưu file vào thư mục không thành công", ex);
                }
            }

            var fileUploadService = new FileUploadService();
            fileUploadService.Create(lstFile);

            if (fileUploadService.State == false)
            {
                throw fileUploadService.Exception;
            }
        }

        public static string CreatePath(string parentPath)
        {
            string path;
            try
            {
                var now = DateTime.Now;
                parentPath = "D:\\FileAttach";
                path = Path.Combine(parentPath, now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return path;
        }
    }

}