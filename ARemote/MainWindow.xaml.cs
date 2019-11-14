using System;
using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Managed.Adb;

namespace ARemote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IShellOutputReceiver
    {
        private AndroidDebugBridge _mADB;
        private IDevice _device;
        private Timer _timer;
        private double _XSize = 1080;
        private double _YSize = 2220;
        private System.Drawing.Point _dragFrom;

        public bool IsCancelled
        { get; set; }

        public void AddOutput(byte[] data, int offset, int length)
        {

        }

        public void Flush()
        {

        }

        public System.Drawing.Point GetDevPoint(System.Drawing.Point from)
        {
            double dX = (double)_XSize/(double)_shotImg.ActualWidth;
            double dY = (double)_YSize / (double)_shotImg.ActualHeight;

            return new System.Drawing.Point((int)(from.X * dX), (int)(from.Y * dY));
        }


        public MainWindow()
        {
            InitializeComponent();

            _mADB = AndroidDebugBridge.CreateBridge(
        "C:\\Program Files (x86)\\Android\\android-sdk\\platform-tools\\adb.exe", false);
            _mADB.Start();

            List<Device> devices =
            AdbHelper.Instance.GetDevices(AndroidDebugBridge.SocketAddress);
            _device = devices[0];

            _timer = new Timer(new TimerCallback(OnTimer));
            _timer.Change(2000, 500);

        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);


        private void OnTimer(object o)
        {
            System.Drawing.Bitmap image = (System.Drawing.Bitmap)_device.Screenshot.ToImage();
            IntPtr intPtr = image.GetHbitmap();

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                intPtr,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            bitmapSource.Freeze();
            DeleteObject(intPtr);

            _shotImg.Dispatcher.Invoke(() =>
            {
                _shotImg.Source = bitmapSource;
            });
        }

        private void _backBtn_Click(object sender, RoutedEventArgs e)
        {

            _device.ExecuteShellCommand("input keyevent 3", this);
        }

        private void _homeBtn_Click(object sender, RoutedEventArgs e)
        {
            _device.ExecuteShellCommand("input keyevent 4", this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsCancelled = true;
        }

        private void _shotImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                return;
            }

            System.Windows.Point from = e.GetPosition(_shotImg);
            _dragFrom = new System.Drawing.Point((int)from.X, (int)from.Y);
            _dragFrom = GetDevPoint(_dragFrom);
        }

        private void _shotImg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point to = e.GetPosition(_shotImg);
            System.Drawing.Point _dragTo = new System.Drawing.Point((int)to.X, (int)to.Y);
            _dragTo = GetDevPoint(_dragTo);

            _device.ExecuteShellCommand(string.Format("input touchscreen swipe {0} {1} {2} {3}",
                _dragFrom.X, _dragFrom.Y, _dragTo.X, _dragTo.Y), this);
        }
        
        
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           System.Windows.Point point = e.GetPosition(_shotImg);
            System.Drawing.Point pointTo = GetDevPoint(new System.Drawing.Point((int)point.X, (int)point.Y));
            string cmd = string.Format("input tap {0} {1}", pointTo.X, pointTo.Y);

            _device.ExecuteShellCommand(cmd, this);
        }

        private void _typeBtn_Click(object sender, RoutedEventArgs e)
        {
            _device.ExecuteShellCommand(string.Format("input text {0}", _typeTxt.Text), this);
        }

        private void _pwdBtn_Click(object sender, RoutedEventArgs e)
        {
            _device.ExecuteShellCommand(string.Format("input text 2128506#"), this);
        }
    }
}
