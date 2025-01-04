using Chess.ChessBoard;

namespace Chess.Game;

//If both chip White and chip Black are unselected the colors of each player will be determined by a coin flip
public interface IChessGame
{
    /*[IgnoreDataMember]*/
    public Dictionary<(string, int), IPiece> AllPieces { get; }
    /*[IgnoreDataMember]*/
    public Dictionary<(char, int), ISpace> Board { get; }
    /*[IgnoreDataMember]*/
    public List<IMove>? Moves { get; set; }
    /*[DataMember]*/
    public IPlayer? Player1 { get; set; }
    /*[DataMember]*/
    public IPlayer? Player2 { get; set; }
    /*[DataMember]*/
    public IMove? LastMove { get; set; }
    /*[DataMember]*/
    public IPiece? Selected { get; set; }
}
