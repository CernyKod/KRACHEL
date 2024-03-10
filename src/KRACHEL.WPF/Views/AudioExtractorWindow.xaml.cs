using KRACHEL.Core.Service;
using KRACHEL.WPF.Services;
using KRACHEL.WPF.ViewModels;
using Microsoft.Extensions.Logging;
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
    /// Interaction logic for AudioExtractorWindow.xaml
    /// </summary>
    public partial class AudioExtractorWindow : Window
    {
        private readonly IDialogService _dialogService;

        private readonly IVideoService _videoService;

        private AudioExtractorViewModel _viewModel;
        
        public AudioExtractorWindow(IDialogService dialogService, IVideoService videoService)
        {
            _dialogService = dialogService;
            _videoService = videoService;            

            _viewModel = new AudioExtractorViewModel(_dialogService, _videoService);
            DataContext = _viewModel;
            InitializeComponent();            
        }

        private void SourceVideoFilePathSelect_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SourceVideoFilePathSelect();
        }        

        private void ResultAudioFilePathSelect_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResultAudioFilePathSelect();
        }

        private async void Process_Click(object sender, RoutedEventArgs e)
        {
            this.Process.IsEnabled = false;
            this.Process.Content = WPF.Resources.General.InformationRequestIsProcessed;

            try
            {                
                await _viewModel.Extract();
            } catch
            {
                throw;
            } finally
            {
                this.Process.IsEnabled = true;
                this.Process.Content = WPF.Resources.General.ElementProcess;
            }             
        }         
    }
}
