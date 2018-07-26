**********************
***** Copyright ******
***  Saba Sabrin  ****
**********************

These are actual project codes from my current Company and it is only provided to demonstrate my programming knowledge and style ****


There are two projects provided as work samples:

1. Computerized Adaptive Testing (CAT API)

=> It has one backend application which is published as an API. The original library was written in R. Following a Wrapper/Mediator based pattern, the library functions are developed in C#.
Necessary Tests are written using NUnit package.

=> A Web module prepared for Linux OS using .Net Mono framework. It was a challenging task for me to configure a sample Virtual machine with the necessary configuration and host it.
Afterwards, the web application was accessible and necessary functions can be invoked.


2. Data_Warehouse

=> This project aims is to import json data within a flat but relational hierarchy with maximum optimization inside a MySQL data warehouse. 

=> The data extraction from json is done in an automatic way using C# reflection mechanism. 

=> The challenge lies within the automatic representation of data warehouse depending on the json data structure and large scale data processing as it might contain millions of data.


4. "Distributed Task Scheduler" project implements a distributed task scheduling system through a simple calendar interface using a Publish/SUbscribe based Middleware known as "uMundo".

==> **Link of the uMundo Library: https://github.com/tklab-tud/umundo

This application is written in C# Language and the tasks are shared through the middleware platform with another task scheduling system implemented in java. 
If one user creates a task through any of the C# or java application, another connected user automatically gets notifies if the user has subscribed or accepted the request 
to the particular task or event.


5. "Smart Security System" is an IoT based data processing application which can be imagined as a part of a large Smart Home security system. 

This application reads different motion based sensor data, able to generate security-alarms, synchronize time through LAN server, provides user interface through touch panel, 
authenticates based on previously configured user data and real-time motions. I have used the platform "Microsoft Gadgeteer" environment to develop this project. 
The link for the platform is, "https://www.microsoft.com/en-us/research/project/net-gadgeteer/".
