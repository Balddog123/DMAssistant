using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DMAssistant.ViewModel
{
    public class ItemViewModel : ObservableObject
    {
        public Item Item { get; }

        public List<Item.ItemRank> AvailableRanks { get; } =
            Enum.GetValues(typeof(Item.ItemRank)).Cast<Item.ItemRank>().ToList();

        public ItemViewModel(Item item)
        {
            Item = item;
        }
    }

}
