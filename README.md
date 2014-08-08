Entelect Challenge 2014: Pacman Duel Bot AI
===========================================

# Introduction

My entry to the 2014 Entelect Challenge.

## Rules

1. Develop an artificially intelligent bot that is able to navigate the pacman maze and collect more pills than an opponent.

 A list of rules can be found here [here](http://challenge.entelect.co.za/DisplayLink.aspx?group=Rules&name=N/A)

## Solution strategy

1. I use [A*](http://en.wikipedia.org/wiki/A*_search_algorithm) for path finding.
2. If we can score point in 1 move, score the point, and in the case where we have more than one path to collect points, we normally prefer the longest streak.
3. We break the maze into 2 portions :- Lower part, lower than the respawn region, and upper part, above and including the respawn region. Then,
   1. If we are in the lower part and we can win, we stay there. The same happens for the upper part.
   2. If opponent is in the lower part, and she can win, the strategy is to go there and disturb her. The same happnens for the upper part.
4. All above happen while we search the neighbourhood for bonus pill.

## Some shortcomings

1. The longest path may not be best way to go at all times. The reason being that, the shorter path may end near more populated region.
2. One other tricky scenario :- taking the decision to head to one of those nearby pills, 1 pill may take you near more pills, while other may take you away.

# Tools used

Visual Studio 2013

## Building the solution

1. First, ensure that MSBuild is installed on your system, and the path to MSbuild is set in Environment Variables.
2. Double click the "compile.bat", and the project will build and produce an executable in "E:\on cloud\Projects\VS\r2k14pacmanduelbot\pacmanduelbot\bin\Release" or
3. Simply build the solution through visual studio 2013.

## Running the application

Run the solution by building and running the test harness provided [here](https://github.com/rm2k/2014-PacMan-TestHarness). You'll need to provide 2 arguments, botA and botB to the test harness, i.e.

    PacManDuel.exe C:\pacman\botA start.bat C:\pacman\botB start.bat
  
 where this solution can be either botA or botB or just create two folders for this very same bot and watch it fight agaist itself. C:\pacman\botA is the directory for BotA, and start.bat is a batch file to start the bot executable.
