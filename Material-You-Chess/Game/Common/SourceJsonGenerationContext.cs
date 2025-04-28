using System.Text.Json.Serialization;
using Material.You.Chess.Game.Board;
using Material.You.Chess.Game.Moves;

namespace Material.You.Chess.Game.Common;

[JsonSerializable(typeof(Move))]
[JsonSerializable(typeof(BoardSpace))]
[JsonSerializable(typeof(BoardPiece))]
[JsonSerializable(typeof(App.Common.User))]
[JsonSerializable(typeof(App.Common.Client))]
[JsonSerializable(typeof(Game.Player.Player), TypeInfoPropertyName = $"{nameof(Player)}")]
[JsonSerializable(typeof(Game.Networked.Player), TypeInfoPropertyName = $"{nameof(Networked)}{nameof(Player)}")]
[JsonSourceGenerationOptions(IncludeFields = true)]
internal partial class SourceJsonGenerationContext : JsonSerializerContext;
