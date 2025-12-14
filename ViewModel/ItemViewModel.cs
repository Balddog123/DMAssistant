using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DMAssistant.ViewModel
{
    public class ItemViewModel : ObservableObject
    {
        public Item Item { get; }

        public string Name
        {
            get => Item != null ? Item.Name : string.Empty;
            set
            {
                if (Item.Name != value)
                {
                    Item.Name = value;
                }
            }
        }

        public PlayerCharacter Owner { get; set; }
        private string _ownerName;
        public string OwnerName
        {
            get => _ownerName;
            set => SetProperty(ref _ownerName, value);
        }
        public Item.ItemRank Rank
        {
            get => Item != null ? Item.Rank : Item.ItemRank.Common;
            set => Item.Rank = value;
        }
        public Item.ItemType Type
        {
            get => Item != null ? Item.Type : Item.ItemType.Minor;
            set => Item.Type = value;
        }

        public bool RequiresAttunement
        {
            get => Item != null ? Item.RequiresAttunement : false;
            set => Item.RequiresAttunement = value;
        }

        public List<Item.ItemRank> AvailableRanks { get; } = Enum.GetValues(typeof(Item.ItemRank)).Cast<Item.ItemRank>().ToList();
        public List<Item.ItemType> ItemTypes { get; } = Enum.GetValues(typeof(Item.ItemType)).Cast<Item.ItemType>().ToList();

        public ItemViewModel(Item item, PlayerCharacter owner = null)
        {
            Item = item;

            if(Item != null)
            {
                Item.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Item.Name)) OnPropertyChanged(nameof(Name));
                    if (e.PropertyName == nameof(Item.Rank)) OnPropertyChanged(nameof(Rank));
                    if (e.PropertyName == nameof(Item.RequiresAttunement)) OnPropertyChanged(nameof(RequiresAttunement));

                };
            }
            

            if (owner != null)
            {
                Owner = owner;
                OwnerName = owner.Name;
                Owner.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Owner.Name)) OwnerName = Owner.Name;
                };
            }
        }
    }

}
