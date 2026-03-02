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
using System.Windows.Documents;
using System.Windows.Input;
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
                if (IsNew) IsNew = false;

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

        public bool IsNew
        {
            get => Item != null ? Item.IsNew : false;
            set => Item.IsNew = value;
        }

        public string OriginText
        {
            get
            {
                if (Item?.Origin == null)
                    return string.Empty;

                return new TextRange(
                    Item.Origin.ContentStart,
                    Item.Origin.ContentEnd
                ).Text.Trim();
            }
        }

        public string AppearanceText
        {
            get
            {
                if (Item?.Appearance == null)
                    return string.Empty;

                return new TextRange(
                    Item.Appearance.ContentStart,
                    Item.Appearance.ContentEnd
                ).Text.Trim();
            }
        }
        public string FunctionText
        {
            get
            {
                if (Item?.Function == null)
                    return string.Empty;

                return new TextRange(
                    Item.Function.ContentStart,
                    Item.Function.ContentEnd
                ).Text.Trim();
            }
        }



        public List<Item.ItemRank> AvailableRanks { get; } = Enum.GetValues(typeof(Item.ItemRank)).Cast<Item.ItemRank>().ToList();
        public List<Item.ItemType> ItemTypes { get; } = Enum.GetValues(typeof(Item.ItemType)).Cast<Item.ItemType>().ToList();

        public ICommand RandomizeNameCommand { get; }
        public ICommand RandomizeFunctionCommand { get; }
        public ICommand RandomizeOriginCommand { get; }
        public ICommand RandomizeAppearanceCommand { get; }

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
            
            //used only on PC viewer
            if (owner != null)
            {
                Owner = owner;
                OwnerName = owner.Name;
                Owner.PropertyChanged += (_, e) =>
                {
                    if (e.PropertyName == nameof(Owner.Name)) OwnerName = Owner.Name;
                };
            }
            RandomizeNameCommand = new RelayCommand(() => Item.Name = Item.GetRandomName());
            RandomizeFunctionCommand = new RelayCommand(() =>
            {
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.Blocks.Add(new Paragraph(new Run(Item.GetRandomFunction())));
                Item.Function = flowDoc;
            });
            RandomizeOriginCommand = new RelayCommand(() =>
            {
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.Blocks.Add(new Paragraph(new Run(Item.GetRandomItemOrigin())));
                Item.Origin = flowDoc;
            });
            RandomizeAppearanceCommand = new RelayCommand(() =>
            {
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.Blocks.Add(new Paragraph(new Run(Item.GetRandomAppearance())));
                Item.Appearance = flowDoc;
            });
        }
    }

}
