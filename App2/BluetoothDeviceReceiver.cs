using System;
using Android.Bluetooth;
using Android.Content;

namespace Chess
{
    public class BluetoothDeviceReceiver : BroadcastReceiver
    {

        private readonly Action<BluetoothDevice> handler;

        public BluetoothDeviceReceiver(Action<BluetoothDevice> handler)
        {
            this.handler = handler;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            context.UnregisterReceiver(this);

            BluetoothDevice device = intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;

            handler(device);
        }
    }
}