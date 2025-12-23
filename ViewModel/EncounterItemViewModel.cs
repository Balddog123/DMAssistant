using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant;
using DMAssistant.Model;
using DMAssistant.View;
using DMAssistant.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class EncounterItemViewModel : ObservableObject
    {
        public EncounterItem EncounterItem { get; }

        // Event raised for ANY change that affects TotalCR
        public event Action? EncounterItemChanged;

        public EncounterItemViewModel(EncounterItem item)
        {
            EncounterItem = item;

            // Listen to model-level changes
            EncounterItem.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName is nameof(MonsterGroup.Quantity) ||
                    e.PropertyName is nameof(MonsterGroup.MonsterId) ||
                    e.PropertyName is nameof(EncounterItem.CR) ||
                    e.PropertyName is nameof(EncounterItem.IsAlly))
                {
                    EncounterItemChanged?.Invoke();
                }

                // Update visibility when changing item type
                OnPropertyChanged(nameof(MonsterGroupVisibility));
                OnPropertyChanged(nameof(EncounterEventVisibility));
                OnPropertyChanged(nameof(TypeName));
                OnPropertyChanged(nameof(TotalCR));
                OnPropertyChanged(nameof(CR));
                OnPropertyChanged(nameof(Quantity));
            };

            // Initialize MonsterViewModel if needed
            if (item is MonsterGroup mg && !string.IsNullOrWhiteSpace(mg.MonsterId))
            {
                App.CampaignStore.MonsterIndex.TryGetValue(mg.MonsterId, out _monster);
                if(_monster != null) EncounterItemMonsterViewModel = new MonsterViewModel(_monster);
                else
                {
                    Debug.WriteLine($"!!!could not find monster with ID: {mg.MonsterId}. MonsterGroup: {mg.Name}");
                }
            }

            OpenMonsterWindowCommand = new RelayCommand(OpenMonsterSelector);
        }


        // ----------------------------------------------------------------------
        //  Basic Properties
        // ----------------------------------------------------------------------

        public string Name
        {
            get => EncounterItem.Name;
            set
            {
                if (EncounterItem.Name != value)
                {
                    EncounterItem.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public int RoundNumber
        {
            get => EncounterItem.RoundNumber;
            set
            {
                if (EncounterItem.RoundNumber != value)
                {
                    EncounterItem.RoundNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeName => EncounterItem?.TypeName ?? string.Empty;

        public bool IsAlly
        {
            get => EncounterItem?.IsAlly ?? false;
            set
            {
                if (EncounterItem.IsAlly != value)
                {
                    EncounterItem.IsAlly = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalXP));
                    OnPropertyChanged(nameof(TotalCR));
                }
            }
        }
        public FlowDocument Description
        {
            get => EncounterItem.Description;
            set
            {
                if (EncounterItem.Description != value)
                {
                    EncounterItem.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        // ----------------------------------------------------------------------
        //  Monster-related properties
        // ----------------------------------------------------------------------

        private Monster _monster;

        public Monster Monster
        {
            get => _monster;
            set
            {
                if (SetProperty(ref _monster, value))
                {
                    if (EncounterItem is MonsterGroup mg)
                    {
                        mg.MonsterId = value.ID;
                        EncounterItemMonsterViewModel = new MonsterViewModel(value);
                    }

                    EncounterItemChanged?.Invoke();
                }
            }
        }

        public int Quantity
        {
            get => EncounterItem is MonsterGroup mg ? mg.Quantity : 0;
            set
            {
                if (EncounterItem is MonsterGroup mg && mg.Quantity != value)
                {
                    mg.Quantity = value;
                    OnPropertyChanged();
                    EncounterItemChanged?.Invoke();
                }
            }
        }

        public string CR
        {
            get
            {
                if (EncounterItem != null && EncounterItem is MonsterGroup mg)
                {
                    return mg.CR;
                }
                else
                {
                    return "0";
                }
            }
        }
        public int TotalCR
        {
            get
            {
                int cr = 0;
                if (EncounterItem != null && EncounterItem is MonsterGroup mg)
                {
                    if(int.TryParse(mg.CR, out int challenge))
                    {
                        cr += challenge * mg.Quantity;
                    }
                    else
                    {
                        if (mg.CR.Contains('/'))
                        {

                        }
                    }
                }
                return cr;
            }
        }

        public int TotalXP
        {
            get
            {
                int cr = 0;
                if (EncounterItem != null && EncounterItem is MonsterGroup mg)
                {
                    cr += mg.XP * mg.Quantity;
                }
                return cr;
            }
        }

        private MonsterViewModel _encounterItemMonsterViewModel;
        public MonsterViewModel EncounterItemMonsterViewModel
        {
            get => _encounterItemMonsterViewModel;
            set => SetProperty(ref _encounterItemMonsterViewModel, value);
        }

        // ----------------------------------------------------------------------
        //  Event-specific properties
        // ----------------------------------------------------------------------

        public int Initiative
        {
            get => EncounterItem is EncounterEvent ev ? ev.Initiative : 0;
            set
            {
                if (EncounterItem is EncounterEvent ev && ev.Initiative != value)
                {
                    ev.Initiative = value;
                    OnPropertyChanged();
                }
            }
        }

        // ----------------------------------------------------------------------
        //  UI helpers
        // ----------------------------------------------------------------------

        public Visibility MonsterGroupVisibility =>
            EncounterItem is MonsterGroup ? Visibility.Visible : Visibility.Collapsed;

        public Visibility EncounterEventVisibility =>
            EncounterItem is EncounterEvent ? Visibility.Visible : Visibility.Collapsed;

        // ----------------------------------------------------------------------
        //  Commands
        // ----------------------------------------------------------------------

        public ICommand OpenMonsterWindowCommand { get; }

        private void OpenMonsterSelector()
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
                Monster = window.SelectedMonster;
            }
        }
    }

}