using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public class ShopViewModel
    {
        private static int MAXITEMS = 20;

        public ObservableCollection<ItemViewModel> Items = new ObservableCollection<ItemViewModel>();
        public ICollectionView ItemsView { get; set; }
        public ShopViewModel()
        {
            if (App.CampaignStore.CurrentCampaign.Items.Count == 0) return;

            Random rand = new Random();
            int numItems = rand.Next(MAXITEMS * 3 / 4, MAXITEMS);

            for (int i = 0; i < numItems; i++)
            {
                int index = rand.Next(0, App.CampaignStore.CurrentCampaign.Items.Count);
                Items.Add(new ItemViewModel(App.CampaignStore.CurrentCampaign.Items[index]));
            }

            if(numItems < MAXITEMS)
            {
                int dif = MAXITEMS - numItems;
                for(int i = 0; i < dif; i++)
                {
                    int index = rand.Next(Items.Count);
                    Items.Add(new ItemViewModel(Items[index].Item));
                }
            }


            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.SortDescriptions.Add(new SortDescription("Item.Name", ListSortDirection.Ascending));
        }
    }
}
