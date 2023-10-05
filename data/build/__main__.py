#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Builder          ##
## Written By: Ryan Smith        ##
##-------------------------------##

## Imports
import json
import sys
from blocks import BLOCKS, Block
from items import load_items, Item

## Body
with open(sys.argv[1]) as f:
    data = json.load(f)
items = load_items(data['treasures'])
