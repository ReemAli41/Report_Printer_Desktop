using NT.Integration.SharedKernel.OracleManagedHelper;
using Oracle.ManagedDataAccess.Client;
using Report_Printer_Desktop.Helpers;
using System;

namespace Report_Printer_Desktop.DSL
{

    public abstract class BaseDbDSL
    {

        protected static T Execute<T>(Func<OracleManager, T> processDelegate, Helper_Logger logger)
        {
            using (var oracleManager = new OracleManager(GetConnectionString()))
            {
                try
                {
                    oracleManager.OpenConnectionAsync();

                    return processDelegate(oracleManager);
                }
                catch (OracleException ex)
                {
                    logger.LogException(ex, "OracleException");
                    throw;
                }
                catch (Exception ex)
                {
                    // Log exceptions
                    logger.LogException(ex);
                    throw;
                }
                finally
                {
                    if (oracleManager.IsLiveConnection())
                    {
                        oracleManager.CloseConnection();
                    }
                }
            }
        }

        private static string GetConnectionString()
        {
            return "User Id=_user;Password=password;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=_host)(PORT=1234))(CONNECT_DATA=(SERVICE_NAME=_service)))";
        }
    }
}