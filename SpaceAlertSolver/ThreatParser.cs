namespace SpaceAlertSolver;

public static class ThreatParser
{
    static ThreatName[] ex_threats = new ThreatName[]
    {
        new ThreatName("Armored Grappler", true, 0),
        new ThreatName("Amoebe", true, 1),
        new ThreatName("Gunship", true, 2),
        new ThreatName("Fighter", true, 3),
        new ThreatName("Gyro Fighter", true, 4),
        new ThreatName("Energy Cloud", true, 5),
        new ThreatName("Meteorite", true, 6),
        new ThreatName("Pulse Ball", true, 7),
        new ThreatName("Destroyer", true, 8),
        new ThreatName("Spacecruiser", true, 8),
        new ThreatName("Stealth Fighter", true, 9),
        new ThreatName("Jellyfish", true, 10),
        new ThreatName("Minor Asteroid", true, 11),
        new ThreatName("Kamikaze", true, 12),
        new ThreatName("Swarm", true, 13),
        new ThreatName("Phantom Fighter", true, 14),
        new ThreatName("Scout", true, 15),
        new ThreatName("Fregat", true, 16),
        new ThreatName("Gyro Fregat", true, 17),
        new ThreatName("Man-o-War", true, 18),
        new ThreatName("Interstellar Octopus", true, 19),
        new ThreatName("Maelstorm", true, 20),
        new ThreatName("Asteroid", true, 21),
        new ThreatName("Pulse Satellite", true, 22),
        new ThreatName("Nemesis", true, 23),
        new ThreatName("Nebula Crab", true, 24),
        new ThreatName("Psionic Satellite", true, 25),
        new ThreatName("Major Asteroid", true, 26),
        new ThreatName("Moloch", true, 27),
        new ThreatName("Behemoth", true, 28),

        new ThreatName("Gepanserde Grijper", true, 0),
        new ThreatName("Slagschip", true, 2),
        new ThreatName("Jager", true, 3),
        new ThreatName("Cryoschild Jager", true, 4),
        new ThreatName("Energiewolk", true, 5),
        new ThreatName("Meteoroïde", true, 6),
        new ThreatName("Impulsbal", true, 7),
        new ThreatName("Ruimtekruiser", true, 8),
        new ThreatName("Stealth Jager", true, 9),
        new ThreatName("Kwal", true, 10),
        new ThreatName("Kleine Asteroïde", true, 11),
        new ThreatName("Zwerm", true, 13),
        new ThreatName("Spookjager", true, 14),
        new ThreatName("Verkenner", true, 15),
        new ThreatName("Cryoschild Fregat", true, 17),
        new ThreatName("Oorlogsbodem", true, 18),
        new ThreatName("Interstellaire Octopus", true, 19),
        new ThreatName("Maalstroom", true, 20),
        new ThreatName("Asteroïde", true, 21),
        new ThreatName("Impulssatelliet", true, 22),
        new ThreatName("Nevelkrab", true, 24),
        new ThreatName("Psionische Satelliet", true, 25),
        new ThreatName("Grote Asteroïde", true, 26),

        new ThreatName("Phasing Fighter", true, 100),
        new ThreatName("Phasing Frigate", true, 101),
        new ThreatName("Plastmatic Needlship", true, 102),
        new ThreatName("Polarized Fighter", true, 103),
    };
    static ThreatName[] in_threats = new ThreatName[]
    {
        new ThreatName("Red Saboteur", false, 0),
        new ThreatName("Blue Saboteur", false, 1),
        new ThreatName("Red Skirmisher", false, 2),
        new ThreatName("Blue Skirmisher", false, 3),
        new ThreatName("Red Soldiers", false, 4),
        new ThreatName("Blue Soldiers", false, 5),
        new ThreatName("Virus", false, 6),
        new ThreatName("Red Hacked Shields", false, 7),
        new ThreatName("Blue Hacked Shields", false, 8),
        new ThreatName("Overheated Reactor", false, 9),
        new ThreatName("Unstable Warheads", false, 10),
        new ThreatName("Blue Slime", false, 11),
        new ThreatName("Red Slime", false, 12),
        new ThreatName("Red Commandos", false, 13),
        new ThreatName("Blue Commandos", false, 14),
        new ThreatName("Alien", false, 15),
        new ThreatName("Eliminator", false, 16),
        new ThreatName("Seeker", false, 17),
        new ThreatName("Atomic Bomb", false, 18),
        new ThreatName("Rebellious Robots", false, 19),
        new ThreatName("Crossed Wire", false, 20),
        new ThreatName("Power System Overload", false, 21),
        new ThreatName("Fissure", false, 22),
        new ThreatName("Contamination", false, 23),

        new ThreatName("Rode Saboteur", false, 0),
        new ThreatName("Blauwe Saboteur", false, 1),
        new ThreatName("Rode Tirailleurs", false, 2),
        new ThreatName("Blauwe Tirailleurs", false, 3),
        new ThreatName("Rode Soldaten", false, 4),
        new ThreatName("Blauwe Soldaten", false, 5),
        new ThreatName("Virus", false, 6),
        new ThreatName("Rode Gehackte Schilden", false, 7),
        new ThreatName("Blauwe Gehackte Schilden", false, 8),
        new ThreatName("Oververhitte Reactor", false, 9),
        new ThreatName("Onstabiele Kernkoppen", false, 10),
        new ThreatName("Blauw Slijm", false, 11),
        new ThreatName("Rood Slijm", false, 12),
        new ThreatName("Rode Commandos", false, 13),
        new ThreatName("Blauwe Commandos", false, 14),
        new ThreatName("Zoekrobot", false, 17),
        new ThreatName("Atoombom", false, 18),
        new ThreatName("Rebellerende Robots", false, 19),
        new ThreatName("Verwisselde Kabels", false, 20),
        new ThreatName("Overbelast Energienet", false, 21),
        new ThreatName("Scheur", false, 22),
        new ThreatName("Besmetting", false, 23),

        new ThreatName("Siren", false, 100),
        new ThreatName("Driller", false, 101),
    };

