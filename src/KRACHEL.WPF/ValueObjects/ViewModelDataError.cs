using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.WPF.ValueObjcts
{
    internal class ViewModelDataError
    {
        public string PropertyName { get; set; }

        public string PropertyLabel { get; set; }   

        public List<string> Errors { get; set; } = new List<string>();
    }
}
