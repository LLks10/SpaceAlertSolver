using SpaceAlertSolver;

namespace SimulationGenerator;

internal class Program
{
    const int NUM_SIMULATIONS = 100;
    const string FILE_NAME = "simulations.txt";

    public static void Main()
    {
        GamePool.EnableConcurrent = true;
        Simulation[] simulations = new Simulation[NUM_SIMULATIONS];
        Parallel.For(0, NUM_SIMULATIONS, i =>
        {
            MissionGenerator generator = new(i);
            simulations[i] = generator.GetNextSimulation();
        });
        WriteSimulationsToFile(simulations, FILE_NAME);
    }

    private static void WriteSimulationsToFile(Simulation[] simulations, string fileName)
    {
        using FileStream fs = new(fileName, FileMode.CreateNew, FileAccess.Write);
        using StreamWriter sw = new(fs);
        for (int i = 0; i < simulations.Length; i++)
        {
            sw.WriteLine(simulations[i].Serialize());
        }
    }
}