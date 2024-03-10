using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using KRACHEL.WPF.Views.ViewsManager;
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
    /// Interaction logic for VideoEditorWindow.xaml
    /// </summary>
    public partial class VideoEditorWindow : Window
    {
        private readonly IViewsManager _viewsManager;

        private readonly IDialogService _dialogService;

        private readonly IVideoService _videoService;

        private ViewModels.VideoEditorViewModel _viewModel;
        public VideoEditorWindow(IViewsManager viewsManager, IDialogService dialogService, IVideoService videoService)
        {
            _viewsManager = viewsManager;
            _dialogService = dialogService;
            _videoService = videoService;

            _viewModel = new ViewModels.VideoEditorViewModel(_dialogService ,_videoService);
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void SelectAudioFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectSourceAudioFile();
        }

        private void AddVideoPart_Click(object sender, RoutedEventArgs e)
        {
            _viewsManager.OpenWindow<VideoPartWindow>(this, _viewModel);
        }

        private void EditVideoPart_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.VideoPartList.SelectedItem;

            if (selectedItem == null)
            {
                _dialogService.WarningDialog(WPF.Resources.General.WarningSelectionIsEmpty);
                return;
            }

            _viewsManager.OpenWindow<VideoPartWindow>(this, _viewModel, selectedItem);
        }

        private void DeleteVideoPart_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = this.VideoPartList.SelectedItem;

            _viewModel.RemoveVideoPart((ViewModels.VideoPartViewModel)selectedItem);
        }

        private void DeleteAllVideoParts_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveAllVideoParts();
        }

        private void AnalyzeAudioFile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AnalyzeSourceAudioFile();
        }

        private void SelectResultVideoFilePath_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SelectResultVideoFile();
        }

        private async void CreateVideo_Click(object sender, RoutedEventArgs e)
        {
            this.CreateVideo.IsEnabled = false;
            this.CreateVideo.Content = WPF.Resources.General.InformationRequestIsProcessed;
            try
            {
                await _viewModel.Process();
            } catch
            {
                throw;
            } finally
            {
                this.CreateVideo.Content = WPF.Resources.General.ElementProcess;
                this.CreateVideo.IsEnabled = true;
            }   
        }

        private void SortVideoPart_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SortVideoParts();
        }
    }
}
