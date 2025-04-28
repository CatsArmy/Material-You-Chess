using System.Text.Json.Serialization;
using Material.You.Chess.App.Common;

namespace Material.You.Chess.Game.Networked;

[method: JsonConstructor]
public record Player([property: JsonInclude] User FirebaseUser, [property: JsonInclude] bool IsWhite);