##----------------------------------------------##
## Junk Jack X World Editor - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## File Description								##
##----------------------------------------------##

## Imports
import json, tkinter, tkinter.filedialog, world, os

## Constants
DEBUG = True
KEEPINPARENTDIRECTORY = True

## Functions
def __Default_SData__(world_properties, world_decompressed, footer_properties, file_path_root = None):
	''''''
	## File Organization
		# -Directory Naming Path
	if file_path_root is not None:
		working_directory = f"{file_path_root}\\{world_properties['world'].get('name')}"
	else:
		working_directory = world_properties['world'].get('name')

		# -Create Directory
	if not os.path.exists(working_directory):
		os.mkdir(working_directory)

	# -World Properties
	with open(f"{working_directory}\\Properties.json", 'w') as properties:
		json.dump(world_properties, properties, indent = 4)

	# -World Decompressed
	with open(f"{working_directory}\\Decompressed.dat", 'wb') as decompressed:
		decompressed.write(world_decompressed)

	# -Footer Properties

	return working_directory

def Save_Imported_World(file_path):
	''''''
	if KEEPINPARENTDIRECTORY:
		directory_path = None
	else:
		directory_path = os.path.dirname(file_path)

	imported_world = world.Import(file_path, DEBUG)

	if imported_world is None:
		print("Invalid World File.")

	elif len(imported_world) == 3:
		print("DEBUG Disabled World Import.")
		__Default_SData__(imported_world[0], imported_world[1], imported_world[2], directory_path)

	else:
		print("DEBUG Enabled World Import.")
		path = __Default_SData__(imported_world[1], imported_world[3], imported_world[6], directory_path)

		# -World Header
		with open(f"{path}\\Header.dat", 'wb') as header:
			header.write(imported_world[0])

		# -World Compressed
		with open(f"{path}\\Compressed.dat", 'wb') as compressed:
			compressed.write(imported_world[2])

		# -World Footer
		with open(f"{path}\\Footer.dat", 'wb') as footer:
			footer.write(imported_world[5])

## Classes
	# -Constructor

	# -Dunder Methods

	# -Class Methods

	# -Static Methods

	# -Instance Methods

	# -Class Variables

## Main

## Temporary
root = tkinter.Tk()
root.withdraw()

world_data = tkinter.filedialog.askopenfilename()
Save_Imported_World(world_data)