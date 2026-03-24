using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// FileBrowserBox.xaml 的交互逻辑
    /// </summary>
    public partial class FileBrowserBox : UserControl
    {
        public FileBrowserBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public FileMode FileMode
        {
            get { return (FileMode)GetValue(FileModeProperty); }
            set { SetValue(FileModeProperty, value); }
        }
        public static readonly DependencyProperty FileModeProperty =
            DependencyProperty.Register("FileMode", typeof(FileMode), typeof(FileBrowserBox), new PropertyMetadata(FileMode.Folder));


        public string SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }
        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string), typeof(FileBrowserBox), new PropertyMetadata(""));


        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileMode == FileMode.Folder)
            {
                System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    SelectedPath = folderBrowserDialog.SelectedPath.Trim();
                }
            }
            else
            {
                //
            }
        }
    }


    public enum FileMode
    {
        File,

        Folder
    }

}
