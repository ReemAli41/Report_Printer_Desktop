using NT.Integration.SharedKernel.OracleManagedHelper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Report_Printer_Desktop.DAL
{
    public abstract class BaseDAL
    {
        protected DataTable GetDataTable(OracleManager oracleManager, string query,
                                       string refCursorName,
                                       CommandType commandType = CommandType.StoredProcedure)
        {
            if (!string.IsNullOrEmpty(refCursorName))
            {
                oracleManager.CommandParameters.Add(refCursorName, OracleDbType.RefCursor,
                    DBNull.Value, ParameterDirection.Output);
            }
            return oracleManager.GetDataTable(query, commandType);
        }


        protected async Task<int> ExecuteNonQuery(OracleManager oracleManager, string procedureName)
        {
            return await oracleManager.ExcuteNonQueryAsync(procedureName, CommandType.StoredProcedure);
        }
    }
}
