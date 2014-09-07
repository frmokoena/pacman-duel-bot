Entelect Challenge 2014: Pacman Duel Bot AI
===========================================

# Introduction

My entry to the 2014 Entelect Challenge.

## Rules

__Rule #1:__ Develop an artificially intelligent bot that is able to navigate the following pacman maze and collect more pills than an opponent.
```
###################
#........#........#
#*##.###.#.###.##*#
#.##.###.#.###.##.#
#.................#
#.##.#.#####.#.##.#
#....#...#...#....#
####.###.#.###.####
####.#.......#.####
####.#.## ##.#.####
   A...#   #...B   
####.#.## ##.#.####
####.#.......#.####
####.#.#####.#.####
#........#........#
#.##.###.#.###.##.#
#*.#...........#.*#
##.#.#.#####.#.#.##
#....#...#...#....#
#.######.#.######.#
#.................#
###################
```

Key:
```
Pill (.): Gain 1 point.
Bonus Pill (*): Gain 10 points.
Wall (#): Well, you can't go through the wall unless you are... Chuck Norris.
```

__Rule #2:__ Can be found [here](http://challenge.entelect.co.za/DisplayLink.aspx?group=Rules&name=N/A)

__Rule #3:__ Can be found [here](http://challenge.entelect.co.za/DisplayLink.aspx?group=Rules&name=N/A)

## Solution strategy

**_as a matter of fact:_** ~~Unless it's the lass pill standing, never ever waste time (and milk) going to a pill you are certain that your opponent will reach it before you.~~ Right?

**_path finding:_** I use [A*](http://en.wikipedia.org/wiki/A*_search_algorithm).

**_the fact is:_** _my bot is very hungry_

**_in making the decision, the bot combines the above and below principles:_**

1. If we can score point in 1 move, score the point, and in the case where we have more than one path to collect points, we normally prefer the longest streak.

2. We break the maze into 2 portions:

 + Lower part: lower than the maze tunnel, and
 + Upper part: above and including the maze tunnel. Then,
 
   + If we are in the lower part and we can win, we stay there. The same happens for the upper part.
    + If opponent is in the lower part, and she can win, the strategy is to go there and disturb her. The same happnens for the upper part.

**_in a watch out:_** All the above happen while we search the neighbourhood for bonus pill.

# Tools used

Visual Studio 2013

## Building the solution

1. First, ensure that MSBuild is installed on your system, and the path to MSbuild is set in Environment Variables.
2. In the solution directory, run compile.bat in the command prompt, or
3. Simply build the solution through visual studio 2013.

## Running the application

Run the solution by building and running the test harness provided [here](https://github.com/rm2k/2014-PacMan-TestHarness). You'll need to provide 2 arguments, botA and botB to the test harness, i.e.

    PacManDuel.exe C:\pacman\botA start.bat C:\pacman\botB start.bat
  
 where this solution can be either botA or botB. C:\pacman\botA is the directory for BotA, and start.bat is a batch file to start the bot executable.
 
How about you write your own bot and wacth it get torn by mine, or just create two folders for this very same bot and watch it fight agaist itself. 
