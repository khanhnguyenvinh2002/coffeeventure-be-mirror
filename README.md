## This is the public version of Coffee Venture back end!

All sensitive data has been removed. This can not be run and just serve to be example of how the code works. 

The hidden files are CVDbContext.cs which is used to connect to the database, and appsettings.json which contains log in information. 

This code is only accessible to the coffee venture site origin. please do not try to access the data! 

## Main features to notice:

### JWT authorization can be foyund in Startup.cs 

and is responsible for making sure that users are viewing allowed contents only

### 3-layered architecture 

1. user interface layer (Controllers folder): responsible to interacting with front-end.
2. business layer (Service folder): performing all logical calculations and operations, calling methods from database layer.
3. database layer: used to connect the business layer with database. It contains methods such as insert, delete, update.

### dependency injection

The use of object to provide dependencies of other objects through an interface to help client class not depend on 1 or more classes

## unit of work pattern (model/ unitsOfWork)

making sure all transactions are done in one single transaction rather than doing multiple database transactions. 

Thank you for reading! Happy coding!