##----------------------------------------------##
## Junk Jack X World Editor - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## World Editor v5 - Import/Export				##
##----------------------------------------------##

## Imports
import struct, gzip, zlib, io
from data import Create_Entity_List_From_ByteString, Create_Chest_List_From_ByteString, Create_ByteString_From_Entity_List, Create_ByteString_From_Chest_List

## Constants
jjxheader = b'\x4A\x4A\x58\x4D\x01\x00\x13\x00\x00\x00\x00\x00\x01\x00\x00\x00\xF0\x00\x00\x00\xE0\x00\x00\x00\x04\x00\x00\x00\xD0\x01\x00\x00'

## Functions
def Import(file_object, debug_import = False):
	'''Takes in a file object of a formatted Junk Jack world and returns it's world data as a dictionary and the decompressed world bytestring. With DEBUG mode enabled, it returns the header, world data dictionary, compressed and decompressed world bytestrings, compressed and decompressed survival footer bytestrings, and the default footer bytestring. '''
						## Read-Through: 1
	file_object.seek(44, 0)			# Jump to position: 0000002B

		## Junk Jack Loader Handler: World Size (in bytes)
	world_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]

	file_object.seek(264, 0)		# Jump to position: 00000107

		## Junk Jack World Properties: World Name
	world_name = file_object.read(31).decode('utf-8').rstrip("\0")

	file_object.seek(21, 1)			# Jump to position: 0000013B

		## Junk Jack World Properties: Player Position
	player_position = struct.unpack('<hh', file_object.read(4))

		## Junk Jack World Properties: Spawn Position
	spawn_position = struct.unpack('<hh', file_object.read(4))

		## Junk Jack World Properties: World Planet
	planet = file_object.read(2)

	if planet == b'\x01\x00':
		planet = "Terra"

	elif planet == b'\x02\x00':
		planet = "Seth"

	elif planet == b'\x04\x00':
		planet = "Alba"

	elif planet == b'\x08\x00':
		planet = "Xeno"

	elif planet == b'\x10\x00':
		planet = "Magmar"

	elif planet == b'\x20\x00':
		planet = "Cryo"

	elif planet == b'\x40\x00':
		planet = "Yuca"

	elif planet == b'\x80\x00':
		planet = "Lilith"

	elif planet == b'\x00\x01':
		planet = "Thetis"

	elif planet == b'\x00\x02':
		planet = "Mykon"

	elif planet == b'\x00\x04':
		planet = "Umbra"

	file_object.seek(3, 1)			# Jump to position: 00000148

		## Junk Jack World Properties: Gamemode
	gamemode = file_object.read(1)
	if gamemode == b'\x00':
		gamemode = "Survival"

	elif gamemode == b'\x01':
		gamemode = "Creative"

	elif gamemode == b'\x02':
		gamemode = "Flat"

	elif gamemode == b'\x03':
		gamemode = "Adventure"

		## Junk Jack Loader Handler: Bytestring Sizes
	if gamemode == "Survival" or gamemode == "Adventure":
		file_object.seek(56, 0)		# Jump to position: 00000037

			## Bytestring: Survival Footer Size
		survival_footer_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]

		file_object.seek(32, 1)		# Jump to position: 0000005B

			## Bytestring: Chest Data Size
		chest_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]

		file_object.seek(140, 1)	# Jump to position: 000000EB

			## Bytestring: Entity Data Size
		entity_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]
	else:
		file_object.seek(80, 0)		# Jump to position: 0000004F

			## Bytestring: Chest Data Size
		chest_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]

		file_object.seek(140, 1)	# Jump to position: 000000EB

			## Bytestring: Entity Data Size
		entity_size_in_bytes = struct.unpack('<i', file_object.read(4))[0]

	file_object.seek(330, 0)	# Jump to position: 000000EB

		## Junk Jack World Properties: World Size
	world_size_in_string = file_object.read(1)
	if world_size_in_string == b'\x00':
		world_size_in_string = "Tiny"
		border_size = 512

	elif world_size_in_string == b'\x01':
		world_size_in_string = "Small"
		border_size = 768

	elif world_size_in_string == b'\x02':
		world_size_in_string = "Normal"
		border_size = 1024

	elif world_size_in_string == b'\x03':
		world_size_in_string = "Large"
		border_size = 2048

	elif world_size_in_string == b'\x04':
		world_size_in_string = "Huge"
		border_size = 4096

		## Junk Jack World Properties: Skybox Offset
	skybox_offset = int(file_object.read(1).hex(), 16)

	file_object.seek(132, 1)	# Jump to position: 000001CF

		## Junk Jack World Properties: Border Array
	border_array = []
	border_counter = 0
	while border_counter < (border_size / 16):
		__border_array = struct.unpack('<16h', file_object.read(32))	# Unpack border segment

		for i in __border_array:
			border_array.append(i)	# Add border segment to array

		border_counter += 1

		## Junk Jack World Body: Compressed
	d_compressed_world = file_object.read(world_size_in_bytes)

		## Junk Jack World Body: Decompressed
	decompressed_world = zlib.decompress(d_compressed_world, 32)

		## Junk Jack Survival Footer
	if (gamemode == "Survival" or gamemode == "Adventure") and survival_footer_size_in_bytes > 0:
			# -Compressed
		d_compressed_survival_footer = file_object.read(survival_footer_size_in_bytes)

			# -Decompressed
		d_decompressed_survival_footer = zlib.decompress(d_compressed_survival_footer, 32)
	else:
		d_compressed_survival_footer = ''
		d_decompressed_survival_footer = ''

		## Junk Jack Default Footer: Time In Game
	time_in_game = struct.unpack('<i', file_object.read(4))[0]

		## Junk Jack Default Footer: Skybox Type
	skybox = struct.unpack('<b', file_object.read(1))[0]

		## Junk Jack Default Footer: Weather Type
	file_object.seek(7, 1)	# Jump to position: Weather

	weather = file_object.read(1)

	if weather == b'\x00':
		weather = "Clear"

	elif weather == b'\x01':
		weather = "Rain"

	elif weather == b'\x02':
		weather = "Snow"

	elif weather == b'\x03':
		weather = "Acid Rain"
	
		## Junk Jack Default Footer: Chest/Entity Data
	file_object.seek(3, 1)	# Jump to position: Chests
			# -Chest Data
	if chest_size_in_bytes > 4:
		chest_data = file_object.read(chest_size_in_bytes)
		file_object.seek(60, 1)	# Jump to position: Entities
	else:
		file_object.seek(64, 1)	# Jump to position: Entities
		chest_data = None

			# -Entity Data
	if entity_size_in_bytes > 4:
		entity_data = file_object.read(entity_size_in_bytes)
	else:
		entity_data = None

						## Read-Through: 2
	if debug_import:
		file_object.seek(0, 0)	# Jump to position: 00000000

		d_header = file_object.read(464 + (border_size * 2))
	
		file_object.seek(world_size_in_bytes, 1)	# Jump to position: Footer
	
		if gamemode == "Survival" or gamemode == "Adventure":
			file_object.seek(survival_footer_size_in_bytes, 1)	# Jump to position: Default Footer

		d_default_footer = file_object.read()

	## World Properties Creation
		## Chest Creation
	chest_list = []

	if chest_data is not None:
		chest_list = Create_Chest_List_From_ByteString(chest_data)

		## Entity Creation
	entity_list = []

	if entity_data is not None:
		entity_list = Create_Entity_List_From_ByteString(entity_data)

		## Property Creation
	world_properties_dictionary = {
		'world' :
		{
			'name' : world_name,
			'size' : world_size_in_string,
			'planet' : planet,
			'gamemode' : gamemode,
			'player' : player_position,
			'spawn' : spawn_position,
			'time' : time_in_game,
			'weather' : weather,
			'skybox' : skybox,
			'skybox_offset' : skybox_offset,
		},
		'border' : border_array,
		'chests' : chest_list,
		'entities' : entity_list,
	}

	if debug_import:
		## Console Print
		#print(f"World Name: {world_name}")
		#print(f"Planet: {planet}")
		#print(f"Gamemode: {gamemode}")
		#print(f"World Size (in bytes): {world_size_in_bytes}")
		#print(f"World Size (in string): {world_size_in_string}")
		#print(f"Spawn Position: ({spawn_position[0]}, {spawn_position[1]})")
		#print(f"Player Position: ({player_position[0]}, {player_position[1]})")
		#print(f"Time In-Game: {time_in_game}")
		#if gamemode == "Survival" or gamemode == "Adventure":
		#	print(f"Survival Footer Size (in bytes): {survival_footer_size_in_bytes}")
		#	print(f"Survival Footer Compressed: {d_compressed_survival_footer}")
		#	print(f"Survival Footer Decompressed: {d_decompressed_survival_footer}")
		#print(f"Chest Data Size (in bytes): {chest_size_in_bytes}")
		#print(f"Entity Data Size (in bytes): {entity_size_in_bytes}")
		#print(f"Skybox Offset: {skybox_offset}")
		#print(f"Border Array:\n{border_array}")
		#print(f"Compressed World:\n{d_compressed_world}")
		#print(f"Decompressed World:\n{decompressed_world}")
		#print(f"Chest Data: {chest_data}")
		#print(f"Entity Data: {entity_data}")

		## Return Statement
		return world_properties_dictionary, d_header, d_compressed_world, decompressed_world, d_compressed_survival_footer, d_decompressed_survival_footer, d_default_footer
	else:
		return world_properties_dictionary, decompressed_world

