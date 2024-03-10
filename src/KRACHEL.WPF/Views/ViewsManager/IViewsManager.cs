using System.Windows;

namespace KRACHEL.WPF.Views.ViewsManager
{
    public interface IViewsManager
    {
        void OpenWindow<T>(Window owner, object tag = null, object dataContext = null) where T : Window;
    }
}