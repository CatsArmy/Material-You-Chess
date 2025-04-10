using Android.Views;
using Google.Android.Material.BottomSheet;

namespace Chess.App.Networked.Nearby;

public class BottomSheetCallback(IChessActivity instance) : BottomSheetBehavior.BottomSheetCallback()
{
    public override void OnSlide(View bottomSheet, float newState) { }

    public override void OnStateChanged(View bottomSheet, int newState)
    {
        switch (newState)
        {
            case BottomSheetBehavior.StateExpanded:
                OnStateExpanded(bottomSheet);
                break;

            case BottomSheetBehavior.StateHalfExpanded:
                OnStateHalfExpanded(bottomSheet);
                break;

            case BottomSheetBehavior.StateCollapsed:
                OnStateCollapsed(bottomSheet);
                break;

            case BottomSheetBehavior.StateHidden:
                OnStateHidden(bottomSheet);
                break;

            default:
                OnStateSettling(bottomSheet);
                break;
        }
    }

    public virtual void OnStateExpanded(View bottomSheet) { return; }

    public virtual void OnStateHalfExpanded(View bottomSheet) { return; }

    public virtual void OnStateCollapsed(View bottomSheet) { return; }

    public virtual void OnStateHidden(View view) => instance.BottomSheet!.State = BottomSheetBehavior.StateCollapsed;

    public virtual void OnStateSettling(View bottomSheet) { }
}