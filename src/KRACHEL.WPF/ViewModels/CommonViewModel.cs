using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.WPF.ViewModels
{
    /// <summary>
    /// Obecný ViewModel pro MVVM pattern aplikace
    /// Podporuje rozhraní pro validaci dat a notifikaci o změnách dat
    /// </summary>
    /// <remarks>
    /// Implementace INotifyDataErrorInfo dle https://kmatyaszek.github.io/wpf%20validation/2019/03/13/wpf-validation-using-inotifydataerrorinfo.html
    /// </remarks>
    internal abstract class CommonViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Název ViewModelu
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Model obsahuje chybná/nevalidní data
        /// </summary>
        public bool HasErrors => _errorsByPropertyName.Any();

        /// <summary>
        /// Událost změna dat
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        
        /// <summary>
        /// Událost chybná data
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Seznam chyb pro dané atributy
        /// </summary>
        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        /// <summary>
        /// Výbys chyb dat pro daný atribut
        /// Využívá se i pro dotaz, zda pro daný atribut existují chyby
        /// Jedná se o implementaci <see cref="INotifyDataErrorInfo"/>
        /// Pro textový výpis chyb není příliš praktické, zda je možné využít metodu <see cref="GetErrorsSummary(string)"/>
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
        }

        /// <summary>
        /// Textový souhrn chyb pro celý model případně konkrétní atribut
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetErrorsSummary(string propertyName = null)
        {
            var summaryBuilder = new StringBuilder();
            foreach(var property in _errorsByPropertyName.Where(p => propertyName == null || p.Key == propertyName))
            {
                property.Value.ForEach(err => summaryBuilder.AppendLine($"{property.Key} - {err}"));
            }

            return summaryBuilder.ToString();
        }

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Uložení chyby dat do slovníku
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="error"></param>
        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Vyčištění chyb pro daný atribut ve slovníku
        /// Volá se vždy při revalidaci dat. Metody Validate...()
        /// </summary>
        /// <param name="propertyName"></param>
        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Obecná validace platnosti zadané cesty k souboru
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="filePath"></param>
        protected void ValidateFilePath(string propertyName, string filePath)
        {
            ClearErrors(propertyName);
            if(!System.IO.File.Exists(filePath))
            {
                AddError(propertyName, WPF.Resources.General.WarningFilePathIsNotValid);
            }            
        }

        /// <summary>
        /// Obecná validace obsahu textového řetězce
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void ValidateNullOrEmpty(string propertyName, string value)
        {
            ClearErrors(propertyName);
            if (string.IsNullOrEmpty(value))
            {
                AddError(propertyName, WPF.Resources.General.WarningValueIsNotValid);
            }
        }
    }
}
