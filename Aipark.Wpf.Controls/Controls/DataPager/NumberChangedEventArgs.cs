using System.Windows;

namespace Aipark.Wpf.Controls
{
    public class NumberChangedEventArgs : RoutedEventArgs
    {
        public int OldNumber { get; set; }
        public int NewNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns>新旧值不相同</returns>
        public bool SetNewNumber(int number)
        {
            NewNumber = number;
            return HasValueChanged;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool HasValueChanged => this.OldNumber != this.NewNumber;
    }

}
