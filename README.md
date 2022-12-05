![hotray-logo](hotray-logo.png)

> Hotray is an open-source node graph framewrok developed in CSharp. 

This project is inspired by the Redstone System in the video game Minecraft. You can build your own programs or services by just creating and connecting nodes in Hotray. 


## Concepts

There are 3 main concepts in Hotray. 

1. Ray
2. Tick
3. Node

### Ray

All data is represented as "Ray"s in Hotray. 

> Example: 
> - `IntRay` contains just a 64-bits integer; 
> - `ImageRay` contains an RGBA image; 
> - `SignalRay` contains nothing but used for communication purpose widely. 


### Tick

"Tick" is time unit. 


### Node

"Node" is the minimum unit to execute code. 

> Example: 
> - `PulseNode` generates SignalRay; 
> - `FilterNode` converts a Ray from one type to another type; 
> - `PrintNode` prints input Ray to the screen;  
> - `BoxNode` boxes a set of nodes; 
> - `DelayNode` delays ray one or more ticks. 




