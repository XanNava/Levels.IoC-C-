# Levels.IoC-CS
A C# Interface(eventually also reflection when wanted) IoC library. Trying to focus on clarity and understandability. Still need to test performance.

There isn't too much to learn if you want to use this. My terminology probably will be a bit different than normal DI systems.
I would start by looking at DI.Injection.cs this is what you will implement classes with to be injected.
There are a few test injectables at the bottom of the script.
Scope Doesn't hold most of the logic, it is more used as a key to Exports<>.
I will be updating this over the next few days. With some name changes and a better tutorial.
There is also Unity3D integration if you are interested, but the specific functionality for Unity is a WIP, but base functionality works there, also needs code-generation for the cleanest aproach.

Also, I know my use of interface features in the way I do (default implementations not for just throwing unimplemented errors) is a bit forbidden.
However, my philosophy here is interfaces are a contract for functionality, since the functionality doesn't change for most of the interface, and the parts that do act normal,
we are still honoring that principle. It allows us to avoid boiler plate code. Likewise, this could be achieved from inheriting another class, but at
The core all we are doing is saying we have the injection functionality, where inheritance should relate more to identity.

This currently uses reflection for one thing but will update as new .net C# features allow me to switch it out. This reflection functionality has to do with IsInjectable.GetAttribute, it is virtual so just set up the DI.Settings in your class if you want. I do have another library that I made that uses reflection for normal reflection functionality like constructors, method, and field injection. My plan is to integrate the two, so you can choose when to use reflection for those functionalities, and when to use the interface for performance.

Feel free to message me.
