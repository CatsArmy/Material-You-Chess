namespace Chess.Game;

/*[DataContract]*/
public enum GameOutcome
{
    /*[DataMember]*/
    Draw,
    /*[DataMember]*/
    Resign,
    /*[DataMember]*/
    Win,
    /*[DataMember]*/
    Lose,
    /*[DataMember]*/
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