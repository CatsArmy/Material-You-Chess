namespace Chess;


public interface IBluetoothDevicePicker
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