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
using System.Threading;
using Android.Hardware.Usb;

namespace HidSharp
{
    public sealed class HidStream : IDisposable
    {
        private readonly object locker = new object();
        private UsbDevice m_Device;
        private UsbInterface m_UsbInterface;
        private UsbEndpoint m_EndPointRead;
        private UsbEndpoint m_EndPointWrite;
        private UsbDeviceConnection m_Connection;
       // private Handler m_UiHandler = new Handler();


        private System.Collections.Concurrent.ConcurrentStack<byte> m_ReceivedBytes = new System.Collections.Concurrent.ConcurrentStack<byte>();

        public HidStream(UsbDevice device)
        {
            m_Device = device;

            var usbManager = (UsbManager)Application.Context.GetSystemService(Context.UsbService);
            m_Connection = usbManager.OpenDevice(m_Device);
            m_UsbInterface = m_Device.GetInterface(0);
            if (m_Connection == null)
            {

                //Unable to establish connection
            }

            for (var i = 0; i < m_UsbInterface.EndpointCount; i++)
            {
                var endpoint = m_UsbInterface.GetEndpoint(i);
                if (endpoint.Direction == UsbAddressing.Out)
                {
                    m_EndPointWrite = endpoint;
                    MaxOutputReportLength = endpoint.MaxPacketSize + 1;
                }
                if (endpoint.Direction == UsbAddressing.In)
                {
                    m_EndPointRead = endpoint;
                    MaxInputReportLength = endpoint.MaxPacketSize + 1;
                }
            }
            if (m_EndPointRead == null)
                System.Diagnostics.Debug.WriteLine("Unable to get endpoint for reading");
            if (m_EndPointWrite == null)
                System.Diagnostics.Debug.WriteLine("Unable to get endpoint for writing");
        }

        public int MaxOutputReportLength { get; internal set; }

        public int MaxInputReportLength { get; internal set; }


        public HidStream Open()
        {
            if(!m_Connection.ClaimInterface(m_UsbInterface, true))
            {
                throw new Exception("Can't lock interface");
            }
            return this;
        }
        public void Write(byte[] buffer)
        {
            lock (locker)
            {

                var bytes = new byte[buffer.Length - 1];
                Buffer.BlockCopy(buffer, 1, bytes, 0, bytes.Length);
                int status = m_Connection.BulkTransfer(m_EndPointWrite, bytes, bytes.Length, 250);
            }
        }
        public void Read(byte[] value)
        {
            var data = new byte[value.Length - 1];

            m_Connection.BulkTransfer(m_EndPointRead, data, data.Length, 1000);

            Buffer.BlockCopy(data, 0, value, 1, data.Length);
        }

        public void Dispose()
        {
            if (m_Connection != null)
            {
                m_Connection.ReleaseInterface(m_UsbInterface);
                m_Connection.Close();
                m_Connection.Dispose();
                m_Connection = null;
            }
           
        }

     

       
    }

  
}

