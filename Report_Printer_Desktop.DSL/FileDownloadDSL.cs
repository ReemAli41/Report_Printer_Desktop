using Report_Printer_Desktop.DAL;
using Report_Printer_Desktop.Entities;
using Report_Printer_Desktop.Helpers;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Report_Printer_Desktop.DSL
{

    public class FileDownloadDSL : BaseDbDSL
    {
        private readonly FileDownloadDAL _fileDownloadDAL;
        private readonly Helper_Logger _logger;
        private readonly HttpClient _httpClient;

        public FileDownloadDSL(Helper_Logger logger)
        {
            _fileDownloadDAL = new FileDownloadDAL();
            _logger = logger;
            _httpClient = new HttpClient();
        }

      
        public FileDownloadEntity[] GetFilesToDownload()
        {
            return Execute(oracleManager =>
            {
                // Get files from database
                var dataTable = _fileDownloadDAL.GetFilesToDownload(oracleManager);

                return dataTable.AsEnumerable().Select(row => new FileDownloadEntity
                {
                    FileName = row["file_name"].ToString(),
                    FileLink = row["file_link"].ToString(),
                    SaveLocation = row["save_location"].ToString()
                }).ToArray();
            }, _logger);
        }

        public void LogDownloadStatus(FileDownloadEntity file, bool isSuccess, string errorMessage = null)
        {
            Execute(oracleManager =>
            {
                _fileDownloadDAL.LogDownloadStatus(oracleManager, file, isSuccess, errorMessage);
                return true;
            }, _logger);
        }


        public bool DownloadFileWithRetry(FileDownloadEntity file, int maxRetries = 3)
        {
            int retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    // Ensure directory exists
                    var directory = Path.GetDirectoryName(file.SaveLocation);
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    // Download file synchronously
                    using (var responseStream = _httpClient.GetStreamAsync(file.FileLink).Result)
                    using (var fileStream = new FileStream(file.SaveLocation, FileMode.Create))
                    {
                        responseStream.CopyTo(fileStream);
                    }

                    LogDownloadStatus(file, true);
                    return true;
                }
                catch (Exception ex)
                {
                    retryCount++;

                    // If max retries reached, log failure
                    if (retryCount >= maxRetries)
                    {
                        LogDownloadStatus(file, false, $"Failed after {maxRetries} attempts: {ex.Message}");
                        return false;
                    }

                    // Wait before retrying
                    Thread.Sleep(1000 * retryCount);
                }
            }
            return false;
        }

      
        public void ProcessDownloads()
        {
            try
            {
                var files = GetFilesToDownload();

                // Process each file
                foreach (var file in files)
                {
                    DownloadFileWithRetry(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "Error in ProcessDownloads");
            }
        }

        public Helper_Logger GetLogger() => _logger;
    }
}