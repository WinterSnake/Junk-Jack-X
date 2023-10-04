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
            alternatives: int = 0, decoration: bool = False,
            foreground_only: bool = False, transparent: bool = False
    ) -> None:
        self.id: int = _id
        self.name: str = name
        self.position: tuple[int, int] | None = position
        self.alternatives: int = alternatives
        self.foreground_only: bool = foreground_only
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
    Block(0x0002, (2, 0),  "Iron Lock"),
    Block(0x0003, (3, 0),  "Gold Lock"),
    Block(0x0004, (4, 0),  "Titanium Lock"),
    Block(0x0005, (5, 0),  "UNKNOWN"),
    Block(0x0006, (6, 0),  "UNKNOWN"),
    Block(0x0007, (7, 0),  "UNKNOWN"),
    Block(0x0008, (8, 0),  "UNKNOWN"),
    Block(0x0009, (9, 0),  "UNKNOWN"),
    Block(0x000A, (10, 0), "UNKNOWN"),
    Block(0x000B, (11, 0), "UNKNOWN"),
    Block(0x000C, (12, 0), "UNKNOWN"),
    Block(0x000D, (13, 0), "UNKNOWN"),
    Block(0x000E, (14, 0), "UNKNOWN"),
    Block(0x000F, (15, 0), "UNKNOWN"),
    Block(0x0010, (16, 0), "UNKNOWN"),
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
    Block(0x0020, (0, 1),  "Workbench", foreground_only: True, transparent: True),
    Block(0x0021, (1, 1),  "Forge", foreground_only: True, transparent: True),
    Block(0x0022, (2, 1),  "Anvil", foreground_only: True, transparent: True),
    Block(0x0023, (3, 1),  "Tinkerer's Tools", foreground_only: True, transparent: True),
    Block(0x0024, (4, 1),  "Cooking Ware", foreground_only: True, transparent: True),
    Block(0x0025, (5, 1),  "Cooking Pot", foreground_only: True, transparent: True),
    Block(0x0026, (6, 1),  "Woodworker Bench", foreground_only: True, transparent: True),
    Block(0x0027, (7, 1),  "Spinning Wheel", foreground_only: True, transparent: True),
    Block(0x0028, (8, 1),  "Loom", foreground_only: True, transparent: True),
    Block(0x0029, (9, 1),  "Dying Machine", foreground_only: True, transparent: True),
    Block(0x002A, (10, 1), "Tanning Tools", foreground_only: True, transparent: True),
    Block(0x002B, (11, 1), "Carpentry Bench", foreground_only: True, transparent: True),
    Block(0x002C, (12, 1), "UNKNOWN"),
    Block(0x002D, (13, 1), "Chemistry Lab", foreground_only: True, transparent: True),
    Block(0x002E, (14, 1), "Electrician's Bench", foreground_only: True, transparent: True),
    Block(0x002F, (15, 1), "Mason Bench", foreground_only: True, transparent: True),
    Block(0x0030, (16, 1), "UNKNOWN"),
    Block(0x0031, (17, 1), "Tin Ore", alternatives: 2, decoration: True),
    Block(0x0030, (20, 1), "Coal Ore", alternatives: 2, decoration: True),
    Block(0x0037, (23, 1), "Copper Ore", alternatives: 2, decoration: True),
    Block(0x003A, (26, 1), "Iron Ore", alternatives: 2, decoration: True),
    Block(0x003D, (29, 1), "Silver Ore", alternatives: 2, decoration: True),
    Block(0x0040, (0, 2),  "Gold Ore", alternatives: 2, decoration: True),
    Block(0x0042, (3, 2),  "Diamond Ore", alternatives: 2, decoration: True),
    Block(0x0043, (6, 2),  "Amethyst Ore", alternatives: 2, decoration: True),
    Block(0x0046, (9, 2),  "Topaz Ore", alternatives: 2, decoration: True),
    Block(0x004C, (12, 2), "Emerald Ore", alternatives: 2, decoration: True),
    Block(0x004F, (15, 2), "Ice"),
    Block(0x0050, (16, 2), "Stone", alternatives: 1),
    Block(0x0052, (17, 2), "Basalt"),
    Block(0x0054, (19, 2), "Bedrock"),
    Block(0x0055, (20, 2), "Boulder"),
    Block(0x0056, (21, 2), "Antanium Ore", alternatives: 2, decoration: True),
    Block(0x0059, (25, 2), "UNKNOWN"),
    Block(0x005A, (26, 2), "UNKNOWN"),
    Block(0x005B, (27, 2), "Mithril Ore", alternatives: 2, decoration: True),
    Block(0x005E, (30, 2), "UNKNOWN"),
    Block(0x005F, (31, 2), "UNKNOWN"),
    Block(0x0060, (0, 3),  "Wood Ladder", foreground_only: True, transparent: True),
    Block(0x0061, (1, 3),  "Bamboo Ladder", foreground_only: True, transparent: True),
)
