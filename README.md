# Visualization App

This is the camera control part for the visualization tool of AUV navigation team.

I'm pretty new to Unity and this project is a 'little' messy right now. The structure and my code makes a huge mesh. Will fix it later. I built this from Mac so windows users might encounter some problems at the first time setting it up. 

Important files:

- For most of the times you will only modify files in the Assets folder in ever program folder (for now just MultiCamera).
- The Main.unity is the scene file that contains information about the parameters of all the game objects (e.g. camera, UI, 3D objects).
- In the Scripts sub folder there is an AUVController.cs and SplitScreenController.cs. AUVController.cs provides 2-D control to our dummy AUV and the information text printed on the screen. SplitScreenController.cs provides functions to control how to use the screen to show different views of the enviroment. For example you can split the screen into four pieces and use each part to show one view. Let's say the top-left for the main view, top-right for the top view, bottom-left for the back view and bottom-right for the left view.

Usage:

- By default when you open the project it might give you an empty screen, don't worry, open Main.unity (under Assets/Scenes/ and you should be all good.
- First you have to run the game either in unity or build it as a stand alone application.
- Use WASD or arrow keys to control the sub, moving up and down is not supported yet.
- Use mouse to select the view mode and which camera is used to render the current view from the top menue.
- Use the mouse scroll to move the camera near and far. (won't affect Follow View).
- Right click and drag to translate the camera (won't affect Follow View) and left click to rotate (only for Main View).
- Either right click or left click on a specific view will set it as the current view.


Log File and Stream Format:
'''
[
	{
		"ts":12345,  \\ Unix Timestamp
		"objects":[ \\ array of objects detected by CV
			{"id":0, \\ ID of Object
			"x":0,   \\ Position
			"y":0,		
			"z":0,
			"th":0,   \\ theta angle of rotation
			"p":0	\\ probability
			}
		],
		"x":0,   \\ AUV position
		"y":0,
		"z":0,
		"ax":0, \\ AUV acceleration
		"ay":0,
		"az":0,
		"ux":0,  \\ Speed
		"uy":0,
		"uz":0,
		"d":0   \\ depth
		"th":0  \\ rotation 
	}
]
'''
