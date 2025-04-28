using System.Text.Json.Serialization;

namespace Material.You.Chess.App.Common;

[method: JsonConstructor]
public record Client([property: JsonInclude] string Name);
