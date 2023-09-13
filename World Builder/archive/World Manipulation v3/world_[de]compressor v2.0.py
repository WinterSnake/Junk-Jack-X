##----------------------------------------------##
## Junk Jack X World Builder - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## World Object Data							##
##----------------------------------------------##

## Imports
import os.path as osp, gzip, zlib, json, struct, io, shutil

## Constants
DEBUG = True

## Functions

# -Yield World Key
def Get_World(dictionary):
	for key, value in dictionary.items():
		yield key, value

# -Peek: Call Function
def Peek(file_object, peek_distance):
	''''''
	cur_pos = file_object.tell()
	peeked_data = file_object.read(peek_distance)
	file_object.seek(cur_pos)
	return peeked_data

# -Byte Chunk: Call Function
def Read_Chunk(file_object, chunk_size):
	''''''
	while True:
		chunk_data = file_object.read(chunk_size)
		if chunk_data:
			yield from chunk_data
		else:
			break

# -Byte Chunk: Call Function
def Delimiter_Chunk(file_object, chunk_size, delimiter):
	''''''
	# -Defines end data as empty byte string
	__chunk_end = b''

	while True:
		# -Reads chunk_size from file_object
		__chunk_data = file_object.read(chunk_size)

		# -Breaks loop at empty byte string
		if not __chunk_data:
			break

		# -Splits data at delimiter
		__partition_data = __chunk_data.split(delimiter)

		# -Add unsplit data to end data
		if len(__partition_data) == 1:
			__chunk_end += __partition_data[0]

		else:
			# -Return end data before split + delimiter
			yield __chunk_end + __partition_data[0] + delimiter

			# -Unknown
			# for p in __partition_data[1:-1]:
			# 	yield p

			# -Break end data into new split
			__chunk_end = __partition_data[-1]

	# -Return last end data stream
	yield __chunk_end



# -Import World: Call Function
def Import_World(file_path_world = None):
	''''''
	# -Check File Path
	if file_path_world is not None:
		if osp.isfile(file_path_world):
			# -Open File
			file_object_world = open(file_path_world, 'rb')
			print(f"File object opened @'{file_path_world}'")
		else:
			print(f"Non-existent file @'{file_path_world}'")

	# -Check File Object
	if file_object_world is None:
		return None

	# -Check File Header
	if file_object_world.read(4) == b'\x4A\x4A\x58\x4D':
		print(f"Valid file format found.\n")
		file_object_world.read(28)
		# -Finish Header
	else:
		print(f"Invalid file format found.\n")
		file_object_world.close()
		return None

	## World Reading
		# -Data Unknown :Skip
	file_object_world.read(9)
	
		# -Data: World Lock Size :Get
	__get_size = file_object_world.read(1)
	
		# -Tiny
	if __get_size == b'\x05':
		__border_buffer = 512
		__size_buffer = b'\x00\x00\x10\x00'
	
		# -Small
	elif __get_size == b'\x07':
		__border_buffer = 768
		__size_buffer = b'\x00\x00\x30\x00'
	
		# -Normal
	elif __get_size == b'\x09':
		__border_buffer = 1024
		__size_buffer = b'\x00\x00\x40\x00'
	
		# -Large
	elif __get_size == b'\x11':
		__border_buffer = 2048
		__size_buffer = b'\x00\x00\xC0\x00'
	
		# -Huge
	elif __get_size == b'\x21':
		__border_buffer = 4096
		__size_buffer = b'\x00\x00\x00\x02'

		# -Data Unknown :Skip
	file_object_world.read(222)

		# -Data: World Last Played Time :Get
	pass

		# -Data: World Name :Get
	world_name = file_object_world.read(31).decode('utf-8').rstrip('\0')

		# -Data Unknown :Skip
	file_object_world.read(21)

		# -Data: Player Position :Get
	player_position = [struct.unpack('<h', file_object_world.read(2))[0], struct.unpack('<h', file_object_world.read(2))[0]]

		# -Data: Spawn Position :Get
	spawn_position = [struct.unpack('<h', file_object_world.read(2))[0], struct.unpack('<h', file_object_world.read(2))[0]]

		# -Data: World Planet :Get
	__get_planet = file_object_world.read(2)

	if __get_planet == b'\x01\x00':
		planet = "Terra"
	elif __get_planet == b'\x02\x00':
		planet = "Seth"
	elif __get_planet == b'\x04\x00':
		planet = "Alba"
	elif __get_planet == b'\x08\x00':
		planet = "Xeno"
	elif __get_planet == b'\x10\x00':
		planet = "Magmar"
	elif __get_planet == b'\x20\x00':
		planet = "Cryo"
	elif __get_planet == b'\x40\x00':
		planet = "Yuca"
	elif __get_planet == b'\x80\x00':
		planet = "Lilith"
	elif __get_planet == b'\x00\x01':
		planet = "Thetis"
	elif __get_planet == b'\x00\x02':
		planet = "Mykon"
	elif __get_planet == b'\x00\x04':
		planet = "Umbra"

		# -Data Unknown :Skip
	file_object_world.read(3)

		# -Data: Gamemode :Get
	__get_gamemode = file_object_world.read(1)

	if __get_gamemode == b'\x00':
		gamemode = "Survival"
	elif __get_gamemode == b'\x01':
		gamemode = "Creative"
	elif __get_gamemode == b'\x02':
		gamemode = "Flat"

		# -Data Unknown :Skip
	file_object_world.read(6)

		# -Data: Null Byte List :Skip
	file_object_world.read(128)

		# -Data: World Background Border :Get
	border_array = []
	while __border_buffer > 0:
		border_array.append(struct.unpack('<h', file_object_world.read(2))[0])
		__border_buffer -= 1

	## Dump To File

		# -JSON
	properties = {
		world_name : {
			'planet' : planet,
			'gamemode' : gamemode,
			'spawn position' : spawn_position,
			'player position' : player_position,
			'border array' : border_array
		}
	}
	with open(f"{world_name} Properties.json", 'w') as properties_export:
		json.dump(properties, properties_export, indent = 4)

		# -Header
	if DEBUG:
		__cur_pos = file_object_world.tell()
		file_object_world.seek(0)

		with open(f"{world_name} -Header.dat", 'wb') as header_out:
			header_out.write(file_object_world.read(336))

		file_object_world.seek(__cur_pos)

		# -Data: World Body + World Footer

	for i, bytestr in enumerate(Delimiter_Chunk(file_object_world, 512, __size_buffer)):
			# -World Body: Compressed World Format
		if i == 0:
			if DEBUG:
				print(bytestr)
				with open(f"{world_name} -Compressed.dat", 'wb') as compressed_out:
					compressed_out.write(bytestr)

			decompressed_string = zlib.decompress(bytearray(bytestr), 32)
			with open(f"{world_name} -Decompressed.dat", 'wb') as decompressed_out:
				decompressed_out.write(decompressed_string)

			# -World Footer
		if i == 1:
			footer_string = bytestr
			with open(f"{world_name} -Footer.dat", 'wb') as footer_out:
				footer_out.write(bytestr)

	## DEBUG
	if DEBUG:
		print(f"Debug: Border Size: {__border_buffer}")
		print(f"Debug: Compression Footer: {__size_buffer}")
		print(f"Debug: World Name = {world_name}")
		#print(f"Debug: Player X = {player_position[0]}")
		#print(f"Debug: Player Y = {player_position[1]}")
		print(f"Debug: Player Position = {player_position}")
		#print(f"Debug: Spawn X = {spawn_position[0]}")
		#print(f"Debug: Spawn Y = {spawn_position[1]}")
		print(f"Debug: Spawn Position = {spawn_position}")
		print(f"Debug: Planet = {planet}")
		print(f"Debug: Gamemode = {gamemode}")
		print(f"Debug: Border = {border_array}")

	# -End Import
	file_object_world.close()
	return properties, decompressed_string, footer_string

