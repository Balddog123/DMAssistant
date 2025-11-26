using DMAssistant.Model;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DMAssistant.Helpers
{
    public class GroupHeaderWithMaxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3) return "";

            var rank = values[0] as Item.ItemRank? ?? Item.ItemRank.Common;
            var count = values[1] as int? ?? 0;
            var vm = values[2] as CampaignPCViewModel; // e.g., { "Common": 12, "Uncommon": 6 }

            int max = 0;
            if (vm != null)
            {
                // Here we also need the type (Major/Minor)
                // You can pass it as parameter or assume one per ListView
                var type = (parameter as string)?.Equals("Major", StringComparison.OrdinalIgnoreCase) ?? true
                    ? Item.ItemType.Major
                    : Item.ItemType.Minor;

                max = vm.GetMaxItemsByRank(rank, type);
            }

            return $"{rank} ({count}/{max})";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
