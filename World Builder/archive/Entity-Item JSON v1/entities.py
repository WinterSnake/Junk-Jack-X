##----------------------------------------------##
## Junk Jack X World Builder - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## Entity.json Parser							##
##----------------------------------------------##

## Imports
import json

## Constants

## Functions
def entity_reparser(file_path):
	new_entity_data = {"entities":[]}

	with open(file_path, 'r') as entity_json_data:
		entity_data = json.load(entity_json_data)

	for i, entity in enumerate(entity_data["mobs"]):
		new_entity_object = {
			'name' : entity,
			'num id' : i,
			'hex id' : f"{int(struct.pack('<h', i).hex(), 16):04X}"
		}
		new_entity_data['entities'].append(new_entity_object)

	print(json.dumps(new_entity_data, indent = 4))
	with open("entities.json", 'w') as entity_json_formatted:
		json.dump(new_entity_data, entity_json_formatted, indent = 4)

## Classes
class Entity(object):
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
		entity_reparser(sys.argv[1])
	else:
		print("Invalid parameters. Expected single JSON file ('entity.json'). Recieved {} parameters.".format(len(sys.argv) - 1))
		input()
else:
	pass

## Temporary