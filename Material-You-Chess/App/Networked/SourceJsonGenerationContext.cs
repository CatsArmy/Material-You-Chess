using System.Text.Json.Serialization;
using Chess.Game.Board;
using Chess.Game.Moves;

namespace Chess.App.Networked;

[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(FirebaseUserClient))]
[JsonSourceGenerationOptions(IncludeFields = true)]
internal partial class SourceJsonGenerationContext : JsonSerializerContext;
