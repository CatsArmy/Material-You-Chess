using Android.Views;
using Chess.App;
using Chess.Game.Interfaces;
using Chess.Game.Player;
using Google.Android.Material.BottomSheet;

namespace Chess.Game.Common;

public class ChessBottomSheet(IChessActivity activity)
{
    public void Show(IPlayer winner, IPlayer loser)
    {
        activity.GameOverLayout!.Visibility = ViewStates.Visible;
        activity.StandardBottomSheet!.Visibility = ViewStates.Visible;
        activity.MatchmakingLayout!.Visibility = ViewStates.Gone;
        activity.BottomSheet!.AddBottomSheetCallback(activity.Callback!);
        activity.BottomSheet!.State = BottomSheetBehavior.StateExpanded;

        activity.WinnerUsername!.Text = winner.Name;
        activity.WinnerDescription!.Text = $"{winner.Name} Wins, {loser.Name} loses";
        switch (winner)
        {
            case Black:
                activity.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                activity.Indicator?.SetImageResource(Resource.Drawable.king_black);
                break;
            default:
                activity.WinningPlayer?.SetImageDrawable(activity.WhitePlayerProfilePicture?.Drawable);
                break;
        }

        activity.Home!.Click += (_, _) => activity.Finish();
    }

    public void Show()
    {
        activity.GameOverLayout!.Visibility = ViewStates.Visible;
        activity.StandardBottomSheet!.Visibility = ViewStates.Visible;
        activity.MatchmakingLayout!.Visibility = ViewStates.Gone;
        activity.BottomSheet!.AddBottomSheetCallback(activity.Callback!);
        activity.BottomSheet!.State = BottomSheetBehavior.StateExpanded;

        activity.WinnerUsername!.Text = activity.ConnectedClient.Username;
        activity.WinnerDescription!.Text = $"{activity.ConnectedClient.Username} disconnected, " +
            $"{activity.Client.Username} wins by technicality";
        switch (activity.Game.ClientIsWhite) // Inverse because our client did not disconnect
        {
            case true: //our client is white and the connect client is black
                activity.WinningPlayer?.SetImageDrawable(activity.BlackPlayerProfilePicture?.Drawable);
                activity.Indicator?.SetImageResource(Resource.Drawable.king_black);
                break;
            default: //our client is black and the connect client is white
                activity.WinningPlayer?.SetImageDrawable(activity.WhitePlayerProfilePicture?.Drawable);
                break;
        }

        activity.Home!.Click += (_, _) => activity.Finish();
    }
}
