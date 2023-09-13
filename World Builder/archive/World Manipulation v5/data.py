##----------------------------------------------##
## Junk Jack X World Editor - [Python Edition]	##
## Created By: Ryan Smith						##
##----------------------------------------------##
## World Editor v5 - Data Parsers				##
##----------------------------------------------##

## Imports
import json, struct

## Constants
item_json = json.load(open("items.json", 'r'))
entity_json = json.load(open("entities.json", 'r'))

## Functions

##------------------------Conversion Data------------------------##

def Hex_To_String(hex_char, type):
	'''Takes in a hex value (bytestring or string) and a type comparison ("Entity" or "Item") and returns the object's name.'''
	if isinstance(hex_char, (bytes)):
		hex_char = hex_char.hex().upper()

	if type == "Entity":
		for entity in entity_json['entities']:
			if hex_char == entity['hex id']:
				return entity['name']

	if type == "Item":
		for item in item_json['items']:
			if hex_char == item['hex id']:
				return item['name']

def String_To_Hex(object_name, type):
	'''Takes in an object name and a type comparison ("Entity" or "Item") and returns the object's hex value.'''
	if type == "Entity":
		for entity in entity_json['entities']:
			if object_name == entity['name']:
				return entity['hex id']

	if type == "Item":
		for item in item_json['items']:
			if object_name == item['name']:
				return item['hex id']

##------------------------Entity Data------------------------##

def Create_Entity_List_From_ByteString(bytestring):
	'''Takes a bytestring and returns an list of entity dictionaries.'''
	bytestring_entities = [bytestring[4 + (i * 7):11 + (i * 7)] for i in range(int(len(bytestring[4:]) / 7))]

	return [Create_Entity_From_ByteString(entity) for entity in bytestring_entities]

def Create_ByteString_From_Entity_List(entity_list):
	'''Takes a list of entity dictionaries and returns a bytestring for that list.'''
	entity_num = struct.pack('<i', len(entity_list))
	bytestring_entities = [Create_ByteString_From_Entity(entity) for entity in entity_list]

	return entity_num + b''.join(bytestring_entities)

def Create_Entity_From_ByteString(bytestring):
	'''Takes a bytestring and returns an entity dictionary.'''
	entity_id = Hex_To_String(bytestring[5:], "Entity")
	entity_position = struct.unpack('<hh', bytestring[:4])

	return {
		"id" : entity_id,
		"position" : entity_position
	}

def Create_ByteString_From_Entity(entity_dictionary):
	'''Takes an entity dictionary and returns a bytestring for that entity.'''
	entity_id = bytes.fromhex(String_To_Hex(entity_dictionary['id'], "Entity"))
	entity_position = struct.pack('<2h', *entity_dictionary['position'])

	return entity_position + b'\x00' + entity_id

##------------------------Item Data------------------------##

def Create_Item_From_ByteString(bytestring):
	'''Takes a bytestring and returns an item dictionary. If bytestring returns invalid item, returns None.'''
	item_id = Hex_To_String(bytestring[4:6], "Item")

	if item_id is not None:
		item_count = struct.unpack('<h', bytestring[6:8])[0]
		item_durability = struct.unpack('<h', bytestring[8:10])[0]
		item_modifier = f"{bytestring[:2].hex().upper()}"
	else:
		return None

	return {
		'id' : item_id,
		'count' : item_count,
		'durability' : item_durability,
		'modifier' : item_modifier
	}


def Create_ByteString_From_Item(item_dictionary):
	'''Takes an item dictionary and returns a bytestring for that item. If item_dictionary is None, returns empty item bytestring.'''
	if item_dictionary is not None:
		item_id = bytes.fromhex(String_To_Hex(item_dictionary['id'], "Item"))
		item_count = struct.pack('<h', item_dictionary['count'])
		item_durability = struct.pack('<h', item_dictionary['durability'])
		item_modifier = bytes.fromhex(item_dictionary['modifier'])

		return item_modifier + b'\x00\x00' + item_id + item_count + item_durability + b'\x00\x00'
	else:
		return b'\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00'

##------------------------Chest Data------------------------##

def Create_Chest_List_From_ByteString(bytestring):
	'''Takes a bytestring and returns a list of chest dictionaries.'''
	j = 0
	bytestring_chests = []
	chest_num = struct.unpack('<i', bytestring[:4])[0]
	chest_data = bytestring[4:]

	for i in range(chest_num):
		data = struct.unpack('<h', chest_data[j + 8:j + 10])[0]
		bytestring_chests.append(Create_Chest_From_Bytestring(chest_data[j:j + 12 + (data * 12)]))
		j = j + 12 + (data * 12)

	return bytestring_chests

def Create_ByteString_From_Chest_List(chest_list):
	'''Takes a list of chest dictionaries and returns a bytestring for that list.'''
	chest_num = struct.pack('<i', len(chest_list))
	bytestring_chests = [Create_Bytestring_From_Chest(chest) for chest in chest_list]

	return chest_num + b''.join(bytestring_chests)

