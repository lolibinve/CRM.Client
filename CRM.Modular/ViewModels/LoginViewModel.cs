using Caliburn.Micro;
using CRM.Modular.Help;
using CRM.Modular.Models;
using CRM.Modular.ViewModels;
using HttpLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CRM.Client.ViewModels
{
    public class LoginViewModel : Screen
    {
        public string Password { set; get; }
        public string Uname { set; get; }
        public bool IsCheck { set; get; } = false;

        private readonly IWindowManager windowManager;
        public bool IsProgressIndeterminate { set; get; }


        public LoginViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;

            IsCheck = AppConfigHelp.TryGetBooleanValue("IsRemember", IsCheck);
            if (IsCheck)
            {
                string name = "";
                string psw = "";
                AppConfigHelp.TryGetStringValue("UserName", out name);
                AppConfigHelp.TryGetStringValue("UserPsw", out psw);
                Uname = name;
                Password = psw;
            }
        }

        public async void Login()
        {
            IsProgressIndeterminate = true;
            int? result = await CRMRequest.LoginAsync(Uname, Password, new System.Threading.CancellationTokenSource());
            IsProgressIndeterminate = false;

            if (result != null)
            {
                var temp = IoC.Get<CacheInfo>();
                temp.IsAdmin = result.Value == 1;
                temp.LoginAccount = Uname;

                if (IsCheck)
                {
                    AppConfigHelp.Save("IsRemember", IsCheck);
                    AppConfigHelp.Save("UserName", Uname);
                    AppConfigHelp.Save("UserPsw", Password);
                }
                else
                {
                    AppConfigHelp.Save("IsRemember", false);
                    AppConfigHelp.Save("UserName", "");
                    AppConfigHelp.Save("UserPsw", "");
                }

                await windowManager.ShowWindowAsync(IoC.Get<ShellViewModel>());
                await this.TryCloseAsync();
            }

        }

        public void OnPasswordChanged(PasswordBox source)
        {
            this.Password = source.Password;
        }

        public void CloseFrom()
        {
            TryCloseAsync();
        }
    }
}
