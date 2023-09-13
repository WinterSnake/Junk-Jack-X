import struct

a = struct.pack('<b', 15)

print(a)
# b'\x0f'

print(a.hex())
# 0f

print(int(a.hex(), 16))
# 15

print(f"0x{a.hex()}")
# 0x0f

b = f"0x{a.hex()}"

print(int(b, 16))
# 15

print(f"0x{15:X}")
# 0xF

#print(f"{b:X}")

print(f"{int(a.hex(), 16):X}")
# F

print(f"{int(struct.pack('<h', 15).hex(), 16):04X}")
# 0F00