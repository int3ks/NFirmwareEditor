using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Usb;
using NCore.USB;

namespace HidSharp
{
    public class HidDeviceLoader
    {
        public static PendingIntent permissionPending;
        public HidDevice GetDeviceOrDefault(int vendorId, int productId)
        {
           
            var usbManager = (UsbManager)Application.Context.GetSystemService(Context.UsbService);
            foreach(var device in usbManager.DeviceList.Values)
            {
                if (device.VendorId == vendorId && device.ProductId == productId)
                {
                    if (!usbManager.HasPermission(device))
                    {
                        lock (this)
                        {
                            if (permissionPending==null)
                            {
                               
                                permissionPending = PendingIntent.GetBroadcast(Application.Context, 0, new Intent(HidUsbReceiver.ACTION_USB_PERMISSION), 0);
                                usbManager.RequestPermission(device, permissionPending);
                            }
                        }
                        return null;
                    }
                    

                    return new HidDevice(device);
                }
            }
            return null;
        }
    }
}


