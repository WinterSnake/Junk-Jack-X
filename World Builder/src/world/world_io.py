##---------------------------------------------##
## JJx World Builder [Python Edition]          ##
## Created By: Ryan Smith                      ##
##---------------------------------------------##
## World I/O                                   ##
##---------------------------------------------##

## Imports
import struct, gzip, zlib, io
from typing import Tuple, Union, BinaryIO

if __name__ == "__main__":
    import world_components
else:
    from . import world_components

## Constants
jjx_map_header = b'\x4A\x4A\x58\x4D\x01\x00\x13\x00\x00\x00\x00\x00\x01\x00\x00\x00\xF0\x00\x00\x00\xE0\x00\x00\x00\x04\x00\x00\x00\xD0\x01\x00\x00'
jjx_adv_header = b'\x4A\x4A\x58\x4D\x02\x00\x02\x00\x00\x00\x00\x00\x01\x00\x00\x00\x24\x00\x00\x00\xE0\x00\x00\x00\x14\x00\x00\x00\x04\x01\x00\x00'

## Functions

##-------- Formatting --------##

def Format_Properties_From_ByteString():
    ''''''
    pass

def Format_ByteString_From_Properties(world_properties: dict) -> Tuple[bytes, bytes]:
    ''''''
    pass

##-------- Compression/Decompression --------##

def Compress_Byte_Segment(bytestring: bytes) -> bytes:
    '''Takes in a bytestring and returns the compressed bytestring using GZip - level 6 encoding as well as the compressed bytestrings' length.'''
    compressed_segment = io.BytesIO()
    with gzip.GzipFile(fileobj = compressed_segment, mode = 'wb', compresslevel = 6, mtime = 0.0) as compression_stream:
        compression_stream.write(bytestring)

    compressed_segment = compressed_segment.getvalue()

    return compressed_segment, len(compressed_segment)

def Decompress_Byte_Segment(compressed_bytestring: bytes) -> bytes:
    '''Takes in a GZip - level 6 compressed bytestring and returns the decompressed bytestring.'''
    return zlib.decompress(compressed_bytestring, 32)

##-------- World Data Retrival --------##

def Import_World(file_path: str) -> Tuple[dict, bytes]:
    ''''''
    pass

## Classes
    # -Constructor

    # -Dunder Methods

    # -Class Methods

    # -Static Methods

    # -Instance Methods

    # -Class Variables

## Main

## __Main__
if __name__ == "__main__":
    pass
else:
    pass

## Temporary