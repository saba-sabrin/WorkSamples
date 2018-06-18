from pyspark import SparkConf, SparkContext
from pyspark.mllib.feature import HashingTF
from pyspark.mllib.feature import IDF

# spark config
conf = SparkConf().setMaster("local").setAppName("SparkTFIDF")
sc = SparkContext(conf = conf)

# Load documents (one per line).
rawData = sc.textFile("e:/sundog-consult/Udemy/DataScience/subset-small.tsv")
fields = rawData.map(lambda x: x.split("\t"))
documents = fields.map(lambda x: x[3].split(" "))

# Store the document names for later:
documentNames = fields.map(lambda x: x[1])

# Now hash the words in each document to their term frequencies:
hashingTF = HashingTF(100000)  #100K hash buckets just to save some memory
tf = hashingTF.transform(documents)

# an RDD of sparse vectors representing each document, where each value maps to the term frequency of each unique hash value.
tf.cache()
idf = IDF(minDocFreq=2).fit(tf)
tfidf = idf.transform(tf)

# hash values "Gettysburg" maps to by finding the and index a sparse vector from HashingTF
gettysburgTF = hashingTF.transform(["Gettysburg"])
gettysburgHashValue = int(gettysburgTF.indices[0])

# extract the TF*IDF score for Gettsyburg's hash value into a new RDD for each document
gettysburgRelevance = tfidf.map(lambda x: x[gettysburgHashValue])

# zip in the document names so we can see which is which:
zippedResults = gettysburgRelevance.zip(documentNames)

# print the document with the maximum TF*IDF value:
print("Best document for Gettysburg is:")
print(zippedResults.max())
