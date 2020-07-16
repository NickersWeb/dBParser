# dBParser
## The No Hassle Database Handler

Check the demo and docs: http://dbParser.com/

I wanted an all in one package to handle different SQL based database engines.

Based on CRUD persistent storage.

- - - -

### Supports:

* sql
* sqlite
* oracle
* mysql

### Basic Usage

Head over to the [docs](http://dbParser.com/) for more information.

    //Create a new appsettings.json
    //Fill connectionstring...
    {
     "ConfigManager": {
      "ConnectionStrings": {
       "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DB_HERE;User ID=USERID_HERE;Password=USER_PWD;",
       "DataBaseType": "sql"
      }
     }
    }
    
    //Construct new databaseparser...
    IDBParser dB = DbUtils.GetDBType();
    
    DataTable dt = dB.Read("select * from accountusers");
    
    //Caching via 
    DataTable dt DbUtils.ReadCache("select * from accountusers");
    
    //Full Database Extended Properties
    string value = DbUtils.ReadExtendedPropCache("database_settings", true);
    
    //Can use this to write any query to db
    dB.Read("");
