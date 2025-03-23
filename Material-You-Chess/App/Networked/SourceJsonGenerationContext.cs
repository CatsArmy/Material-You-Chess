using System.Text.Json.Serialization;
using Chess.App.Common;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.App.Networked;

[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(UserClient))]
[JsonSerializable(typeof(SerializedType))]
#if DEBUG
[JsonSourceGenerationOptions(IncludeFields = true, WriteIndented = true)]
#else
[JsonSourceGenerationOptions(IncludeFields = true)]
#endif
internal partial class SourceJsonGenerationContext : JsonSerializerContext;
