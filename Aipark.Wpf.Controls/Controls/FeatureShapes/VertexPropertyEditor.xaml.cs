using System.Collections.Generic;
using System.Windows;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// VertexPropertyEditor.xaml 的交互逻辑
    /// </summary>
    public partial class VertexPropertyEditor
    {
        private MessageBoxResult Result = MessageBoxResult.None;

        private ShapeInfo objectInfo = null;
        private Vertex vertex = null;

        public VertexPropertyEditor(ShapeInfo objectInfo, Vertex vertex)
        {
            InitializeComponent();

            this.objectInfo = objectInfo;
            this.vertex = vertex;
        }

        private void TWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.visibilityCmb.ItemsSource = new List<string>() { "估算点", "可见点" };
            this.visibilityCmb.SelectedIndex = 1;

            this.realityCmb.ItemsSource = new List<string>() { "图外点", "图内点" };
            this.realityCmb.SelectedIndex = 1;

            //序列号
            if (vertex.Properties.ContainsKey("sn"))
            {
                this.serialNumberTbx.Text = vertex.Properties["sn"];
            }
            //可见性
            if (vertex.Properties.ContainsKey("vis"))
            {
                if (int.TryParse(vertex.Properties["vis"], out int vis) && vis > -1 && vis < 2)
                {
                    this.visibilityCmb.SelectedIndex = vis;
                }
            }
            //虚实点
            if (vertex.Properties.ContainsKey("rel"))
            {
                if (int.TryParse(vertex.Properties["rel"], out int rel) && rel > -1 && rel < 2)
                {
                    this.realityCmb.SelectedIndex = rel;
                }
            }
        }

        /// <summary>
        /// 弹出编辑框
        /// </summary>
        /// <param name="objectInfo"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(ShapeInfo objectInfo, Vertex vertex)
        {
            VertexPropertyEditor messageBox = new VertexPropertyEditor(objectInfo, vertex);
            messageBox.ShowDialog();

            return messageBox.Result;
        }

        /// <summary>
        /// 确定按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(this.serialNumberTbx.Text, out int sn))
            {
                this.errorTbx.Text = "序列号不合法";
                return;
            }

            vertex.Properties["sn"] = this.serialNumberTbx.Text;
            vertex.Properties["vis"] = this.visibilityCmb.SelectedIndex.ToString();
            vertex.Properties["rel"] = this.realityCmb.SelectedIndex.ToString();

            Result = MessageBoxResult.OK;
            this.Close();
        }
        /// <summary>
        /// 取消按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 序列号增加按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.serialNumberTbx.Text, out int sn) && sn > 0)
            {
                this.serialNumberTbx.Text = (sn + 1).ToString();
            }
            else
            {
                this.serialNumberTbx.Text = "1";
            }
            this.errorTbx.Text = "";
        }
        /// <summary>
        /// 序列号减少按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.serialNumberTbx.Text, out int sn) && sn > 0)
            {
                this.serialNumberTbx.Text = (sn - 1).ToString();
            }
            else
            {
                this.serialNumberTbx.Text = "1";
            }
            this.errorTbx.Text = "";
        }
    }
}
