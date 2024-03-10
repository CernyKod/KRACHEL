using KRACHEL.Core;
using KRACHEL.Core.Service;
using KRACHEL.WPF.ViewModels;
using KRACHEL.WPF.Views.ViewsManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KRACHEL.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IViewsManager _viewsManager;
        public MainWindow(IViewsManager viewsManager)
        {
            _viewsManager = viewsManager;
            
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        private void VideoEditor_Click(object sender, RoutedEventArgs e)
        {
            _viewsManager.OpenWindow<VideoEditorWindow>(this);
        }

        private void AudioExtractor_Click(object sender, RoutedEventArgs e)
        {            
            _viewsManager.OpenWindow<AudioExtractorWindow>(this);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
