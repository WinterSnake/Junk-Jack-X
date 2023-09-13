##----------------------------------------------##
## Junk Jack X World Editor - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## World Import/Export v4						##
##----------------------------------------------##

## Imports
import struct, zlib

## Constants
jjheader = b'\x4A\x4A\x58\x4D\x01\x00\x13\x00\x00\x00\x00\x00\x01\x00\x00\x00\xF0\x00\x00\x00\xE0\x00\x00\x00\x04\x00\x00\x00\xD0\x01\x00\x00'

## Functions
def Import(file_path, DEBUG = False):
	'''Takes a file path and reads and compresses the world data with debugger options.'''
	with open(file_path, 'rb') as formatted_world:
		## Junk Jack Header Check
		if formatted_world.read(32) != jjheader:
			# -Invalid Format Header
			return None

		## Junk Jack Loader Control
		formatted_world.read(12)

		## Junk Jack World Loader Handler
			# -Loader: World Size
		size_in_b = struct.unpack('<i', formatted_world.read(4))[0]

			# -Loader: Unregistered
		formatted_world.read(216)

		## Junk Jack World Properties
			# -Properties: Name
		world_name = formatted_world.read(31).decode('utf-8').rstrip("\0")

			# -Properties: Unregistered
		formatted_world.read(21)

			# -Properties: Player Position
		player_position = struct.unpack('<hh', formatted_world.read(4))

			# -Properties: Spawn Position
		spawn_position = struct.unpack('<hh', formatted_world.read(4))

			# -Properties: Planet
		planet = formatted_world.read(2)

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

			# -Properties: Unknown
		formatted_world.read(3)

			# -Properties: Gamemode
		gamemode = formatted_world.read(1)
		if gamemode == b'\x00':
			gamemode = "Survival"

		elif gamemode == b'\x01':
			gamemode = "Creative"

		elif gamemode == b'\x02':
			gamemode = "Flat"

		elif gamemode == b'\x03':
			gamemode = "Adventure"

			# -Properties: World Size
		size_in_s = formatted_world.read(1)
		if size_in_s == b'\x00':
			size_in_s = "Tiny"
			border_size = 512

		elif size_in_s == b'\x01':
			size_in_s = "Small"
			border_size = 768

		elif size_in_s == b'\x02':
			size_in_s = "Normal"
			border_size = 1024

		elif size_in_s == b'\x03':
			size_in_s = "Large"
			border_size = 2048

		elif size_in_s == b'\x04':
			size_in_s = "Huge"
			border_size = 4096

			# -Properties: Skybox Offset
		skybox_offset = int(formatted_world.read(1).hex(), 16)

			# -Properties: Null Bye Seperator
		formatted_world.read(132)

			# -Properties: World Border
		border_array = []
		border_counter = 0
		while border_counter < border_size:
			__border_array = struct.unpack('<hhhhhhhhhhhhhhhh', formatted_world.read(32))

			for i in __border_array:
				border_array.append(i)

			border_counter += 16

		## Junk Jack World Body
			# -World: Compressed
		compressed_world = formatted_world.read(size_in_b)

			# -World: Decompressed
		decompressed_world = zlib.decompress(compressed_world, 32)

		## Junk Jack Footer
			# -Footer: Survival Expansion
		survival_footer = ''

			# -Footer: Cap
		footer = formatted_world.read()

		## Classify Properties
			# -World Properties
		world_properties = {
			"world":
			{
				'name' : world_name,
				'size' : size_in_s,
				'planet' : planet,
				'skybox_offset' : skybox_offset,
				'gamemode' : gamemode,
				'spawn_position' : spawn_position,
				'player_position' : player_position,
				'border_array' : border_array
			}
		}
		footer_properties = ''

			# -Manage: Header
		formatted_world.seek(0)
		header = formatted_world.read(464 + (border_size * 2))

	## DEBUG
	if DEBUG:
		# -DEBUG: Console Print
		#print(f"World Size (in bytes): {size_in_b}b")
		#print(f"World Name: {world_name}")
		#print(f"Player Position: ({player_position[0]}, {player_position[1]})")
		#print(f"Spawn Position: ({spawn_position[0]}, {spawn_position[1]})")
		#print(f"Planet: {planet}")
		#print(f"Gamemode: {gamemode}")
		#print(f"World Size (in string): {size_in_s}")
		#print(f"Border Size (in blocks): {border_size} blocks.")
		#print(f"Skybox Offset: {skybox_offset}")
		#print(f"Border Size:{len(border_array)}")
		#print(f"Border Array:\n{border_array}")
		#print(f"Compressed World:\n{compressed_world}")
		#print(f"Decompressed World:\n{decompressed_world}")
		#print(f"Footer:\n{footer}")

		# -DEBUG: Return
		return header, world_properties, compressed_world, decompressed_world, survival_footer, footer, footer_properties

	return world_properties, decompressed_world, footer_properties
	# -Non-DEBUG Return

def __Format_Export_World(world_properties, decompressed_world_bytestring):
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