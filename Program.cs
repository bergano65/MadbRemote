using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managed.Adb;

namespace AdbTst
{
    class Program
    {
        static void Main(string[] args)
        { 
            AndroidDebugBridge mADB = AndroidDebugBridge.CreateBridge(
                "C:\\Program Files (x86)\\Android\\android-sdk\\platform-tools\\adb.exe", false);
            mADB.Start();

            List<Device> devices =
            AdbHelper.Instance.GetDevices(AndroidDebugBridge.SocketAddress);
            IDevice device = devices[0];
            System.Drawing.Bitmap image = (System.Drawing.Bitmap)device.Screenshot.ToImage();
            image.Save("f:\\1.png", System.Drawing.Imaging.ImageFormat.Bmp);
            mADB.Stop();

        }
    }
}
