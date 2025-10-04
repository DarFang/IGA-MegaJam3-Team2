Camera settings in "Main Camera" GameObject > "IsometricCamera" Script.

Instructions for MovementDemo scene:

Playing:

- Instructions and events in console as you play

Waypoints:

1.- Open MovementDemo scene (Assets/Scenes/MovementDemo/Movement.unity)
2.- Place Waypoint prefab instances in the scene to define paths (Assets/Prefabs/Waypoints). 
	- After placing one, set their individual order in the inspector

Event creation: 

To create an event for a waypoint: 
1.- Create an empty GameObject
2.- Add a ItemEvent/CombatEvent component to it
	- For item events, there's a very basic ItemData Scriptable Object for items (Assets/ScriptableObjects/ItemData)
	- Create instances of this Scriptable Object to define items
3.- Configure the event as needed. 
Optional - Save as prefab if desired
4.- Assign the event to a waypoint before testing

- Icon and description are there as placeholders, but not implemented yet
- Assign the event GameObject to the corresponding waypoint's "Event" field in the inspector

Thanks for testing! Please, report any issues you find
// Planning on adding a custom editor tool to make waypoint creation easier