# -Export World: Call Function
def Export_World(file_path_json = None, file_path_world = None, file_path_footer = None):
	''''''
	# -Constants
	__size_modifiers = [8, 8, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 8, 16]

	# -Check File Path: JSON
	if file_path_json is not None:
		if osp.isfile(file_path_json):
			# -Open File
			file_object_json = open(file_path_json, 'rb')
			print(f"File object opened @'{file_path_json}'")
		else:
			print(f"Non-existent file @'{file_path_json}'")

	# -Check File Size: World
	__world_size = osp.getsize(file_path_world)

	if __world_size == 1048576:
		__size_buffer = b'\x05'
		__compression_offset = 1488
	elif __world_size == 3145728:
		__size_buffer = b'\x07'
		__compression_offset = 2000

	elif __world_size == 4194304:
		__size_buffer = b'\x09'
		__compression_offset = 2512

	elif __world_size == 12582912:
		__size_buffer = b'\x11'
		__compression_offset = 4560

	elif __world_size == 33554432:
		__size_buffer = b'\x21'
		__compression_offset = 8656
	else:
		return None

	# -Check File Path: World
	if file_path_world is not None:
		if osp.isfile(file_path_world):
			# -Open File
			file_object_world = open(file_path_world, 'rb')
			print(f"File object opened @'{file_path_world}'")
		else:
			print(f"Non-existent file @'{file_path_world}'")

	# -Check File Path: Footer
	if file_path_footer is not None:
		if osp.isfile(file_path_footer):
			# -Open File
			file_object_footer = open(file_path_footer, 'rb')
			print(f"File object opened @'{file_path_footer}'\n")
		else:
			print(f"Non-existent file @'{file_path_footer}'\n")

	# -Check File Objects
	if file_object_json is None or file_object_world is None or file_object_footer is None:
		return None

	# -Get JSON Contents
	__world_name, __world_properties = next(Get_World(json.load(file_object_json)))

	#print(__world_properties)

	# -Compress World Contents
	__compressed_data = io.BytesIO()

	with gzip.GzipFile(fileobj = __compressed_data, mode = 'wb', compresslevel = 6, mtime = 0.0) as compressed_stream:
		shutil.copyfileobj(file_object_world, compressed_stream)

	__byte_space = __compressed_data.getbuffer().nbytes

	# -Compress World Header Lock
	__header_values = [__byte_space, __byte_space + __compression_offset]
	for val in __size_modifiers:
		__header_values.append(__header_values[-1] + val)

	#print(__header_values)

	# -Format World
	with open(f"{__world_name} -Formatted.dat", 'wb') as formatted_world:
		# -Header
		formatted_world.write(b'\x4A\x4A\x58\x4D\x01\x00\x13\x00\x00\x00\x00\x00\x01\x00\x00\x00\xF0\x00\x00\x00\xE0\x00\x00\x00\x04\x00\x00\x00\xD0\x01\x00\x00')

		# -World Lock Intro
		formatted_world.write(b'\x00\x00\x00\x00\x02\x00\x01\x01\xD0')

		# -World Lock Size
		formatted_world.write(__size_buffer + b'\x00\x00')

		# -World Size Lock
		for index, val in enumerate(__header_values):
			if index == 0:
				spacer = bytes(4)
			elif index == len(__header_values) - 1:
				spacer = bytes(0)
			else:
				spacer = bytes(8)
			formatted_world.write(struct.pack('<i', val) + spacer)

		# -Unknown Data
		formatted_world.write(bytes(40))

		# -World Name
		formatted_world.write(__world_name.encode('utf-8'))
		formatted_world.write(bytes(32 - len(__world_name)))

		# Unknown Data
		formatted_world.write(bytes(20))

		# -Player Position
		formatted_world.write(struct.pack('<h', __world_properties['player position'][0]) + struct.pack('<h', __world_properties['player position'][1]))

		# -Spawn Position
		formatted_world.write(struct.pack('<h', __world_properties['spawn position'][0]) + struct.pack('<h', __world_properties['spawn position'][1]))

		# -Planet
		if __world_properties['planet'] == "Terra":
			formatted_world.write(b'\x01\x00')

		elif __world_properties['planet'] == "Seth":
			formatted_world.write(b'\x02\x00')

		elif __world_properties['planet'] == "Alba":
			formatted_world.write(b'\x04\x00')

		elif __world_properties['planet'] == "Xeno":
			formatted_world.write(b'\x08\x00')

		elif __world_properties['planet'] == "Magmar":
			formatted_world.write(b'\x10\x00')

		elif __world_properties['planet'] == "Cryo":
			formatted_world.write(b'\x20\x00')

		elif __world_properties['planet'] == "Yuca":
			formatted_world.write(b'\x40\x00')

		elif __world_properties['planet'] == "Lilith":
			formatted_world.write(b'\x80\x00')

		elif __world_properties['planet'] == "Thetis":
			formatted_world.write(b'\x00\x01')

		elif __world_properties['planet'] == "Mykon":
			formatted_world.write(b'\x00\x02')

		elif __world_properties['planet'] == "Umbra":
			formatted_world.write(b'\x00\x04')

		else:
			formatted_world.write(b'\x01\x00')

		# -Unknown Data
		formatted_world.write(bytes(3))

		# -Game Mode
		if __world_properties['gamemode'] == "Survival":
			formatted_world.write(b'\x00')
		elif __world_properties['gamemode'] == "Creative":
			formatted_world.write(b'\x01')
		elif __world_properties['gamemode'] == "Flat":
			formatted_world.write(b'\x02')
		else:
			formatted_world.write(b'\x01')
		
		# -Unknown Data
		formatted_world.write(b'\x00\x04\x00\x00\x00\x00')
			
		# -Null Byte Spacer
		formatted_world.write(bytes(128))

		# -World Border
		for x in __world_properties['border array']:
			formatted_world.write(struct.pack('<h', x))

		# -Compressed World
		formatted_world.write(__compressed_data.getvalue())

		# -World Footer
		shutil.copyfileobj(file_object_footer, formatted_world)

	# -End Export
	file_object_json.close()
	file_object_world.close()
	file_object_footer.close()

	return formatted_world

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
	import sys

	## Import World
	if len(sys.argv) == 2:
		Import_World(file_path_world = sys.argv[1])

	## Export World
	elif len(sys.argv) == 3 or len(sys.argv) == 4:
		json_path = decompressed_path = footer_path = None

		for arg in sys.argv:
			if osp.splitext(arg)[1] == ".json":
				json_path = arg
			elif osp.split(osp.splitext(arg)[0])[1][-13:] == "-Decompressed":
				decompressed_path = arg
			elif osp.split(osp.splitext(arg)[0])[1][-7:] == "-Footer":
				footer_path = arg

		Export_World(file_path_json = json_path, file_path_world = decompressed_path, file_path_footer = footer_path)
else:
	pass

## Temporary