    // https://gist.github.com/Davidblkx/e12ab0bb2aff7fd8072632b396538560 Thanks!
    /// <summary>
    ///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
    /// </summary>
    /// <param name="source1">First string</param>
    /// <param name="source2">Second string</param>
    /// <returns></returns>
    public static int LevenshteinDistance(string source1, string source2) //O(n*m)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        // First calculation, if one entry is empty return full length
        if (source1Length == 0)
            return source2Length;

        if (source2Length == 0)
            return source1Length;

        // Initialization of matrix with row size source1Length and columns size source2Length
        for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
        for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

        // Calculate rows and collumns distances
        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }
        // return result
        return matrix[source1Length, source2Length];
    }

    /**
     * <summary>Returns the external threat that is most similar to the input</summary>
     * <returns>A <c>ThreatName</c> object</returns>
     * <param name="input">The string to examine</param>
     */
    public static ThreatName ParseExThreat(string input)
    {
        int min_lev_dist = int.MaxValue;
        ThreatName most_likely_threat = ex_threats[0];
        foreach (ThreatName tn in ex_threats)
        {
            int lev_dist = LevenshteinDistance(tn.name.ToLower(), input.ToLower());
            if (lev_dist < min_lev_dist)
            {
                min_lev_dist = lev_dist;
                most_likely_threat = tn;
            }
            if (lev_dist == 0)
                break;
        }

        return most_likely_threat;
    }

    /**
     * <summary>Returns the internal threat that is most similar to the input</summary>
     * <returns>A <c>ThreatName</c> object</returns>
     * <param name="input">The string to examine</param>
     */
    public static ThreatName ParseInThreat(string input)
    {
        int min_lev_dist = int.MaxValue;
        ThreatName most_likely_threat = in_threats[0];
        foreach (ThreatName tn in in_threats)
        {
            int lev_dist = LevenshteinDistance(tn.name.ToLower(), input.ToLower());
            if (lev_dist < min_lev_dist)
            {
                min_lev_dist = lev_dist;
                most_likely_threat = tn;
            }
            if (lev_dist == 0)
                break;
        }

        return most_likely_threat;
    }
}

public struct ThreatName
{
    public string name;
    public bool external;
    public int id;

    public ThreatName(string name, bool external, int id)
    {
        this.name = name;
        this.external = external;
        this.id = id;
    }
}
