<img src="logo.svg" width="256" >

> HotRay is an open-source node graph framewrok developed in CSharp. 

HotRay is inspired by the Redstone System in Minecraft. You can build your own programs or services by just creating and connecting nodes in HotRay. 



## Concepts

There are 3 main concepts in HotRay. 

1. Ray ☄️
2. Tick ⏱️
3. Node 📦

### Ray ☄️

All data is represented as "Ray"s in HotRay. 

> Example: 
> - `IntRay` contains just a 64-bits integer; 
> - `ImageRay` contains an RGBA image; 
> - `SignalRay` contains nothing but used for communication purpose widely. 


### Tick ⏱️

"Tick" is time unit. 


### Node 📦

"Node" is the minimum unit to execute code. 

> Example: 
> - `PulseNode` generates SignalRay; 
> - `FilterNode` converts a Ray from one type to another type; 
> - `PrintNode` prints input Ray to the screen;  
> - `BoxNode` boxes a set of nodes; 
> - `DelayNode` delays ray one or more ticks. 



## Quick Start

Check the `HotRay/Program.cs`


