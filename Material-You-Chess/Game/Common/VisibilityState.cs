using Google.Android.Material.BottomSheet;

namespace Material.You.Chess.Game.Common;

public enum VisibilityState
{
    Dragging = BottomSheetBehavior.StateDragging,
    Settling = BottomSheetBehavior.StateSettling,
    Expanded = BottomSheetBehavior.StateExpanded,
    HalfExpanded = BottomSheetBehavior.StateHalfExpanded,
    Collapsed = BottomSheetBehavior.StateCollapsed,
    Hidden = BottomSheetBehavior.StateHidden,
}
