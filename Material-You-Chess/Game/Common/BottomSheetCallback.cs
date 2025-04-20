using Android.Views;
using Google.Android.Material.BottomSheet;

namespace Material.You.Chess.Game.Common;

public class BottomSheetCallback(ChessActivity instance) : BottomSheetBehavior.BottomSheetCallback()
{
    public VisibilityState? ToState = VisibilityState.Collapsed;
    public VisibilityState? OnState = VisibilityState.Hidden;

    public bool OnLessVisible = true;
    public bool OnMoreVisible = false;

    /// <summary> overridable method for if i ever need to use it</summary>
    public virtual void OnStateSettling(View bottomSheet) { }

    /// <summary> overridable method for if i ever need to use it</summary>
    public virtual void OnStateDragging(View bottomSheet) { }

    /// <summary> override the method as it is a part of the callback class we inherit </summary>
    public override void OnSlide(View bottomSheet, float newState) { }

    /// <summary>
    /// Check if we should or shouldn't change the bottom sheet visibility state to be our visibility state
    /// this.ToState only when we have one (not null)
    /// </summary>
    public override void OnStateChanged(View bottomSheet, int newState)
    {
        if (OnState == null) return;
        var state = (VisibilityState)newState;

        if (state is VisibilityState.Dragging)
        {
            OnStateDragging(bottomSheet);
            return;
        }

        else if (state is VisibilityState.Settling)
        {
            OnStateSettling(bottomSheet);
            return;
        }

        if (state == OnState)
        {
            ChangeState();
        }

        else if (OnMoreVisible)
        {
            if (OnState is VisibilityState.HalfExpanded) //HalfExpanded(6) is more visible than Collapsed(4), Hidden(5)
            {
                if (state is VisibilityState.Expanded) //Expanded is the only state that is more visible than HalfExpanded(6)
                {
                    ChangeState();
                }
            }
            else if (state < OnState) //the lower the int value of the state the more visible it is
            {
                ChangeState();
            }
        }

        else if (OnLessVisible)
        {
            if (state > OnState) //the lower the int value of the state the more visible it is
            {
                ChangeState();
            }
            else if (OnState is VisibilityState.HalfExpanded) //HalfExpanded(6) is less visible than Expanded(3)
            {
                ChangeState();
            }
        }
    }

    private void ChangeState()
    {
        if (ToState is VisibilityState toState)
        {
            instance.BottomSheet!.Behavior!.State = (int)toState;
        }
    }
}
