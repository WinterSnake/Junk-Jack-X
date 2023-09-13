# Compare compressed/compressed
echo "Comparing python.compressed to cs.compressed.."
cmp -l 'python/debug.compressed' 'cs/debug.compressed' | gawk '{printf "%08X %02X %02X\n", $1, strtonum(0$2), strtonum(0$3)}'
echo "Comparing python.decompressed to cs.decompressed.."
cmp -l 'python/debug.decompressed' 'cs/debug.decompressed' | gawk '{printf "%08X %02X %02X\n", $1, strtonum(0$2), strtonum(0$3)}'
echo "Comparing python.recompressed to cs.recompressed.."
cmp -l 'python/debug.recompressed' 'cs/debug.recompressed' | gawk '{printf "%08X %02X %02X\n", $1, strtonum(0$2), strtonum(0$3)}'
# Compare compressed/recompressed
echo "Comparing python.compressed to python.recompressed.."
cmp -l python/debug.compressed python/debug.recompressed | gawk '{printf "%08X %02X %02X\n", $1, strtonum(0$2), strtonum(0$3)}'
echo "Comparing cs.compressed to cs.recompressed.."
cmp -l cs/debug.compressed cs/debug.recompressed | gawk '{printf "%08X %02X %02X\n", $1, strtonum(0$2), strtonum(0$3)}'
