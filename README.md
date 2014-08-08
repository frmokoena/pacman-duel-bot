pacman duel bot
===============

My entry to the <a href="http://challenge.entelect.co.za/" target="_blank">2104 Entelect Challenge.</a>


<h2>rules</h2>
<ol>
<li>Develop an artificially intelligent bot that is able to navigate the pacman maze and collect more pills than an opponent.</li>
</ol>

1. PROJECT STRUCTURE AND SOLUTION STRATEGY

a) STRATEGY IN SHORT:

In this solution, I use A* for path finding: http://en.wikipedia.org/wiki/A*_search_algorithm.

Otherwise,
If we can score point in 1 move, score the point, and in the case where we have more than one path to collect points, we normally prefer the longest path.

Again,
We break the maze into 2 portions :- Lower part, lower than the respawn region, and upper part, above and including the respawn region. Then,

i) If we are in the lower part and we can win, we stay there. The same happens for the upper part.

ii) If opponent is in the lower part, and she can win, the strategy is to go there and disturb her. The same happnens for the upper part.

All above happen while we search the neighbourhood for bonus pill.

Some shortcomings encountered,

i) The longest path may not be best way to go at all times. The reason being that, the shorter path may end near more populated region. Tried to think how can I overcome this, but scratched my head way to much.

ii) One other tricky scenario :- taking the decision to head to one of those nearby pills, 1 pill may take you near more pills, while other may take you away.
    

b) PROJECT STRUCTURE

/brainbox/
=================================================================

Bot
----
where the game decsions are made :- where to move, the next point/pill to head to, when to drop poison pill, when to respawn, when to kill an opponent.


/helpers/
=================================================================

MovesGenerator
------
i)   Compute list of next available/valid positions to move to.
ii)  Implement A* for path finding.
iii) Build path for collecting points where we have more than one path to collect path. We normally prefer longest path of points.
 
ScoreKeeper
----------
i)  Keep track of scores for both players - me & opponent.
ii) Also keeps track of TurnsWithNoPointScored.


/models/
=================================================================

Maze
-----
keep the game maze.


PathFinderNode
---------------
Define the path node used in path finding.


PoisonBucket
-------------
Handles all things poison pill.


/shared/
=================================================================

Symbols
--------
Define game symbols.


/store/
=================================================================

Stores game related files - Score card, Poison pill status file, etc.


2. BUIDLING THE SOLUTION

a) First, ensure that MSBuild is installed on your system, and the path to MSbuild is set in Environment Variables.

b) Double click the "compile.bat", and the project will build and produce an executable in "E:\on cloud\Projects\VS\r2k14pacmanduelbot\pacmanduelbot\bin\Release"

or simply build the solution through visual studio 2013.


3. RUNNING THE SOLUTION

 Run the solution by running the test harness provided at https://github.com/EntelectChallenge/2014-PacMan-TestHarness or https://github.com/rm2k/2014-PacMan-TestHarness.

You'll need to provide 2 arguments, botA and botB to the test harness, i.e.

PacManDuel.exe C:\pacman\botA start.bat C:\pacman\botB start.bat, where this solution can be either botA or botB. 

ENJOY!	
