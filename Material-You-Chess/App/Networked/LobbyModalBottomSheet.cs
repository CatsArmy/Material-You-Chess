using Android.Views;
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
    public bool IsConnected() => activity.EstablishedConnections.Count > 0;

    public override View? OnCreateView(LayoutInflater inflater, ViewGroup? container, Bundle? savedInstanceState)
        => inflater.Inflate(Resource.Layout.select_device_dialog, container, false);

    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        var @base = base.OnCreateDialog(savedInstanceState);
        if (@base is not BottomSheetDialog dialog)
            return @base;

        dialog.Behavior.State = BottomSheetBehavior.StateExpanded;
        dialog.Behavior.Hideable = false;
        dialog.Behavior.AddBottomSheetCallback(new Callback(this));
        dialog.OnBackPressedDispatcher.AddCallback(new BackCallback(this));
        return dialog;
    }

    public override void OnViewCreated(View view, Bundle? savedInstanceState)
    {
        base.OnViewCreated(view, savedInstanceState);
        this.SearchingIndicator = view.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator);
        this.SearchingText = view.FindViewById<TextView>(Resource.Id.SearchingText);
        this.White = view.FindViewById<Chip>(Resource.Id.white_chip);
        this.Black = view.FindViewById<Chip>(Resource.Id.black_chip);
        this.MatchmakingPreferences = view.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref);
        this.MatchmakingPreferences!.CheckedChange += (_, _) =>
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
        };
    }
}
