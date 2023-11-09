using System.Diagnostics;

namespace SpaceAlertSolver;

public readonly struct Position : IEquatable<Position>
{
    public readonly byte PositionIndex;
    public readonly byte Zone;

    public static readonly Position TopLeft = new(0);
    public static readonly Position TopMiddle = new(1);
    public static readonly Position TopRight = new(2);
    public static readonly Position BottomLeft = new(3);
    public static readonly Position BottomMiddle = new(4);
    public static readonly Position BottomRight = new(5);
    public static readonly Position Space = new(6);

    internal Position(int index)
    {
        PositionIndex = (byte)index;
        if (index >= 6)
            Zone = 3;
        else
            Zone = (byte)(index % 3);
    }

    public bool Equals(Position other) => PositionIndex == other.PositionIndex;

    public override bool Equals(object? obj) => obj is Position pos && Equals(pos);

    public override int GetHashCode() => PositionIndex.GetHashCode();

    public static bool operator ==(Position a, Position b) => a.Equals(b);

    public static bool operator !=(Position a, Position b) => !a.Equals(b);

    public static Position operator +(Position a, int v)
    {
        int newIndex = a.PositionIndex + v;
        Debug.Assert(newIndex >= 0 && newIndex <= 6, "Operation results in invalid position");
        return new(newIndex);
    }

    public static Position operator -(Position a, int v) => a + (-v);

    public static Position GetTop(int zone) => new(zone);

    public static Position GetBottom(int zone) => new(zone + 3);

    public Position GetLeft()
    {
        if (Zone == 0 || Zone == 3)
            return this;

        return this - 1;
    }

    public Position GetRight()
    {
        if (Zone >= 2)
            return this;

        return this + 1;
    }

    public Position GetElevator()
    {
        if (Zone == 3)
            return this;

        if (PositionIndex < 3)
            return new(PositionIndex + 3);

        return new(PositionIndex - 3);
    }

    public bool IsInShip() => PositionIndex < 6;

    public bool IsTop() => PositionIndex < 3;

    public bool IsBottom() => PositionIndex >= 3 && PositionIndex < 6;

    public bool IsLeft() => Zone == 0;

    public bool IsMiddle() => Zone == 1;

    public bool IsRight() => Zone == 2;
}
