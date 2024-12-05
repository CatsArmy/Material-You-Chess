using Android.Bluetooth;
using Android.Content;
using static Chess.BluetoothDeviceManager;

namespace Chess
{
    //[BroadcastReceiver]
    //public class BluetoothDiscoveryReceiver : BroadcastReceiver
    //{
    //    public List<BluetoothDevice> deivces;
    //    public BluetoothDiscoveryReceiver(ref List<BluetoothDevice> _devices) : base()
    //    {
    //        this.deivces = _devices;
    //    }

    //    public override void OnReceive(Context context, Intent intent)
    //    {
    //        string action = intent.Action;
    //        if (BluetoothDevice.ActionFound != action)
    //            return;

    //        BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

    //        Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
    //    }

    //}

    public class BluetoothDeviceManager : BluetoothDevicePicker
    {
        public BluetoothDeviceManager(Context context) : base()
        {
            this.context = context;
        }
        protected Context context;
        public void PickDevice(BluetoothDevicePickResultHandler handler)
        {
            context.RegisterReceiver(new BluetoothDeviceManagerReceiver(handler), new IntentFilter(BluetoothDevicePicker.ACTION_DEVICE_SELECTED));

            context.StartActivity(new Intent(BluetoothDevicePicker.ACTION_LAUNCH)
                    .PutExtra(BluetoothDevicePicker.EXTRA_NEED_AUTH, false)
                    .PutExtra(BluetoothDevicePicker.EXTRA_FILTER_TYPE, BluetoothDevicePicker.FILTER_TYPE_ALL)
                    .SetFlags(ActivityFlags.ExcludeFromRecents));
        }

        public interface BluetoothDevicePickResultHandler
        {
            public void OnDevicePicked(BluetoothDevice device);
        }
    }

    public class BluetoothDeviceManagerReceiver : BroadcastReceiver
    {

        private readonly BluetoothDevicePickResultHandler handler;

        public BluetoothDeviceManagerReceiver(BluetoothDevicePickResultHandler handler)
        {
            this.handler = handler;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            context.UnregisterReceiver(this);

            BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

            handler.OnDevicePicked(device);
        }
    }
    public interface BluetoothDevicePicker
    {
        public const string EXTRA_NEED_AUTH = "android.bluetooth.devicepicker.extra.NEED_AUTH";
        public const string EXTRA_FILTER_TYPE = "android.bluetooth.devicepicker.extra.FILTER_TYPE";
        public const string EXTRA_LAUNCH_PACKAGE = "android.bluetooth.devicepicker.extra.LAUNCH_PACKAGE";
        public const string EXTRA_LAUNCH_CLASS = "android.bluetooth.devicepicker.extra.DEVICE_PICKER_LAUNCH_CLASS";

        /**
         * Broadcast when one BT device is selected from BT device picker screen.
         * Selected {@link BluetoothDevice} is returned in extra data named
         * {@link BluetoothDevice#EXTRA_DEVICE}.
         */
        public const string ACTION_DEVICE_SELECTED = "android.bluetooth.devicepicker.action.DEVICE_SELECTED";

        /**
         * Broadcast when someone want to select one BT device from devices list.
         * This intent contains below extra data:
         * - {@link #EXTRA_NEED_AUTH} (boolean): if need authentication
         * - {@link #EXTRA_FILTER_TYPE} (int): what kinds of device should be
         * listed
         * - {@link #EXTRA_LAUNCH_PACKAGE} (string): where(which package) this
         * intent come from
         * - {@link #EXTRA_LAUNCH_CLASS} (string): where(which class) this intent
         * come from
         */
        public const string ACTION_LAUNCH = "android.bluetooth.devicepicker.action.LAUNCH";

        /**
         * Ask device picker to show all kinds of BT devices
         */
        public const int FILTER_TYPE_ALL = 0;
        /**
         * Ask device picker to show BT devices that support AUDIO profiles
         */
        public const int FILTER_TYPE_AUDIO = 1;
        /**
         * Ask device picker to show BT devices that support Object Transfer
         */
        public const int FILTER_TYPE_TRANSFER = 2;
        /**
         * Ask device picker to show BT devices that support
         * Personal Area Networking User (PANU) profile
         */
        public const int FILTER_TYPE_PANU = 3;
        /**
         * Ask device picker to show BT devices that support Network Access Point (NAP) profile
         */
        public const int FILTER_TYPE_NAP = 4;
    }
}