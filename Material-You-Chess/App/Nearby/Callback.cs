using Android.Views;
using AndroidX.Activity;
using Chess.App.Networked;
using Google.Android.Material.BottomSheet;

namespace Chess.App.Nearby;

public class Callback(LobbyBottomSheet instance) : BottomSheetBehavior.BottomSheetCallback()
{
    public override void OnSlide(View bottomSheet, float newState) { }

    public override void OnStateChanged(View bottomSheet, int newState)
    {
        Action<View> onState = newState switch
        {
            BottomSheetBehavior.StateExpanded => OnStateExpanded,
            BottomSheetBehavior.StateHalfExpanded => OnStateHalfExpanded,
            BottomSheetBehavior.StateCollapsed => OnStateCollapsed,
            BottomSheetBehavior.StateHidden => OnStateHidden,
            _ => OnStateSettling
        };
        onState(bottomSheet);
    }

    public virtual void OnStateExpanded(View bottomSheet) { return; }

    public virtual void OnStateHalfExpanded(View bottomSheet) { return; }

    public virtual void OnStateCollapsed(View bottomSheet) { return; }

    public virtual void OnStateHidden(View view)
    {
        instance.BottomSheet!.State = BottomSheetBehavior.StateCollapsed;
    }

    public virtual void OnStateSettling(View bottomSheet) { }
}

public class BackCallback(LobbyBottomSheet instance) : OnBackPressedCallback(true)
{
    public override void HandleOnBackPressed() => Enabled = !instance.IsConnected;
}