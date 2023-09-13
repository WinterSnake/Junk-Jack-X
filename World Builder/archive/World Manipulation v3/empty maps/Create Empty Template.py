x_max = 512
# Tiny = 512
# Small = 768
# Normal = 1024
# Large = 2048
# Huge = 4096
y_max = 128
# Tiny = 128
# Small = 256
# Normal = 256
# Large = 384
# Huge = 512

air_array = bytearray([0, 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0])
bedrock_array = bytearray([84, 128, 84, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0])

with open("[x] Empty - (Decompressed).dat",'wb') as b_file:
    for x in range(0, x_max):
        for y in range(0, y_max):
            if y == 0:
                b_file.write(bedrock_array)
                print(f"({x}, {y}) = Bedrock")
            else:
                b_file.write(air_array)
                print(f"({x}, {y}) = Air")
    
input()