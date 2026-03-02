using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using System.Diagnostics;
using System.Windows.Documents;

namespace DMAssistant.ViewModel
{
    public class MonsterViewModel : ObservableObject
    {
        public Monster Monster { get; private set; }

        public string Name
        {
            get => Monster != null ? Monster.Name : string.Empty;
            set
            {
                if (IsNew) IsNew = false;
                SetProperty(Monster.Name, value, Monster, (m, v) => m.Name = v);
            }
        }

        public string Meta
        {
            get => Monster != null ? Monster.Meta : string.Empty;
            set
            {
                if (Monster != null) SetProperty(Monster.Meta, value, Monster, (m, v) => m.Meta = v);
            }
        }

        public string Challenge
        {
            get => Monster != null ? Monster.Challenge : string.Empty;
            set
            {
                if (Monster != null) SetProperty(Monster.Challenge, value, Monster, (m, v) => m.Challenge = v);
            }
        }

        public string ArmorClass
        {
            get => Monster != null ? Monster.ArmorClass : string.Empty;
            set
            {
                if (Monster != null) SetProperty(Monster.ArmorClass, value, Monster, (m, v) => m.ArmorClass = v);
            }
        }

        public string HitPoints
        {
            get => Monster != null ? Monster.HitPoints : string.Empty;
            set
            {
                if (Monster != null) SetProperty(Monster.HitPoints, value, Monster, (m, v) => m.HitPoints = v);
            }
        }

        public string Speed
        {
            get => Monster != null ? Monster.Speed : string.Empty;
            set
            {
                if (Monster != null) SetProperty(Monster.Speed, value, Monster, (m, v) => m.Speed = v);
            }
        }

        public string STR
        {
            get => Monster != null ? Monster.STR : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.STR, value, Monster, (m, v) => m.STR = v);
                    OnPropertyChanged(nameof(STR_Mod));
                }
            }
        }
        private string GetModFromStatInt(int stat)
        {
            int mod = Monster.GetMod(stat);
            string prefix = "";
            if (mod > 0) prefix = "+";
            
            return prefix + (mod).ToString();
        }
        public string STR_Mod
        {
            get
            {
                if(Monster != null && int.TryParse(Monster.STR, out int str))
                {
                    return GetModFromStatInt(str);
                }
                else return string.Empty;
            }
        }
        public string DEX
        {
            get => Monster != null ? Monster.DEX : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.DEX, value, Monster, (m, v) => m.DEX = v);
                    OnPropertyChanged(nameof(DEX_Mod));
                }
            }
        }
        public string DEX_Mod
        {
            get
            {
                if (Monster != null && int.TryParse(Monster.DEX, out int num))
                {
                    return GetModFromStatInt(num);
                }
                else return string.Empty;
            }
        }
        public string CON
        {
            get => Monster != null ? Monster.CON : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.CON, value, Monster, (m, v) => m.CON = v);
                    OnPropertyChanged(nameof(CON_Mod));
                }
            }
        }
        public string CON_Mod
        {
            get
            {
                if (Monster != null && int.TryParse(Monster.CON, out int num))
                {
                    return GetModFromStatInt(num);
                }
                else return string.Empty;
            }
        }
        public string INT
        {
            get => Monster != null ? Monster.INT : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.INT, value, Monster, (m, v) => m.INT = v);
                    OnPropertyChanged(nameof(INT_Mod));
                }
            }
        }
        public string INT_Mod
        {
            get
            {
                if (Monster != null && int.TryParse(Monster.INT, out int num))
                {
                    return GetModFromStatInt(num);
                }
                else return string.Empty;
            }
        }
        public string WIS
        {
            get => Monster != null ? Monster.WIS : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.WIS, value, Monster, (m, v) => m.WIS = v);
                    OnPropertyChanged(nameof(WIS_Mod));
                }
            }
        }
        public string WIS_Mod
        {
            get
            {
                if (Monster != null && int.TryParse(Monster.WIS, out int num))
                {
                    return GetModFromStatInt(num);
                }
                else return string.Empty;
            }
        }
        public string CHA
        {
            get => Monster != null ? Monster.CHA : string.Empty;
            set
            {
                if (Monster != null)
                {
                    SetProperty(Monster.CHA, value, Monster, (m, v) => m.CHA = v);
                    OnPropertyChanged(nameof(CHA_Mod));
                }
            }
        }
        public string CHA_Mod
        {
            get
            {
                if (Monster != null && int.TryParse(Monster.CHA, out int num))
                {
                    return GetModFromStatInt(num);
                }
                else return string.Empty;
            }
        }

        public bool IsNew
        {
            get => Monster != null ? Monster.IsNew : false;
            set => Monster.IsNew = value;
        }


        // Editable plain text versions of Traits, Actions, LegendaryActions
        public FlowDocument TraitsText
        {
            get => Monster != null ? Monster.Traits : new FlowDocument();
            set
            {
                if (Monster.Traits != value)
                {
                    Monster.Traits = value;
                    OnPropertyChanged();
                }
            }
        }
        public FlowDocument ActionsText
        {
            get => Monster != null ? Monster.Actions : new FlowDocument();
            set
            {
                if (Monster.Actions != value){
                    Monster.Actions = value;
                    OnPropertyChanged();
                }
            }
        }
        public FlowDocument LegendaryText
        {
            get => Monster != null ? Monster.LegendaryActions : new FlowDocument();
            set
            {
                if (Monster.LegendaryActions != value)
                {
                    Monster.LegendaryActions = value;
                    OnPropertyChanged();
                }
            }
        }


        public MonsterViewModel()
        {

        }

        public MonsterViewModel(Monster monster)
        {
            Monster = monster;

            // Convert initial HTML to plain text
            
        }
    }
}
