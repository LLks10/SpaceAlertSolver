using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceAlertSolver
{
    public class Player
    {
        public int position, lastAction;
        public bool alive, inIntercept;
        public Act[] actions;
        public Androids team;

        public Player(Player other, Androids[] otherAndroids, Androids[] androids)
        {
            position = other.position;
            lastAction = other.lastAction;
            alive = other.alive;
            inIntercept = other.inIntercept;
            actions = Extension.CopyArray(other.actions);
            
            if (other.team == null)
            {
                team = null;
            }
            else
            {
                int androidIndex = -1;
                for (int i = 0; i < otherAndroids.Length; i++)
                {
                    if (other.team == otherAndroids[i])
                    {
                        androidIndex = i;
                        break;
                    }
                }

                team = androids[androidIndex];
            }
        }

        public Player()
        {
            alive = true;
            team = null;
            position = 1;
        }

        public void Move(int pos)
        {
            position = pos;
            if (team != null)
                team.position = pos;
        }

        public void Kill()
        {
            alive = false;
            if (team != null)
                team.alive = false;
        }

        public void Delay(int action)
        {
            if (action >= 11 || actions[action] == Act.empty)
            {
                if(action < 12)
                    actions[action] = Act.empty;
                return;
            }

            //Delay subsequent actions
            if (actions[action + 1] != Act.empty)
                Delay(action + 1);

            //Move action
            actions[action + 1] = actions[action];
            actions[action] = Act.empty;
        }

        public void SetActions(int[] a)
        {
            actions = new Act[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                switch (a[i])
                {
                    case 0:
                        actions[i] = Act.left;
                        break;
                    case 1:
                        actions[i] = Act.right;
                        break;
                    case 2:
                        actions[i] = Act.lift;
                        break;
                    case 3:
                        actions[i] = Act.A;
                        break;
                    case 4:
                        actions[i] = Act.B;
                        break;
                    case 5:
                        actions[i] = Act.C;
                        break;
                    case 6:
                        actions[i] = Act.fight;
                        break;
                    case 7:
                        actions[i] = Act.empty;
                        break;
                }
            }
        }

        public Player Copy()
        {
            Player p = new Player();
            p.actions = new Act[actions.Length];
            for (int i = 0; i < actions.Length; i++)
                p.actions[i] = actions[i];
            return p;
        }
    }


    public class Androids
    {
        public bool active;
        public int position;
        public bool alive;

        public Androids(Androids other)
        {
            active = other.active;
            position = other.position;
            alive = other.alive;
        }

        public Androids(int position)
        {
            active = false;
            alive = true;
        }
    }
}
