using System.Collections.Immutable;
using System.Text.Json;

namespace SpaceAlertSolver.ResolverInterop;

internal class BatchResolver
{
	private List<ResolverModel> games = new();

	public void AddGame(Gene gene, ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events)
	{
		games.Add(gene.ToResolverModel(trajectories, events));
	}

	public List<ResolverResult> BatchResolve()
	{
		using var http = new HttpClient();
		var json = JsonSerializer.Serialize(new { Games = games});
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        try
        {
			var resp = http.PostAsync("http://localhost:5000/BatchProcessGames", content).Result;
			var str = resp.Content.ReadAsStringAsync().Result;
			var results = JsonSerializer.Deserialize<ResolverResultsModel>(str, new JsonSerializerOptions()
			{
				PropertyNameCaseInsensitive = true,
			});
			return results.Results;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
			throw;
		}
	}

	private class ResolverResultsModel
	{
		public List<ResolverResult> Results { get; set; }
	}
}

internal class ResolverResult
{
	public bool Won { get; set; }
	public int Score { get; set; }
}
