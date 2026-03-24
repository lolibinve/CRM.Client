using Caliburn.Micro;
using CRM.Model;
using CRM.Modular.Models;
using HttpLib;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace CRM.Modular.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AddRoleViewModel : Screen
    {

        public RoleData role { set; get; } = new RoleData();
        public bool IsAdmin { set; get; }
        public bool IsOperate { set; get; }
        public string Title { set; get; }
     

        public AddRoleViewModel(RoleData data, bool IsModify = false)
        {
            Title = IsModify ? "修改角色信息" : "新增角色";
            if (IsModify)
            {
                if(data.Admin == 1)
                {
                    IsAdmin = true;
                }
                else
                {
                    IsOperate = true;
                }
                this.role.Clone(data);
            }
            else
            {
                IsOperate = true;
                this.role.PassWord = "123456";
            }
        }

        public async void Sure()
        {
            role.Admin = IsAdmin == true ? 1 : 0;
            var result = await CRMRequest.ModifyRole(role);
            if (result != null)
            {
                role.Clone(result);
                var temp = this.GetView();
                if (temp is Window win)
                {
                    win.DialogResult = true;
                }
                await TryCloseAsync();
            }
        }

        public Task CloseForm()
        {
            var temp = this.GetView();
            if (temp is Window win)
            {
                win.DialogResult = false;
            }
            return TryCloseAsync();
        }
    }
}
