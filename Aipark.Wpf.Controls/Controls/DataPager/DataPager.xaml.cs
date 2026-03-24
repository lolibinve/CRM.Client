using System;
using System.Windows;
using System.Windows.Controls;

namespace Aipark.Wpf.Controls
{
    /// <summary>
    /// DataPager.xaml 的交互逻辑
    /// https://www.cnblogs.com/scheshan/archive/2012/06/19/2554550.html
    /// </summary>
    public partial class DataPager : UserControl
    {
        /// <summary>
        /// 分页前处理的事件,如果设置e.IsCancel=True将取消分页
        /// </summary>
        public event Action<DataPager, PageChangingEventArgs> PageChanging;
        /// <summary>
        /// 分页后处理的事件
        /// </summary>
        public event Action<DataPager, PageChangedEventArgs> PageChanged;


        public DataPager()
        {
            InitializeComponent();
        }


        #region 附加的依赖属性

        /// <summary>
        /// 当前页码[1,PageCount]
        /// </summary>
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(DataPager), new UIPropertyMetadata(1, (sender, e) =>
            {
                if (sender is DataPager dp)
                {
                    int index = (int)(e.NewValue);
                    if (index < 1)
                    {
                        dp.PageIndex = 1;
                    }
                    if (index > dp.PageCount)
                    {
                        dp.PageIndex = dp.PageCount;
                    }
                    dp.CanGoFirstOrPrev = index > 1 ? true : false;
                    dp.CanGoLastOrNext = index < dp.PageCount ? true : false;
                }
            }));

        /// <summary>
        /// 单页记录数量
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(DataPager), new UIPropertyMetadata(50, (sender, e) =>
            {
                if (sender is DataPager dp)
                {
                    int size = (int)(e.NewValue);
                    if (size < 1)
                    {
                        dp.PageSize = 50;
                    }
                    dp.ResetPageIndexCount();
                }
            }));

        /// <summary>
        /// 记录总数
        /// </summary>
        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(DataPager), new UIPropertyMetadata(0, (sender, e) =>
            {
                if (sender is DataPager dp)
                {
                    int count = (int)(e.NewValue);
                    if (count < 0)
                    {
                        dp.TotalCount = 0;
                    }
                    dp.ResetPageIndexCount();
                }
            }));

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            private set { SetValue(PageCountProperty, value); }
        }
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(DataPager), new UIPropertyMetadata(1));


        /// <summary>
        /// 可以返回首页或上一页
        /// </summary>
        public bool CanGoFirstOrPrev
        {
            get { return (bool)GetValue(CanGoFirstOrPrevProperty); }
            private set { SetValue(CanGoFirstOrPrevProperty, value); }
        }
        public static readonly DependencyProperty CanGoFirstOrPrevProperty =
            DependencyProperty.Register("CanGoFirstOrPrev", typeof(bool), typeof(DataPager), new UIPropertyMetadata(true));

        /// <summary>
        /// 可以跳转尾页或下一页
        /// </summary>
        public bool CanGoLastOrNext
        {
            get { return (bool)GetValue(CanGoLastOrNextProperty); }
            private set { SetValue(CanGoLastOrNextProperty, value); }
        }
        public static readonly DependencyProperty CanGoLastOrNextProperty =
            DependencyProperty.Register("CanGoLastOrNext", typeof(bool), typeof(DataPager), new UIPropertyMetadata(true));

        #endregion


        /// <summary>
        /// 跳转到首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToFirstPage_Click(object sender, RoutedEventArgs e)
        {
            AdaptPageChanging(1);
        }
        /// <summary>
        /// 跳转到上一页
        /// </summary>
        private void GoToPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            AdaptPageChanging(this.PageIndex - 1);
        }
        /// <summary>
        /// 跳转到下一页
        /// </summary>
        private void GoToNextPage_Click(object sender, RoutedEventArgs e)
        {
            AdaptPageChanging(this.PageIndex + 1);
        }
        /// <summary>
        /// 跳转到末页
        /// </summary>
        private void GoToLastPage_Click(object sender, RoutedEventArgs e)
        {
            AdaptPageChanging(this.PageCount);
        }
        /// <summary>
        /// 跳转到输入的页码
        /// </summary>
        private void GoToInputPageIndex_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.inputPageIndexTbx.Text, out int pageIndex))
            {
                AdaptPageChanging(pageIndex);
                if (pageIndex != this.PageIndex)
                {
                    //修正输入的错误页码
                    this.PageIndex--;
                    this.PageIndex++;
                }
            }
        }

        /// <summary>
        /// 应用页面跳转
        /// </summary>
        /// <param name="index"></param>
        internal void AdaptPageChanging(int index)
        {
            if (index < 1) index = 1;
            if (index > this.PageCount) index = this.PageCount;

            bool canceled = false;
            if (this.PageChanging != null)
            {
                var eventArgs = new PageChangingEventArgs() { OldPageIndex = this.PageIndex, NewPageIndex = index };
                this.PageChanging.Invoke(this, eventArgs);
                canceled = eventArgs.IsCancel;
            }

            if (!canceled)
            {
                this.PageIndex = index;
                this.PageChanged?.Invoke(this, new PageChangedEventArgs() { CurrentPageIndex = this.PageIndex });
            }
        }

        /// <summary>
        /// 重置总页数和当前页码
        /// </summary>
        void ResetPageIndexCount()
        {
            if (this.TotalCount == 0)
            {
                this.PageCount = 1;
            }
            else
            {
                this.PageCount = this.TotalCount % this.PageSize > 0 ? (this.TotalCount / this.PageSize) + 1 : this.TotalCount / this.PageSize;
            }
            this.PageIndex = 1;
            this.CanGoFirstOrPrev = this.PageIndex > 1 ? true : false;
            this.CanGoLastOrNext = this.PageIndex < this.PageCount ? true : false;
        }
    }


    public class PageChangedEventArgs : RoutedEventArgs
    {
        public int CurrentPageIndex { get; set; }
    }


    public class PageChangingEventArgs : RoutedEventArgs
    {
        public int OldPageIndex { get; set; }
        public int NewPageIndex { get; set; }
        public bool IsCancel { get; set; }
    }

}
