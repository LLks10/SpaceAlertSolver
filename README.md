SpaceAlert Solver that tries to maximize score in SpaceAlert games using Simulated Annealing
Mission 1 can be input as follows:
```
r
2 1 r 1
3 3 r 0
4 2 r 0
5 3 r 0
6 2 r 0
7 0 r 1
start
```

`r` at the start is for random trajectories, alternatively you could choose trajectories 1, 2, 3 and 4 by inputting `1234`.
Following lines consist of
<turn> <trajectory> <threat>
<trajectory> is 0 for red, 1 for white, 2 for blue and 3 for internal
<threat> can be `r 0` for random common and `r 1` for random severe, or you can write the name of the threat
then write `start` to start finding a solution.

If you're running https://github.com/LLks10/space-alert-resolver at `localhost:5000`, then the Solver will send it's solution to the Resolver where you can view it.
