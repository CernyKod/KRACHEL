using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace KRACHEL.WPF.ViewModels
{
    internal class VideoPartViewModel : CommonViewModel
    {
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyPropertyChanged(nameof(FilePath));      
                ValidateFilePath(nameof(FilePath), _filePath);
            }
        } 
        private string _filePath;

        public string Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                ValidateTimeSpanPart(nameof(Hour), _hour);
                UpdateInTime();
            }
        }
        private string _hour;

        public string Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                ValidateTimeSpanPart(nameof(Minute), _minute);
                UpdateInTime();
            }
        }
        private string _minute;

        public string Second
        {
            get { return _second; }
            set
            {
                _second = value;
                ValidateTimeSpanPart(nameof(Second), _second);
                UpdateInTime();
            }
        }
        private string _second;

        public string Millisecond
        {
            get { return _millisecond; }
            set
            {
                _millisecond = value;
                ValidateTimeSpanPart(nameof(Millisecond), _millisecond);
                UpdateInTime();
            }
        }
        private string _millisecond;

        public TimeSpan InTime { get; private set; }

        private readonly IDialogService _deialogService;

        private readonly IVideoService _videoService;

        public VideoPartViewModel(IDialogService dialogService, IVideoService videoService) 
        {
            _deialogService = dialogService;
            _videoService = videoService;

            this.Title = WPF.Resources.General.NameVideoPart;
            FilePath = string.Empty;
            Hour = 0.ToString();
            Minute = 0.ToString();
            Second = 0.ToString();
            Millisecond = 0.ToString();
        }

        public void FileSelect()
        {
            FilePath = _deialogService.OpenFileDialog(
                _deialogService.DicToFileFilterString(
                    _videoService.GetSupportedImageFormatList(), true));
        }

        public void SavePart(ViewModels.VideoEditorViewModel editorViewModel)
        {
            if(HasErrors)
            {
                _deialogService.WarningDialog(GetErrorsSummary());
                return;
            }
            
            editorViewModel.AddVideoPart(this);
        }

        public override string ToString()
        {
            return $"{WPF.Resources.General.PartAtTime} {Hour}:{Minute}:{Second}:{Millisecond} {WPF.Resources.General.PartFile} {System.IO.Path.GetFileName(FilePath)}";
        }

        private void ValidateTimeSpanPart(string propertyName, string value)
        {
            ClearErrors(propertyName);
            uint result;
            if(!uint.TryParse(value, out result))
            {
                AddError(propertyName, WPF.Resources.General.WarningValueIsNotNumber);
            }
        }

        private void UpdateInTime()
        {
            InTime = new TimeSpan(0, Convert.ToInt32(Hour), Convert.ToInt32(Minute), Convert.ToInt32(Second), Convert.ToInt32(Millisecond));
        }
    }
}
