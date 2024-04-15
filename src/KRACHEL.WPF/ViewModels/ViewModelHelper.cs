using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace KRACHEL.WPF.ViewModels
{
    internal class ViewModelHelper
    {
        internal static string GetLabelForProperty(CommonViewModel viewModel, string propertyName)
        {
            var propertyLabel = propertyName;
            
            var displayAttribute = viewModel.GetType().GetProperty(propertyName).GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute != null)
            {
                propertyLabel = displayAttribute.Name;
                if( displayAttribute.ResourceType != null )
                {
                    propertyLabel = new ComponentResourceManager(displayAttribute.ResourceType).GetString(displayAttribute.Name);
                }
            }

            return propertyLabel;
        }
    }
}
