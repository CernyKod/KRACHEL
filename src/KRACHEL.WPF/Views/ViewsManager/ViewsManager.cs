using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KRACHEL.WPF.Views.ViewsManager
{
    internal class ViewsManager : IViewsManager
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewsManager() { }

        public ViewsManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenWindow<T>(Window owner, object tag = null, object dataContext = null) where T : Window
        {
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            if (window != null)
            {
                window.Activate();
            }
            else
            {
                var newWindow = _serviceProvider.GetRequiredService<T>();
                newWindow.Owner = owner;
                if (tag != null) newWindow.Tag = tag;
                if (dataContext != null) newWindow.DataContext = dataContext;
                newWindow.Show();
            }
        }
    }
}
