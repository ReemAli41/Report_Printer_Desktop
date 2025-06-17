using NT.Integration.SharedKernel.OracleManagedHelper;
using Oracle.ManagedDataAccess.Client;
using Report_Printer_Desktop.Entities;
using System.Data;


namespace Report_Printer_Desktop.DAL
{
    public class FileDownloadDAL : BaseDAL
    {
        public DataTable GetFilesToDownload(OracleManager oracleManager)
        {
            oracleManager.CommandParameters.Clear();
            oracleManager.CommandParameters.Add("file_cursor", OracleDbType.RefCursor,
                DBNull.Value, ParameterDirection.Output);
            return GetDataTable(oracleManager, "get_files_to_download", "file_cursor");
        }

        public void LogDownloadStatus(OracleManager oracleManager, FileDownloadEntity file,
                                    bool isSuccess, string errorMessage = null)
        {
            oracleManager.CommandParameters.Clear();
            oracleManager.CommandParameters.Add("p_file_name", file.FileName);
            oracleManager.CommandParameters.Add("p_is_success", isSuccess ? 1 : 0);
            oracleManager.CommandParameters.Add("p_error_message", errorMessage ?? string.Empty);

            ExecuteNonQuery(oracleManager, "log_file_download_status");
        }
    }
}
