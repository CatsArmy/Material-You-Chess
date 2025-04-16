using Chess.App.Common;

namespace Chess.Game.Networked;
public class EndPoint(string id, string name)
{
    public readonly string Id = id;
    public readonly string Name = name;

    public EndPoint(EndPoint other) : this(other.Id, other.Name) => Logger.Debug($"Endpoint copied: {other}");
    public override string ToString() => $"{nameof(EndPoint)}{{id={this.Id}, name={this.Name}}}";
    public override int GetHashCode() => this.Id.GetHashCode();
    public override bool Equals(object? obj) => obj switch
    {
        EndPoint other => Id.Equals(other),
        _ => false
    };
}