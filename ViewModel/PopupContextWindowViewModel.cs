using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class PopupContextWindowViewModel
    {
        public object ContentView { get; set; }

        public PopupContextWindowViewModel(object contentView)
        {
            ContentView = contentView;
        }
    }
}
