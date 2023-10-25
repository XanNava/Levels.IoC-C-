# Levels.IoC-C-
A C# Interface(eventually also reflection when needed) IoC library. Trying to focus on clarity and understandability. Still need to test performance.

There is isn't to much to learn if you want to use this. My terminology probably will be a bit different than normal DI systems.
I would start by looking at DI.Injection.cs this is what you will make classes with to be injected.
There are a few test injectables at the bottom of the script.
Scope Doesn't hold most of the logic, it is more used as a key to Exports<>.
I will be updating this over the next few days. With some name changes and better tutorial.
There is also Unity3D integration if you are interested, but the specific functionality for Unity is a WIP, but base functionality works there.
