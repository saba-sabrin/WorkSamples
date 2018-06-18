freq <- DF$Value
range(freq, na.rm = T)
breaks = seq(1, 18005, by=1000)
freq.cut <- cut(freq, breaks, right = FALSE)
freq.f <- table(freq.cut)
cbind(freq.f)

max(freq.f, na.rm = T)

# Create a second subset
SubsetFreq_2 <- subset(DF, subset = DF$Value<=1000)

freq_2 <- SubsetFreq_2$Value
range(freq_2, na.rm = T)
breaks_2 = seq(1, 1000, by=100)
freq.cut_2 <- cut(freq_2, breaks_2, right = FALSE)
freq.f_2 <- table(freq.cut_2)
cbind(freq.f_2)


# Create a third subset
SubsetFreq_3 <- subset(SubsetFreq_2, subset = SubsetFreq_2$Value<=110)

freq_3 <- SubsetFreq_3$Value
range(freq_3, na.rm = T)
breaks_3 = seq(1, 100, by=10)
freq.cut_3 <- cut(freq_3, breaks_3, right = FALSE)
freq.f_3 <- table(freq.cut_3)
cbind(freq.f_3)


# Create a fourth subset
SubsetFreq_4 <- subset(SubsetFreq_3, subset = SubsetFreq_3$Value<=11)

freq_4 <- SubsetFreq_4$Value
range(freq_4, na.rm = T)
breaks_4 = seq(1, 11, by=1)
freq.cut_4 <- cut(freq_4, breaks_4, right = FALSE)
freq.f_4 <- table(freq.cut_4)
cbind(freq.f_4)


SubsetFreq_5 <- subset(SubsetFreq_4, subset = SubsetFreq_4$Value<=2)

freq_5 <- SubsetFreq_5$Value
range(freq_5, na.rm = T)
breaks_5 = seq(1, 2, by=0.2)
freq.cut_5 <- cut(freq_5, breaks_5, right = FALSE)
freq.f_5 <- table(freq.cut_5)
cbind(freq.f_5)


