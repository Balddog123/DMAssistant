using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DMAssistant.Model
{
    public partial class Item : ObservableObject
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            Debug.WriteLine($"Changed property of Item: {Name}.\nProperty changed: {name}");
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
        public enum ItemRank
        {
            Common, Uncommon, Rare, VeryRare, Legendary
        }
        private ItemRank _rank;
        public ItemRank Rank
        {
            get => _rank;
            set
            {
                if (_rank != value)
                {
                    _rank = value;
                    OnPropertyChanged(nameof(Rank));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public enum ItemType
        {
            Minor,
            Major
        }
        private ItemType _type;
        public ItemType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public bool RequiresAttunement { get; set; }
        public string Function { get; set; }
        public string Appearance { get; set; }
        public string Origin { get; set; }
        public string DisplayName => $"{Name}{RankSuffix}";
        public string RankSuffix => Rank switch
        {
            ItemRank.Common => "",
            ItemRank.Uncommon => " (UC)",
            ItemRank.Rare => " (R)",
            ItemRank.VeryRare => " (VR)",
            ItemRank.Legendary => " (L)",
            _ => ""
        };


        [ObservableProperty] public Visibility expandedVisibility = Visibility.Collapsed;

        public Item()
        {
            
        }
        public Item(Item itemToCopy)
        {
            Name = "Copy of " + itemToCopy.Name;
            Rank = itemToCopy.Rank;
            Type = itemToCopy.Type;
            RequiresAttunement = itemToCopy.RequiresAttunement;
            Function = itemToCopy.Function;
            Appearance = itemToCopy.Appearance;
            Origin = itemToCopy.Origin;
        }
    }
}
