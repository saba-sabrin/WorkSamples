# Connecting with "BigR"
library(bigr)
bigr.connect(host="bi-hadoop-prod-067.services.dal.bluemix.net", port=7052, database="default", user="biblumix", password="p@~ePDVFz3j9")

# Check Status of the Connection
is.bigr.connected()

# Loading Data from "Big Sheets"

circuits <- bigr.frame(dataPath = "sheets.circuits", dataSource = "BIGSQL", useMapReduce = TRUE)

# doors <- bigr.frame(dataPath = "sheets.door", dataSource = "BIGSQL", useMapReduce = TRUE)

env <- bigr.frame(dataPath = "sheets.env", dataSource = "BIGSQL", useMapReduce = TRUE)

# meters <- bigr.frame(dataPath = "sheets.meter_123", dataSource = "BIGSQL", useMapReduce = TRUE)
# motions <- bigr.frame(dataPath = "sheets.motion", dataSource = "BIGSQL", useMapReduce = TRUE)

phases <- bigr.frame(dataPath = "sheets.phases", dataSource = "BIGSQL", useMapReduce = TRUE)

#switches <- bigr.frame(dataPath = "sheets.switch", dataSource = "BIGSQL", useMapReduce = TRUE)

# Loading Data using "BigR Frame"
# Circuit Data loaded using Big-Sheet
#circuits <- bigr.frame(dataPath = "/user/biblumix/Home/circuit/circuitAll_HomeA.csv.gz", dataSource = "DEL", useMapReduce = TRUE, header=TRUE, coltypes=c("character","integer","integer","numeric","numeric"))

door <- bigr.frame(dataPath = "/user/biblumix/Home/door/door_All_HomeA.csv", 
                   dataSource = "DEL", useMapReduce = TRUE, 
                   header=TRUE, 
                   coltypes=c("character","integer","integer"))

# env <- bigr.frame(dataPath = "/user/biblumix/Home/env/env_All_HomeA.csv", dataSource = "DEL", useMapReduce = TRUE, header=TRUE, coltypes=c("integer","numeric","numeric","numeric","numeric","numeric","integer","integer","integer","numeric","numeric","numeric","numeric"))

meters <- bigr.frame(dataPath = "/user/biblumix/Home/meter/meter_All_HomeA.csv", 
                  dataSource = "DEL", useMapReduce = TRUE, 
                  header=TRUE, 
                  coltypes=c("character","integer","numeric","integer"))

motions <- bigr.frame(dataPath = "/user/biblumix/Home/motion/motion_All_HomeA.csv", 
                   dataSource = "DEL", useMapReduce = TRUE, 
                   header=TRUE, 
                   coltypes=c("character","integer","integer"))

# phases <- bigr.frame(dataPath = "/user/biblumix/Home/phase/phase_All_HomeA.csv.gz", dataSource = "DEL", useMapReduce = TRUE, header=TRUE, coltypes=c("integer","numeric","numeric"))

switches <- bigr.frame(dataPath = "/user/biblumix/Home/switch/switch_All_HomeA.csv", 
                   dataSource = "DEL", useMapReduce = TRUE, 
                   header=TRUE, 
                   coltypes=c("character","integer","numeric","numeric","integer"))

# Check Circuits Data
head(circuits, 10)

# Check Environment Data
head(env, 10)

# Check column names and types (example)
coltypes(env)
colnames(circuits)

# Random sampling of Circuits Data
circuitSample = bigr.sample(circuits,0.01)
head(circuitSample,5)
unique(circuitSample$circuitnumber)

# Subset of Circuit data by "CircuitNumber"
attach(circuclits)
cirSubset3 <- circuits[circuitnumber == 1, c("circuitname","circuitnumber","timestamputc","realpowerwatts","apparentpowervas")]

# Merging circuits and environment Data
envEx = merge(env, circuits, by = "timestamputc")
head(envEx)

# Graph Plot on circuit & environment data
plot(envEx$insidetemp,envEx$circuitname)

# Correlation on circuit & environment data
cor(envEx[,c("insidetemp","realpowerwatts")])
cor(env[,c("outsidetemp","realpowerwatts")])
cor(envEx[,c("insidetemp","outsidetemp")])
cor(env[,c("insidehumidity","outsidehumidity")])
cor(env[,c("insidehumidity","realpowerwatts")])
cor(env[,c("outsidehumidity","realpowerwatts")])
