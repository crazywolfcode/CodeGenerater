using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace CodeGenerater
{
    class WindowBehavior
    {
        private const int WM_NCHITTEST = 0X0084; //测试消息
        private const int WM_GETMINMAXINFO = 0X0024; //窗口大小
        private const int WM_LBUTTONUP = 0x202fffe; //鼠标左弹起
        private const int WM_LBUTTONDOWN = 0x201fffe;//鼠标左按下
        private const int WM_SETCURSOR = 32;//鼠标左按下
        // 0x202fffe: WM_LBUTTONUP and HitTest
        // 0x201fffe: WM_LBUTTONDOWN and HitTest

        private System.Windows.Window targetWindow; //目标窗口
        private int CornerWidth = 3;        //拐角的宽度
        private int BorderThickness = 4; //边框的宽度
        private int BorderThicknessTransparent = 5;  //边框阴影宽度
        private Point MousePoint = new Point(); //鼠标坐标

        static Storyboard blinkStoryboard;
        // static DropShadowEffect dropShadowEffect;
        Effect originalEffect;



        public enum HitTest : int
        {
            #region 下面列出的鼠标击中测试枚举值之一
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,

            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,

            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21
            #endregion
        }

        public WindowBehavior(Window window)
        {
            this.targetWindow = window;
        }

        public void RepairWindowDefaultBehavior()
        {
            if (targetWindow == null) return;
            this.targetWindow.SourceInitialized += delegate
            {
                WindowInteropHelper wHelper = new WindowInteropHelper(targetWindow);
                IntPtr handle = wHelper.Handle;
                HwndSource hwndSource = HwndSource.FromHwnd(handle);

                if (hwndSource != null)
                {
                    hwndSource.AddHook(WindowProc);
                }
            };

        }
        /// <summary>
        /// 消息拦截
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="Iparam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr Iparam, ref bool handled)
        {
            switch (msg)
            {
                //缩放
                case WM_NCHITTEST:
                    if (targetWindow.WindowState != WindowState.Normal)
                    {
                        break;
                    }

                    this.MousePoint.X = (Iparam.ToInt32() & 0xFFF);
                    this.MousePoint.Y = (Iparam.ToInt32() >> 16);

                    //window left top 
                    if (this.MousePoint.X > this.targetWindow.Left + this.BorderThicknessTransparent
                        && this.MousePoint.X <= this.targetWindow.Left + this.BorderThicknessTransparent + this.CornerWidth
                        && this.MousePoint.Y > this.targetWindow.Top + this.BorderThicknessTransparent
                        && this.MousePoint.Y <= this.targetWindow.Top + this.BorderThicknessTransparent + this.CornerWidth
                        )
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOPLEFT);
                    }
                    //窗口左下角
                    else if (
                        this.MousePoint.X > this.targetWindow.Left + this.BorderThicknessTransparent
                        && this.MousePoint.X <= this.targetWindow.Left + this.BorderThicknessTransparent + this.CornerWidth
                        && this.MousePoint.Y < this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent
                        && this.MousePoint.Y >= this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent - this.CornerWidth
                        )
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOMLEFT);
                    }
                    //窗口右上角
                    else if (
                        this.MousePoint.X < this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent
                        && this.MousePoint.X >= this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent - this.CornerWidth
                        && this.MousePoint.Y > this.targetWindow.Top + this.BorderThicknessTransparent
                        && this.MousePoint.Y <= this.targetWindow.Top + this.BorderThicknessTransparent + this.CornerWidth
                        )
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOPRIGHT);
                    }
                    //窗口右下角
                    else if (this.MousePoint.X < this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent
                        && this.MousePoint.X >= this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent - this.CornerWidth
                        && this.MousePoint.Y < this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent
                        && this.MousePoint.Y >= this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent - this.CornerWidth)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
                    }
                    //窗口左侧
                    else if (this.MousePoint.X > this.targetWindow.Left + this.BorderThicknessTransparent
                        && this.MousePoint.X <= this.targetWindow.Left + this.BorderThicknessTransparent + this.BorderThickness
                        && this.MousePoint.Y > this.targetWindow.Top + this.BorderThicknessTransparent
                        && this.MousePoint.Y < this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTLEFT);
                    }
                    //窗口右侧
                    else if (this.MousePoint.X < this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent
                        && this.MousePoint.X >= this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent - this.BorderThickness
                        && this.MousePoint.Y > this.targetWindow.Top + this.BorderThicknessTransparent
                        && this.MousePoint.Y < this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTRIGHT);
                    }
                    //窗口上方
                    else if (this.MousePoint.X > this.targetWindow.Left + this.BorderThicknessTransparent
                        && this.MousePoint.X < this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent
                        && this.MousePoint.Y > this.targetWindow.Top + this.BorderThicknessTransparent
                        && this.MousePoint.Y <= this.targetWindow.Top + this.BorderThicknessTransparent + this.BorderThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTTOP);
                    }
                    //窗口下方
                    else if (this.MousePoint.X > this.targetWindow.Left + this.BorderThicknessTransparent
                        && this.MousePoint.X < this.targetWindow.Left + this.targetWindow.ActualWidth - this.BorderThicknessTransparent
                        && this.MousePoint.Y < this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent
                        && this.MousePoint.Y >= this.targetWindow.Top + this.targetWindow.ActualHeight - this.BorderThicknessTransparent - this.BorderThickness)
                    {
                        handled = true;
                        return new IntPtr((int)HitTest.HTBOTTOM);
                    }
                    //其他消息
                    else
                    {
                        break;
                    }

                case WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, Iparam);
                    handled = true;
                    break;
                //Blend 模态窗口
                case WM_SETCURSOR:
                    WmSetCursor(Iparam, ref handled);
                    break;
                default:

                    break;
            }
            return IntPtr.Zero;
        }

        private void WmSetCursor(IntPtr lParam, ref bool handled)
        {

            if (lParam.ToInt32() == 0x202fffe || lParam.ToInt32() == 0x201fffe)
            {
                // if the wnd is not actived
                if (!targetWindow.IsActive)
                {
                    // we find the actived childwnd in parent's children wnds ,then blink it
                    if (targetWindow.OwnedWindows.Count > 0)
                    {
                        foreach (Window child in targetWindow.OwnedWindows)
                        {
                            if (child.IsActive)
                            {
                                // FlashWindowEx cann't use for non-border window...
                                Blink(child);
                                handled = true;
                                return;
                            }
                        }
                    }
                    else
                    {
                        // if target window  has 0 children 
                        // then , find current active wnd and blink it.
                        // eg: MessageBox.Show("hello!"); the box without
                        // owner, when setcursor to target window , we will
                        // blink this box.
                        IntPtr pWnd = new WindowInteropHelper(targetWindow).Handle;
                        if (pWnd != IntPtr.Zero)
                        {
                            HwndSource hs = HwndSource.FromHwnd(pWnd);

                            Window activeWnd = null == hs ? null : hs.RootVisual as Window;
                            if (null != activeWnd && activeWnd.IsActive)
                            {
                                Blink(activeWnd);
                                handled = true;
                                return;
                            }
                        }

                    }
                }
            }
            handled = false;
        }

        public void Blink(Window window)
        {
            if (null != window)
            {
                if (null == NameScope.GetNameScope(targetWindow))
                    NameScope.SetNameScope(targetWindow, new NameScope());

                blinkStoryboard = initBlinkStory();
                originalEffect = targetWindow.Effect;

                if (null == targetWindow.Effect || targetWindow.Effect.GetType() != typeof(DropShadowEffect))
                {
                    targetWindow.Effect = InitDropShadowEffect();
                }

                targetWindow.RegisterName("_blink_effect", targetWindow.Effect);

                Storyboard.SetTargetName(blinkStoryboard.Children[0], "_blink_effect");

                blinkStoryboard.Begin(targetWindow, true);

                targetWindow.UnregisterName("_blink_effect");

            }
        }

        private Storyboard initBlinkStory()
        {
            #region xaml code

            /*
        <Storyboard x:Key="BlinkStory">
            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty="(UIElement.Effect).(DropShadowEffect.BlurRadius)" Storyboard.TargetName="border">
                <EasingDoubleKeyFrame KeyTime="0" Value="8">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ElasticEase EasingMode="EaseOut"/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
                <EasingDoubleKeyFrame KeyTime="0:0:0.3" Value="26">
                    <EasingDoubleKeyFrame.EasingFunction>
                        <ElasticEase EasingMode="EaseOut"/>
                    </EasingDoubleKeyFrame.EasingFunction>
                </EasingDoubleKeyFrame>
            </DoubleAnimationUsingKeyFrames>
        </Storyboard>
             */

            #endregion // xaml code

            Storyboard storyboard = new Storyboard();

            DoubleAnimationUsingKeyFrames keyFrames = new DoubleAnimationUsingKeyFrames();

            EasingDoubleKeyFrame kt1 = new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)));
            EasingDoubleKeyFrame kt2 = new EasingDoubleKeyFrame(8, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)));

            kt1.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut };
            kt2.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut };

            keyFrames.KeyFrames.Add(kt1);
            keyFrames.KeyFrames.Add(kt2);

            storyboard.Children.Add(keyFrames);
            Storyboard.SetTargetProperty(keyFrames, new PropertyPath(System.Windows.Media.Effects.DropShadowEffect.BlurRadiusProperty));

            return storyboard;
        }
        private DropShadowEffect InitDropShadowEffect()
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect();
            dropShadowEffect.BlurRadius = 8;
            dropShadowEffect.ShadowDepth = 0;
            dropShadowEffect.Direction = 0;
            dropShadowEffect.Color = System.Windows.Media.Colors.Black;
            return dropShadowEffect;
        }
        /// <summary>
        /// 更改最小化最大化时窗口位置大小
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="lParam"></param>
        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitor, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left) - 3;
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top) - 3;
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left) + 6;
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top) + 6;
                mmi.ptMinTrackSize.x = (int)this.targetWindow.MinWidth;
                mmi.ptMinTrackSize.y = (int)this.targetWindow.MinHeight;
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #region Nested type: MINMAXINFO
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }
        #endregion

        #region Nested type: MONITORINFO
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }
        #endregion

        #region Nested type: POINT
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        #endregion

        #region Nested type: RECT
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static readonly RECT Empty;

            public int Width
            {
                get { return Math.Abs(right - left); }
            }
            public int Height
            {
                get { return bottom - top; }
            }

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(RECT rcSrc)
            {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }

            public override string ToString()
            {
                if (this == Empty)
                {
                    return "RECT {Empty}";
                }
                return "RECT { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Rect))
                {
                    return false;
                }
                return (this == (RECT)obj);
            }

            public override int GetHashCode()
            {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }

            public static bool operator ==(RECT rect1, RECT rect2)
            {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            public static bool operator !=(RECT rect1, RECT rect2)
            {
                return !(rect1 == rect2);
            }
        }
        #endregion
        //---
    }
}
