import json

def Test_0():
	test = json.load(open("sample.json", "r"))
	
	print(test)
	
	print(test['mobs'])
	
	print(test['chests'])
	
	new_ent = {
		"id" : "New Mob",
		"position" : [
			45, 89
		]
	}
	
	test['mobs'].append(new_ent)
	
	for entity in test['mobs']:
		print(entity)
	
	
	print(test)

def Test_1():
	'''Converting Hex ID (world format) to String ID (name).'''
	mob_id = "3202"
	entity_list = json.load(open("entities.json", "r"))

	for entity in entity_list['entities']:
		if mob_id == entity['hex id']:
			return entity['name']

def Test_2():
	'''Converting String ID (name) to Hex ID (world format).'''
	mob_name = "Pentalober Blue"
	entity_list = json.load(open("entities.json", "r"))

	for entity in entity_list['entities']:
		if mob_name == entity['name']:
			return entity['hex id']

def String_To_Hex(name):
	'''Input mob name string and return mob hex id.'''
	entity_list = json.load(open("entities.json", "r"))

	for entity in entity_list['entities']:
		if name == entity['name']:
			return entity['hex id']

def Hex_To_String(hex):
	'''Input mob hex id and return mob name string.'''
	entity_list = json.load(open("entities.json", "r"))

	for entity in entity_list['entities']:
		if hex == entity['hex id']:
			return entity['name']

print(Hex_To_String("1500"))