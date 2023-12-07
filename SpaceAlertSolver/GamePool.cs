using System.Collections.Concurrent;
using System.Diagnostics;

namespace SpaceAlertSolver;

public static class GamePool
{
    private static readonly ConcurrentBag<Game> _freeGamesConcurrent = new();
    private static readonly List<Game> _freeGames = new();
    public static bool EnableConcurrent = false;

    public static void Clear()
    {
        _freeGamesConcurrent.Clear();
        _freeGames.Clear();
    }

    public static Game GetGame()
    {
        if (EnableConcurrent)
        {
            if (_freeGamesConcurrent.TryTake(out Game? game))
                return game;

            return new Game();
        }
        else
        {
            if (_freeGames.Count == 0)
                return new();

            Game game = _freeGames[^1];
            _freeGames.RemoveAt(_freeGames.Count - 1);
            return game;
        }
    }

    public static void FreeGame(Game game)
    {
        if (EnableConcurrent)
        {
            Debug.Assert(!_freeGamesConcurrent.Contains(game), "Game must not already be freed");
            _freeGamesConcurrent.Add(game);
        }
        {
            Debug.Assert(!_freeGames.Contains(game), "Game must not already be freed");
            _freeGames.Add(game);
        }
    }
}
