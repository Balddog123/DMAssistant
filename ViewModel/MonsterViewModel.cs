using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using System.Diagnostics;

namespace DMAssistant.ViewModel
{
    public class MonsterViewModel : ObservableObject
    {
        public Monster Monster { get; private set; }

        public string Name
        {
            get => Monster != null ? Monster.Name : string.Empty;
            set => SetProperty(Monster.Name, value, Monster, (m, v) => m.Name = v);
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
        public string STR_Mod
        {
            get
            {
                if(int.TryParse(Monster.STR, out int str))
                {
                    return "+" + ((str - 10) / 2).ToString();
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
                    Debug.WriteLine("Changed DEX...");
                }
            }
        }
        public string DEX_Mod
        {
            get
            {
                if (int.TryParse(Monster.DEX, out int num))
                {
                    return Monster.GetMod(num).ToString();
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
                if (int.TryParse(Monster.CON, out int num))
                {
                    return "+" + ((num - 10) / 2).ToString();
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
                if (int.TryParse(Monster.INT, out int num))
                {
                    return "+" + ((num - 10) / 2).ToString();
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
                if (int.TryParse(Monster.WIS, out int num))
                {
                    return "+" + ((num - 10) / 2).ToString();
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
                if (int.TryParse(Monster.CHA, out int num))
                {
                    return "+" + ((num - 10) / 2).ToString();
                }
                else return string.Empty;
            }
        }


        // Editable plain text versions of Traits, Actions, LegendaryActions
        private string _traitsText;
        public string TraitsText
        {
            get => _traitsText;
            set
            {
                if (SetProperty(ref _traitsText, value))
                    Monster.Traits = value; // update the underlying Monster
            }
        }

        private string _actionsText;
        public string ActionsText
        {
            get => _actionsText;
            set
            {
                if (SetProperty(ref _actionsText, value))
                    Monster.Actions = value;
            }
        }

        private string _legendaryText;
        public string LegendaryText
        {
            get => _legendaryText;
            set
            {
                if (SetProperty(ref _legendaryText, value))
                    Monster.LegendaryActions = value;
            }
        }


        public MonsterViewModel()
        {

        }

        public MonsterViewModel(Monster monster)
        {
            Monster = monster;

            // Convert initial HTML to plain text
            _traitsText = HtmlParser.HtmlToPlainText(monster.Traits);
            _actionsText = HtmlParser.HtmlToPlainText(monster.Actions);
            _legendaryText = HtmlParser.HtmlToPlainText(monster.LegendaryActions);
        }
    }
}
