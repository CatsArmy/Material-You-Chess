using System.Runtime.Serialization;

namespace Chess.ChessBoard;

public interface ICapture : IMove
{
    [DataMember] public IPiece Piece { get; }
}