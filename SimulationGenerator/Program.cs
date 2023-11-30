namespace SimulationGenerator;

internal class Program
{
    public static void Main()
    {
        MissionGenerator generator = new(0);
        List<Simulation> someSimulations = new();
        for (int i = 0; i < 10; i++)
        {
            someSimulations.Add(generator.GetNextSimulation());
        }
    }
}