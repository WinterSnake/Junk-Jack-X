##----------------------------------------------##
## Junk Jack X Texture Editor - [Python Edition]##
## Created By: Ryan Smith						##
##----------------------------------------------##
## File Description								##
##----------------------------------------------##

## Imports
import gzip, zlib, shutil, io

## Functions
def compress_test_file():
	with open("test-O.dat", 'rb') as f_in:
		with gzip.GzipFile('', 'wb', 6, open("test-C.dat", 'wb'), 0.0) as f_out:
			shutil.copyfileobj(f_in, f_out)

def decompress_test_file():
	with gzip.open("test-C.dat", 'rb') as f_in:
		with open("test-D.dat", 'wb') as f_out:
			shutil.copyfileobj(f_in, f_out)

def decompress_test_str():
	compressed_bytestr = b'\x1F\x8B\x08\x00\x00\x00\x00\x00\x02\xFF\x13\x54\x16\x15\x76\x34\x11\x02\x00\x57\xBA\x9A\x0E\x07\x00\x00\x00'
	print(compressed_bytestr)
	bytestr_array = bytearray(compressed_bytestr)
	print(bytestr_array)
	decompressed_bytestr = zlib.decompress(bytestr_array, 24)
	print(decompressed_bytestr)

def compress_test_str():
	out = io.BytesIO()
	decompressed_bytestr = b'\x11\x23\x15\x13\x41\x34\x12'
	compressed_bytestr = b'\x1F\x8B\x08\x00\x00\x00\x00\x00\x02\xFF\x13\x54\x16\x15\x76\x34\x11\x02\x00\x57\xBA\x9A\x0E\x07\x00\x00\x00'
	print(decompressed_bytestr)

	with gzip.GzipFile(fileobj = out, mode = 'wb', compresslevel = 6, mtime = 0.0) as f_out:
		f_out.write(decompressed_bytestr)

	print(compressed_bytestr)
	print(out.getvalue())

def combine_bytestr():
	bytestr1 = b'\x1F\x8B\x08\x00\x00\x00'
	bytestr2 = b'\x02\xFF\x13\x54\x16\x15'

	combined_bytes = bytestr1 + bytestr2


	print(bytestr1)
	print(bytestr2)
	print(combined_bytes)

## Main