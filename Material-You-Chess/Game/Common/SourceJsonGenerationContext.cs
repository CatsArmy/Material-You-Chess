using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.Game.Common;

[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(PlayerClient))]
[JsonSourceGenerationOptions(IncludeFields = true)]
internal partial class SourceJsonGenerationContext : JsonSerializerContext;
