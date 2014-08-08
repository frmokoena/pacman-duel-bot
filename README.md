Pacman Duel Bot
===============

# Introduction

My entry to the 2104 Entelect Challenge.


## Rules
1. Develop an artificially intelligent bot that is able to navigate the pacman maze and collect more pills than an opponent.

A list of rulles can be found here [here](http://challenge.entelect.co.za/DisplayLink.aspx?group=Rules&name=N/A)

## Solution Strategy

In this solution, I use [A*](http://en.wikipedia.org/wiki/A*_search_algorithm) for path finding 

1. If we can score point in 1 move, score the point, and in the case where we have more than one path to collect points, we normally prefer the longest streak.

2. We break the maze into 2 portions :- Lower part, lower than the respawn region, and upper part, above and including the respawn region. Then,

i) If we are in the lower part and we can win, we stay there. The same happens for the upper part.

ii) If opponent is in the lower part, and she can win, the strategy is to go there and disturb her. The same happnens for the upper part.

All above happen while we search the neighbourhood for bonus pill.

Some shortcomings encountered,
i. hhh
ii.. 

i) The longest path may not be best way to go at all times. The reason being that, the shorter path may end near more populated region. Tried to think how can I overcome this, but scratched my head way to much.

ii) One other tricky scenario :- taking the decision to head to one of those nearby pills, 1 pill may take you near more pills, while other may take you away.

# Tools used

Visual Studio 2013

## Building the solution

a) First, ensure that MSBuild is installed on your system, and the path to MSbuild is set in Environment Variables.

b) Double click the "compile.bat", and the project will build and produce an executable in "E:\on cloud\Projects\VS\r2k14pacmanduelbot\pacmanduelbot\bin\Release"

or simply build the solution through visual studio 2013.


# Running the application

 Run the solution by running the test harness provided at https://github.com/EntelectChallenge/2014-PacMan-TestHarness or https://github.com/rm2k/2014-PacMan-TestHarness.

You'll need to provide 2 arguments, botA and botB to the test harness, i.e.

PacManDuel.exe C:\pacman\botA start.bat C:\pacman\botB start.bat, where this solution can be either botA or botB. 

ENJOY!	
