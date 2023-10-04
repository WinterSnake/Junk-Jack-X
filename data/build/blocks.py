#!/usr/bin/python
##-------------------------------##
## JJx: Data Builder             ##
## Written By: Ryan Smith        ##
##-------------------------------##
## Blocks                        ##
##-------------------------------##

## Imports
from __future__ import annotations

## Constants
BLOCKS: tuple[Block]

## Functions


## Classes
class Block:

    # -Constructor
    def __init__(
            self,
            _id: int, position: tuple[int, int] | None, name: str,
            alternatives: int = 0, decoration: bool = False, transparent: bool = False
    ) -> None:
        self.id: int = _id
        self.name: str = name
        self.position: tuple[int, int] | None = position
        self.alternatives: int = alternatives
        self.decoration: bool = decoration
        self.transparent: bool = transparent

    # -Dunder Methods

    # -Instance Methods

    # -Class Methods

    # -Static Methods

    # -Properties

    # -Class Properties

    # -Sub-Classes


## Body
BLOCKS = (
        Block(0x0000, None,    "Air", transparent: True),
    Block(0x0001, (1, 0),  "Wooden Lock"),
    Block(0x0001, (2, 0),  "Iron Lock"),
    Block(0x0002, (3, 0),  "Gold Lock"),
    Block(0x0003, (4, 0),  "Titanium Lock"),
    Block(0x0004, (5, 0),  "UNKNOWN"),
    Block(0x0005, (6, 0),  "UNKNOWN"),
    Block(0x0006, (7, 0),  "UNKNOWN"),
    Block(0x0007, (8, 0),  "UNKNOWN"),
    Block(0x0008, (9, 0),  "UNKNOWN"),
    Block(0x0009, (10, 0), "UNKNOWN"),
    Block(0x000A, (11, 0), "UNKNOWN"),
    Block(0x000B, (12, 0), "UNKNOWN"),
    Block(0x000C, (13, 0), "UNKNOWN"),
    Block(0x000D, (14, 0), "UNKNOWN"),
    Block(0x000E, (15, 0), "UNKNOWN"),
    Block(0x000F, (16, 0), "UNKNOWN"),
    Block(0x0011, (17, 0), "White Wool"),
    Block(0x0012, (18, 0), "Grey Wool"),
    Block(0x0013, (19, 0), "Black Wool"),
    Block(0x0014, (20, 0), "Brown Wool"),
    Block(0x0015, (21, 0), "Orange Wool"),
    Block(0x0016, (22, 0), "Red Wool"),
    Block(0x0017, (23, 0), "Rose Wool"),
    Block(0x0018, (24, 0), "Magenta Wool"),
    Block(0x0019, (25, 0), "Purple Wool"),
    Block(0x001A, (26, 0), "Blue Wool"),
    Block(0x001B, (27, 0), "Light Blue Wool"),
    Block(0x001C, (28, 0), "Cyan Wool"),
    Block(0x001D, (29, 0), "Lime Green Wool"),
    Block(0x001E, (30, 0), "Green Wool"),
    Block(0x001F, (31, 0), "Yellow Wool"),
    Block(0x0020, (0, 1),  "Workbench", transparent: True),
    Block(0x0021, (1, 1),  "Forge", transparent: True),
    Block(0x0022, (2, 1),  "Anvil", transparent: True),
    Block(0x0023, (3, 1),  "Tinkerer's Tools", transparent: True),
    Block(0x0024, (4, 1),  "Cooking Ware", transparent: True),
    Block(0x0025, (5, 1),  "Cooking Pot", transparent: True),
    Block(0x0026, (6, 1),  "Woodworker Bench", transparent: True),
    Block(0x0027, (7, 1),  "Spinning Wheel", transparent: True),
    Block(0x0028, (8, 1),  "Loom", transparent: True),
    Block(0x0029, (9, 1),  "Dying Machine", transparent: True),
    Block(0x002A, (10, 1), "Tanning Tools", transparent: True),
    Block(0x002B, (11, 1), "Carpentry Bench", transparent: True),
    Block(0x002C, (12, 1), "UNKNOWN"),
    Block(0x002D, (13, 1), "Chemistry Lab", transparent: True),
    Block(0x002E, (14, 1), "Electrician's Bench", transparent: True),
    Block(0x002F, (15, 1), "Mason Bench", transparent: True),
    Block(0x0030, (16, 1), "UNKNOWN"),
    Block(0x0031, (17, 1), "Tin Ore", alternatives: 2, decoration: True),
    Block(0x0030, (20, 1), "Coal Ore", alternatives: 2, decoration: True),
    Block(0x0037, (23, 1), "Copper Ore", alternatives: 2, decoration: True),
    Block(0x003A, (26, 1), "Iron Ore", alternatives: 2, decoration: True),
    Block(0x003D, (29, 1), "Silver Ore", alternatives: 2, decoration: True),
)
