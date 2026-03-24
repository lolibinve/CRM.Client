using Caliburn.Micro;
using CRM.Modular.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace CRM.Client.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        public ShellViewModel()
        {
            var viewmodel = IoC.Get<MainMenuViewModel>();
            ActivateItemAsync(viewmodel, new CancellationToken());
        }

        public void btn_Close()
        {
            this.TryCloseAsync();
        }

        public void button_MaxSize()
        {

            SystemCommands.MaximizeWindow((Window)this.GetView());
            //SystemCommands.RestoreWindow((Window)this.GetView());

        }

        public void button_MiniSize()
        {
            SystemCommands.MinimizeWindow((Window)this.GetView());
        }

    }
}
