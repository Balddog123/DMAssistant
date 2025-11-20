using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
        public string Type { get; set; }
        public string Function { get; set; }
        public string Appearance { get; set; }
        public string Origin { get; set; }

        public string DisplayName => $"{Name}{RankSuffix}";
        public string RankSuffix => Rank switch
        {
            ItemRank.Common => "",
            ItemRank.Uncommon => " (^)",
            ItemRank.Rare => " (*)",
            ItemRank.VeryRare => " (^*)",
            ItemRank.Legendary => " (***)",
            _ => ""
        };

        [ObservableProperty] public Visibility expandedVisibility = Visibility.Collapsed;

        public Item(string name, ItemRank rank, string type, string function, string appearance, string origin)
        {
            Name = name;
            Rank = rank;
            Type = type;
            Function = function;
            Appearance = appearance;
            Origin = origin;
        }
    }
}
