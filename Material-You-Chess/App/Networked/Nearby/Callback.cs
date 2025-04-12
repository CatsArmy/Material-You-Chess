using Android.Views;
using Chess.Game.Common;
using Google.Android.Material.BottomSheet;
using static Chess.Game.Common.ChessBottomSheet;

namespace Chess.App.Networked.Nearby;

public class BottomSheetCallback(ChessBottomSheet? instance) : BottomSheetBehavior.BottomSheetCallback()
{
    public BottomSheetCallback(IChessActivity instance) : this(instance.BottomSheet) { }

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
    /// Check if we should or shouldnt change the bottom sheet visibility state to be our visibility state <see cref="ToState"/>
    /// only when we have one
    /// </summary>
    public override void OnStateChanged(View bottomSheet, int newState)
    {
        if (this.OnState == null) return;
        var state = (VisibilityState)newState;

        if (state is VisibilityState.Dragging)
        {
            this.OnStateDragging(bottomSheet);
            return;
        }

        else if (state is VisibilityState.Settling)
        {
            this.OnStateSettling(bottomSheet);
            return;
        }

        if (state == this.OnState)
        {
            this.ChangeState();
        }

        else if (this.OnMoreVisible)
        {
            if (this.OnState is VisibilityState.HalfExpanded) //HalfExpanded(6) is more visible than Collapsed(4), Hidden(5)
            {
                if (state is VisibilityState.Expanded) //Expanded is the only state that is more visible than HalfExpanded(6)
                {
                    this.ChangeState();
                }
            }
            else if (state < this.OnState) //the lower the int value of the state the more visible it is
            {
                this.ChangeState();
            }
        }

        else if (this.OnLessVisible)
        {
            if (state > this.OnState) //the lower the int value of the state the more visible it is
            {
                this.ChangeState();
            }
            else if (this.OnState is VisibilityState.HalfExpanded) //HalfExpanded(6) is less visible than Expanded(3)
            {
                this.ChangeState();
            }
        }
    }

    private void ChangeState()
    {
        if (this.ToState is VisibilityState toState)
        {
            instance!.Behavior!.State = (int)toState;
        }
    }
}
