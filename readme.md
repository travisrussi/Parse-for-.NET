#Parse for .NET
##A very basic library for accessing [Parse](www.parse.com, "Parse") data from a .NET app.

I recently needed to access Parse data from .NET for an iOS app I'm writing and wrote this library in a couple of hours. I'm not sure if anybody else would find this useful, but I'm releasing it as open source (MIT licence) just in case.

The code isn't particularly complex and, although sparsely commented, mildly documented. If you have any questions, I'll be happy to answer them. There are also some unit tests that go over normal use cases.

### Parse Object Creation

	Parse.ParseClient myClient = new Parse.ParseClient("myappid", "mysecretkey");
	Parse.ParseObject myObject = myClient.CreateObject("MyClass", new { foo = "bar" });
	Dictionary<String,String> allObjects = myClient.GetObjectsWithQuery("MyClass", new { foo = "bar" });
    
### Parse File Creation
	ParseFile file = new ParseFile("c:\path\to\file.txt");
	
	myClient.CreateFile(file);
	Console.WriteLine(String.Format("File name: {0}", file.Name);
	Console.WriteLine(String.Format("File url: {0}", file.Url);

#### Parse File Delete
	localClient.DeleteFile(file);
	
#### File relational reference:
	myObject["file"] = file;

### Parse Querying using Constraints

These methods can be used to query Parse objects in a more complex way. Note that many of these can be combined, 
such as the less than/greater than constraints, to narrow down your results even further. The Parse-for.Net library
relies on Parse's REST API to create the queries and as such any issues can be debugged by looking into the generated
JSON to be passed to Parse and cross-checking it with the [Query Constraints](https://parse.com/docs/rest#queries-constraints) 
section of the REST API documentation.
    
**Less Than Inclusive/Exclusive and Greater Than Inclusive/Exclusive**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        Price = new Constraint(lessThan: 1500),
        NegativeReviews = new Constraint(lessThanOrEqualTo: 2),
        Gigahertz = new Constraint(greaterThanOrEqualTo: 3),
        WarrantyYears = new Constraint(greaterThan: 0)
    });

**Not Equal To**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        Brand = new Constraint(notEqualTo: "HP")
    });

**In List**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        Brand = new Constraint(@in: new string[3] {"Apple", "Dell", "IBM"})
    });

**Not In List**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        Brand = new Constraint(notIn: new string[3] {"Apple", "Dell", "IBM"})
    });

**Value Does or Does Not Exist**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        PCIExpress = new Constraint(exists: true)
    });
    
**Select**

Used to perform more complex queries across multiple Parse objects. The example below was created using 
Parse-for-.Net's Constraint class, based on the $select example on the following page: [https://parse.com/docs/rest#queries-constraints](https://parse.com/docs/rest#queries-constraints)

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("User", new
    {
        Hometown = new Constraint(select: new
        {
            query = new
            {
                className = "Team",
                where = new 
                { 
                    winPct = new Constraint(greaterThan: 0.5)
                }
            }
        })
    });

**Regex (Perl-based)**

    var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
    var results = parseClient.GetObjectsWithQuery("Computer", new
    {
        ManufacturerEmail = new Constraint(regex: @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}\b")
    });

 **Regex Options**

Note that 'i' and 'm' are the two possible values, which can also be concatenated to get both options.
The option 'i' specifies that the regex will be case-insensitive while 'm' specifies that the regex 
should search multiple lines. More information can be found here: [https://parse.com/docs/rest#queries-strings](https://parse.com/docs/rest#queries-strings)

     var parseClient = new Parse.ParseClient("myappid", "mysecretkey");
     var results = parseClient.GetObjectsWithQuery("Computer", new
     {
         ManufacturerEmail = new Constraint(regex: @"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,6}\b", regexOptions: "im")
     });


#### Known Issues

* When creating an object, the returned object contains only an object reference. This is due to a design choice with Parse.
