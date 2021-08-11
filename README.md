# Norman's Sky

![image](https://img.itch.zone/aW1hZ2UvNjI5MDUvMjgzODI5LmdpZg==/original/4eilX2.gif)

The source code for Normans Sky, a game I made in 10 hours for lowrezjam 2016.

[You can play the game on itch here!](https://nothke.itch.io/normans-sky)

* Features a fully procedural endless universe

![image](https://i.imgur.com/skC8t6n.gif)

If you want to read about how I made the game, the technical details and the aftermath of the game, [I wrote a postmortem blog post!](http://nothkedev.blogspot.com/2018/04/normans-sky-2-years-later-joke-made-in.html)

### Changes to the original
* This repo is a "fixed" version, with a lot of 10-hour shenanigans refactored
* Endless universe creation fixed (in the original it was actually broken!)
* Master branch has been updated to Unity 2019 LTS
* Spacecraft feel is slightly different due to the refactorings of Motion.cs
* Planets now have different radii
* Stars now have different colors depending on the temperature

### Versions and Branches
There are currently 4 versions of the game:
* Jam version is the original 10-hour version, only the build exists now on [itch](https://nothke.itch.io/normans-sky). As I didn't use git on the first couple of days, the source in the original 10 hour state doesn't exist any more
* The post-Jam version is the version completed a few days after the jam, and it fixes some bugs, adds improvements a few more features. You can see the list of changes and the build of it on [itch](https://nothke.itch.io/normans-sky). The source of it is basically the first commit of this repo.
* v1 has the same features as the post-jam version, You fly a single ship and explore a procgen universe. The difference from the previous version is that the code is refactored and more bugs are fixed ([see above](#changes-to-the-original)); Build of it will be available soon on [itch](https://nothke.itch.io/normans-sky)
* v2 is a messy, experimental WIP version which I was working on couple of years ago. It features a ship with an improved, more complex cockpit ("s2" scene), and also some art style trials, and experiments with an FPS controller for spacewalking ("walker" scene)

Branches reflect the versions:
* [Master branch](https://github.com/nothke/normans-sky/tree/master) is the stable version of v1
* [v1-2019](https://github.com/nothke/normans-sky/tree/v1-2019) is the current WIP branch of v1, based in Unity 2019 LTS
* [v2-2019](https://github.com/nothke/normans-sky/tree/v2-2019) is the current v2 branch, based in Unity 2019 LTS