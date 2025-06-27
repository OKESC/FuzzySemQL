#
#   Copyright (c) OKESC, Alberto Romo Valverde
#
#   This source code is licensed under the MIT license found in the
#   LICENSE file in the root directory of this source tree.
#
#

# -*- coding: utf-8 -*-
input_file_path = "cc.en.300.vec"
output_file_path = "cc.en.300.short.vec"
vocabulary_path = "vocabulary.txt"  # One word per line
MAX_WORDS = 40000  # Adjust this number according to desired final size (~50MB with 110k)

# 1. Load custom vocabulary
with open(vocabulary_path, 'r', encoding='utf-8') as vfile:
    vocab_user = set(line.strip().lower() for line in vfile if line.strip())

selected_words = set()
lines_to_write = []

# 2. Iterate through the embeddings file
with open(input_file_path, "r", encoding="utf-8", errors="ignore") as fin:
    header = fin.readline()  # Header can be ignored, your loader skips it as well
    count = 0

    for line in fin:
        parts = line.strip().split(' ')
        if len(parts) < 10:
            continue
        word = parts[0].lower()
        # Add words, Priority: Most frequent first, then from custom vocabulary
        if count < MAX_WORDS or word in vocab_user:
            if word not in selected_words:
                lines_to_write.append(line)
                selected_words.add(word)
            count += 1

# 3. Check if all vocabulary words are present
missing = vocab_user - selected_words
if missing:
    print("Vocabulary words NOT present in embedding model:", missing)

# 4. Write the resulting file
with open(output_file_path, "w", encoding="utf-8") as fout:
    for line in lines_to_write:
        fout.write(line)