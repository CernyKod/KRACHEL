using System.Collections.Generic;

namespace KRACHEL.WPF.Services
{
    /// <summary>
    /// UI system dialogy
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Zpráva typu chyba
        /// </summary>
        /// <param name="message"></param>
        void ErrorDialog(string message);
        
        /// <summary>
        /// Zpráva typu informace
        /// </summary>
        /// <param name="message"></param>
        void InformationDialog(string message);
        
        /// <summary>
        /// Zpráva typu varování
        /// </summary>
        /// <param name="message"></param>
        void WarningDialog(string message);
        
        /// <summary>
        /// Dialoga - otevřít soubor
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string OpenFileDialog(string filter);
        
        /// <summary>
        /// Dialog uložit soubor
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string SaveFileDialog(string filter);
        
        /// <summary>
        /// Konverze slovníku [Přípona souboru; Název formátu] na filtrační kriterium pro <see cref="OpenFileDialog(string)"/> <see cref="SaveFileDialog(string)"/>
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="addAllSupportedFormatOption"></param>
        /// <returns></returns>
        string DicToFileFilterString(IDictionary<string, string> dic, bool addAllSupportedFormatOption);
    }
}