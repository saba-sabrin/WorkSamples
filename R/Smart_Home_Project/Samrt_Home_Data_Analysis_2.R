
library(bigr)
bigr.connect(host="bi-hadoop-prod-450.services.dal.bluemix.net", 
             port=7052, database="default", user="biblumix", 
             password="bK3WE0@Y4~oP")

is.bigr.connected()

circuits <- bigr.frame(dataPath = "/user/biblumix/combined_circuits.csv", dataSource = "DEL", useMapReduce = TRUE, 
                                    header=TRUE,coltypes=c("character","numeric","numeric","numeric","numeric"))
head(circuits)
cd<-as.data.frame(circuits);
head(cd)
# subset for all the circuit readings for fridge
fd<-subset(cd,cd$CircuitNumber==15)

doordata <- bigr.frame(dataPath = "/user/biblumix/combined_door.csv", dataSource = "DEL", useMapReduce = TRUE, 
                       header=TRUE,coltypes=c("character","numeric","numeric","numeric","numeric"))
head(doordata)
dd<-as.data.frame(doordata);
head(dd)

mddfd<-merge(fd,dd,by = c("TimestampUTC"))
cor(mddfd[,c("RealPowerWatts","OpenOrClosed")])
cor.test(mddfd$OpenOrClosed,mddfd$RealPowerWatts)

#pattern discovery
wm<-subset(cd,cd$CircuitNumber==14)
hist(wm$RealPowerWatts)
#as the maximum readings for washing machine was seen between 300 to 750 watts
#took average and plotted the graph by considering all the readings above average 
#as an indication that the washing machine is in use
wmOn<-subset(wm, wm$RealPowerWatts >=avwm )
head(wmOn)

cd$TimestampEST<-as.POSIXlt(cd$TimestampUTC,origin="1970-01-01", tz="EST")
cd$Date<-as.Date(cd$TimestampEST,origin="1970-01-01", tz="EST")
cd$Time<-strftime(cd$TimestampEST,"%H")

dd$TimestampUTC<-as.POSIXlt(dd$TimestampUTC,origin="1970-01-01", tz="EST")
dd$Date<-as.Date(dd$TimestampUTC,origin="1970-01-01", tz="EST")
dd$Time<-strftime(dd$TimestampUTC,"%H")

wmOn$TimestampEST<-as.POSIXlt(wmOn$TimestampUTC,origin="1970-01-01", tz="EST")
wmOn$Date<-as.Date(wmOn$TimestampEST,origin="1970-01-01", tz="EST")
wmOn$Time<-strftime(wmOn$TimestampEST,"%H")

#usage of the washing machine in all the months 
plot(as.Date(wmOn$Date), wmOn$RealPowerWatts, 
       xlab= "Months", ylab= "Power Consumption", type='p', col='red')
#Now just for June month and July month
wmOnJune <-subset(wmOn, wmOn$Date >="2012-06-01" & wmOn$Date <= "2012-06-30")
wmOnJuly <-subset(wmOn, wmOn$Date >="2012-07-01" & wmOn$Date <= "2012-07-31")

plot(as.Date(wmOnJune$Date), wmOnJune$RealPowerWatts, xlab= "June Month", ylab= "Power Consumption", type='p', col='red')
#Pattern seen in June is every weekend there the washing machine is in use.
plot(as.Date(wmOnJuly$Date), wmOnJuly$RealPowerWatts, xlab= "July Month", ylab= "Power Consumption", type='p', col='red')
#Pattern seen random. The use of washing machine is in the weekdays but not on specific days

#To check the in general utilities related to washing machien like the dryer.
dry<-subset(cd,cd$CircuitNumber == 2)
dry$TimestampEST<-as.POSIXlt(dry$TimestampUTC,origin="1970-01-01", tz="EST")
dry$Date<-as.Date(dry$TimestampEST,origin="1970-01-01", tz="EST")
dry$Time<-strftime(dry$TimestampEST,"%H")

# ALl dryer data plot
plot(as.Date(dry$Date), dry$RealPowerWatts, xlab= "Month", ylab= "Power Consumption for dryer", type='p', col='red')

# Only dryer in use :
min(dry$RealPowerWatts)
max(dry$RealPowerWatts)
hist(dry$RealPowerWatts)
dryonav<-ave(dry$RealPowerWatts)
dryOn<-subset(dry,dry$RealPowerWatts >= 2650.425)
plot(as.Date(dryOn$Date), dryOn$RealPowerWatts, xlab= "Month", ylab= "Power Consumption for dryer", type='p', col='red')
dryOnJune<-subset(dryOn,dryOn$Date >= "2012-06-01" & dryOn$Date<="2012-06-30"    )
plot(as.Date(dryOnJune$Date), dryOnJune$RealPowerWatts, xlab= "June", ylab= "Power Consumption for dryer", type='p', col='blue')
dryOnJuly<-subset(dryOn,dryOn$Date >= "2012-07-01" & dryOn$Date<="2012-07-31"    )
plot(as.Date(dryOnJuly$Date), dryOnJuly$RealPowerWatts, xlab= "July", ylab= "Power Consumption for dryer", type='p', col='blue')

#To see what is the trend on one of the weekend, chose 2nd June. 
cdJune2<-subset(cd,cd$Date =="2012-06-02")
plot(cdJune2$Time, cdJune2$RealPowerWatts, xlab= "Time on 2nd June", ylab= "Power Consumption", type='p', col='red')
#to check which circuit consumed how much power on 2nd of June.
plot(cdJune2$CircuitNumber, cdJune2$RealPowerWatts, xlab= "CircuitNumber", ylab= "Power Consumption", type='p', col='blue')
#as grid corresponds to sum of all other circuits , remove the grid data.
plot(cdJune2NoGrid$CircuitNumber, cdJune2NoGrid$RealPowerWatts, xlab= "CircuitNumber", ylab= "Power Consumption", type='p', col='blue')

cdJune2NoGridAfter15<-subset(cdJune2NoGrid,cdJune2NoGrid$Time >= 15 & cdJune2NoGrid$Time <= 20)
plot(cdJune2NoGridAfter15$Time, cdJune2NoGridAfter15$RealPowerWatts, xlab= "Time After 15:00 on 2nd June", ylab= "Power Consumption", type='p', col='blue')

#when did home A have visitors?
guest<-subset(cd,cd$CircuitNumber == 5)
head(guest)
max(guest$ RealPowerWatts)
min(guest$RealPowerWatts)
ave(guest$RealPowerWatts)
guestOn<-subset( guest,guest$RealPowerWatts >= 332.2858)
plot(as.Date(guestOn$Date), guestOn$RealPowerWatts, xlab= "Months", ylab= "Power Consumption", type='p', col='blue')


masterBath<-subset(cd,cd$CircuitNumber == 6)
min(masterBath$RealPowerWatts)
max(masterBath$RealPowerWatts)
ave(masterBath$RealPowerWatts)
masterBathOn<-subset(masterBath,masterBath$RealPowerWatts >= 658.4029)
plot(as.Date(masterBathOn$Date), masterBathOn$Time, xlab= "Months", ylab= "Time", type='p', col='green')

masterBathOnJuly<-subset(masterBathOn,masterBathOn$Date >= "2012-07-01" & masterBathOn$Date <= "2012-07-31")
plot(as.Date(masterBathOnJuly$Date), masterBathOnJuly$RealPowerWatts, xlab= "Days", ylab= "Power Consumption", type='p', col='green')
plot(masterBathOnJuly$Time,as.Date(masterBathOnJuly$Date), xlab= "Time", ylab= "Date", type='p', col='green')

