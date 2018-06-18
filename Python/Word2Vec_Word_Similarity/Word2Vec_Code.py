# -*- coding: utf-8 -*-
"""
Created on Thu Nov 13 01:16:19 2016

@author: Saba Sabrin
"""

# import modules & set up logging
import gensim, logging
logging.basicConfig(format='%(asctime)s : %(levelname)s : %(message)s', level=logging.INFO)
 
# train word2vec
#sentences = [['first', 'sentence'], ['second', 'sentence']]
sentences = ["the quick brown fox jumps over the lazy dogs","yoyoyo you go home now to sleep"]
vocab = [s.split() for s in sentences]
#print(vocab)
#voc_vec = gensim.models.Word2Vec(vocab, min_count=1)

all_words = [line.split() for line in open("toyCorpus.txt")]
                 
#print(all_words)

model = gensim.models.Word2Vec(all_words, min_count=1)

model.doesnt_match("breakfast cereal dinner lunch".split())

#model.save("myModel")