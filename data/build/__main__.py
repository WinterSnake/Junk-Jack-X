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
# -Items
items = load_items(data['treasures'])
# --Item: Consts
with open("Items.cs", 'w') as f:
    item_consts: str = []
    f.write("public const ushort None = 0xFFFF;\n")
    for item in items:
        f.write(f"public const ushort {item.name.replace(' ', '')} = 0x{item.id:0>4X};\n")
