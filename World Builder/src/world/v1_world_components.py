##---------------------------------------------##
## JJx World Builder [Python Edition]          ##
## Created By: Ryan Smith                      ##
##---------------------------------------------##
## World I/O Componenets                       ##
##---------------------------------------------##

## Imports
from typing import Union, Tuple, List
import json, os.path as ospath, struct

## Constants
__location = ospath.abspath(ospath.join(ospath.dirname(__file__), "world_components.json"))

## Functions

##-------- Update Data --------##
def __Fix_JSON_Component(str_id: str, num_id: int) -> dict:
    '''Recieves a string and a numerical id and returns a value specific dictionary with the string, numerical id, and hexadecimal id (in little endian format) of the given data.'''
    return {
        'str_id' : str_id,
        'num_id' : num_id,
        'hex_id' : struct.pack('<h', num_id).hex().upper()
    }

def Update_JSON_Components(file_path: str) -> None:
    '''For updating world json data (items | entities) to current english.json game data. Called with a file path string to the english.json and outputs the updated syntax into "world_components.json" in this directory.'''
    ## Get JSON Data
    with open(file_path, 'r', encoding = 'utf-8') as updating_file:
        update_data = json.load(updating_file)

    ## Reconstruct Items
    items = [__Fix_JSON_Component(item['name'], item['id']) for item in update_data['treasures']]

    ## Reconstruct Entities
    entities = [__Fix_JSON_Component(mob, i) for i, mob in enumerate(update_data['mobs'])]

    with open(__location, 'w') as updated_file:
        json.dump(
        {
            'item'   : items,
            'entity' : entities,
        }, updated_file, indent = 4)

##-------- Component Data --------##

def String_To_Hex(str_id: str, component_type: Union['item', 'entity']) -> hex:
    '''Takes in a component name string and a component type string and returns the component type's hex string.'''
    for component in world_components[component_type]:
        if str_id == component['str_id']:
            return component['hex_id']

def Hex_To_String(hex_id: Union[bytes, hex], component_type: Union['item', 'entity']) -> str:
    '''Takes in a byte string or hex string and a component type string and returns the component type's string name.'''
    if isinstance(hex_id, bytes):
        hex_id = hex_id.hex().upper()

    for component in world_components[component_type]:
        if hex_id == component['hex_id']:
            return component['str_id']

##-------- Entity Conversion --------##

def Create_Entity_List_From_ByteString(entity_list_bytestring: bytes) -> list:
    ''''''
    pass

def Create_ByteString_From_Entity_List(entity_list: list) -> bytes:
    ''''''
    pass

##-------- Chest Conversion --------##

def Create_Chest_List_From_ByteString(chest_list_bytestring: bytes) -> list:
    ''''''
    pass

def Create_ByteString_From_Chest_List(chest_list: list) -> bytes:
    ''''''
    pass

## Classes
class Item(object):
    pass
    # -Constructor
    def __init__(self):
        self.id = ""
        self.count = 0
        self.durability = 0
        self.render_id = 0
        self.modifier = ""

    # -Dunder Methods

    # -Class Methods
    @classmethod
    def Create_From_ByteString(cls, bytestring: bytes):
        ''''''
        pass

    # -Static Methods

    # -Instance Methods
    def Translate_To_Dictionary(self) -> dict:
        ''''''
        pass

    def Translate_To_ByteString(self) -> bytes:
        ''''''
        pass

    # -Class Variables

class Entity(object):
    pass
    # -Constructor
    def __init__(self):
        self.id = ""
        self.position = tuple()

    # -Dunder Methods

    # -Class Methods
    @classmethod
    def Create_From_ByteString(cls, bytestring: bytes):
        ''''''
        pass

    # -Static Methods

    # -Instance Methods
    def Translate_To_ByteString(self) -> bytes:
        ''''''
        pass

    # -Class Variables

class Chest(object):
    pass
    # -Constructor
    def __init__(self):
        self.slots = 0
        self.position = tuple()
        self.item_container = list()

    # -Dunder Methods

    # -Class Methods
    @classmethod
    def Create_From_ByteString(cls, bytestring: bytes):
        ''''''
        pass

    # -Static Methods

    # -Instance Methods
    def Translate_To_ByteString(self) -> bytes:
        ''''''
        pass

    # -Class Variables

