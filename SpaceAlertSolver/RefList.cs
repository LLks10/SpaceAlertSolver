using System.Diagnostics;

namespace SpaceAlertSolver;

/// <summary>
/// Implementation of List that can return by ref
/// </summary>
internal class RefList<T>
{
    private const int INITIAL_CAPACITY = 8;

    private int _count = 0;
    private T[] _data = new T[INITIAL_CAPACITY];

    public int Count => _count;

    public ref T this[int index] => ref _data[index];

    public RefList() { }

    public void Add(in T item)
    {
        if (_data.Length == _count)
            GrowTo(_data.Length << 1);
        _data[_count] = item;
        _count++;
    }

    public void AddRange(RefList<T> range)
    {
        int newCount = _count + range._count;
        if (newCount >= _data.Length)
        {
            while (newCount >= _data.Length)
                newCount <<= 1;
            GrowTo(newCount);
        }
        range._data.CopyTo(_data, _count);
        _count = newCount;
    }

    private void GrowTo(int size)
    {
        T[] newData = new T[size];
        _data.CopyTo(newData, 0);
        _data = newData;
    }

    public void RemoveAt(int index)
    {
        for (int i = index + 1; i < _count; i++)
        {
            _data[i - 1] = _data[i];
        }
        _count--;
    }

    public void Clear()
    {
        _count = 0;
    }
}
