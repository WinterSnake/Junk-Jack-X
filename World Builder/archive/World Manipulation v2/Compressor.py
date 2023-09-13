##----------------------------------##
### Junk Jack World Compressor		##
## Created By: Winter_Snake			##
##----------------------------------##

## Imports
import os, sys, gzip, shutil, struct

## Constants

## Functions	
def get_world_data():
	## File Data
	file_size = os.path.getsize(sys.argv[1])
	file_name = os.path.splitext(sys.argv[1])[0]

	## World Data
	if file_size == 1048576:
		world_type = "Tiny"
		compression_type = 5
		compression_offset = 1488

	elif file_size == 3145728:
		world_type = "Small"
		compression_type = 7
		compression_offset = 2000

	elif file_size == 4194304:
		world_type = "Normal"
		compression_type = 9
		compression_offset = 2512

	elif file_size == 12582912:
		world_type = "Large"
		compression_type = 17
		compression_offset = 4560

	elif file_size == 33554432:
		world_type = "Huge"
		compression_type = 33
		compression_offset = 8656
	else:
		return None

	return file_name, sys.argv[1], file_size, world_type, compression_type, compression_offset
	# File Name, File Path, File Size (in bytes), World Type (Tiny/Small/Normal/Large/Huge), Compression Ratio (for header lock), Compression Ration (for offset)

def get_compressed_data(file_path, world_size):
	## Constants
	file_position_modifiers = [8, 8, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 8, 16]

	## Compress World Data
	with open(file_path, 'rb') as f_in, gzip.GzipFile(filename = "", compresslevel = 6, fileobj = open("Compressed World.dat", 'wb')) as f_out:
		shutil.copyfileobj(f_in, f_out)

	## Compressed Data
	compressed_data = open("Compressed World.dat", 'rb').read()

	## World Header Lock Data
	file_size = os.path.getsize("Compressed World.dat")
	header_values = [file_size, file_size + world_size]
	for val in file_position_modifiers:
		header_values.append(header_values[-1] + val)

	## Clean Up
	os.remove("Compressed World.dat")
	return compressed_data, header_values
	# Compressed World Format (in gzip compression:6), Header Lock Values

## Main
if len(sys.argv) == 2:
	## Get World Data
	world_data = get_world_data()

	if world_data != None:
		## Compressed World
		compressed_data, header_values = get_compressed_data(world_data[1], world_data[5])

		## Completed World Data
		with open(f"{world_data[0]} - Complete.dat", 'wb') as complete_world:
			## Junk Jack World Header
			complete_world.write(bytes
				([74, 74, 88, 77, 1, 0, 19, 0, 0, 0, 0, 0, 1, 0, 0, 0,
				240, 0, 0, 0, 224, 0, 0, 0, 4, 0, 0, 0, 208, 1, 0, 0]))

			## Junk Jack Header Lock
			# Reader Size
			complete_world.write(bytes
				([0, 0, 0, 0, 2, 0, 1, 1, 208, world_data[4], 0, 0]))

			# World Size Lock
			for index, val in enumerate(header_values):
				if index == 0:
					spacer = bytes(4)
				elif index == len(header_values) - 1:
					spacer = bytes(0)
				else:
					spacer = bytes(8)
				complete_world.write(struct.pack('<i', val) + spacer)
			
			## Junk Jack World Data
			# Unknown Data
			complete_world.write(bytes(40))

			# World Name
			complete_world.write(bytes("Compressed World", "utf-8"))
			complete_world.write(bytes(15))

			# Unknown Data
			complete_world.write(bytes(19))

			# World Size
			if world_data[3] == "Tiny":
				complete_world.write(b'\x00\x00')
			elif world_data[3] == "Small" or world_data[3] == "Normal":
				complete_world.write(b'\x00\x01')
			elif world_data[3] == "Large":
				complete_world.write(b'\x80\x01')
			elif world_data[3] == "Huge":
				complete_world.write(b'\x00\x02')

			# Player Position
			complete_world.write(b'\x00\x00\x01\x00')

			# World Spawn Position
			complete_world.write(b'\x00\x00\x01\x00')

			# Planet Data
			complete_world.write(b'\x01\x00')

			# Unknown Data
			complete_world.write(bytes(3))

			# Game Mode
			complete_world.write(b'\x01')

			# Unknown Data
			complete_world.write(b'\x00\x04\x00\x00\x00\x00')
			
			## Junk Jack Null Byte Spacer
			complete_world.write(bytes(128))
			
			## Junk Jack Background Spacer
			if world_data[3] == "Tiny":
				for i in range(0, 512):
					complete_world.write(b'\x5A\x00')

			elif world_data[3] == "Small":
				for i in range(0, 768):
					complete_world.write(b'\xB4\x00')

			elif world_data[3] == "Normal":
				for i in range(0, 1024):
					complete_world.write(b'\xB4\x00')

			elif world_data[3] == "Large":
				for i in range(0, 2048):
					complete_world.write(b'\x5A\x01')

			elif world_data[3] == "Huge":
				for i in range(0, 4096):
					complete_world.write(b'\x67\x01')
			
			## Junk Jack Compressed World
			complete_world.write(compressed_data)
			
			## Junk Jack Footer
			complete_world.write(bytes(84))
		
		## Clean Up
		print("World successfully converted!")
	else:
		## Error: Unknown File Size
		print("Error: Unknown file size.")

elif len(sys.argv) > 2:
	## Error: Multiple Inputs
	print("Error: Only one input file allowed.")

else:
	## Error: No Input
	print("Error: Please input a decompressed file.")
	
#input()