/*
    Copyright (c) 2011 Alastair Paterson

    Permission is hereby granted, free of charge, to any person obtaining a copy of this
    software and associated documentation files (the "Software"), to deal in the Software
    without restriction, including without limitation the rights to use, copy, modify,
    merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
    permit persons to whom the Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be included in all copies
    or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
    INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
    PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
    CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
    OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 */

using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace ParseTests
{
    /// <summary>
    /// Conversion of the tests initially provided with Parse for .Net to work
    /// with NUnit.
    /// </summary>
    [TestFixture]
    public class Tests
    {
        Parse.ParseClient localClient;
		
        public Tests()
        {
			localClient = new Parse.ParseClient("LGN3l8M0YnFnJYZFVcOpbjKFM9pW031Ix74xLY1S", "WuOVNEOholg20gBoUnvguoMvL9nZBwDSO8SgiR19", "Kx7YbbUawvWwBQLqXRbY16URoGWvvPVfpjTUbncL");
		}
		

        [Test]
        public void TestMethod1()
        {
            string fileContents = "This is a test file.";
			string testFileLocalPath = "testFile.txt";
            File.WriteAllText(testFileLocalPath, fileContents);
            Parse.ParseFile parseFile = new Parse.ParseFile(testFileLocalPath);
			Parse.ParseFile testFile = localClient.CreateFile(parseFile);
			//Test to make sure test file is returned after creation.
            Assert.IsNotNull(testFile);
			Console.WriteLine(testFileLocalPath + " uploaded to :" + testFile.Url);
			//access the file outside the parse api
			WebRequest fileRequest = WebRequest.Create(testFile.Url);
            fileRequest.Method = "GET";
            HttpWebResponse foundTestFile = (HttpWebResponse)fileRequest.GetResponse();
			// we don't have to stream the body content - just check the status code
			Assert.AreEqual (HttpStatusCode.OK, foundTestFile.StatusCode);
			
            Parse.ParseObject testObject = new Parse.ParseObject("ClassOne");
            testObject["foo"] = "bar";
            //Create a new object
            testObject = localClient.CreateObject(testObject);
			
            //Test to make sure we returned a ParseObject
            Assert.IsNotNull(testObject);
            //Test to make sure we were assigned an object id and the object was actually remotely created
            Assert.IsNotNull(testObject.objectId);

            //Search for the newly-created object on the server
            Parse.ParseObject searchObject = localClient.GetObject("ClassOne", testObject.objectId);
            //Test to make sure the same object was returned
            Assert.AreEqual(testObject.objectId, searchObject.objectId);

            testObject["foo"] = "notbar";
            //Change a value on the server
            localClient.UpdateObject(testObject);

            searchObject = localClient.GetObject("ClassOne", testObject.objectId);
            //Test to make sure the object was updated on the server
            Assert.AreEqual("notbar", searchObject["foo"]);

            //Test to make sure we can retrieve objects from Parse
            Parse.ParseObject[] objList = localClient.GetObjectsWithQuery("ClassOne", new { foo = "notbar" });
            Assert.IsNotNull(objList);

            // Test to make sure the same object was returned
			// We can't assume that the first object will be the one we've just
			// sent, since some other people may be running tests as well.
			Assert.IsTrue(objList.Any(x => (string)x["objectId"] == testObject.objectId));

			//Cleanup
            localClient.DeleteObject(testObject);
			
			localClient.DeleteFile (testFile);
			//verify that the deleted file is no longer accessible
			try {
				WebRequest deletedFileRequest = WebRequest.Create(testFile.Url);
	            deletedFileRequest.Method = "GET";
	            HttpWebResponse deletedTestFile = (HttpWebResponse)deletedFileRequest.GetResponse();
				Assert.That(deletedTestFile.StatusCode == HttpStatusCode.Forbidden || deletedTestFile.StatusCode == HttpStatusCode.NotFound);
			} catch (WebException accessException) {
				Assert.That(accessException.Message.Contains("403") || accessException.Message.Contains("404"));
			}
				
        }

        [Test]
        public void TestMP3()
        {
            Parse.ParseFile parseFile = new Parse.ParseFile(
                Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "ParseTests", "sweep.mp3"));
            Parse.ParseFile testFile = localClient.CreateFile(parseFile);

            //Test to make sure test file is returned after creation.
            Assert.IsNotNull(testFile);

            Parse.ParseObject testObject = new Parse.ParseObject("ClassOne");
            testObject["foo"] = "bar";
            //Create a new object
            testObject = localClient.CreateObject(testObject);

            //Test to make sure we returned a ParseObject
            Assert.IsNotNull(testObject);
            //Test to make sure we were assigned an object id and the object was actually remotely created
            Assert.IsNotNull(testObject.objectId);

            //Search for the newly-created object on the server
            Parse.ParseObject searchObject = localClient.GetObject("ClassOne", testObject.objectId);
            //Test to make sure the same object was returned
            Assert.AreEqual(testObject.objectId, searchObject.objectId);

            testObject["foo"] = "notbar";
            //Change a value on the server
            localClient.UpdateObject(testObject);

            searchObject = localClient.GetObject("ClassOne", testObject.objectId);
            //Test to make sure the object was updated on the server
            Assert.AreEqual("notbar", searchObject["foo"]);

            //Test to make sure we can retrieve objects from Parse
            Parse.ParseObject[] objList = localClient.GetObjectsWithQuery("ClassOne", new { foo = "notbar" });
            Assert.IsNotNull(objList);

            //Test to make sure the same object was returned
			Assert.IsTrue(objList.Any(x => (string)x["objectId"] == testObject.objectId));

            //Cleanup
            localClient.DeleteObject(testObject);
        }
    }
}
