#!/usr/bin/python
##-------------------------------##
## Junk Jack X: Json Builder     ##
## Written By: Ryan Smith        ##
##-------------------------------##

## Imports
import json
from typing import Any

## Body
with open("english.json", 'r') as f:
    data: dict[str, Any] = json.load(f)
print(data)
