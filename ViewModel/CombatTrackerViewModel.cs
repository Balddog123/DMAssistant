using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class CombatTrackerViewModel : ObservableObject
    {
        public Encounter Encounter { get; set; }
        private int _currentRound = 0;
        public int CurrentRound 
        { 
            get => _currentRound; 
            set
            {
                SetProperty(ref _currentRound, value);
                Encounter.CurrentRound = _currentRound;
                Debug.WriteLine($"Setting encounter current round to: {Encounter.CurrentRound}");

            }
        }
        private ObservableCollection<CombatItem> _combatItems = new();
        public ObservableCollection<CombatItem> CombatItems
        {
            get => _combatItems;
            set
            {
                SetProperty(ref _combatItems, value);
                Encounter.CombatItems = _combatItems;
                Debug.WriteLine($"Setting encounter combatitems to: {Encounter.CombatItems.Count}");
            }
        }
        private CombatItem _selectedCombatItem;
        public CombatItem SelectedCombatItem
        {
            get => _selectedCombatItem;
            set { SetProperty(ref _selectedCombatItem, value); }
        }

        public ICommand NextRoundCommand { get; }
        public CombatTrackerViewModel(Encounter encounter, ObservableCollection<CombatItem> combatItems)
        {
            Encounter = encounter;
            CurrentRound = Encounter.currentRound;

            if (combatItems == null || combatItems.Count == 0) CreateCombatItemsThisRound();
            else DisplayOriginalCombatItems(combatItems);

            NextRoundCommand = new RelayCommand(MoveToNextRound);
            Debug.WriteLine(encounter.CombatItems.Count);
        }

        private void MoveToNextRound()
        {
            CurrentRound++;
            CreateCombatItemsThisRound();
            //check encounter for new combat items to make
        }

        private void DisplayOriginalCombatItems(ObservableCollection<CombatItem> combatItems)
        {
            CombatItems = combatItems;

            SortCombatItemsByInitiative();
        }

        private void CreateCombatItemsThisRound()
        {
            if(CurrentRound == 0)
            {
                foreach(PlayerCharacter pc in App.CampaignStore.CurrentCampaign.PCs)
                {
                    CombatItem newItem = new CombatItem(pc.Name, 0, pc.hp, pc.hp, pc.armorClass, CombatItem.Type.Player, pc.playerName);
                    CombatItems.Add(newItem);
                }
            }

            foreach (EncounterItem encounterItem in Encounter.EncounterItems)
            {
                if (encounterItem.roundNumber != CurrentRound) continue;
                CreateEncounterCombatItem(encounterItem);
            }

            SortCombatItemsByInitiative();
        }

        private void SortCombatItemsByInitiative()
        {
            var sorted = CombatItems.OrderByDescending(c => c.Initiative).ToList();
            CombatItems.Clear();
            foreach (var item in sorted)
            {
                CombatItems.Add(item);
            }

            CombatItems = CombatItems;
        }

        private void CreateEncounterCombatItem(EncounterItem encounterItem)
        {
            if (encounterItem is MonsterGroup mg && mg.monsterId != string.Empty)
            {
                Monster monster = App.CampaignStore.MonsterIndex[mg.monsterId];
                for (int i = 0; i < mg.quantity; i++)
                {
                    int hp = 0;
                    if (int.TryParse(monster.HitPoints.Split(' ')[0], out int parsed))
                    {
                        hp = parsed;
                    }
                    //"Armor Class": "17 (Natural Armor)"
                    int ac = 0;
                    if (int.TryParse(monster.ArmorClass.Split(' ')[0], out parsed))
                    {
                        ac = parsed;
                    }
                    CombatItem newItem = new CombatItem(monster.Name, new Random().Next(1, 21), hp, hp, ac, CombatItem.Type.Monster, mg.name);
                    CombatItems.Add(newItem);
                }
            }
            else
            {
                EncounterEvent ev = encounterItem as EncounterEvent;
                CombatItem newItem = new CombatItem(ev.name, ev.Initiative, 0, 0, 0, CombatItem.Type.Event, ev.name);
                CombatItems.Add(newItem);
            }
        }
    }
}
