using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace CodeGenerater
{
    class ImageButton : Button
    {
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ImageButton), new PropertyMetadata(Brushes.Gray));

        public Brush MouseEnterBackground
        {
            get { return (Brush)GetValue(MouseEnterBackgroundProperty); }
            set { SetValue(MouseEnterBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseEnterBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseEnterBackgroundProperty =
            DependencyProperty.Register("MouseEnterBackground", typeof(Brush), typeof(ImageButton), new PropertyMetadata(Brushes.White));

    }
}
