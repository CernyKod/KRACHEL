using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace KRACHEL.WPF.ViewModels
{
    internal class VideoPartViewModel : CommonViewModel
    {
        public List<ComboItem> VideoPartTypes { get; } = new List<ComboItem>() 
        {
            new ComboItem() { ID = 0, Value = "Vyberte" },
            new ComboItem() { ID = 1, Value = Resources.General.SelectItemVideoPartTypeGraphic },
            new ComboItem() { ID = 2, Value = Resources.General.SelectItemVideoPartTypeBlankScreenText },
            new ComboItem() { ID = 3, Value = Resources.General.SelectItemVideoPartTypeBlankScreen }
        };

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelVideoPartType))]
        public int VideoPartType
        {
            get { return _videoPartType; }
            set
            {
                _videoPartType = value;
                ValidateComboSelect(this, nameof(VideoPartType), _videoPartType);
                UpdateVideoPartTypeProperties();
            }
        }
        private int _videoPartType;

        public Visibility GraphicVideoPartType
        {
            get { return _graphicVideoPartType; }
            set
            {
                _graphicVideoPartType = value;
                NotifyPropertyChanged(nameof(GraphicVideoPartType));
            }
        }
        private Visibility _graphicVideoPartType;

        public Visibility BlankScreenTextVideoPartType
        {
            get { return _blankScreenTextVideoPartType; }
            set
            {
                _blankScreenTextVideoPartType = value;
                NotifyPropertyChanged(nameof(BlankScreenTextVideoPartType));
            }
        }
        private Visibility _blankScreenTextVideoPartType;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelFilePath))]
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyPropertyChanged(nameof(FilePath));    
                ValidateFilePath(this, nameof(FilePath), _filePath);
            }
        } 
        private string _filePath;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelText))]
        public string VideoPartText
        {
            get { return _videoPartText; }
            set
            {
                _videoPartText = value;
                NotifyPropertyChanged(nameof(VideoPartText));
                ValidateNullOrEmpty(this, nameof(VideoPartText), _videoPartText);
            }
        }
        private string _videoPartText;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelHour))]
        public string Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                ValidateTimeSpanPart(nameof(Hour), _hour, null);
                UpdateInTime();
            }
        }
        private string _hour;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelMinute))]
        public string Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                ValidateTimeSpanPart(nameof(Minute), _minute, 59);
                UpdateInTime();
            }
        }
        private string _minute;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelSecond))]
        public string Second
        {
            get { return _second; }
            set
            {
                _second = value;
                ValidateTimeSpanPart(nameof(Second), _second, 59);
                UpdateInTime();
            }
        }
        private string _second;

        [Display(ResourceType = typeof(WPF.Resources.General), Name = nameof(WPF.Resources.General.LabelMilisecond))]
        public string Millisecond
        {
            get { return _millisecond; }
            set
            {
                _millisecond = value;
                ValidateTimeSpanPart(nameof(Millisecond), _millisecond, 999);
                UpdateInTime();
            }
        }
        private string _millisecond;

        public TimeSpan AtTime { get; private set; }

        private readonly IDialogService _deialogService;

        private readonly IVideoService _videoService;

        public VideoPartViewModel(IDialogService dialogService, IVideoService videoService) 
        {
            _deialogService = dialogService;
            _videoService = videoService;

            this.Title = WPF.Resources.General.NameVideoPart;            
            Hour = 0.ToString();
            Minute = 0.ToString();
            Second = 0.ToString();
            Millisecond = 0.ToString();
            VideoPartType = 0;
        }

        public void FileSelect()
        {
            FilePath = _deialogService.OpenFileDialog(
                _deialogService.DicToFileFilterString(
                    _videoService.GetSupportedImageFormatList(), true));
        }

        public bool SavePart(ViewModels.VideoEditorViewModel editorViewModel)
        {
            if(HasErrors)
            {
                _deialogService.WarningDialog(GetErrorsSummary());
                return false;
            }
            
            editorViewModel.AddVideoPart(this);
            return true;
        }

        public override string ToString()
        {
            var resultString = $"{WPF.Resources.General.PartAtTime} {Hour}:{Minute}:{Second}:{Millisecond}";

            resultString += $" {VideoPartTypes.Where(t => t.ID == VideoPartType).First().Value.ToLower()}";

            if (VideoPartType == 1)
            {
                resultString += $" \"{System.IO.Path.GetFileName(FilePath)}\"";
            }

            if (VideoPartType == 2)
            {
                var videoPartTextShorted = VideoPartText;
                if (videoPartTextShorted.Length > 50)
                {
                    videoPartTextShorted = $"{videoPartTextShorted.Substring(0, 50)}...";
                } 
                resultString += $" \"{videoPartTextShorted}\"";
            }

            return resultString;
        }

        private void ValidateTimeSpanPart(string propertyName, string value, int? maxValue)
        {
            ClearErrors(propertyName);
            uint validatedValue;
            if(!uint.TryParse(value, out validatedValue))
            {
                AddError(this, propertyName, WPF.Resources.General.WarningValueIsNotNumber);
                return;
            }

            if(maxValue.HasValue && !(validatedValue <= maxValue))
            {
                AddError(this, propertyName, WPF.Resources.General.WarningValueNumberOutOfRange);
                return;
            }
        }

        private void UpdateInTime()
        {
            AtTime = new TimeSpan(0, Convert.ToInt32(Hour), Convert.ToInt32(Minute), Convert.ToInt32(Second), Convert.ToInt32(Millisecond));
        }

        private void UpdateVideoPartTypeProperties()
        {
            ClearErrors(nameof(FilePath));
            ClearErrors(nameof(VideoPartText));

            GraphicVideoPartType = Visibility.Collapsed;
            BlankScreenTextVideoPartType = Visibility.Collapsed;

            switch(_videoPartType)
            {
                default:
                case 0:
                case 3:
                    break;
                case 1:
                    GraphicVideoPartType = Visibility.Visible;
                    if (string.IsNullOrWhiteSpace(FilePath)) { FilePath = string.Empty; }
                    break;
                case 2:
                    BlankScreenTextVideoPartType = Visibility.Visible;
                    if (string.IsNullOrWhiteSpace(VideoPartText)) { VideoPartText = string.Empty; }
                    break;
            }
        }
    }
}
