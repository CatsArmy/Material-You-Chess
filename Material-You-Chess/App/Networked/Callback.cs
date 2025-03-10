using Android.Views;
using AndroidX.Activity;
using Google.Android.Material.BottomSheet;

namespace Chess.App.Networked;

public class Callback(LobbyModalBottomSheet instance) : BottomSheetBehavior.BottomSheetCallback()
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

public class BackCallback(LobbyModalBottomSheet instance) : OnBackPressedCallback(true)
{
    public override void HandleOnBackPressed() => base.Enabled = !instance.IsConnected();
}