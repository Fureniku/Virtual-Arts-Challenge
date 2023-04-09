# Coding challenge for Virtual Arts job application.

## Overview

The task is to create a modular building system out of primitive shapes in a game engine of my choice - I chose Unity. This required as a minimum:

- Be able to select a 3D shape and place it into an environment.
- Be able to edit, configure and move the placed objects and shapes.
- Be able to delete/remove placed objects and shapes from the environment.

The project is now complete and meets the above requirements.

## Building

The project was made with Unity 2021.3.21f1. Import the project into Unity and it should be buildable. The project should largely work with most other Unity versions, as the main Unity features used were rendering and physics which are rarely changed.

## Controls

***General controls***
**WASD:** Movement (Mouse to aim)
**Space:** ascend
**Left Control:** descend
**Left shift:** sprint
**Left alt:** Toggle mouse lock
**F2:** Switch between flying free-cam and first person player cam
**F:** toggle snapping for all object placement/edit modes
**P:** toggle physics enabled for object placement. Does not affect existing objects.

***With no objects selected***
**Q:** open object selection screen
**Mouse 1:** Select the object you’re aiming at

***When creating a new object***
**Mouse 1:** Place the object and stop
**Mouse 2:** Place the object, with the option to place more.

***When an existing object is selected***
**Mouse 1:** Confirm and finish changes (only when mouse is locked to screen)
**E:** Pick up and move the object
**Enter:** Confirm and finish changes
**Delete:** Delete the object
**Escape:** Cancel changes and revert object to original location

When either creating a new or editing an existing object
Z / X: Rotate horizontally (Slowly by default, snapping 45 degrees with snap enabled)
C / V: Rotate vertically
[ / ]: Scale the object up or down uniformly
U / J: Scale X
I / K: Scale Y
O / L: Scale Z

## A note on physics

I'm concerned physics isn't very intuitive, as this is a prototype style build and there's not much space to explain it within the program.

By default, physics is disabled on placed objects. Pressing P will enable physics placement mode - Anything you place now (except the Piston and Spinner) will have physics enabled. Physics is paused when you are editing an object with the mouse unlocked.

If you have physics enabled and you edit a non-physics object, physics will become enabled on that object. Likewise, if you have physics disabled and edit a physics object, physics will be disabled on that object.

The free-cam does not have physics enabled, and will clip through objects. The player controller style cam has physics and can move objects around by colliding with them.

