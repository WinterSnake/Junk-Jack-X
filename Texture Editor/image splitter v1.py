##----------------------------------------------##
## Junk Jack X Texture Editor - [Python Edition]##
## Created By: Ryan Smith                       ##
##----------------------------------------------##
## File Description                             ##
##----------------------------------------------##

## Imports
import os, os.path as osp
from PIL import Image

## Constants
image_object = Image.open("rocks.png")

## Main

# -Directory
if not osp.isdir("rocks"):
    os.mkdir("rocks")

# -Split Image [Subsection]
for sub_x in range(0, 2):
    for sub_y in range(0, 8):

        # -Crop Subsection
        new_tuple = (sub_x * 1024, sub_y * 256, 1024 + (sub_x * 1024), 256 + (sub_y * 256))
            # -Get Co-Ords
        subsection = image_object.crop(new_tuple)
            # -Crop Position

        # -Format Subsection
        subsection_format = format((sub_y - (sub_y * sub_x)) + ((sub_y + 8) * sub_x), 'X').zfill(2)

        # -Format Directory
        if not osp.isdir(fr"rocks\Subsection XX{subsection_format}"):
            os.mkdir(fr"rocks\Subsection XX{subsection_format}")
        #if not osp.isdir(fr"rocks\Subsection XX{subsection_format}\objects"):
            #os.mkdir(fr"rocks\Subsection XX{subsection_format}\objects")

        # -Save Subsection Image
        subsection.save(fr"rocks\Subsection XX{subsection_format}.png")

        # -Split Image [Single]
        for x in range(0, 32):
            for y in range(0, 8):

                # -Crop Singlesection
                new_tuple = (x * 32, y * 32, 32 + (x * 32), 32 + (y * 32))
                    # -Get Co-Ords
                singlesection = subsection.crop(new_tuple)
                    # -Crop Position

                # -Format Subsection
                singlesection_format = format((x - (y * x)) + ((x + 32) * y), 'X').zfill(2)

                # -Save Subsection Image
                singlesection.save(fr"rocks\Subsection XX{subsection_format}\{singlesection_format}{subsection_format}.png")