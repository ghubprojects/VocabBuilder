import sys
import os
from gtts import gTTS

if len(sys.argv) < 3:
    print("Usage: tts.py \"text\" \"file_path\"")
    sys.exit(1)

text = sys.argv[1]
file_path = sys.argv[2]

tts = gTTS(text, lang='en', tld='com')
tts.save(file_path)

print(f"Saved to {file_path}")