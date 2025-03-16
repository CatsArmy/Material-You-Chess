using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.CoordinatorLayout.Widget;
using Chess.App.Nearby;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Chip;
using Google.Android.Material.ProgressIndicator;

namespace Chess.App.Networked;

public abstract class LobbyBottomSheet : ConnectionsActivity
{
    public bool IsConnected => this.EstablishedConnections.Count > 0;

    public BottomSheetBehavior? BottomSheet { get; set; }
    public CoordinatorLayout? StandardBottomSheet { get; set; }
    public ConstraintLayout? BottomSheetLayout { get; set; }

    public TextView? SearchingText { get; set; }
    public CircularProgressIndicator? SearchingIndicator { get; set; }
    public ChipGroup? MatchmakingPreferences { get; set; }
    public Chip? White { get; set; }
    public Chip? Black { get; set; }

    public virtual void OnSelectNone()
    {
        this.SearchingIndicator?.Hide();
        this.SearchingText!.Text = "Please select a matchmaking preference";
    }

    public virtual void OnSelectWhite()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Advertising itself for other devices that discovering in your area";
    }

    public virtual void OnSelectBlack()
    {
        this.SearchingIndicator?.Show();
        this.SearchingText!.Text = "Your device is now Discovering other devices that are advertising in your area";
    }

    public void OnCreate()
    {
        this.StandardBottomSheet = base.FindViewById<CoordinatorLayout>(Resource.Id.standard_bottom_sheet);
        this.BottomSheetLayout = base.FindViewById<ConstraintLayout>(Resource.Id.bottom_sheet);
        this.BottomSheet = BottomSheetBehavior.From(this.BottomSheetLayout!);
        this.BottomSheet!.AddBottomSheetCallback(new Callback(this));
        this.BottomSheet!.State = BottomSheetBehavior.StateHalfExpanded;
        this.StandardBottomSheet!.Visibility = ViewStates.Visible;

        this.SearchingIndicator = base.FindViewById<CircularProgressIndicator>(Resource.Id.SearchingIndicator);
        this.SearchingText = base.FindViewById<TextView>(Resource.Id.SearchingText);
        this.White = base.FindViewById<Chip>(Resource.Id.white_chip);
        this.Black = base.FindViewById<Chip>(Resource.Id.black_chip);
        this.MatchmakingPreferences = base.FindViewById<ChipGroup>(Resource.Id.matchmaking_pref);
        this.MatchmakingPreferences!.CheckedChange += (_, _) =>
        {
            if (!this.White!.Checked && !this.Black!.Checked)
            {
                this.OnSelectNone();
            }

            if (this.White!.Checked)
            {
                this.OnSelectWhite();
            }

            if (this.Black!.Checked)
            {
                this.OnSelectBlack();
            }
        };
    }
}
