using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;

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

        public IRelayCommand DeleteCommand { get; set; }

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
