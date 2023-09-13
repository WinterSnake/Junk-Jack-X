#-------------------------------------#
# Junk Jack World Compress/Decompress #
#                                     #
# Created By: Ryan Smith              #
#-------------------------------------#
#-------------------------------------#
# Parameters:
# -s: Strip
# -c: Compress
# -d: Decompress
# -o: Output File Name
# -h: Help
# -q: Quit
#-------------------------------------#

## Imports
import os
import gzip
import shutil
import os.path as osp

## Constants
eventEnded = False;
eventError = False;
#-------------------------------------#
## Multi-OS Clear Screen
def cls():
	if os.name == 'nt':
		os.system("cls");
	else:
		os.system("clear");
#-------------------------------------#
## Main Menu
def main_menu():
	cls();
	print("Junk Jack World De/Compressor\n");
	print("Parameters:" +
		"\n\t-c: Compress World" +
		"\n\t-d: Decompress World" +
		"\n\t-h: Help" +
		"\n\t-q: Quit Program");
	print("\n\n");
	return input("World Parameters: ");
#-------------------------------------#
## Parameter Parser
def parameter_parser(parameters):
	parameters = parameters.strip();
	if parameters[0] == '-':
		if parameters[1] == 'q':
			exit();
		elif parameters[1] == 'h':
			print_help();
		elif parameters[1] == 'd' or parameters[1] == 'c':
			compFlag = parameters[1];
			wrldFlag = parameters[3:]
			return {'Compress Flag':compFlag, 'World Name':wrldFlag};
		else:
			print_error(1);
	else:
		print_error(1);
#-------------------------------------#
## Print Help
def print_help():
	global eventError;
	print("\n\nHelp Screen:" + 
		"\n\t-c or -d MUST be defined first!" +
		"\n\tAfter a parameter has been assigned, use a space and type of the name of your world file." +
		"\n\n\tExample: -d World1.dat" +
		"\n\t\t-This will decompress the world \"World1.dat\"." +
		"\n\n\tExample: -c World2 -decompressed.dat" + 
		"\n\t\t-This will recompress the world \"World2 -decompressed.dat\".");
	input("\n\nPress Enter to return.");
	eventError = True;
#-------------------------------------#
## Print Error
def print_error(errorValue):
	global eventError;
	if errorValue == 1:
		print("Unknown parameter passed into parser")
	elif errorValue == 2:
		print("World does not exist in current directory.")
	else:
		print("Unknown error level.")
	input("\n\nPress Enter to try again.");
	eventError = True;
#-------------------------------------#
## Compressing World File
def compress_world(world_format):
	if len(world_format) == 2:
		o_world = os.getcwd() + "\\" + world_format['World Name'];
		n_world = o_world[:-4] + "-c.dat";
	else:
		o_world = os.getcwd() + "\\" + world_format['Old World Name'];
		n_world = os.getcwd() + "\\" + world_format['New World Name'];

	if osp.isfile(o_world):
		with open(o_world, 'rb') as dw_in:
			with gzip.GzipFile('', 'wb', 6, open(n_world, 'wb'), 0.0) as cw_out:
				shutil.copyfileobj(dw_in, cw_out);
		print("\n\nWorld successfully compressed");
	else:
		print_error(2);
#-------------------------------------#
## Decompressing World File
def decompress_world(world_format):
	if len(world_format) == 2:
		o_world = os.getcwd() + "\\" + world_format['World Name'];
		n_world = o_world[:-4] + "-d.dat";
	else:
		o_world = os.getcwd() + "\\" + world_format['Old World Name'];
		n_world = os.getcwd() + "\\" + world_format['New World Name'];
	if osp.isfile(o_world):
		with gzip.open(o_world, 'rb') as cw_in:
			with open(n_world, 'wb') as dw_out:
				shutil.copyfileobj(cw_in, dw_out);
		print("\n\nWorld successfully decompressed");
	else:
		print_error(2);
#-------------------------------------#
#-------------------------------------#
## Main Entry
while (eventEnded == False):
	w_param = main_menu();
	w_formt = parameter_parser(w_param);
	if eventError == False:
		if w_formt['Compress Flag'] == 'c':
			compress_world(w_formt);
		elif w_formt['Compress Flag'] == 'd':
			decompress_world(w_formt);
		input()
#-------------------------------------#