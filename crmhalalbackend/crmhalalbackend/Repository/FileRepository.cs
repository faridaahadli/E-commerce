using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.File;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class FileRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<FileDto> ReturnInsertFileId(List<File> file, string userId)
        {
            List<FileDto> fileForId = new List<FileDto>();
            var json = JsonConvert.SerializeObject(file);
            int sessionId;
            try
            {
                using (var con = new DbHandler())
                {
                    sessionId = con.ExecStoredProcWithReturnIntValue("UploadFileInsert", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input,
                            json),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int,10, ParameterDirection.Input,
                            userId),

                    });
                    fileForId = GetFileBySessionId(sessionId);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UploadFileInsert...");
                Log.Error(ex);
                throw;
            }


            return fileForId;
        }

        public List<FileDto> GetFileBySessionId(int sessionId)
        {
            const string sql =
                @"SELECT
	                [UPLOAD_FILE_ID],
                    [ORIGINAL_FILE_NAME],
	                [PATH],
	                [FILENAME],
	                [EXTENSION]
                FROM
	            NEW_UPLOAD_FILE 
                WHERE
	                USESSION_ID = @groupId";

            List<FileDto> fileDtos = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@groupId", SqlDbType.Int, 10, ParameterDirection.Input, sessionId)
                    });
                    fileDtos = new List<FileDto>();
                    while (reader.Read())
                    {
                        FileDto fileDto = new FileDto()
                        {
                            Id = reader.GetInt("UPLOAD_FILE_ID"),
                            OriginalFileName = reader["ORIGINAL_FILE_NAME"].ToString(),
                            FilePath = reader["PATH"].ToString() + reader["FILENAME"].ToString() +
                                       reader["EXTENSION"].ToString()
                        };
                        fileDtos.Add(fileDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetFileBySessionId...");
                Log.Error(ex);
                throw;
            }

            return fileDtos;
        }

        public void DeleteFileById(List<int> ids,int userId)
        {
            string json = JsonConvert.SerializeObject(ids);
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("UploadFileDelete", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input,
                            json),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int,10, ParameterDirection.Input,
                            userId),

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteFileById...");
                Log.Error(ex);
                throw;
            }
        }

        public List<FileDto> GetFileByIds(List<int> ids)
        { 
            var sql = "select * from dbo.GetUploadFileIds(@ids)";
            var fileDtos = new List<FileDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(ids);
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@ids", SqlDbType.NVarChar, -1, ParameterDirection.Input, json)
                    });
                    fileDtos = new List<FileDto>();
                    while (reader.Read())
                    {
                        FileDto fileDto = new FileDto()
                        {
                            Id = reader.GetInt("UPLOAD_FILE_ID"),
                            FilePath = reader["PATH"].ToString() + reader["FILENAME"].ToString() +
                                       reader["EXTENSION"].ToString()
                        };
                        fileDtos.Add(fileDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetFileByIds...");
                Log.Error(ex);
                throw;
            }

            return null;
        }

    }
}