using System.Text.Json.Serialization;
using Material.You.Chess.App.Common;

namespace Material.You.Chess.Game.Player;

[method: JsonConstructor]
public record Player([property: JsonInclude] Client User, [property: JsonInclude] bool IsWhite);
