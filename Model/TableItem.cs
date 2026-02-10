using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class TableItem : ObservableObject
    {
        private string text;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }
        [JsonIgnore] private bool isRolled;
        [JsonIgnore] public bool IsRolled
        {
            get => isRolled;
            set => SetProperty(ref isRolled, value);
        }
    }
}
