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
				colours[i],
				i)
			).ToArray();


		return new(
			pms,
			new(trajectories[0].number + 1),
			new(trajectories[1].number + 1),
			new(trajectories[2].number + 1),
			new(trajectories[3].number + 1),
			events.Where(x => x.Zone == 0).Select(x => new ThreatModel(x.CreatureId.ConvId(isExternal: true), x.Turn)).ToArray(),
			events.Where(x => x.Zone == 1).Select(x => new ThreatModel(x.CreatureId.ConvId(isExternal: true), x.Turn)).ToArray(),
			events.Where(x => x.Zone == 2).Select(x => new ThreatModel(x.CreatureId.ConvId(isExternal: true), x.Turn)).ToArray(),
			events.Where(x => x.Zone == 3).Select(x => new ThreatModel(x.CreatureId.ConvId(isExternal: false), x.Turn)).ToArray(),
			new InitialDamageModel[] { });
	}

	private static string ConvId(this int id, bool isExternal)
	{
		if (isExternal)
		{
			return id switch
			{
				0 => "E1-08",
				1 => "E1-09",
				2 => "E1-05",
				3 => "E1-07",
				4 => "E1-06",
				5 => "E1-04",
				6 => "E1-10",
				7 => "E1-01",
				8 => "E1-02",
				9 => "E1-03",
				10 => "E2-05",
				11 => "E2-07",
				12 => "E2-01",
				13 => "E2-04",
				14 => "E2-03",
				15 => "E2-02",
				16 => "SE1-01",
				17 => "SE1-05",
				18 => "SE1-02",
				19 => "SE1-06",
				20 => "SE1-07",
				21 => "SE1-08",
				22 => "SE1-04",
				23 => "SE2-05",
				24 => "SE2-04",
				25 => "SE2-03",
				26 => "SE2-06",
				27 => "SE2-02",
				28 => "SE2-01",
				_ => throw new UnreachableException(),
			};
		}

		return id switch
		{
			0 => "I1-04",
			1 => "I1-03",
			2 => "I1-01",
			3 => "I1-02",
			4 => "I2-03",
			5 => "I2-04",
			6 => "I2-05",
			7 => "I1-06",
			8 => "I1-05",
			9 => "I2-06",
			10 => "I1-07",
			11 => "I2-02",
			12 => "I2-01",
			13 => "I2-03",
			14 => "I2-04",
			15 => "SI1-03",
			16 => "SI2-01",
			17 => "SI2-02",
			18 => "SI2-05",
			19 => "SI1-06",
			20 => "SI1-05",
			21 => "SI2-03",
			22 => "SI1-04",
			23 => "SI2-04",
			_ => throw new UnreachableException(),
		};
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

