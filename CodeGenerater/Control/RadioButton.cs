using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace CodeGenerater
{
    public class RadioButton : System.Windows.Controls.RadioButton
    {
        /// <summary>
        ///  选中时的前景颜色
        /// </summary>
        public Brush ActiveForground
        {
            get { return (Brush)GetValue(AcitiveForegroundProperty); }
            set { SetValue(AcitiveForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveForground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcitiveForegroundProperty =
            DependencyProperty.Register("ActiveForground", typeof(Brush), typeof(RadioButton), new PropertyMetadata(Brushes.Black));

        public Brush ActiveBackground
        {
            get { return (Brush)GetValue(ActiveBackgroundProperty); }
            set { SetValue(ActiveBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveBackgroundProperty =
            DependencyProperty.Register("ActiveBackground", typeof(Brush), typeof(RadioButton), new PropertyMetadata(Brushes.LightSlateGray));

        public Brush ActiveIndicatorColor
        {
            get { return (Brush)GetValue(ActiveIndicatorColorProperty); }
            set { SetValue(ActiveIndicatorColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveIndicatorColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveIndicatorColorProperty =
            DependencyProperty.Register("ActiveIndicatorColor", typeof(Brush), typeof(RadioButton), new PropertyMetadata(Brushes.RoyalBlue));

        /// <summary>
        /// indicator height
        /// </summary>
        public int IndicatorHeight
        {
            get { return (int)GetValue(IndicatorHeightProperty); }
            set { SetValue(IndicatorHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IndicatorHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(int), typeof(RadioButton), new PropertyMetadata(1));

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set { SetValue(IconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(RadioButton), new PropertyMetadata(new Thickness(0, 0, 2, 0)));

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(RadioButton), new PropertyMetadata(""));



        /// <summary>
        /// 按钮字体图标大小
        /// </summary>
        public int IconSize
        {
            get { return (int)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(RadioButton), new PropertyMetadata(20));

        public Orientation ContentOritation
        {
            get { return (Orientation)GetValue(ContentOritationProperty); }
            set { SetValue(ContentOritationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentOritation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentOritationProperty =
            DependencyProperty.Register("ContentOritation", typeof(Orientation), typeof(RadioButton), new PropertyMetadata(Orientation.Horizontal));
        
        static RadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadioButton), new FrameworkPropertyMetadata(typeof(RadioButton)));
        }

    }
}
