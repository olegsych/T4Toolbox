---
title: Pre-Requisites
layout: default
---
# Pre-Requisites

To follow the T4 Toolbox tutorial you will need latest versions of:

* [Visual Studio 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx).
* [SQL Server Data Tools](https://msdn.microsoft.com/en-us/library/mt204009.aspx) 

A local instance of SQL Server should be installed with SQL Server Data Tools. This tutorial assumes that SQL Server 2016 (v13) is running. 
You should be able to use earlier and, presumably, later versions by using the appropriate server instance.

Install the Northwind sample database.
* Install the [SQL2000SampleDb.msi](http://www.microsoft.com/en-us/download/details.aspx?id=23654).
* Open the `C:\SQL Server 2000 Sample Databases\instnwnd.sql` script in Visual Studio and execute it. 
* When prompted, select the appropriate local SQL server instance, such as `(localdb)\ProjectsV13` for SQL Server 2016.
* Verify that the database was created successfully using the _SQL Server Object Explorer_.    
