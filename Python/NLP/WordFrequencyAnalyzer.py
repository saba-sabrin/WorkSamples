# -*- coding: utf-8 -*-
"""
Created on Wed Oct 25 01:25:54 2017

@author: Saba
"""

import os
import re

#!/usr/bin/python3

# name of the data file
fileName = "data.txt"
resultFileName = "result.txt";

# get directory information of the file
fileDir = os.path.dirname(os.path.realpath(fileName))

# reading words from the file
data = open(os.path.join(fileDir, fileName),'r+')

# a dictionary for holding word occurences
wordFrequency = { }

# iterating through each word
for term in data.read().lower().split():
    # using regular expression to get only the words and ommitting the symbols
    words = re.compile('\\w+').findall(term)
    # iterating through the result values
    for value in words:
        # checking whether the result values are alphabetical or not and ignoring the numeric data
        if(value.isalpha()):
            finalWord = str(value)
            # this if-else section checks whether a word is added in the dictionary or not
            # For an existing word, the count value will be increased every time
            if finalWord not in wordFrequency:
                wordFrequency[finalWord] = 1
            else:
                wordFrequency[finalWord] += 1
        else:
            ''
# close the data file
data.close()
     
# writing the data to an existing result file
result = open(os.path.join(fileDir, resultFileName),'a')
  
# this section sorts the dictionary items based on their values   
for key in sorted(wordFrequency, key = wordFrequency.get, reverse=True):
    result.write("%s (%s)" % (str(key), wordFrequency[key]) + '\n')


# close the result file
result.close()