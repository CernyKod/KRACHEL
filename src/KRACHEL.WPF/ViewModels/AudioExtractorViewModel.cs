using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KRACHEL.WPF.ViewModels
{
    /// <summary>
    /// ViewModel pro <see cref="Views.AudioExtractorWindow"/>
    /// </summary>
    internal class AudioExtractorViewModel : CommonViewModel
    {
        /// <summary>
        /// Dialogová služba
        /// </summary>
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Služba zpracování videa
        /// </summary>
        private readonly IVideoService _videoService;

        /// <summary>
        /// Zdrojový soubor - video
        /// </summary>
        public string SourceVideoFilePath
        {
            get { return _sourceVideoFilePath; }
            set
            {
                _sourceVideoFilePath = value;
                NotifyPropertyChanged(nameof(SourceVideoFilePath));
                ValidateFilePath(nameof(SourceVideoFilePath), _sourceVideoFilePath);
            }
        }
        private string _sourceVideoFilePath;

        /// <summary>
        /// Výsledný soubor extrahované audio stopy
        /// </summary>
        public string ResultAudioFilePath {
            get { return _resultAudioFilePath; }
            set
            {
                _resultAudioFilePath = value;
                NotifyPropertyChanged(nameof(ResultAudioFilePath));
                ValidateNullOrEmpty(nameof(ResultAudioFilePath), _resultAudioFilePath);
            }
        
        }     
        private string _resultAudioFilePath;

        public AudioExtractorViewModel(IDialogService dialogService, IVideoService videoService)
        {
            _dialogService = dialogService;
            _videoService = videoService;
            this.Title = Resources.General.NameAudioExtractor;

            SourceVideoFilePath = string.Empty;
            ResultAudioFilePath = string.Empty;
        }
        
        /// <summary>
        /// Vybrání zdrojového video souboru
        /// </summary>
        public void SourceVideoFilePathSelect()
        {
            this.SourceVideoFilePath = 
                _dialogService.OpenFileDialog(
                    _dialogService.DicToFileFilterString(
                        _videoService.GetSupportedVideoFormatList(), true));          
        }

        /// <summary>
        /// Výběr uložení výsledného audio souboru
        /// </summary>
        public void ResultAudioFilePathSelect()
        {
            this.ResultAudioFilePath = 
                _dialogService.SaveFileDialog(
                    _dialogService.DicToFileFilterString(
                        _videoService.GetSupportedAudioFormatList(), false));           
        }

        /// <summary>
        /// Akce extrahování audio stopy z video souboru
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Extract()
        {
            if(HasErrors)
            {
                _dialogService.WarningDialog(GetErrorsSummary());
                return;
            }
            
            try
            {
                await _videoService.ExtractAudioAsync(SourceVideoFilePath, ResultAudioFilePath);
                _dialogService.InformationDialog(Resources.General.SuccessProcessingOK);
            }
            catch (Exception ex)
            {
                throw new Exception(WPF.Resources.General.ErrorProcessingFailed, ex);
            }
        }
    }
}
