using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    public partial class DiceRollCalculator : UserControl
    {
        public DiceRollCalculator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double result = 0.0;
            if (int.TryParse(DiceCount.Text, out int numDice))
            {
                if (int.TryParse(AdditionalValue.Text, out int numAdditionalValue))
                {
                    switch (DiceType.SelectedIndex)
                    {
                        case 0:
                            result = CalculateDiceRoll(numDice, 4.0, numAdditionalValue);
                            break;
                        case 1:
                            result = CalculateDiceRoll(numDice, 6.0, numAdditionalValue);
                            break;
                        case 2:
                            result = CalculateDiceRoll(numDice, 8.0, numAdditionalValue);
                            break;
                        case 3:
                            result = CalculateDiceRoll(numDice, 10.0, numAdditionalValue);
                            break;
                        case 4:
                            result = CalculateDiceRoll(numDice, 12.0, numAdditionalValue);
                            break;
                        default:
                            result = CalculateDiceRoll(numDice, 20.0, numAdditionalValue);
                            break;
                    }
                }
            }
            
            Output.Text = result.ToString();
        }

        private double CalculateDiceRoll(int numDice, double diceType, int additionalValue)
        {
            double averageDiceRoll = (diceType + 1) / 2;

            return numDice * averageDiceRoll + additionalValue;
        }
    }
}
