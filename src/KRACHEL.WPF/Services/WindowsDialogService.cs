using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KRACHEL.WPF.Services
{
    /// <summary>
    /// Implementace <see cref="IDialogService"/> pro prostředí MS Windows
    /// </summary>
    public class WindowsDialogService : IDialogService
    {
        public void InformationDialog(string message)
        {
            MessageBox.Show(message, Resources.General.NameInformation, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void WarningDialog(string message)
        {
            MessageBox.Show(message, Resources.General.NameWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public void ErrorDialog(string message)
        {
            MessageBox.Show(message, Resources.General.NameError, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public string OpenFileDialog(string filter)
        {
            string openFilePath = null;
            var openFileDialog = new OpenFileDialog()
            {
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
                openFilePath = openFileDialog.FileName;

            return openFilePath;
        }

        public string SaveFileDialog(string filter)
        {
            string saveFilePath = null;
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = filter
            };

            if (saveFileDialog.ShowDialog() == true)
                saveFilePath = saveFileDialog.FileName;

            return saveFilePath;
        }

        public string DicToFileFilterString(IDictionary<string, string> dic, bool addAllSupportedFormatOption)
        {
            var fileFilterString = string.Empty;
            var groupedFilterString = string.Empty;

            if (dic != null && dic.Count > 0)
            {
                foreach (var kp in dic)
                {
                    fileFilterString += $"{kp.Value} (*.{kp.Key})|*.{kp.Key}|";
                    groupedFilterString += $"*.{kp.Key};";
                }
                fileFilterString = fileFilterString.Remove(fileFilterString.Length - 1);
                groupedFilterString = groupedFilterString.Remove(groupedFilterString.Length - 1);

                if (addAllSupportedFormatOption)
                {
                    fileFilterString = fileFilterString
                    .Insert(0, $"{Resources.General.PartAllSupportedFormats} ({groupedFilterString})|{groupedFilterString}|");
                }
            }

            return fileFilterString;
        }
    }
}
