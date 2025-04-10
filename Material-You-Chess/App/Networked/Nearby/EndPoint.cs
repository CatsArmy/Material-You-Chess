using Chess.App.Common;

namespace Chess.App.Networked.Nearby;

public class EndPoint
{
    public readonly string Id;
    public readonly string Name;

    public EndPoint(string id, string name)
    {
        this.Id = id;
        this.Name = name;
        Logger.Debug($"Endpoint created: {this}");
    }

    public EndPoint(EndPoint other)
    {
        this.Id = other.Id;
        this.Name = other.Name;
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is EndPoint other)
            return this.Id.Equals(other);

        return false;
    }

    public override string ToString()
    {
        return $"{nameof(EndPoint)}{{id={this.Id}, name={this.Name}}}";
    }
}