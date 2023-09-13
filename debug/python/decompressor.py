#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Decompressor     ##
## Written By: Ryan Smith        ##
##-------------------------------##

## Imports
import gzip

## Constants
world_string: bytes

## Body
with open("../debug.dat", 'rb') as f:
    f.seek(0x5D0)
    world_string = f.read(81660)
uncompressed = gzip.decompress(world_string)
print(f"{len(uncompressed)}")
with open("debug.compressed", 'wb') as f:
    f.write(world_string)
with open("debug.decompressed", 'wb') as f:
    f.write(uncompressed)
with open("debug.recompressed", 'wb') as f:
    f.write(gzip.compress(uncompressed, compresslevel=6))
