using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.WPF.ViewModels
{
    internal class MainViewModel : CommonViewModel
    {
        public MainViewModel() 
        {
            this.Title = Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