def Create_Chest_From_Bytestring(bytestring):
	'''Takes a bytestring and returns a chest dictionary.'''
	x_co = struct.unpack('<h', bytestring[:2])[0]
	y_co = struct.unpack('<h', bytestring[4:6])[0]
	slot_count = int(struct.unpack('<h', bytestring[8:10])[0] / 12)
	data_list = [bytestring[12 + (i * 12):(i * 12) + 24] for i in range(int(len(bytestring[12:]) / 12))]
	item_list = [dict(item, slot = i) for i, item in enumerate(map(Create_Item_From_ByteString, data_list)) if item is not None]

	return {
		'pages' : slot_count,
		'position' : (x_co, y_co),
		'contents' : item_list
	}

def Create_Bytestring_From_Chest(chest_dictionary):
	'''Takes a chest dictionary and returns a bytestring for that item.'''
	item_string = b''
	x_co = struct.pack('<h', chest_dictionary['position'][0])
	y_co = struct.pack('<h', chest_dictionary['position'][1])
	slot_count = struct.pack('<h', (chest_dictionary['pages'] * 12))

	for i in range(chest_dictionary['pages'] * 12):
		if not chest_dictionary['contents']:
			item_string += Create_ByteString_From_Item(None)
		else:
			for j, item in enumerate(chest_dictionary['contents']):
				if item['slot'] == i:
					item_string += Create_ByteString_From_Item(item)
					break
				elif j == len(chest_dictionary['contents']) - 1:
					item_string += Create_ByteString_From_Item(None)
					break


	return x_co + b'\x00\x00' + y_co + b'\x00\x00' + slot_count + b'\x00\x00' + item_string

## Main

## __Main__
if __name__ == "__main__":

	## Entities
	print(Create_Entity_From_ByteString(b'\x15\x00\x4C\x00\x00\xF1\x02'))
	print(Create_ByteString_From_Entity(
	{
		'id':"Mykon Slime Orange Big",
		'position':(65, 74)
	}))

	## Items
	print(Create_Item_From_ByteString(b'\x31\xa5\x00\x00\x4E\x08\x04\x00\x00\x00\x00\x00'))
	print(Create_Item_From_ByteString(b'\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00'))
	print(Create_ByteString_From_Item(
		{
			'id':"White Potion",
			'count':4,
			'durability':0,
			'modifier':"31A5"
		}))
	print(Create_ByteString_From_Item(None))

	## Chest - No Items
	print(Create_Chest_From_Bytestring(b'\x2D\x00\x00\x00\x26\x00\x00\x00\x0C\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xFF\xFF\x00\x00\x00\x00\x00\x00'))
	print(Create_Bytestring_From_Chest(
	{
		'pages':1,
		'position':(45, 38),
		'contents':
		[

		]
	}))

	## Chest - Items
	print(Create_Chest_From_Bytestring(b'\x00\x00\x00\x00\x01\x00\x00\x00\x0C\x00\x00\x00\x00\x00\x00\x00\x87\x05\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\xFF\xFF\x01\x00\x00\x00\x01\x00\x00\x00\x00\x00\x89\x07\x01\x00\x00\x00\x00\x00'))
	print(Create_Bytestring_From_Chest(
	{
		'pages':1,
		'position':(0, 1),
		'contents':
		[{
			'id' : "Banana Pie",
			'count' : 4,
			'durability' : 0,
			'modifier' : "0000",
			'slot' : 4
		},
		{
			'id' : "Black Tuxedo Pants",
			'count' : 16,
			'durability' : 0,
			'modifier' : "0000",
			'slot' : 7
		},
		{
			'id' : "Prison Bricks",
			'count' : 255,
			'durability' : 0,
			'modifier' : "0000",
			'slot' : 9
		}]
	}))

	## Entity Array
	print(Create_Entity_List_From_ByteString(b'\x05\x00\x00\x00\x03\x00\x01\x00\x00\x02\x00\x00\x00\x01\x00\x00\x03\x00\x05\x00\x01\x00\x00\x47\x00\x01\x00\x01\x00\x00\x7A\x01\x03\x00\x01\x00\x00\x18\x00'))
	print(Create_ByteString_From_Entity_List(
		[{
			'id': 'Bear Brown',
			'position': (3, 1)
		},
		{
			'id': 'Bear White',
			'position': (0, 1)
		},
		{
			'id': 'Dog Black Puppy',
			'position': (5, 1)},
		{
			'id': 'Demon Gator',
			'position': (1, 1)
		},
		{
			'id': 'Camel Brown',
			'position': (3, 1)
		}]))

	## Chest Array
	print(Create_Chest_List_From_ByteString(b'\x02\x00\x00\x00\x01\x00\x00\x00\x04\x00\x00\x00\x0c\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00V\x08\x15\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x03\x00\x00\x00\x01\x00\x00\x00\x0c\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00M\x08\x03\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\xff\xff\x00\x00\x00\x00\x00\x00'))
	print(Create_ByteString_From_Chest_List(
		[{
			'pages':1,
			'position':(1, 4),
			'contents':
			[{
				'id':"Black Tuxedo Pants",
				'count':21,
				'durability':0,
				'modifier':"0000",
				'slot':5
			}]
		},{
			'pages':1,
			'position':(3, 1),
			'contents':
			[{
				'id':"Banana Pie",
				'count':3,
				'durability':0,
				'modifier':"0000",
				'slot':1
			}]
		}]))

	print("Separator")
	print(Create_ByteString_From_Chest_List([]))

	pass
else:
	pass

## Temporary