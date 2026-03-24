using Caliburn.Micro;
using CRM.Model;
using HttpLib;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class WareDetailViewModel : Screen
    {
        public LanguageModel LanModel { set; get; } = new LanguageModel();


        public WareDetailViewModel(LanguageModel language, bool isModify = false)
        {
            if (language == null)
            {
                LanModel = new LanguageModel();
            }
            else
            {
                LanModel = language;
            }
            if (LanModel.ItemNames == null || LanModel.ItemNames.Count == 0)
            {
                LanModel.ItemNames = new ObservableCollection<NameClass>();
                LanModel.ItemNames.Add(new NameClass() { ItemDetailName = string.Empty });
            }
        }

        public void AddTb()
        {
            if(LanModel.ItemNames == null || LanModel.ItemNames.Count==0)
            {
                LanModel.ItemNames = new ObservableCollection<NameClass>();
            }
            LanModel.ItemNames.Add(new NameClass() { ItemDetailName = string.Empty });
        }


        public void SubBtn(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.Tag is string obj)
            {
                if(LanModel.ItemNames.Any(x=>x.ItemDetailName == obj))
                {
                    var first = LanModel.ItemNames.First(x => x.ItemDetailName == obj);
                    LanModel.ItemNames.Remove(first);
                }
            }
        }
    }


}
