# dBParser
## The No Hassle Database Handler

Check the demo and docs: http://dbParser.com/

I wanted a all in one package to handle different SQL based database engines. Currently for MVC.NET && .NET Core

### Basic Usage

Head over to the [docs](http://dbParser.com/) for more information.

    //Create a new appsettings.json
    //Fill connectionstring...
    
    //Construct new databaseparser...
    IDBParser dB = DbUtils.GetDBType();
    
    DataTable dt = dB.Read("select * from accountusers");
