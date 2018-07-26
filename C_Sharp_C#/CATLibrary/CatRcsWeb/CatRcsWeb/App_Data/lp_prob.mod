/**/
param numItems, integer, >=1; 
param numContentAreas, integer, >=1; 
param testLength, integer, >=1;
set positions;
set Items:={0..numItems-1};
set contentAreas:={0..numContentAreas-1};
param itemInfo{Items} >= 0;
param alreadyAdministered{Items}, integer, >= 0;
param assignedLinkItem{positions}, integer;
param isLinkItem{i in Items, p in positions} binary;
param minLinkItems, integer, >=0;
param appliesForContentArea{i in Items, c in contentAreas} binary;
param minContentAreas {contentAreas}, integer, >= 0;
#xs for each linking position
var x{p in positions, i in Items} binary;
#ys for the remaining positions
var y{i in Items} binary;

/*objective*/
maximize testInformation: sum{p in positions, i in Items} itemInfo[i]*x[p,i] + sum{i in Items} itemInfo[i]*y[i];

/*constraints*/
s.t. c_testLength: sum{p in positions, i in Items} x[p, i] + sum{i in Items} y[i] = testLength;
s.t. c_onlyOnce{i in Items}: sum{p in positions} x[p, i] + y[i] <=1;
s.t. c_onePerPosition{p in positions}: sum{i in Items} x[p,i]<=1;
s.t. c_minlinkItems: sum{p in positions, i in Items} isLinkItem[i,p] * x[p,i] >= minLinkItems;
s.t. c_includeAlreadyAdministered: sum{p in positions, i in Items} alreadyAdministered[i] * x[p,i] + sum{i in Items} alreadyAdministered[i] * y[i] = sum{i in Items} alreadyAdministered[i];
s.t. c_onlyLinkItemsAtPosition{p in positions}: sum{i in Items} (if isLinkItem[i,p] then 0 else 1)*x[p,i] = 0;
s.t. c_keepLinkItems{p in positions}: (if assignedLinkItem[p]>=0 then x[p, assignedLinkItem[p]] else 1) = 1; 
s.t. c_cannotChangePastPositions{p in positions: p<sum{i in Items} alreadyAdministered[i]}: sum{i in Items} x[p,i]=if assignedLinkItem[p]>=0 then 1 else 0;
s.t. c_onlyFeadibleLinkItems{p in positions: p>=sum{i in Items} alreadyAdministered[i]}: sum{i in Items} x[p,i]*alreadyAdministered[i] = 0;
s.t. c_contentArea{c in contentAreas}: sum{p in positions, i in Items} x[p, i] * appliesForContentArea[i,c] + sum{i in Items} y[i] * appliesForContentArea[i,c] >= minContentAreas[c];

