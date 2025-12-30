# Junk Jack X Tools

## References
While most of the data for the player and worlds were figured out by hand - I did end up using the World Manager [header](https://github.com/pixbits/junk-jack-modding-api/blob/master/src/core/data/persistence/WorldManager.h) and [C++](https://github.com/pixbits/junk-jack-modding-api/blob/master/src/core/data/persistence/WorldManager.cpp) files given by Jack (the creator of the game) for some guidance when they were made available. Even with these pieces available, there was much to figure out in turning it into a viable player and world editor.
In addition, the use of the Network Protocol [header](https://github.com/pixbits/junk-jack-modding-api/blob/master/src/core/common/net/NetProtocol.h) was very useful in breaking down the interaction between client and server to write a dedicated server backend.
