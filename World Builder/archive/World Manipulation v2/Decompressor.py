##----------------------------------##
### Junk Jack World Decompressor	##
## Created By: Winter_Snake			##
##----------------------------------##

## Imports
import os, sys, gzip, shutil, struct

## Functions
def peek(file, peek_distance):
	pos = file.tell()
	data = file.read(peek_distance)
	file.seek(pos)
	return data

## Main
if len(sys.argv) == 2:
	with open(sys.argv[1], 'rb') as completed_world:
		## Get World Size
		completed_world.read(41)
		# Skip To Size Byte
		world_size = completed_world.read(1)

		## Split World Size
		if world_size == b'\x05':
			world_size = "Tiny"
			world_buffer = b'\x00\x00\x10\x00'

		elif world_size == b'\x07':
			world_size = "Small"
			world_buffer = b'\x00\x00\x30\x00'

		elif world_size == b'\x09':
			world_size = "Normal"
			world_buffer = b'\x00\x00\x40\x00'

		elif world_size == b'\x11':
			world_size = "Large"
			world_buffer = b'\x00\x00\xC0\x00'

		elif world_size == b'\x21':
			world_size = "Huge"
			world_buffer = b'\x00\x00\x00\x02'

		else:
			## Error: Invalid World File
			world_size = "Invalid"
			print("Invalid world file.")

		if world_size != "Invalid":
			print(f"{world_size} world converting...")
			## Find World Data
			current_byte = completed_world.read(1)
			while current_byte != b'':
				if peek(completed_world, 2) == b'\x1F\x8b':
					current_byte = completed_world.read(1)
					break
				current_byte = completed_world.read(1)

			## Copy World Data
			with open("compressed_world.dat", 'wb') as f_in:
				while current_byte != b'':
					if peek(completed_world, 4) == world_buffer:
						for i in range(5):
							print(current_byte)
							f_in.write(current_byte)
							current_byte = completed_world.read(1)
						break
					print(current_byte)
					f_in.write(current_byte)
					current_byte = completed_world.read(1)

	## Decompress World File
	with gzip.open("compressed_world.dat", 'rb') as f_in, open(os.path.splitext(sys.argv[1])[0] + " - Decompressed.dat", 'wb') as f_out:
		shutil.copyfileobj(f_in, f_out);
		
	## Clean Up
	os.remove("compressed_world.dat")
		
elif len(sys.argv) > 2:
	## Error: Multiple Inputs
	print("Error: Only one input file allowed.")

else:
	## Error: No Input
	print("Error: Please input a world file.")

#input()