solve;
display{p in positions, i in Items}: x[p,i];
display{i in Items} y[i];
data;
param testLength:=21;
param numItems:=68;
param numContentAreas:=4;
param minLinkItems:=4;
param alreadyAdministered:=[0] 1 [1] 0 [2] 0 [3] 0 [4] 1 [5] 0 [6] 0 [7] 0 [8] 1 [9] 1 [10] 0 [11] 0 [12] 1 [13] 0 [14] 0 [15] 0 [16] 0 [17] 0 [18] 1 [19] 0 [20] 0 [21] 0 [22] 1 [23] 0 [24] 0 [25] 0 [26] 0 [27] 1 [28] 0 [29] 0 [30] 0 [31] 0 [32] 0 [33] 1 [34] 0 [35] 0 [36] 0 [37] 0 [38] 0 [39] 0 [40] 0 [41] 1 [42] 0 [43] 0 [44] 0 [45] 0 [46] 0 [47] 1 [48] 0 [49] 0 [50] 1 [51] 0 [52] 0 [53] 0 [54] 1 [55] 1 [56] 0 [57] 0 [58] 0 [59] 0 [60] 0 [61] 0 [62] 0 [63] 1 [64] 0 [65] 0 [66] 0 [67] 1;
param itemInfo:=[0] 0.224174 [1] 0.099381 [2] 0.063288 [3] 0.100232 [4] 0.24993 [5] 0.047628 [6] 0.076673 [7] 0.050306 [8] 0.111809 [9] 0.229021 [10] 0.056206 [11] 0.125204 [12] 0.081511 [13] 0.178043 [14] 0.138981 [15] 0.223812 [16] 0.197298 [17] 0.038457 [18] 0.226924 [19] 0.249182 [20] 0.050261 [21] 0.058522 [22] 0.232476 [23] 0.153287 [24] 0.097389 [25] 0.249976 [26] 0.199361 [27] 0.249332 [28] 0.096252 [29] 0.054065 [30] 0.077442 [31] 0.09136 [32] 0.143921 [33] 0.117545 [34] 0.026953 [35] 0.003352 [36] 0.031327 [37] 0.102656 [38] 0.216364 [39] 0.047972 [40] 0.240796 [41] 0.136489 [42] 0.016987 [43] 0.119867 [44] 0.034999 [45] 0.032038 [46] 0.180903 [47] 0.205895 [48] 0.114572 [49] 0.067732 [50] 0.185919 [51] 0.247352 [52] 0.11474 [53] 0.144202 [54] 0.24648 [55] 0.165465 [56] 0.051121 [57] 0.023647 [58] 0.178425 [59] 0.144014 [60] 0.106969 [61] 0.172486 [62] 0.179368 [63] 0.241122 [64] 0.057657 [65] 0.194198 [66] 0.136305 [67] 0.231226;
set positions:=0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20;
param assignedLinkItem:=[0] -1 [1] -1 [2] -1 [3] -1 [4] -1 [5] -1 [6] -1 [7] -1 [8] 9 [9] -1 [10] -1 [11] -1 [12] -1 [13] 18 [14] -1 [15] -1 [16] -1 [17] -1 [18] -1 [19] -1 [20] -1;
param isLinkItem: 0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20:=
0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
1 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
2 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
3 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
4 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
5 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
6 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
7 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
8 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
9 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0
10 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0
11 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0
12 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
13 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
14 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0
15 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0
16 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0
17 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
18 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0
19 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0
20 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0
21 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
22 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
23 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0
24 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0
25 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
26 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
27 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
28 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
29 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0
30 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
31 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
32 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
33 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
34 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
35 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
36 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
37 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
38 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
39 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
40 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
41 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0
42 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0
43 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
44 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
45 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
46 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0
47 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0
48 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0
49 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
50 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
51 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
52 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
53 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0
54 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
55 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0
56 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0
57 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
58 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
59 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
60 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
61 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1
62 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
63 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0
64 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
65 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
66 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0
67 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1
;
param appliesForContentArea: 0 1 2 3:=
0 0 1 0 0
1 0 0 1 0
2 0 1 0 0
3 0 1 0 0
4 0 0 1 0
5 0 1 0 0
6 1 0 0 0
7 0 0 0 1
8 0 0 0 1
9 0 0 0 1
10 0 0 1 0
11 0 0 1 0
12 0 1 0 0
13 1 0 0 0
14 1 0 0 0
15 0 0 1 0
16 0 1 0 0
17 0 0 1 0
18 0 1 0 0
19 0 1 0 0
20 0 1 0 0
21 0 0 0 1
22 0 1 0 0
23 0 0 1 0
24 0 0 0 1
25 1 0 0 0
26 1 0 0 0
27 1 0 0 0
28 0 0 0 1
29 0 0 1 0
30 0 0 0 1
31 0 1 0 0
32 0 0 0 1
33 0 0 1 0
34 0 0 1 0
35 0 0 1 0
36 0 1 0 0
37 0 0 0 1
38 1 0 0 0
39 0 0 0 1
40 0 1 0 0
41 0 1 0 0
42 0 0 0 1
43 0 0 1 0
44 1 0 0 0
45 1 0 0 0
46 1 0 0 0
47 0 1 0 0
48 0 0 0 1
49 0 0 0 1
50 0 0 1 0
51 0 0 0 1
52 0 0 0 1
53 0 1 0 0
54 0 0 0 1
55 1 0 0 0
56 1 0 0 0
57 0 0 1 0
58 0 0 1 0
59 0 0 1 0
60 0 0 1 0
61 0 0 0 1
62 1 0 0 0
63 0 0 1 0
64 0 1 0 0
65 0 0 0 1
66 1 0 0 0
67 0 0 0 1
;
param minContentAreas:=[0] 3 [1] 3 [2] 3 [3] 3;
end;