class World_Properties(object):
    pass
    # -Constructor
    def __init__(self, name: str, size: str, planet: str, gamemode: str, spawn: List[int], player: List[int], border: List[int], skybox_offset: int, last_played: int):
        self.world_name = name
        self.world_size = size
        self.planet = planet
        self.gamemode = gamemode
        self.spawn_position = spawn
        self.player_position = player
        self.border_array = border
        self.skybox_offset = skybox_offset
        self.last_played = last_played

    # -Dunder Methods
    def __str__(self):
        return (f"{self.world_name} Properties:"
                f"\n\tSize: {self.world_size}"
                f"\n\tPlanet: {self.planet}"
                f"\n\tGamemode: {self.gamemode}"
                f"\n\tSpawn Position: {self.spawn_position}"
                f"\n\tPlayer Position: {self.player_position}"
                f"\n\tWorld Border: {self.border_array}"
                f"\n\tSkybox Offset: {self.skybox_offset}"
                f"\n\tLast Played: {self.last_played}")

    def __repr__(self):
        return (f"Properties('{self.world_name}', '{self.world_size}', '{self.planet}', '{self.gamemode}', {self.spawn_position}, {self.player_position}, {self.border_array}, {self.skybox_offset}, {self.last_played})")

    # -Class Methods
    @classmethod
    def Create_From_ByteString(cls, bytestring: bytes):
        '''Returns a world properties object from the given bytestring.'''

        ## Parameter Strip
        world_name = bytestring[8:39].decode('utf-8').strip('\0')
        player_position = list(struct.unpack('<2h', bytestring[60:64]))
        spawn_position = list(struct.unpack('<2h', bytestring[64:68]))
        last_played = struct.unpack('<Q', bytestring[0:8])[0]
        border_array = World_Properties._Create_Border_Array_From_ByteString(bytestring[208:])
        skybox_offset = struct.unpack('<B', bytestring[75:76])[0]
        planet, gamemode, world_size = World_Properties._Compare_Property_Flag(planet_bytestring = bytestring[68:70], gamemode_bytestring = bytestring[73:74], size_bytestring = bytestring[74:75])

        return cls(world_name, world_size, planet, gamemode, spawn_position, player_position, border_array, skybox_offset, last_played)

    # -Static Methods
    @staticmethod
    def _Compare_Property_Flag(planet_bytestring: bytes = None, gamemode_bytestring: bytes = None, size_bytestring: bytes = None) -> Tuple[bytes, bytes, bytes]:
        '''Recieves bytestring for planet, gamemode, and world size. Returns the string variant of the given bytestrings.'''
        # -Planet Parameter
        if planet_bytestring == b'\x01\x00':
            planet = "Terra"
        elif planet_bytestring == b'\x02\x00':
            planet = "Seth"
        elif planet_bytestring == b'\x04\x00':
            planet = "Alba"
        elif planet_bytestring == b'\x08\x00':
            planet = "Xeno"
        elif planet_bytestring == b'\x10\x00':
            planet = "Magmar"
        elif planet_bytestring == b'\x20\x00':
            planet = "Cryo"
        elif planet_bytestring == b'\x40\x00':
            planet = "Yuca"
        elif planet_bytestring == b'\x80\x00':
            planet = "Lilith"
        elif planet_bytestring == b'\x00\x01':
            planet = "Thetis"
        elif planet_bytestring == b'\x00\x02':
            planet = "Mykon"
        elif planet_bytestring == b'\x00\x04':
            planet = "Umbra"
        else:
            planet = None

        # -Gamemode Parameter
        if gamemode_bytestring == b'\x00':
            gamemode = "Survival"
        elif gamemode_bytestring == b'\x01':
            gamemode = "Creative"
        elif gamemode_bytestring == b'\x02':
            gamemode = "Flat"
        elif gamemode_bytestring == b'\x03':
            gamemode = "Adventure"
        else:
            gamemode = None

        # -Size Parameter
        if size_bytestring == b'\x00':
            size = "Tiny"
        elif size_bytestring == b'\x01':
            size = "Small"
        elif size_bytestring == b'\x02':
            size = "Normal"
        elif size_bytestring == b'\x03':
            size = "Large"
        elif size_bytestring == b'\x04':
            size = "Huge"
        else:
            size = None

        return planet, gamemode, size

    @staticmethod
    def _Create_Border_Array_From_ByteString(border_bytestring: bytes):
        '''Returns a list of integers for each size of the border.'''
        border_array = []
        for i in range(int(len(border_bytestring) / 32)):
            if i % 2 == 0:
                pre_border_array = struct.unpack('<32h', border_bytestring[i : i + 64])

                for border_value in pre_border_array:
                    border_array.append(border_value)

        return border_array

    # -Instance Methods
    def Translate_To_Dictionary(self) -> dict:
        ''''''
        return {
            "world" : {
                "name" : self.world_name,
                "size" : self.world_size
            },
            "border" : self.border_array
        }

    def Translate_To_ByteString(self) -> bytes:
        ''''''
        pass

    # -Class Variables


## Main

## __Main__
if __name__ == "__main__":
    if ospath.exists("english.json"):
        print("Updating from english.json!")
        Update_JSON_Components("english.json")
    else:
        print("english.json not found!")
else:
    with open(__location, 'r') as component_data:
        world_components = json.load(component_data)

## Temporary