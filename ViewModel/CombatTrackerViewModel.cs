using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
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
            }
        }
        public ICollectionView CombatItemsView { get; }

        private CombatItem _selectedCombatItem;
        public CombatItem SelectedCombatItem
        {
            get => _selectedCombatItem;
            set { SetProperty(ref _selectedCombatItem, value); }
        }

        private Dictionary<EncounterItem, List<CombatItem>> EncounterItemCombatItemMap = new Dictionary<EncounterItem, List<CombatItem>>();
        public ICommand NextRoundCommand { get; }
        public ICommand DeleteCombatItemCommand { get; }
        public ICommand CommitHPComand { get; }
        public CombatTrackerViewModel(Encounter encounter, ObservableCollection<CombatItem> combatItems)
        {
            Encounter = encounter;
            CurrentRound = Encounter.currentRound;

            if (combatItems == null || combatItems.Count == 0) CreateCombatItemsThisRound();
            else DisplayOriginalCombatItems(combatItems);

            NextRoundCommand = new RelayCommand(MoveToNextRound);
            DeleteCombatItemCommand = new RelayCommand<CombatItem>(itemToDelete => DeleteCombatItem(itemToDelete));

            CombatItemsView = CollectionViewSource.GetDefaultView(CombatItems);
            CombatItemsView.SortDescriptions.Add(new SortDescription(nameof(CombatItem.Initiative), ListSortDirection.Descending));
            foreach(var item in CombatItems)
            {
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CombatItem.Initiative)) CombatItemsView.Refresh();
                };
            }
        }

        private void DeleteCombatItem(CombatItem? itemToDelete)
        {
            if (itemToDelete == null) return;
            CombatItems.Remove(itemToDelete);
            CombatItemsView.Refresh();

            foreach (EncounterItem encounterItem in Encounter.EncounterItems)
            {
                if (encounterItem.id == itemToDelete.EncounterItemId && EncounterItemCombatItemMap.TryGetValue(encounterItem, out List<CombatItem> list) && list.Contains(itemToDelete))
                {
                    list.Remove(itemToDelete);
                    break;
                }
            }
        }

        private void MoveToNextRound()
        {
            CurrentRound++;
            CreateCombatItemsThisRound();
        }

        private void DisplayOriginalCombatItems(ObservableCollection<CombatItem> combatItems)
        {
            CombatItems = combatItems;
            foreach (CombatItem combatItem in CombatItems)
            {
                foreach (EncounterItem encounterItem in Encounter.EncounterItems)
                {
                    if (encounterItem.id == combatItem.EncounterItemId)
                    {
                        if (!EncounterItemCombatItemMap.TryGetValue(encounterItem, out List<CombatItem> list)) EncounterItemCombatItemMap[encounterItem] = new List<CombatItem>();
                        EncounterItemCombatItemMap[encounterItem].Add(combatItem);
                        break;
                    }
                }
            }
        }

        private void CreateCombatItemsThisRound()
        {
            if(CurrentRound == 0)
            {
                foreach(PlayerCharacter pc in App.CampaignStore.CurrentCampaign.PCs)
                {
                    CombatItem newItem = new CombatItem(pc.Name, 50, pc.hp, pc.hp, pc.armorClass, 1, CombatItem.Type.Player, pc.playerName, "");
                    CombatItems.Add(newItem);
                }
            }

            foreach (EncounterItem encounterItem in Encounter.EncounterItems)
            {
                if (encounterItem.roundNumber != CurrentRound) continue;
                CreateEncounterCombatItem(encounterItem);
            }

            Debug.WriteLine($"Finished creating combat items: Keys:{EncounterItemCombatItemMap.Keys.Count}");

        }

        private void CreateEncounterCombatItem(EncounterItem encounterItem)
        {
            if (encounterItem == null) return;

            if (encounterItem is MonsterGroup mg && mg.monsterId != string.Empty)
            {
                Monster monster = App.CampaignStore.CurrentCampaign.Monsters.FirstOrDefault(monster => monster.ID == mg.monsterId);

                int numToCreate = mg.Quantity;

                for (int g = 0; g < mg.NumberOfGroups; g++)
                {
                    int numToCreateSingle = CalculateCombatItemAmount(mg, numToCreate);
                    CreateSingleMonsterCombatItem(encounterItem, mg, monster, numToCreateSingle);
                    numToCreate -= numToCreateSingle;
                }

            }
            else if(encounterItem is EncounterEvent ev)
            {
                CombatItem newItem = new CombatItem(ev.name, ev.InitialInitiative, 0, 0, 0, 0, CombatItem.Type.Event, ev.name, ev.id);
                CombatItems.Add(newItem);

                //add to dictionary
                if (!EncounterItemCombatItemMap.TryGetValue(encounterItem, out List<CombatItem> list))
                {
                    EncounterItemCombatItemMap[encounterItem] = new List<CombatItem>();
                }

                EncounterItemCombatItemMap[encounterItem].Add(newItem);
            }
        }

        private void CreateSingleMonsterCombatItem(EncounterItem encounterItem, MonsterGroup mg, Monster monster, int quantity)
        {
            int hp = Monster.GetHPAsInt(monster.HitPoints);
            int ac = Monster.GetACAsInt(monster.ArmorClass);
            int.TryParse(monster.DEX, out int dex);
            int roll = new Random().Next(1, 21);
            int initiative = mg.InitialInitiative != 0 ? mg.InitialInitiative : roll + Monster.GetMod(dex);

            CombatItem newItem = new CombatItem(monster.Name, initiative, hp * quantity, hp * quantity, ac, quantity, CombatItem.Type.Monster, mg.name, mg.id);
            CombatItems.Add(newItem);

            //add to dictionary
            
            if (!EncounterItemCombatItemMap.TryGetValue(encounterItem, out List<CombatItem> list))
            {
                EncounterItemCombatItemMap[encounterItem] = new List<CombatItem>();
                
            }

            EncounterItemCombatItemMap[encounterItem].Add(newItem);
            Debug.WriteLine($"Created single monster. Adding to dictionary: Keys:{EncounterItemCombatItemMap.Keys.Count}, Count for this encounter item: {EncounterItemCombatItemMap[encounterItem].Count}");
        }

        public void Update()
        {
            Debug.WriteLine($"Attempting to update {EncounterItemCombatItemMap.Keys.Count} encounter items in this combat...");
            foreach (EncounterItem encounterItem in Encounter.EncounterItems)
            {
                if (!EncounterItemCombatItemMap.TryGetValue(encounterItem, out List<CombatItem> list)) EncounterItemCombatItemMap[encounterItem] = new List<CombatItem>();
                Debug.WriteLine($"Updating {encounterItem.Name}...");
                MonsterGroup mg = encounterItem as MonsterGroup;

                if (mg != null)
                {
                    Monster monster = App.CampaignStore.CurrentCampaign.Monsters.FirstOrDefault(monster => monster.ID == mg.monsterId);
                    int diff = mg.NumberOfGroups - EncounterItemCombatItemMap[encounterItem].Count;

                    if (diff > 0)
                    {
                        for (int i = 0; i < Math.Abs(diff); i++)
                        {
                            CreateSingleMonsterCombatItem(encounterItem, mg, monster, mg.Quantity / diff);
                        }
                    }
                    else if(diff < 0)
                    {
                        EncounterItemCombatItemMap[encounterItem].Sort((item1, item2) => item2.CurrentHP.CompareTo(item1.CurrentHP));
                        for (int i = 0; i < Math.Abs(diff); i++)
                        {
                            DeleteCombatItem(EncounterItemCombatItemMap[encounterItem][0]);
                        }
                    }
                }

                int calculatedMaxQuantity = mg != null ? mg.Quantity : 1;
                foreach (CombatItem combatItem in EncounterItemCombatItemMap[encounterItem])
                {
                    combatItem.EncounterItemName = encounterItem.Name;

                    if (mg != null)
                    {
                        Monster monster = App.CampaignStore.CurrentCampaign.Monsters.First(m => m.ID == mg.monsterId);

                        combatItem.Name = monster.Name;
                        combatItem.ArmorClass = Monster.GetACAsInt(monster.ArmorClass);

                        int hpPerMonster = Monster.GetHPAsInt(monster.HitPoints);
                        int newAmount = CalculateCombatItemAmount(mg, calculatedMaxQuantity);

                        int damageAlreadyDealt = combatItem.MaxHP - combatItem.CurrentHP;
                        Debug.WriteLine($"Damage dealt: {combatItem.MaxHP} - {combatItem.CurrentHP} = {damageAlreadyDealt}");

                        combatItem.Update(hpPerMonster, newAmount, damageAlreadyDealt);

                        calculatedMaxQuantity -= newAmount;
                    }
                    else if (encounterItem is EncounterEvent ev)
                    {
                        combatItem.Name = ev.Name;
                    }
                }

            }
        }

        private int CalculateCombatItemAmount(MonsterGroup mg, int numToCreate)
        {
            int numToCreateSingle = numToCreate % mg.NumberOfGroups == 0 ? mg.Quantity / mg.NumberOfGroups : mg.Quantity / mg.NumberOfGroups + (mg.Quantity % mg.NumberOfGroups);

            //Debug.WriteLine($"Calculating amount for {mg.Name}:\n numToCreate % mg.NumberOfGroups = {numToCreate % mg.NumberOfGroups}. " +
            //    $"\nEven (mg.Quantity / mg.NumberOfGroups): {mg.Quantity / mg.NumberOfGroups}" +
            //    $"\nOdd (mg.Quantity / mg.NumberOfGroups + (mg.Quantity % mg.NumberOfGroups): {mg.Quantity / mg.NumberOfGroups + (mg.Quantity % mg.NumberOfGroups)}" +
            //    $"\nNumber to create this instance: {numToCreateSingle}");
            return numToCreateSingle;
        }
    }
}
