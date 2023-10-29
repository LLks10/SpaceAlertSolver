namespace SpaceAlertSolver;

public class Trajectory
{ 
    public int maxDistance;
    public int[] actions;
    public int dist2, dist1;
    public int number;

    public Trajectory(int number)
    {
        this.number = number;
        switch (number)
        {
            case 0:
                maxDistance = 9;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[4] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 1:
                maxDistance = 10;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[7] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 2:
                maxDistance = 11;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[2] = 2;
                actions[7] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 3:
                maxDistance = 12;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[4] = 2;
                actions[8] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 4:
                maxDistance = 13;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[6] = 2;
                actions[10] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 5:
                maxDistance = 14;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[2] = 2;
                actions[6] = 2;
                actions[9] = 1;
                dist1 = 4;
                dist2 = 9;
                break;

            case 6:
                maxDistance = 15;
                actions = new int[maxDistance + 1];
                actions[0] = 3;
                actions[4] = 2;
                actions[7] = 2;
                actions[11] = 1;
                dist1 = 4;
                dist2 = 9;
                break;
        }
    }
}
