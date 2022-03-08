# Traffic-Simulation-Zirk-Coetsee
 Fuzzy Logic Traffic Sim Project

Description:

This project is a simple Traffic simulation game where players can create roads, place buildings and have pedestrians travel between these buildings on the sidewalks of the roads and interact with cars at crosswalks taking priority when walking over. Cars travel between buildings on the left-hand side of the road and avoid a collision at road intersections.

On a more technical note, the system uses AStar search algorithm to place road prefabs on the shortest path between two points on the main world graph. These road prefabs contain two different types of graphs for pedestrians and cars. Pedestrian graph markers are placed on the sidewalks and connected to each neighbouring pedestrian point, then each pedestrian graph endpoint on the road prefab is set to be open for connections and are connected when a neighbouring road prefab is placed next to it. Thus pedestrians can find the shortest path between points A and B and walk on the sidewalks.

Road prefabs also contain car graphs that are placed on either side of the road to determine the right and left lanes and how cars should traverse road intersections. Similar to the pedestrian graph, the car graph has markers that are connected to neighbouring car graph points, endpoints are classified between incoming and outgoing to make sure the cars do not travel the shortest path and end up on the wrong side of the road. The 3 Way uses a directional graph system to deal with the use-case of cars finding the shortest path in the wrong lane.

At road intersections cars the road prefab will check if there are any pedestrians walking on that intersection if so then-incoming cars must stop, it will also check if there are any waiting pedestrians in that intersection then also stop incoming cars. Most of this functionality is handled with events. Cars also use raycasts to detect collision with other car objects on the cars layer and will stop before colliding with other cars

Buildings:
This project has two types of buildings Houses and Special Structures (Shops, Schools) as endpoints.

Cars and Pedestrians spawn at houses and special structures and travel between these two types, thus you require at least 1 House and 1 Special Structure.

This project allows players to place 2 different size houses such as houses 1x1 and larger houses 2x2.

The system can take several House and Special Structure Prefabs and with weights determine which prefab gets placed on the map.

Roads:
Players can create roads by clicking or dragging road sections, the system will automatically choose the correct prefab between (Road Straight, Road Corner, Road 3 Way, Road 4 Way).

Using AStar algorithm the system will create a path between your start click position to your mouse's current position and add the correct prefabs.

Roads contain pedestrian markers that make up the path between different road prefabs. When placing a road the system then connects the different markers (graph points).

Roads contain car markers that make up the path between different road prefabs. These dictate the positions cars can travel

Cars:
Cars spawn at all buildings on the nearest left side car marker on the road prefab.
At intersections, cars will turn into the correct lane and yield to the first car, using a queue system.
Once cars reach their destination they will be destroyed.
The Car prefab contains a list of different car prefab meshes, that is set as a child object on instantiation and the system will randomly select a different car prefab mesh on the spawn of cars.

Pedestrians:
Pedestrians spawn at all buildings on the nearest sidewalk marker on the road prefabs and travel to their locations using the shortest path between their start and endpoint.
At intersections, pedestrians will wait until there are no cars driving and then cross the road.
Different pedestrian prefabs can be added and random prefabs will be selected on instantiation.

Camera movement:
Once in the game, you can move the mouse camera the same as you would in the unity editor.

Move Left : A or Left Arrow
Move Right: D or Right Arrow
Move Forward: W or Up Arrow
Move Back: S or Down Arrow
Rotate Camera: Q or E or Click and Hold Right Mouse Button And Drag

Credit: 
Implementation based on tutorial series by Sunny Valley Studio
https://www.sunnyvalleystudio.com/

And there you have it thank you for having a look!

Time Sheet:
Project Received on the 22 Feb 2022

23 Feb 2022
Day 1: 19:00 - 23:30 (Including breaks)
Research And Planning
	Pathfinding systems
	Implement Unity ECS? (Not implemented)
	Graph System
	Map builder
	Vehicle Logic Examples
Start Initial City Builder

25 Feb 2022
Day 2: 19 - 00:00 (Including breaks)
Implement Road Builder
Check Neighbour logic
Scaling and debugging issues
Prefab creation (Did not find time to create custom models :( )

1 March 2022
Day 3: 20:30 - 00:06 (Including breaks)
Implement Build UI
Fix delegate incorrect usage and assign
Place buildings and Special Structures
Improve path-building logic to work with AStar

2 March 2022
Day 4: 21:00 - 00:30 (Including breaks)
Initial AI car implementation
	The car moves from 1 point to another
	The car turns while moving
	The car acceleration, torque, and max speed

5 March 2022
Day 5: 12:00 - 16:00 (Including breaks)
Initial AI pedestrian implementation
	Code Improvement
	Bug fixing

6 March 2022
Day 6: 
09:30 - 11:16Improved AI pedestrians
16:20 - 18:00 Road Pedestrian Markers setup
18:30 - 21:30 Improve Road Pedestrian Markers logic

7 March 2022
Day 7: 21:00 - 01:30 (Including breaks)
Final Car AI implementation
Car and pedestrian crosswalk interaction logic
Fix for 3Way crosswalk
