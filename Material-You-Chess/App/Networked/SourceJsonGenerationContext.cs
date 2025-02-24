using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.App.Networked;

[JsonSerializable(typeof(SpecialPiece))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(SerializedType))]
[JsonSerializable(typeof(Bishop))]
[JsonSerializable(typeof(King))]
[JsonSerializable(typeof(Knight))]
[JsonSerializable(typeof(Pawn))]
[JsonSerializable(typeof(Queen))]
[JsonSerializable(typeof(Rook))]
[JsonSerializable(typeof(Move))]
[JsonSourceGenerationOptions(IncludeFields = true, WriteIndented = true, RespectNullableAnnotations = true, RespectRequiredConstructorParameters = false
    )]
internal partial class SourceJsonGenerationContext : JsonSerializerContext;
