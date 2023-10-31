using System.Diagnostics;

namespace SpaceAlertSolver;

public static class GamePool
{
    private static readonly List<Game> _freeGames = new();

    public static void Clear()
    {
        _freeGames.Clear();
    }

    public static Game GetGame()
    {
        if (_freeGames.Count <= 0)
            return new();

        Game game = _freeGames[^1];
        _freeGames.RemoveAt(_freeGames.Count - 1);
        return game;
    }

    public static void FreeGame(Game game)
    {
        Debug.Assert(!_freeGames.Contains(game), "Game must not already be freed");
        _freeGames.Add(game);
    }
}
