using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Report_Printer_Desktop.ViewModels
{
    public class ResultListViewModel : MainViewModel
    {
        // Singleton instance
        public static ResultListViewModel Instance { get; } = new ResultListViewModel();

        // Collection bound to the ListView (always has at most 1 item)
        public ObservableCollection<FileResultItem> Results { get; private set; }

        //private readonly ResultDSL resultDSL;

        public ResultListViewModel()
        {
            Results = new ObservableCollection<FileResultItem>();

            //resultDSL = ResultDSL.Instance;

            // Subscribe to DSL event when ready
            //resultDSL.OnEvent_FillPendingResultListInUI += (resultList) => FillFileNameList(resultList);
        }

        /// <summary>
        /// Clear the file name list (asks for confirmation)
        /// </summary>
        public void ClearResultsList()
        {
            var confirm = MessageBox.Show(
                "Are you sure you want to clear the current file name?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm == MessageBoxResult.Yes)
            {
                Results.Clear();
            }
        }

        /// <summary>
        /// Replace the current file name with the newest one from the DB.
        /// This always keeps exactly one file in the list.
        /// </summary>
        //private void FillFileNameList(List<UploadResultDTO> resultList)
        //{
        //    if (resultList == null || !resultList.Any()) return;

        //    // Always use the first valid file only
        //    var dto = resultList.FirstOrDefault(x => x?.Report != null);
        //    if (dto == null) return;

        //    var fileName = dto.Report?.Name ?? string.Empty;

        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        Results.Clear();
        //        Results.Add(new FileResultItem { FileName = fileName });
        //    });
        //}

        /// <summary>
        /// Simple item with only FileName for binding.
        /// </summary>
        public class FileResultItem
        {
            public string FileName { get; set; }
        }
    }
}
