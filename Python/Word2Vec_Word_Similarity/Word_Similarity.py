# -*- coding: utf-8 -*-
"""
Created on Thu Nov 17 12:20:10 2016

@author: Saba
"""

#import gzip
import numpy as np

class WordMap(object):
    def __init__(self, embeddings, wordMapping):
        self.embeddings = embeddings
        self.wordMapping = wordMapping

# You can use this code to read in the embeddings:
def getEmbeddings():
	idx = 0
	embeddings = []  # list declaration
	word2Idx = {}  # Dictionary for Mapping of words to the row in the embeddings matrix
	#with open.gzip(2014_tudarmstadt_german_100mincount.vocab.gz)
	with open('sample_data.txt', 'r') as fIn:
	    idx = 0
	    for line in fIn:
		    split = line.strip().split(' ')
		    embeddings.append(np.array([float(num) for num in split[1:]]))
		    word2Idx[split[0]] = idx
		    idx += 1
	
	objWordMap = WordMap(embeddings, word2Idx)
	return objWordMap

def getCosineDist(x_vec, y_vec):

	dotprod = np.dot(x_vec, y_vec)
	print("Multiplication result", dotprod)
	
	# Normalize X
	sum_x_vec = 0.0
	for i in range(len(x_vec)):
		sum_x_vec += sum_x_vec + (x_vec[i] * x_vec[i])
		#print(sum_x_vec)
	#sum_x_vec = np.sum(sum_x_vec)
	print("Final", sum_x_vec)
	
	norm_x_vec = np.sqrt(sum_x_vec)
	print("Normalized", norm_x_vec)
	
	# Normalize Y vector
	sum_y_vec = 0.0
	for i in range(len(y_vec)):
		sum_y_vec += sum_x_vec + (y_vec[i] * y_vec[i])
		#print(sum_y_vec)
	#sum_x_vec = np.sum(sum_x_vec)
	print("Final", sum_y_vec)
	
	norm_y_vec = np.sqrt(sum_y_vec)
	print("Normalized", norm_y_vec)
	
	distance = dotprod / np.abs((sum(x_vec) * sum(y_vec)))
	distance = distance / 2;
	return distance
	
def getMostSimilarPairs(sentence1, sentence2, calcType, threshold):
	result = getEmbeddings()
	#print(len(result.embeddings))
	#for v in result.embeddings:
		#print("Value", v) 
	
	print("Distance", getCosineDist(result.embeddings[2], result.embeddings[6]))
	print(result.wordMapping)
	return


# Make sure to use reasonable variable names and add comments.
getMostSimilarPairs("die katze frisst gern mäuse .", "die hunde jagen oft kaninchen .", "cosine", 0.7)
