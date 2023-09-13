##----------------------------------------------##
## Junk Jack X World Builder - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## Item.json Parser								##
##----------------------------------------------##

## Imports
import json

## Constants

## Functions
def item_reparser(file_path):
	new_item_data = {"items":[]}

	with open(file_path, 'r') as item_json_data:
		item_data = json.load(item_json_data)

	for item_object in item_data["treasures"]:
		new_item_object = {
			'name' : item_object['name'],
			'num id' : item_object['id'],
			'hex id' : f"{int(struct.pack('<h', item_object['id']).hex(), 16):04X}"
			#struct.pack('<h', item_object['id']).hex(),
		}
		new_item_data['items'].append(new_item_object)

	print(json.dumps(new_item_data, indent = 4))
	with open("items.json", 'w') as item_json_formatted:
		json.dump(new_item_data, item_json_formatted, indent = 4)

## Classes
class Item(object):
	# -Constructor
	def __init__(self, name, numerical_id, hexadecimal_id):
		pass

	# -Dunder Methods

	# -Class Methods

	# -Static Methods

	# -Instance Methods

	# -Class Variables

## Main

## __Main__
if __name__ == "__main__":
	import sys, struct

	if len(sys.argv) == 2:
		item_reparser(sys.argv[1])
	else:
		print("Invalid parameters. Expected single JSON file ('items.json'). Recieved {} parameters.".format(len(sys.argv) - 1))
		input()
else:
	pass

## Temporary