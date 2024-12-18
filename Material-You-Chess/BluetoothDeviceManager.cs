using System;
using Android.Bluetooth;
using Android.Content;

namespace Chess.Util;

public class BluetoothDeviceManager : IBluetoothDevicePicker
{
    public BluetoothDeviceManager(Context context) : base()
    {
        this.context = context;
    }
    protected Context context;
    public void PickDevice(Action<BluetoothDevice> handler)
    {
        context.RegisterReceiver(new BluetoothDeviceReceiver(handler), new IntentFilter(IBluetoothDevicePicker.ACTION_DEVICE_SELECTED));

        context.StartActivity(new Intent(IBluetoothDevicePicker.ACTION_LAUNCH)
                .PutExtra(IBluetoothDevicePicker.EXTRA_NEED_AUTH, false)
                .PutExtra(IBluetoothDevicePicker.EXTRA_FILTER_TYPE, IBluetoothDevicePicker.FILTER_TYPE_ALL)
                .SetFlags(ActivityFlags.ExcludeFromRecents));
    }
}


//public class BluetoothDiscovery
//{
//    // event will be fired on discovery completed
//    public event EventHandler<List<BluetoothDevice>> OnDiscoveryFinished;
//    // AuthReset event which signals the end of the discovery process
//    public static AutoResetEvent bluetoothDiscoveryEndEvent = new AutoResetEvent(false);
//    // resulting list of bluetooth devices
//    public static List<BluetoothDevice> bluetoothDevices = new List<BluetoothDevice>();
//    // Bluetooth reciever
//    public static BluetoothReceiver bluetoothReceiver = new BluetoothReceiver();

//    protected Context context;

//    /// <summary>
//    /// BluetoothReceiver which get bluetooth discovery events
//    /// </summary>
//    [BroadcastReceiver]
//    public class BluetoothReceiver : BroadcastReceiver
//    {
//        public event EventHandler<List<BluetoothDevice>> OnDiscoveryFinished;

//        public override void OnReceive(Context context, Intent intent)
//        {
//            if (intent.Action != null && intent.Action == BluetoothDevice.ActionFound)
//            {
//                // Device discovered
//                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
//                bluetoothDevices.Add(device);
//            }
//            else if (intent.Action != null && intent.Action == BluetoothAdapter.ActionDiscoveryStarted)
//            {
//                //discovery starts
//            }
//            else if (intent.Action != null && intent.Action == BluetoothAdapter.ActionDiscoveryFinished)
//            {
//                //discovery finishes
//                bluetoothDiscoveryEndEvent.Set();
//            }
//        }
//    }

//    /// <summary>
//    /// Starts bluetooth discovery.
//    /// Results are delivered using OnDiscoveryFinished event.
//    /// </summary>
//    public bool StartBluetoothDiscovery(Context context)
//    {
//        this.context = context;
//        BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
//        if (adapter.State == Android.Bluetooth.State.Off)
//            return false;
//        IntentFilter filter = new IntentFilter();
//        filter.AddAction(BluetoothDevice.ActionFound);
//        filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
//        filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
//        bluetoothReceiver = new BluetoothReceiver();
//        bluetoothReceiver.OnDiscoveryFinished += DiscoveryFinished;
//        context.RegisterReceiver(bluetoothReceiver, filter);
//        adapter.StartDiscovery();
//        return true;
//    }

//    /// <summary>
//    /// Method called by Bluetooth Receiver when discovery finishes
//    /// </summary>
//    /// <param name="sender"></param>
//    /// <param name="e"></param>
//    private void DiscoveryFinished(object sender, List<BluetoothDevice> e)
//    {
//        context.UnregisterReceiver(bluetoothReceiver);
//        bluetoothReceiver = null;

//        if (OnDiscoveryFinished != null)
//            OnDiscoveryFinished(this, e);
//    }

//    public static async Task<List<BluetoothDevice>> GetAvailableBluetoothDevicesSync(Context context)
//    {
//        try
//        {
//            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
//            if (adapter.State == Android.Bluetooth.State.Off)
//            {
//                return null;
//            }
//            IntentFilter filter = new IntentFilter();
//            filter.AddAction(BluetoothDevice.ActionFound);
//            filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
//            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
//            bluetoothReceiver = new BluetoothReceiver();
//            bluetoothDiscoveryEndEvent = new AutoResetEvent(false);
//            context.RegisterReceiver(bluetoothReceiver, filter);
//            bluetoothDevices = new List<BluetoothDevice>();
//            adapter.StartDiscovery();
//            await Task.Run(() => { bluetoothDiscoveryEndEvent.WaitOne(); });
//            return bluetoothDevices;
//        }
//        finally
//        {
//            if (bluetoothReceiver != null)
//            {
//                context.UnregisterReceiver(bluetoothReceiver);
//                bluetoothReceiver = null;
//            }
//        }
//        return null;
//    }
//}