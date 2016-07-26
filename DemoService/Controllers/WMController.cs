using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using BasaService2;
using System.IO;
using System.Web.Hosting;
using DecisionTree.Data;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using System.Diagnostics;

namespace BasaService2.Controllers
{
    public class WaterMController : ApiController
    {
        public static int examples_number = 0;
        public ApiServices Services { get; set; }


        // GET api/WM
        public string GetnewWM(string greenLevel,
        string yellowSpaceLevel,
        string mass,
        string volume,
        string distanceBetweenLines,
        string lowCircleDiameter,
        string sevok,
        string exampleType)
        {
            //check if value bigger than 5
            if (Int32.Parse(greenLevel) >= 5)
            {
                greenLevel = "5";
            }
            if (Int32.Parse(yellowSpaceLevel) >= 5)
            {
                yellowSpaceLevel = "5";
            }
            if (Int32.Parse(mass) >= 5)
            {
                mass = "5";
            }
            if (Int32.Parse(volume) >= 5)
            {
                volume = "5";
            }
            if (Int32.Parse(distanceBetweenLines) >= 5)
            {
                distanceBetweenLines = "5";
            }
            if (Int32.Parse(lowCircleDiameter) >= 5)
            {
                lowCircleDiameter = "5";
            }

            //check if value smaller than 1
            if (Int32.Parse(greenLevel) < 1)
            {
                greenLevel = "0";
            }
            if (Int32.Parse(yellowSpaceLevel) < 1)
            {
                yellowSpaceLevel = "0";
            }
            if (Int32.Parse(mass) < 1)
            {
                mass = "0";
            }
            if (Int32.Parse(volume) < 1)
            {
                volume = "0";
            }
            if (Int32.Parse(distanceBetweenLines) < 1)
            {
                distanceBetweenLines = "0";
            }
            if (Int32.Parse(lowCircleDiameter) < 1)
            {
                lowCircleDiameter = "0";
            }

            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("wmexamples");

            // Retrieve reference to a blob named "examples".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("examples.txt");

            //read blob to string
            string blob_data = blockBlob.DownloadText();
            if (examples_number == 0) 
            {
                return "-1";
            }
                if (exampleType == "0")  //case: learn
            {
                //write the new example to the examples file
                blob_data = blob_data + greenLevel + "," + yellowSpaceLevel + "," + mass + "," +
                   volume + "," + distanceBetweenLines + "," +
                   lowCircleDiameter + "," + sevok + "\n";
                blockBlob.UploadTextAsync(blob_data);
                return "success";
            }
            else    //case: exam
            {
                //create decision tree from all the examples in the file
                //always skip the first line in the examples file 
                var set = new DecisionTreeSet();
                set.Instances = new List<Instance>();

                //split readed string from blob to lines
                string[] examples_by_line = blob_data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                for (int i = 1; i < examples_number + 1; i++)
                {
                    //split each line (example) to features 
                    List<string> one_example = examples_by_line[i].Split(',').ToList<string>();
                    Instance wm_example_read = new Instance
                    {
                        Output = new Output(one_example[7], "sevok"),
                        Features = new List<Feature>
                                  {
                                      new Feature(one_example[0], "greenLevel"),
                                      new Feature(one_example[1].ToString(), "yellowSpaceLevel"),
                                      new Feature(one_example[2], "mass"),
                                      new Feature(one_example[3], "volume"),
                                      new Feature(one_example[4], "distanceBetweenLines"),
                                      new Feature(one_example[5], "lowCircleDiameter"),
                                  }
                    };
                    set.Instances.Add(wm_example_read);
                }

                //build tree with all examples
                var tree = set.BuildTree();


                //create new instance with the input
                Instance wm_new = new Instance
                {
                    Features = new List<Feature>
                                  {
                                      new Feature(greenLevel.ToString(), "greenLevel"),
                                      new Feature(yellowSpaceLevel.ToString(), "yellowSpaceLevel"),
                                      new Feature(mass.ToString(), "mass"),
                                      new Feature(volume.ToString(), "volume"),
                                      new Feature(distanceBetweenLines.ToString(), "distanceBetweenLines"),
                                      new Feature(lowCircleDiameter.ToString(), "lowCircleDiameter"),
                                  }
                };


                //call exam (with new example) 
                return Tree.ProcessInstance(tree, wm_new).Value;

                //return the result
                //return result;
            }
        }


        
        // call this function only one time when we start the learning phase
        public string GetsevokWM(string greenLevel,
        string yellowSpaceLevel,
        string mass,
        string volume,
        string distanceBetweenLines,
        string lowCircleDiameter,
        string sevok)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("wmexamples");

            // Retrieve reference to a blob named "examples".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("examples.txt");

            //read blob to string
            string blob_data = blockBlob.DownloadText();

            //write the new example to the examples file
            blob_data = "first,line,example" + "\n" ;

            blockBlob.UploadTextAsync(blob_data);
            return "success";
        }
    }
}
