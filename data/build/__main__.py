#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Builder          ##
## Written By: Ryan Smith        ##
##-------------------------------##

## Imports
import json
import sys

## Body
with open(sys.argv[1]) as f:
    data = json.load(f)
items = data['treasures']
mobs = data['mobs']
print(mobs)
