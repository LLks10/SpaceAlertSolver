using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SpaceAlertSolver;


internal static class ResolverModelMapper
{
	private static string[] colours = new string[] { "Purple", "Red", "Yellow", "Green", "Blue" };

	public static ResolverModel ToResolverModel(this Gene gene, ImmutableArray<Trajectory> trajectories, ImmutableArray<Event> events)
	{
		Act[][] actions = new Act[Gene.user_actions.Length + gene.players][];
		for (int i = 0; i < Gene.user_actions.Length; i++)
			actions[i] = Gene.user_actions[i];
		for (int i = Gene.user_actions.Length; i < Gene.user_actions.Length + gene.players; i++)
			actions[i] = gene.gene.Skip(i * 12).Take(12).Cast<Act>().ToArray();


		PlayerModel[] pms = actions.Select(
			(acts, i) => new PlayerModel(
				acts.Select(act => act switch
				{
					Act.A => new ActModelFilled(0),
					Act.B => new ActModelFilled(1),
					Act.C => new ActModelFilled(2),
					Act.Left => new ActModelFilled(3),
					Act.Right => new ActModelFilled(4),
					Act.Lift => new ActModelFilled(5),
					Act.Fight => new ActModelFilled(6),
					Act.HeroicA => new ActModelFilled(7),
					Act.HeroicB => new ActModelFilled(8),
					Act.HeroicFight => new ActModelFilled(9),
					Act.HeroicTopLeft => new ActModelFilled(10),
					Act.HeroicTopMiddle => new ActModelFilled(11),
					Act.HeroicTopRight => new ActModelFilled(12),
					Act.HeroicDownLeft => new ActModelFilled(13),
					Act.HeroicDownMiddle => new ActModelFilled(14),
					Act.HeroicDownRight => new ActModelFilled(15),
					_ => new ActModel(),
				}).ToArray(),
				colours[i % colours.Length],
				i)
			).ToArray();


		return new(
			pms,
			new(trajectories[0].number + 1),
			new(trajectories[1].number + 1),
			new(trajectories[2].number + 1),
			new(trajectories[3].number + 1),
			events.Where(x => x.Zone == 0).Select(x => new ThreatModel(ThreatFactory.Instance.ResolverIdsById[x.CreatureId], x.Turn)).ToArray(),
			events.Where(x => x.Zone == 1).Select(x => new ThreatModel(ThreatFactory.Instance.ResolverIdsById[x.CreatureId], x.Turn)).ToArray(),
			events.Where(x => x.Zone == 2).Select(x => new ThreatModel(ThreatFactory.Instance.ResolverIdsById[x.CreatureId], x.Turn)).ToArray(),
			events.Where(x => x.Zone == 3).Select(x => new ThreatModel(ThreatFactory.Instance.ResolverIdsById[x.CreatureId], x.Turn)).ToArray(),
			new InitialDamageModel[] { });
	}
}

internal record ResolverModel(
	PlayerModel[] Players,
	TrackModel RedTrack,
	TrackModel WhiteTrack,
	TrackModel BlueTrack,
	TrackModel InternalTrack,
	ThreatModel[] RedThreats,
	ThreatModel[] WhiteThreats,
	ThreatModel[] BlueThreats,
	ThreatModel[] InternalThreats,
	InitialDamageModel[] InitialDamageModels);

internal record PlayerModel(
	ActModel[] Actions,
	string PlayerColor,
	int Index);

[JsonDerivedType(typeof(ActModel), typeDiscriminator: "base")]
[JsonDerivedType(typeof(ActModelFilled), typeDiscriminator: "filled")]
internal record ActModel();

internal record ActModelFilled(int FirstAction) : ActModel;

internal record TrackModel(int TrackIndex);

internal record ThreatModel(string Id, int TimeAppears);

internal record InitialDamageModel();

