# Junk Jack X Editor
This is a cross-platform player and world editor for the game Junk Jack written in C# and using the Raylib_cs for GUI and window/event handling.
While I have only tested the Steam version of the game, the editor should work for: iOS, Android, and Switch versions as well.

## References
While most of the data for the player and worlds were figured out by hand - I did use the WorldManager [header](https://github.com/pixbits/junk-jack-modding-api/blob/master/src/core/data/persistence/WorldManager.h) and [C++](https://github.com/pixbits/junk-jack-modding-api/blob/master/src/core/data/persistence/WorldManager.cpp) file given by Jack (the creator of the game) for some guidance when they were made available. Even with these pieces available, there was much to figure out in turning it into a viable player and world editor.