def Export(world_properties_dictionary, decompressed_world_bytestring):
	'''Takes in a world properties dictionary and a decompressed world bytestring and returns a formatted Junk Jack world bytestring.'''
	## Conversions
		# -Header
			# -World Name
	world_name = world_properties_dictionary['world'].get('name').encode('utf-8')
	world_name = world_name + bytes(32 - len(world_name))
			# -Player Position
	player_position = struct.pack('<2h', *world_properties_dictionary['world'].get('player'))
			# -Spawn Position
	spawn_position = struct.pack('<2h', *world_properties_dictionary['world'].get('spawn'))
			# -Planet
	planet = world_properties_dictionary['world'].get('planet')
	if planet == "Terra":
		planet = b'\x01\x00'
	elif planet == "Seth":
		planet = b'\x02\x00'
	elif planet == "Alba":
		planet = b'\x04\x00'
	elif planet == "Xeno":
		planet = b'\x08\x00'
	elif planet == "Magmar":
		planet = b'\x10\x00'
	elif planet == "Cryo":
		planet = b'\x20\x00'
	elif planet == "Yuca":
		planet = b'\x40\x00'
	elif planet == "Lilith":
		planet = b'\x80\x00'
	elif planet == "Thetis":
		planet = b'\x00\x01'
	elif planet == "Mykon":
		planet = b'\x00\x02'
	elif planet == "Umbra":
		planet = b'\x00\x04'
			# -Gamemode
	gamemode = world_properties_dictionary['world'].get('gamemode')
	if gamemode == "Survival":
		gamemode = b'\x00'
	elif gamemode == "Creative":
		gamemode = b'\x01'
	elif gamemode == "Flat":
		gamemode = b'\x02'
	elif gamemode == "Adventure":
		gamemode = b'\x03'
			# -World Size
	world_size = world_properties_dictionary['world'].get('size')
	if world_size == "Tiny":
		world_size = b'\x00'
	elif world_size == "Small":
		world_size = b'\x01'
	elif world_size == "Normal":
		world_size = b'\x02'
	elif world_size == "Large":
		world_size = b'\x03'
	elif world_size == "Huge":
		world_size = b'\x04'
			# -Skybox Offset
	skybox_offset = bytes([world_properties_dictionary['world'].get('skybox_offset')])
			# -Border List Bytestring
	border_bytestring = b''.join([struct.pack('<h', border) for border in world_properties_dictionary['border']])

		# -Body
			# -Compressed World
	compressed_world = io.BytesIO()
	with gzip.GzipFile(fileobj = compressed_world, mode = 'wb', compresslevel = 6, mtime = 0.0) as decompressed_stream:
		decompressed_stream.write(decompressed_world_bytestring)
	compressed_world_bytestring = compressed_world.getvalue()
	compressed_world_bytestring_length = len(compressed_world_bytestring)

		# -Footer
			# -In-Game Time
	time_in_game = struct.pack('<i', world_properties_dictionary['world'].get('time'))
			# -Skybox
	skybox = bytes([world_properties_dictionary['world'].get('skybox')])
			# -Weather
	weather = world_properties_dictionary['world'].get('weather')
	if weather == "Clear":
		weather = b'\x00'
	elif weather == "Rain":
		weather = b'\x01'
	elif weather == "Snow":
		weather = b'\x02'
	elif weather == "Acid Rain":
		weather = b'\x03'
			# -Chests
	chest_bytestring = Create_ByteString_From_Chest_List(world_properties_dictionary['chests'])
	chest_bytestring_length = len(chest_bytestring)
			# -Entities
	entity_bytestring = Create_ByteString_From_Entity_List(world_properties_dictionary['entities'])
	entity_bytestring_length = len(entity_bytestring)

	## Assembly
		# -Loader
	header_size = 464 + len(border_bytestring)
	pre_loader = [compressed_world_bytestring_length, compressed_world_bytestring_length + header_size]
	loader_value = [8, 8, chest_bytestring_length, 4, 4, 4, 4, 4, 4, 4, 4, 4, 8, 16]
	post_loader = []

	for i in loader_value:
		pre_loader.append(pre_loader[-1] + i)

	for i, value in enumerate(pre_loader):
		#print(f"{i} = {value}")
		if i == 0:		# -World Size + Gamemode Loader Preset
			post_loader.append(struct.pack('<i', value) + b'\x00\x00\x00\x00')
		elif i == 1:
			post_loader.append(struct.pack('<i', value) + struct.pack('<i', loader_value[i]) + b'\x00\x00\x00\x00')
		elif i == 2:
			post_loader.append(struct.pack('<i', value) + struct.pack('<i', loader_value[i - 2]) + b'\x05\x00\x00\x00')
		elif i == len(pre_loader) - 2:
			post_loader.append(struct.pack('<i', value) + struct.pack('<i', loader_value[i - 2]) + b'\x13\x00\x00\x00')
		elif i == len(pre_loader) - 1:
			post_loader.append(struct.pack('<i', value) + struct.pack('<i', entity_bytestring_length) + b'\x00\x00\x00\x00')
		else:
			post_loader.append(struct.pack('<i', value) + struct.pack('<i', loader_value[i - 1]) + b'\x00\x00\x00\x00')



	assembled_loader = bytes([0, 0, 0, 0, 2, 0, 1, 1]) + struct.pack('<i', header_size) + b''.join(post_loader) + bytes(8)
		# -World Properties
	assembled_world_properties = bytes(24) + world_name + bytes(20) + player_position + spawn_position + planet + bytes(3) + gamemode + world_size + skybox_offset
		# -Header
	assembled_header = jjxheader + assembled_loader + assembled_world_properties + bytes(132) + border_bytestring

		# -Footer
	assembled_footer = time_in_game + skybox + bytes(7) + weather + bytes(3) + chest_bytestring + bytes(60) + entity_bytestring

	## DEBUG
	#print(world_name)
	#print(player_position)
	#print(spawn_position)
	#print(border_bytestring)
	#print(header_size)
	#print(assembled_loader)
	#print(pre_loader)
	#print(compressed_world_bytestring_length)
	#print(post_loader)
	#print(entity_bytestring_length)

	return assembled_header + compressed_world_bytestring + assembled_footer