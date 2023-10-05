#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Builder          ##
## Written By: Ryan Smith        ##
##-------------------------------##
## Items                         ##
##-------------------------------##

## Imports
from __future__ import annotations

## Constants
ALTERNATIVES: dict[int, int] = {
    # -Id / Alternatives
      64: 2,  # -Coal
      65: 2,  # -Copper
     256: 2,  # -Iron
     257: 2,  # -Silver
     448: 2,  # -Gold
     449: 2,  # -Mithril
     640: 2,  # -Antanium
     641: 2,  # -Tin
     832: 2,  # -Titanium
    1024: 2,  # -Galvanium
    1216: 2,  # -Silicium
    1413: 1,  # -Citrine
    1414: 1,  # -Tanzanite
    1415: 1,  # -Tourmaline
    1541: 1,  # -Diamond
    1532: 1,  # -Sapphire
    1669: 1,  # -Amethyst
    1670: 1,  # -Emerald
    1797: 1,  # -Topaz
    1798: 1,  # -Ruby
}


## Functions
def load_items(_items: list[dict[str, int | str]]) -> list[Item]:
    """Reads the english treasures list and generate a list of items with sprite positions"""
    items: list[Item] = []
    for _item in _items:
        _id: int = _item['id']
        if _id == 0:
            continue
        name: str = _item['name']
        alternatives: int = ALTERNATIVES[_id] if _id in ALTERNATIVES else 0
        x: int = _id // 64
        y: int = _id % 64
        item = Item(_id, name, alternatives, (x, y))
        items.append(item)
    return items


## Classes
class Item:
    """"""

    # -Constructor
    def __init__(
        self, _id: int, name: str, alternatives: int = 0,
        position: tuple[int, int] | None = None
    ) -> None:
        self.id: int = _id
        self.name: str = name
        self.position: tuple[int, int] | None = position
        self.alternatives: int = alternatives

    # -Properties
    @property
    def sprite_position(self) -> tuple[int, int]:
        return (self.position[0] * 16, self.position[1] * 16)

    # -Class Properties
    size: tuple[int, int] = (16, 16)
