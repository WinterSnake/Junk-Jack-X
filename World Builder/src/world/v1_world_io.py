##---------------------------------------------##
## JJx World Builder [Python Edition]          ##
## Created By: Ryan Smith                      ##
##---------------------------------------------##
## World I/O                                   ##
##---------------------------------------------##

## Imports
from typing import Tuple, BinaryIO

import struct, gzip, zlib, io

if __name__ == "__main__":
    import world_components
else:
    from . import world_components

## Constants
jjx_map_header = b'\x4A\x4A\x58\x4D\x01\x00\x13\x00\x00\x00\x00\x00\x01\x00\x00\x00\xF0\x00\x00\x00\xE0\x00\x00\x00\x04\x00\x00\x00\xD0\x01\x00\x00'
jjx_adv_header = b'\x4A\x4A\x58\x4D\x02\x00\x02\x00\x00\x00\x00\x00\x01\x00\x00\x00\x24\x00\x00\x00\xE0\x00\x00\x00\x14\x00\x00\x00\x04\x01\x00\x00'

## Functions

##-------- Format Compression/Decompression --------##

def Decompress_Byte_Segment(bytestring: bytes) -> bytes:
    '''Returns the decompressed bytestring of the given bytestring using GZip encoding - level 6.'''
    return zlib.decompress(bytestring, 32)

def Compress_Byte_Segment(bytestring: bytes) -> bytes:
    '''Returns the compressed bytestring of the given bytestring in GZip encoding - level 6.'''
    compressed_segment = io.BytesIO()
    with gzip.GzipFile(fileobj = compressed_segment, mode = 'wb', compresslevel = 6, mtime = 0.0) as compression_stream:
        compression_stream.write(bytestring)

    return compressed_segment.getvalue()

##-------- Retrieve World Data --------##

def Import_World(file_path: str) -> Tuple[dict, bytes]:
    '''
    Returns a dictionary of the world properties and a bytestring of the decompressed world.'''
    with open(file_path, 'rb') as importing_world:
        file_header = importing_world.read(32)

        # -Single World Import
        if (file_header == jjx_map_header):
            print("Single-World Import")

            return Parse_World_File(importing_world, debug_text = True)

        # -Adventure Worlds Import
        elif (file_header == jjx_adv_header):
            print("Multi-World Import")

        # -Invalid World Format
        else:
            print("Invalid World Format.")
            return None

def Parse_World_File(world_file_object: BinaryIO, **kwargs) -> Tuple[dict, bytes]:
    '''
    Returns a dictionary of the world properties and a bytestring of the decompressed world.
    If debug mode is enabled, will return world properties, decompressed world, full header (bytes), and discover footer (bytes) and the default footer (bytes).'''

    ## Header Data
    world_file_object.seek(40, 0)
    header_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]
    border_size_numerical = (header_size_numerical - 464)
    world_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]
    gamemode_loader = world_file_object.read(4)
    footer_location = struct.unpack('<I', world_file_object.read(4))[0]

        # -Gamemode Handler
    if gamemode_loader == b'\x12\x00\x00\x00' or gamemode_loader == b'\x00\x00\x00\x00': # -Creative / Flat || Auto
        pass
        # -Discover Footer Segment Null
        discover_footer_size_numerical = None
        discover_footer_location = None

        ## Footer Normal Data
            # -Chest Location/Size
        world_file_object.seek(20, 1)
        chest_location = struct.unpack('<I', world_file_object.read(4))[0]
        chest_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]

            # -Entity Location/Size
        world_file_object.seek(136, 1)
        entity_location = struct.unpack('<I', world_file_object.read(4))[0]
        entity_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]

    elif gamemode_loader == b'\x03\x00\x00\x01': # -Adventure / Survival
        pass
        # -Discover Footer Segment Set
        discover_footer_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]
        discover_footer_location = footer_location
        world_file_object.seek(4, 1)
        footer_location = struct.unpack('<I', world_file_object.read(4))[0]

        ## Footer Normal Data
            # -Chest Location/Size
        world_file_object.seek(20, 1)
        chest_location = struct.unpack('<I', world_file_object.read(4))[0]
        chest_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]

            # -Entity Location/Size
        world_file_object.seek(136, 1)
        entity_location = struct.unpack('<I', world_file_object.read(4))[0]
        entity_size_numerical = struct.unpack('<I', world_file_object.read(4))[0]

    ## Properties Data
    world_file_object.seek(256, 0)
    world_properties_data = world_file_object.read(header_size_numerical - 256)
    world_properties = world_components.World_Properties.Create_From_ByteString(world_properties_data)

    ## Decompressed Data
    world_file_object.seek(header_size_numerical, 0)
    compressed_world = world_file_object.read(world_size_numerical)
    decompressed_world = Decompress_Byte_Segment(compressed_world)

    ## Footer Data
        # -Chest Data
    world_file_object.seek(chest_location, 0)
    chest_data = world_file_object.read(chest_size_numerical)

        # -Entity Data
    world_file_object.seek(entity_location, 0)
    entity_data = world_file_object.read(entity_size_numerical)

    ## Debug
        # -Debug Mode

        # -Debug Text
    if 'debug_text' in kwargs and kwargs['debug_text'] == True:
        print(f"Header Size: {header_size_numerical} bytes.")
        print(f"Border Size: {border_size_numerical} bytes.")
        print(f"World Size: {world_size_numerical} bytes.")

        if discover_footer_location:
            print(f"Discover Footer Location: {discover_footer_location}.")
            print(f"Discover Footer Size: {discover_footer_size_numerical} bytes")

        print(f"Footer Location: {footer_location}.")

        print("Footer Data:"
            f"\n\tChest Location: {chest_location}"
            f"\n\tChest Size: {chest_size_numerical}"
            f"\n\tEntity Location: {entity_location}"
            f"\n\tEntity Size: {entity_size_numerical}")

        print(world_properties)

    return decompressed_world, world_properties

##-------- Store World Data --------##

def Export_World(world_properties: dict, decompressed_world: bytes, folder_path: str) -> None:
    '''
    Returns none, but writes the compressed file format to the folder path given.'''
    pass

## Main

## Temporary
data = Import_World("debug/Debug_Map[0].dat")
#data = Import_World("debug/Debug_Map[1].dat")
#data = Import_World("debug/Debug_Adv[0].dat")
#data = Import_World("debug/[Entity] Bear Brown-O.dat")
#data = Import_World("debug/[B-Data] Wooden Chest-O.dat")

#print(data)