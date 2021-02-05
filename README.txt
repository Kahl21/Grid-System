Creator: Kyle Partlow
Website: www.kylepartlow.com
Github: www.github.com/Kahl21

/------------------------------------/
INTRO:

This "test game" is a visual representation on how combat works with grid-based combat in a game.

The game runs off of two 2D-arrays(one for terrain and one for Characters). Characters move across terrain and subtract
movement points (varies from character to character) based on the terrain they moved across. Since this is just a test
project all terrain was set to 1.

/------------------------------------/
HOW THE GAME WORKS(SET-UP):
All grids are built into squares or rectangles.

Grid Size*:
	-Vertical +/-: changes the length of the grid.
	-Horizontal +/-: changes the Width of the grid.

Number of Teams*: this sets how many teams there are that will all battle to the death.
Enemies per Team*: this sets how many units will be spawned per team.
	-All units stats are randomized, not all units are created equal (:sadge:)

Fight(button): Clicking this will initiate combat and take you to the combat screen.

Exit(button): Clicking this will exit the game.

*: Amount of units to spawn will automatically trunkcate to fit the size of the set grid
	ex) 4 teams of 4 cannot fit on a 2x2 grid, it will auto change to 2 teams of 2

/------------------------------------/
HOW THE GAME WORKS(COMBAT):

Top Left Window(Main View): here you can see the representation of the grid and all of its combatants (may look a little weird if there are more than 100 units)
	- If a blue square labeled "DD" pops up it means that there are only 5 people left to fight and "Double Damage" will occur on all attacks. This is to stop 
	  unwinnable conflicts.(i.e. last two units are low damage and high defense, so they do no damage to one another)
	- Pressing the Red "END" button at anytime will stop combat and bring you back to the main menu to set up another fight

Bottom Left Window(Text View): here, after a character does something, it will record the current units actions. (moving, attacking, using a skill)

Right Side Window(Player Stats): This window shows all info on the current acting Unit. 
	- From top to bottom: 
		-Name 
		-Team
		-Weapon
		-Main Stats 
		-Resistances
		-Skills
		-Targets(if any)
		-Grid Position

Bottom Center(Interactions):
	-Arrows and Actions(Bottom): pressing left or right on the arrows near the bottom (or on your keyboard) will go forwards(Right) and backwards(Left) through Unit Turns
	
	-Auto-Mode(Top): This is a function that allows you to just watch the fight go on continuously until the there is a victor.
		--To Turn this on simply press the button labeled "Off" and it will switch to "On"
		--To Turn this off just press on the button labeled "On" or press the arrow keys on screen or on your Keyboard		
		--The speed at which Auto-Mode plays can be changed with the slider next to the "Off/On" button


/------------------------------------/