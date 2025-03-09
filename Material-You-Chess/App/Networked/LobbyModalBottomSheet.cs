using Android.Views;
using AndroidX.Activity;
using Chess.App.Nearby;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.ProgressIndicator;

namespace Chess.App.Networked;

public class LobbyModalBottomSheet(NetworkedChessActivity activity) : BottomSheetDialogFragment()
{
    public new const string Tag = nameof(LobbyModalBottomSheet);
    public TextView? SearchingText { get; set; }
    public CircularProgressIndicator? SearchingIndicator { get; set; }
    public ChipGroup? MatchmakingPreferences { get; set; }
    public Chip? White { get; set; }
    public Chip? Black { get; set; }


    public void Show() => this.Show(activity.SupportFragmentManager, Tag);
    public void Show(AndroidX.Fragment.App.FragmentManager fragmentManager) => this.Show(fragmentManager, Tag);

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.select_device_dialog, container, false);

    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        var baseDialog = base.OnCreateDialog(savedInstanceState);
        if (baseDialog is not BottomSheetDialog dialog)
            return baseDialog;

        dialog.Behavior.State = BottomSheetBehavior.StateExpanded;
        dialog.Behavior.Hideable = false;
        var back = new BackCallback(this);
        var callback = new Callback(this, back);
        dialog.Behavior.AddBottomSheetCallback(callback);
        dialog.OnBackPressedDispatcher.AddCallback(back);
        dialog.ShowEvent += this.Show;
        return dialog;
    }

    public void Show(object? s, EventArgs e)
    {
        this.SearchingIndicator = this.Dialog?.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator);
        this.SearchingText = this.Dialog?.FindViewById<TextView>(Resource.Id.SearchingText);
        this.White = this.Dialog?.FindViewById<Chip>(Resource.Id.white_chip);
        this.Black = this.Dialog?.FindViewById<Chip>(Resource.Id.black_chip);
        this.MatchmakingPreferences = this.Dialog?.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref);
        this.MatchmakingPreferences!.CheckedChange += this.IsHost_CheckedChange;
    }

    public bool IsConnected() => activity.EstablishedConnections.Count > 0;

    private void IsHost_CheckedChange(object? sender, ChipGroup.CheckedChangeEventArgs e)
    {
        const string SelectedNone = "Please select a matchmaking preference";
        const string SelectedWhite = "Your device is now Advertising itself for other devices that discovering in your area";
        const string SelectedBlack = "Your device is now Discovering other devices that are advertising in your area";

        if (!this.White!.Checked && !this.Black!.Checked)
        {
            this.SearchingIndicator?.Hide();
            this.SearchingText!.Text = SelectedNone;
            activity.State = State.Idle;
        }

        if (this.White!.Checked)
        {
            this.SearchingIndicator?.Show();
            this.SearchingText!.Text = SelectedWhite;
            activity.State = State.Advertising;
        }

        if (this.Black!.Checked)
        {
            this.SearchingIndicator?.Show();
            this.SearchingText!.Text = SelectedBlack;
            activity.State = State.Discovering;
        }
    }
}

public class Callback(LobbyModalBottomSheet instance, BackCallback back) : BottomSheetBehavior.BottomSheetCallback()
{
    public override void OnSlide(View bottomSheet, float newState) { }

    public override void OnStateChanged(View bottomSheet, int newState)
    {
        Action<View> onState = newState switch
        {
            BottomSheetBehavior.StateExpanded => this.OnStateExpanded,
            BottomSheetBehavior.StateHalfExpanded => this.OnStateHalfExpanded,
            BottomSheetBehavior.StateCollapsed => this.OnStateCollapsed,
            BottomSheetBehavior.StateHidden => this.OnStateHidden,
            _ => this.OnStateSettling
        };
        onState(bottomSheet);
    }

    public void OnStateExpanded(View bottomSheet) { }

    public void OnStateHalfExpanded(View bottomSheet) { }

    public void OnStateCollapsed(View bottomSheet) { }

    public void OnStateHidden(View view)
    {
        (instance.Dialog as BottomSheetDialog)!.Behavior.State = BottomSheetBehavior.StateCollapsed;
    }

    public void OnStateSettling(View bottomSheet) { }
}

public class BackCallback(LobbyModalBottomSheet instance) : OnBackPressedCallback(enabled: true)
{
    public override void HandleOnBackPressed() => base.Enabled = !instance.IsConnected();

}