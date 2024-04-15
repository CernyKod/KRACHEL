using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KRACHEL.WPF.Views
{
    /// <summary>
    /// Interaction logic for VideoPartWindow.xaml
    /// </summary>
    public partial class VideoPartWindow : Window
    {
        private readonly IDialogService _dialogService;

        private readonly IVideoService _videoService;

        public VideoPartWindow(IDialogService dialogService, IVideoService videoService)
        {
            _dialogService = dialogService;
            _videoService = videoService;

            DataContext = new ViewModels.VideoPartViewModel(_dialogService, _videoService);
            InitializeComponent();
        }        

        private void SavePart_Click(object sender, RoutedEventArgs e)
        {
            var currentViewModel = (ViewModels.VideoPartViewModel)this.DataContext;
            var parentEditorViewModel = (ViewModels.VideoEditorViewModel)this.Tag;

            if(currentViewModel.SavePart(parentEditorViewModel))
            {
                this.Close();
                this.Owner.Activate();
            }   
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            ((ViewModels.VideoPartViewModel)DataContext).FileSelect();
        }

        private void VideoPartType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender != null && ((ComboBox)sender).SelectedValue != null)
            {
                ((ViewModels.VideoPartViewModel)DataContext).VideoPartType = (int)((ComboBox)sender).SelectedValue;
            }            
        }
    }
}
