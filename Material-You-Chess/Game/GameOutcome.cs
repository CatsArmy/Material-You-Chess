namespace Chess.Game;


public enum GameOutcome
{
    
    Draw,
    
    Resign,
    
    Win,
    
    Lose,
    
    Ongoing,
}

//public interface INetworkedPlayer : IPlayer
//{
//    public bool HasColorPreference { get; set; }
//}
//public class ChessGameManager
//{
//    public ChessGameManager(IChessGame game)
//    {
//        this.Game = game;
//    }

//    public IChessGame Game;
//}

//public class LocalGame : ILocalGame
//{

//}

//public class OnlineGame : IOnlineGame
//{

//}