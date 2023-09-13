#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Potion Recipes   ##
## Written By: Ryan Smith        ##
##-------------------------------##

## Imports
import shutil
from pathlib import Path

## Constants
PLAYER = Path("Debug.dat")
OFFSET = 4

## Body
for i in range(1, 256):
    name = f"{i:03}"
    nplayer = Path(f"recipes/{PLAYER.stem}{name}{PLAYER.suffix}")
    shutil.copy(PLAYER, nplayer)
    with nplayer.open('r+b') as f:
        f.seek(0x5D, 0)
        f.write(bytes(name, 'ascii'))
        f.seek(OFFSET + 0x498)
        f.write(i.to_bytes(1))
