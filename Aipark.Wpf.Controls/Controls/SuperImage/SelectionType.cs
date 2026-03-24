
using System;
using System.Windows;
/**
* 
* 设计思路：在开启交互的模式下,通过鼠标左键双击切换交互开关,在图像上选取指定类型的信息。
*  
*/
namespace Aipark.Wpf.Controls.SuperImage
{
    /// <summary>
    /// 图像区域选择模式
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 禁止
        /// </summary>
        Disable = 0,

        /// <summary>
        /// 启用
        /// </summary>
        Enable = 1
    }

    /// <summary>
    /// 图像区域选择类型
    /// </summary>
    public enum SelectionType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point = 0,

        /// <summary>
        /// 连线
        /// </summary>
        Line = 1,

        /// <summary>
        /// 矩形区域
        /// </summary>
        Rectangle = 2
    }

    /// <summary>
    /// 图像区域选择使能状态
    /// </summary>
    public enum SelectionSwitch
    {
        /// <summary>
        /// 未开启
        /// </summary>
        OFF = 0,

        /// <summary>
        /// 已开启
        /// </summary>
        ON = 1
    }

    /// <summary>
    /// 图像区域选择使能状态切换方式
    /// </summary>
    public enum SelectionSwitchChangeMode
    {
        /// <summary>
        /// 通过参数设置
        /// </summary>
        Default = 0,

        /// <summary>
        /// 鼠标右键双击
        /// </summary>
        RightButtonDoubleClicked = 1
    }

    /// <summary>
    /// 选择区域
    /// </summary>
    public class SelectionEventArgs : EventArgs
    {
        /// <summary>
        /// 开始点
        /// </summary>
        public Point Point1 { get; set; }

        /// <summary>
        /// 结束点
        /// </summary>
        public Point Point2 { get; set; }
    }
}
