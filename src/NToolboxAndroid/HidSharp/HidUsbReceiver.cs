using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using HidSharp;
using System;
using System.Collections.Generic;

namespace NCore.USB
{

    [BroadcastReceiver(Enabled = false)]
    //[IntentFilter(new[] {
    //    Android.Hardware.Usb.UsbManager.ActionUsbDeviceAttached,
    //    Android.Hardware.Usb.UsbManager.ActionUsbDeviceDetached })]
    //[MetaData(Android.Hardware.Usb.UsbManager.ActionUsbDeviceAttached, Resource = "@xml/usb_device_filter")]
    //[MetaData(Android.Hardware.Usb.UsbManager.ActionUsbDeviceDetached, Resource = "@xml/usb_device_filter")]
    public class HidUsbReceiver : BroadcastReceiver
    {
        public const string ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";

        private UsbDeviceConnection mConnection;
        private UsbEndpoint mEndpointRead;
        private UsbEndpoint mEndpointWrite;

        public event Action<UsbDevice, bool> DeviceConnected;

        public HidUsbReceiver()
        {

        }

        public override void OnReceive(Context context, Intent intent)
        {
            var usbManager = (UsbManager)context.GetSystemService(Context.UsbService);
            if (intent.Action == ACTION_USB_PERMISSION)
            {
                lock (this)
                {
                    UsbDevice device = (UsbDevice)intent.GetParcelableExtra(UsbManager.ExtraDevice);
                    if (device != null)
                    {
                       
                        bool hasPermision = usbManager.HasPermission(device);
                        if (!hasPermision)
                        {
                            hasPermision = intent.GetBooleanExtra(UsbManager.ExtraPermissionGranted, false);
                        }
                        if (hasPermision)
                        {
                          //  HidConnector.Instance.RefreshState();
                            return;
                        }
                        HidDeviceLoader.permissionPending = null;

                    }
                }
            }
            if (intent.Action == UsbManager.ActionUsbDeviceAttached)
            {
               
                HidDeviceLoader.permissionPending = null;
                //HidConnector.Instance.RefreshState();
            }
            if (intent.Action == UsbManager.ActionUsbDeviceDetached)
            {
                HidDeviceLoader.permissionPending = null;
                //HidConnector.Instance.RefreshState();
            }
          
        }

        private void _RaiseDeviceConnected(UsbDevice device, bool connected)
        {
            if (DeviceConnected != null)
                DeviceConnected(device, connected);
        }

       
        private bool _SetHIDDevice(UsbDevice device, UsbManager manager)
        {
            if (device.InterfaceCount != 1)
                return false;

           var usbInterface = device.GetInterface(0);

           if (usbInterface.EndpointCount != 2)
                return false;


            mEndpointRead = usbInterface.GetEndpoint(0);
            mEndpointWrite = usbInterface.GetEndpoint(1);

            //check that we should be able to read and write
             UsbDeviceConnection connection = manager.OpenDevice(device);
             if (connection != null && connection.ClaimInterface(usbInterface, true))
             {
                 mConnection = connection;
                return true;
             }

            mConnection = null;
            return false;
        }

      
    }
}

