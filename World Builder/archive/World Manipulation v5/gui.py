##----------------------------------------------##
## Junk Jack X World Editor - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## World Editor v5 - GUI Application			##
##----------------------------------------------##

## Imports
import json, tkinter, tkinter.filedialog, world, os, sys

## Constants
DEBUG = False
KEEPINPARENTDIRECTORY = True

## Functions

##------------------------Data Format------------------------##

def Import_To_File(formatted_world_binary_file):
	''''''
	importted_world = None

	with open(formatted_world_binary_file, 'rb') as formatted_world:
		if formatted_world.read(32) == world.jjxheader:
			importted_world = world.Import(formatted_world, DEBUG)
		else:
			print("Invalid World File!")

	if importted_world is not None:
		path = Format_Directory(formatted_world_binary_file, importted_world[0]['world'].get('name'))

		## Non-Debug Mode
		if len(importted_world) == 2:
			# -Properties.json
			with open(f"{path}/Properties.json", 'w') as world_properties:
				json.dump(importted_world[0], world_properties, indent = 4)

			# -Decompressed.dat
			with open(f"{path}/Decompressed.dat", 'wb') as world_decompressed:
				world_decompressed.write(importted_world[1])

		## Debug Mode
		else:
			# -Properties.json
			with open(f"{path}/Properties.json", 'w') as world_properties:
				json.dump(importted_world[0], world_properties, indent = 4)

			# -Header.dat
			with open(f"{path}/Header.dat", 'wb') as world_header:
				world_header.write(importted_world[1])

			# -Compressed.dat
			with open(f"{path}/Compressed.dat", 'wb') as world_compressed:
				world_compressed.write(importted_world[2])

			# -Decompressed.dat
			with open(f"{path}/Decompressed.dat", 'wb') as world_decompressed:
				world_decompressed.write(importted_world[3])

def Export_To_File(*files, world_properties_json_file = None, decompressed_world_binary_file = None):
	''''''
	## Split Files
	for file in files:
		ext = os.path.splitext(file)[1]
		if ext == ".json":
			world_properties_json_file = file
		elif ext == ".dat":
			decompressed_world_binary_file = file

	## Retrieve Data
	world_properties = json.load(open(world_properties_json_file, 'r'))
	world_decompressed = open(decompressed_world_binary_file, 'rb').read()
	path = Format_Directory(decompressed_world_binary_file, world_properties['world'].get('name'))

	with open(f"{path}/{world_properties['world'].get('name')}.dat", 'wb') as formatted_world:
		formatted_world.write(world.Export(world_properties, world_decompressed))

##------------------------Location Format------------------------##

def Format_Directory(file_path, world_name):
	if KEEPINPARENTDIRECTORY:
		new_path = f"{world_name}"
	else:
		new_path = f"{os.path.dirname(file_path)}/{world_name}"

	if not os.path.exists(new_path):
		os.mkdir(new_path)

	return new_path

##------------------------Import/Export Click Events------------------------##

def _Import_Click():
	import_file = tkinter.filedialog.askopenfilename()
	if import_file is not "":
		Import_To_File(import_file)

def _Export_Click():
	world_data = tkinter.filedialog.askopenfilenames()

	if not world_data or len(world_data) == 1 or len(world_data) > 2:
		print("Must select ONLY a properties file AND a decompressed world file.")
	else:
		Export_To_File(world_data[0], world_data[1])

## __Main__
if __name__ == "__main__":
	## GUI Application
	if len(sys.argv) == 1:
		root = tkinter.Tk()
		root.title("Import/Export GUI")
		frame = tkinter.Frame(master = root)
		frame.pack()
		
		import_map = tkinter.Button(master = frame, text = "Import Map", command = _Import_Click)
		import_map.grid(column = 0, row = 0)
		
		export_map = tkinter.Button(master = frame, text = "Export Map", command = _Export_Click)
		export_map.grid(column = 0, row = 1)
		
		root.mainloop()

	## Import Map
	elif len(sys.argv) == 2:
		Import_To_File(sys.argv[1])

	## Export Map
	elif len(sys.argv) == 3:
		Export_To_File(sys.argv[1], sys.argv[2])