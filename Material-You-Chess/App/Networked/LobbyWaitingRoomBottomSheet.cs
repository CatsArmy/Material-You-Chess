using Android.Views;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.MaterialSwitch;

namespace Chess.App.Networked;

public class LobbyWaitingRoomBottomSheet(NetworkedChessActivity activity) : BottomSheetDialogFragment()
{
    public TextView? SearchingText { get; set; }
    public View? SearchingIndicator { get; set; }
    public MaterialSwitch? IsHost { get; set; }
    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        base.OnCreateDialog(savedInstanceState);
        var bottomSheetDialog = new BottomSheetDialog(this.Context!);
        bottomSheetDialog.SetContentView(Resource.Layout.select_device_dialog);
        bottomSheetDialog.Behavior.Draggable = false;
        bottomSheetDialog.Behavior.Hideable = false;
        bottomSheetDialog.Behavior.State = BottomSheetBehavior.StateExpanded;
        bottomSheetDialog.ShowEvent += this.Show;
        return bottomSheetDialog;
    }

    public override void Show(AndroidX.Fragment.App.FragmentManager manager, string? tag)
    {
        base.Show(manager, tag);
        this.Dialog?.Show();
    }

    public void Show(object? s, EventArgs e)
    {
        this.SearchingText = this.Dialog?.FindViewById<TextView>(Resource.Id.bsSearching);
        //this.SearchingIndicator = this.Dialog?.FindViewById(Resource.Id.mliSearching);
        this.IsHost = this.Dialog?.FindViewById<MaterialSwitch>(Resource.Id.msIsHost);
        this.IsHost!.CheckedChange += this.IsHost_CheckedChange;
        this.IsHost!.CheckedChange += activity.IsHost_CheckedChange;
    }

    private void IsHost_CheckedChange(object? sender, CompoundButton.CheckedChangeEventArgs e)
    {
        this.SearchingText!.Text = e.IsChecked switch
        {
            true => "Searching for players",
            false => "Waiting for players"
        };
    }
}