using KRACHEL.Core.DTO;
using KRACHEL.Core.Enumerations;
using KRACHEL.Core.Model;
using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace KRACHEL.WPF.ViewModels
{
    internal class VideoEditorViewModel : ViewModels.CommonViewModel
    {
        private readonly IDialogService _dialogService;

        private readonly IVideoService _videoService;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelVideoParts))]
        public ObservableCollection<ViewModels.VideoPartViewModel> VideoParts { get; set; } = new ObservableCollection<ViewModels.VideoPartViewModel>();

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelVideoFile))]
        public string ResultVideoFilePath
        {
            get { return _resultVideoFilePath; }
            set
            {
                _resultVideoFilePath = value;
                NotifyPropertyChanged(nameof(ResultVideoFilePath));
                ValidateNullOrEmpty(this, nameof(ResultVideoFilePath), _resultVideoFilePath);
            }
        }
        private string _resultVideoFilePath;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelAudioFile))]
        public string SourceAudioFilePath
        {
            get { return _sourceAudioFilePath; }
            set
            {
                _sourceAudioFilePath = value;
                NotifyPropertyChanged(nameof(SourceAudioFilePath));
                ValidateFilePath(this,nameof(SourceAudioFilePath), _sourceAudioFilePath);
            }

        }
        private string _sourceAudioFilePath;

        public VideoEditorViewModel(IDialogService dialogService ,IVideoService videoService) 
        {
            _dialogService = dialogService;
            _videoService = videoService;

            VideoParts.CollectionChanged += this.OnCollectionChanged;
            this.Title = WPF.Resources.General.NameVideoEditor;
            SourceAudioFilePath = string.Empty;
            ResultVideoFilePath = string.Empty;
            VideoParts.Clear();
        }

        public void AddVideoPart(ViewModels.VideoPartViewModel videoPartViewModel)
        {
            if(videoPartViewModel != null)
            {
                var index = VideoParts.IndexOf(videoPartViewModel);

                if (index > -1)
                {
                    VideoParts.RemoveAt(index);
                    VideoParts.Insert(index, videoPartViewModel);
                }
                else
                {
                    VideoParts.Add(videoPartViewModel);
                }
            }     
        }

        public void RemoveVideoPart(ViewModels.VideoPartViewModel videoPartViewModel)
        {
            if(videoPartViewModel == null)
            {
                _dialogService.WarningDialog(WPF.Resources.General.WarningSelectionIsEmpty);
                return;
            }

            VideoParts.Remove(videoPartViewModel);
        }

        public void RemoveAllVideoParts()
        {
            VideoParts.Clear();
        }

        public void SortVideoParts()
        {
            for(int i = 0; i < VideoParts.Count; i++)
            {
                int currentIndex = VideoParts.IndexOf(VideoParts.Skip(i).Where(vp => vp.AtTime == VideoParts.Skip(i).Min(vp => vp.AtTime)).First());
                VideoParts.Move(currentIndex, i);
            }  
        }

        public async Task Process()
        {
            if(this.HasErrors)
            {
                _dialogService.WarningDialog(GetErrorsSummary());
                return;
            }

            try
            {
                await _videoService.CreateVideoFromPictureParts(
                    SourceAudioFilePath,
                    VideoParts
                        .Select(vp => new VideoPartDTO()
                        {
                            VideoPartType = (VideoPartType)vp.VideoPartType,
                            Text = vp.VideoPartText,
                            FilePath = vp.FilePath,
                            AtTime = vp.AtTime
                        }),
                    ResultVideoFilePath);
           
                _dialogService.InformationDialog(WPF.Resources.General.SuccessProcessingOK);
            } catch(Exception ex)
            {
                throw new Exception(WPF.Resources.General.ErrorProcessingFailed, ex);
            }            
        }

        public void SelectResultVideoFile()
        {
            ResultVideoFilePath = 
                _dialogService.SaveFileDialog(
                    _dialogService.DicToFileFilterString(
                        _videoService.GetSupportedVideoFormatList(), false));
        }
        public void SelectSourceAudioFile()
        {
            SourceAudioFilePath =
                _dialogService.OpenFileDialog(
                    _dialogService.DicToFileFilterString(
                        _videoService.GetSupportedAudioFormatList(), true));
        }

        public void AnalyzeSourceAudioFile()
        {
            if (GetErrors(nameof(SourceAudioFilePath)) != null)
            {
                _dialogService.WarningDialog(GetErrorsSummary(nameof(SourceAudioFilePath)));
                return;
            }

            try
            {
                var fileContent = _videoService.FileAnalyze(SourceAudioFilePath);
                _dialogService.InformationDialog(fileContent);
            }
            catch (Exception ex)
            {
                throw new Exception(WPF.Resources.General.ErrorProcessingFailed, ex);
            }
        }

        private void ValidateVideoParts()
        {
            ClearErrors(nameof(VideoParts));
            if(VideoParts.Count > 0) 
            {
                if (!VideoParts.Where(vp => vp.AtTime == new TimeSpan(0, 0, 0, 0, 0)).Any())
                {
                    AddError(this, nameof(VideoParts), WPF.Resources.General.VideoEditorFirstPartMissing);
                }
            } else
            {
                AddError(this, nameof(VideoParts), WPF.Resources.General.WarningSelectionIsEmpty);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ValidateVideoParts();
        }
    }
}
