using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System.Windows;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class EncounterItemViewModel : ObservableObject
    {
        private EncounterItem _encounterItem;
        public EncounterItem EncounterItem
        {
            get => _encounterItem;
            set
            {
                if (SetProperty(ref _encounterItem, value))
                {
                    // Notify the UI that derived visibility properties changed
                    OnPropertyChanged(nameof(MonsterGroupVisibility));
                    OnPropertyChanged(nameof(EncounterEventVisibility));
                }
            }
        }


        public string Name
        {
            get => EncounterItem?.Name ?? string.Empty;
            set => SetProperty(EncounterItem.Name, value, EncounterItem, (e, v) => e.Name = v);
        }

        public int RoundNumber
        {
            get => EncounterItem?.RoundNumber ?? 0;
            set
            {
                SetProperty(EncounterItem.RoundNumber, value, EncounterItem, (e, v) => e.RoundNumber = v);
            }
        }

        public string Description
        {
            get => EncounterItem?.Description ?? string.Empty;
            set => SetProperty(EncounterItem.Description, value, EncounterItem, (e, v) => e.Description = v);
        }

        // Monster-specific properties
        public int Quantity
        {
            get => EncounterItem is MonsterGroup mg ? mg.Quantity : 0;
            set 
            {
                if (EncounterItem is MonsterGroup mg)
                {
                    SetProperty(mg.Quantity, value, mg, (e, v) => e.Quantity = v);
                    
                }
                OnPropertyChanged(nameof(EncounterItem.QuantityDisplay));
            }
        }

        private Monster _monster;
        public Monster Monster
        {
            get => _monster;
            set 
            { 
                if (EncounterItem is MonsterGroup mg)
                {
                    SetProperty(ref _monster, value);
                    EncounterItemMonsterViewModel = new MonsterViewModel(_monster);
                    mg.monsterId = _monster.ID;
                }
            }
        }
        private MonsterViewModel _selectedEncounterItemViewModel;
        public MonsterViewModel EncounterItemMonsterViewModel
        {
            get => _selectedEncounterItemViewModel;
            set => SetProperty(ref _selectedEncounterItemViewModel, value);
        }

        // Event-specific properties
        public int Initiative
        {
            get => EncounterItem is EncounterEvent ev ? ev.Initiative : 0;
            set 
            {
                if (EncounterItem is EncounterEvent ev)
                {
                    SetProperty(ev.Initiative, value, ev, (e, v) => e.Initiative = v);
                }
            }
        }


        public Visibility MonsterGroupVisibility => EncounterItem is MonsterGroup ? Visibility.Visible : Visibility.Collapsed;
        public Visibility EncounterEventVisibility => EncounterItem is EncounterEvent ? Visibility.Visible : Visibility.Collapsed;

        public ICommand OpenMonsterWindowCommand { get; }
        public EncounterItemViewModel(EncounterItem item)
        {
            EncounterItem = item;
            OpenMonsterWindowCommand = new RelayCommand(() =>
            {
                var available = App.CampaignStore.CurrentCampaign.Monsters.ToList();
                
                if (!available.Any())
                {
                    MessageBox.Show("No Monsters available!");
                    return;
                }

                var window = new SelectMonsterWindow(available);
                if (window.ShowDialog() == true && window.SelectedMonster != null)
                {
                    var monster = window.SelectedMonster;

                    Monster = monster;
                }
            });

            if(EncounterItem is MonsterGroup mg && mg.monsterId != string.Empty)
            {
                Monster = App.CampaignStore.MonsterIndex[mg.monsterId];
            }
        }
    }
}