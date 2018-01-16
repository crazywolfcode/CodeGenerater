using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeGenerater
{
    class WindowButton:Button
    {
        public static readonly DependencyProperty MyMoverBrushProperty;

        public static readonly DependencyProperty MyEnterBrushProperty;

        static WindowButton()
        {
            WindowButton.MyMoverBrushProperty = DependencyProperty.Register("MyMoverBrush", typeof(Brush), typeof(WindowButton), new PropertyMetadata(Brushes.Black));
            WindowButton.MyEnterBrushProperty = DependencyProperty.Register("MyEnterBrush", typeof(Brush), typeof(WindowButton), new PropertyMetadata(Brushes.White));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowButton), new FrameworkPropertyMetadata(typeof(WindowButton)));
        }

        public WindowButton()
        {
            base.Content = "";           
        }

        public Brush MyMoverBrush
        {
            get
            {
                return base.GetValue(WindowButton.MyMoverBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(WindowButton.MyMoverBrushProperty, value);
            }
        }

        public Brush MyEnterBrush
        {
            get
            {
                return base.GetValue(WindowButton.MyEnterBrushProperty) as Brush;
            }
            set
            {
                base.SetValue(WindowButton.MyEnterBrushProperty, value);
            }
        }
    }
}
