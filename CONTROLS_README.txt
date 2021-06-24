Creator: Kyle Partlow
Website: www.kylepartlow.com
Github: www.github.com/Kahl21

/------------------------------------/
INTRO:

CONTROLS AT THE BOTTOM!!!!!

This a prototype of a 3D game with grid-based combat. 

I wanted to start making this out of college because I love playing games like these, but due to having 
to worry about rent I worked an overnight shift job and had no time to make things. Then when COVID hit I 
got laid off from starbucks and had a few depressive moments with moving back home and not know what to 
do during COVID. But here we are a year later and we have a single prototype! It's not much in the grand 
scheme of a whole career but I hope this becomes a stepping stone and a good skill showcase.

I tested this extensively by myself but there might still be some small bugs that I've missed. If so, I apologize.

Enjoy!

/------------------------------------/
HOW THE GAME WORKS(SET-UP):

There are two menus that you go through for set-up into combat. 

1. Character Select
	-You can click and drag character portraits from one box to another to add them to your roster. 
	-To remove a character, simply drag their portriat to the red box labeled "trashcan".

2. Grid Creation
	-# of Enemies (Top-left) lets you put however many enemies that you want to fight, I have made it so that 
	 throught the power of math it does auto adjust to fit the size of the grid
	-Grid Size (Top-Kinda-Left) gives you a drop down menu that allows you to change the size of the battle grid
		--Small(6x6), Medium(8x8), Large(10x10)
	-Terrain (Top-Left) here you can see two different things, "Grid Traits" and "Elevation"
		--"Grid traits" is supposed to be terrain types like forest, water, holes in the ground, etc. It is 
		  set up in code to take building blocks from the Resources folder corresponding to the terrain names 
		  in an enum and popluate the 3D spaces with the corresponding blocks.
	-Re-Generate (Bottom) here is a real-time 3D view of the battle field that you will be on.
		--It randomly generates everytime you change an aspect of the grid in anyway, but this button allows 
		  you to re-generate it as much as you want!

/------------------------------------/
HOW THE GAME WORKS(COMBAT):

CONTROLS:

	-W,A,S,D = control for moving the camera forward, backward, left, right. A basic control scheme.
	-Q,E = will rotate the camera left or right around your viewpoint
	
	-Left Click = You can click on characters on the battlefield to move the camera to any character.
		--You can also left click on any button to do what it says
	-Right Click = This is mainly to move backwards through menus inside of combat.
		--You can also right click on characters to bring up a menu that shows all of that specific characters stats

	-Escape (ESC) or P = opens up the pause menu, where you can Resume, Go back to character select, or exit the game entirely



/------------------------------------/