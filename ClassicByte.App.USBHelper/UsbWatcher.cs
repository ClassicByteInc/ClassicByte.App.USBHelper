using System;
using System.Management;

public class UsbWatcher
{
    private static readonly string[] QueryStrings =
    {
        "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 4",
        "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 5"
    };

    public delegate void UsbEventHandler(object sender, UsbEventArgs e);

    public event UsbEventHandler DeviceArrival;
    public event UsbEventHandler DeviceDisconnection;

    private ManagementEventWatcher _deviceEventWatcher;

    public void StartMonitoring()
    {
        _deviceEventWatcher = new ManagementEventWatcher();
        foreach (string queryString in QueryStrings)
        {
            _deviceEventWatcher.Query = new EventQuery(queryString);
            _deviceEventWatcher.EventArrived += DeviceEventWatcher_EventArrived;
        }
        _deviceEventWatcher.Start();
    }

    public void StopMonitoring()
    {
        if (_deviceEventWatcher != null)
        {
            _deviceEventWatcher.Stop();
            _deviceEventWatcher.Dispose();
            _deviceEventWatcher = null;
        }
    }

    private void DeviceEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
    {
        if (e.Context == null)
        {
            return;
        }
        ManagementBaseObject eventData = e.NewEvent;
        uint eventType = (uint)eventData["EventType"];
        if (eventType == 4)
        {
            DeviceArrival?.Invoke(this, new UsbEventArgs());
        }
        else if (eventType == 5)
        {
            DeviceDisconnection?.Invoke(this, new UsbEventArgs());
        }
    }

    public class UsbEventArgs : EventArgs
    {
    }
}
