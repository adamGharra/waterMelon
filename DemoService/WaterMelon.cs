using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Web;
using DecisionTree.Data;
using SampleDataParsers;
using BasaService2;

namespace BasaService2
{
    public class WaterMelon : LineReader
    {
        public void WriteToLearningFile(int greenLevelIn, int yellowSpaceLevelIn, int massIn, int volumeIn,
           int distanceBetweenLinesIn, int lowCircleDiameterIn, int exampleTypeIn)
        {
            StreamWriter file = new StreamWriter(@"LearnExamples.txt",true);
            file.WriteLine(greenLevelIn.ToString()+","+yellowSpaceLevelIn.ToString()+","+ massIn.ToString() + ","+
                volumeIn.ToString() + "," + distanceBetweenLinesIn.ToString() + "," + 
                lowCircleDiameterIn.ToString() + "," + exampleTypeIn.ToString());
            file.Close();

        }
        protected override Instance ParseLine(string line)
        {

            var splits = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var greenLevel = splits[0];
            var yellowSpaceLevel = splits[1];
            var mass = splits[2];
            var volume = splits[3];
            var distanceBetweenLines = splits[4];
            var lowCircleDiameter = splits[5];

            return new Instance
            {
                Features = new List<Feature>
                                  {
                                      new Feature(greenLevel, "greenLevel"),
                                      new Feature(yellowSpaceLevel, "yellowSpaceLevel"),
                                      new Feature(mass, "mass"),
                                      new Feature(volume, "volume"),
                                      new Feature(distanceBetweenLines, "distanceBetweenLines"),
                                      new Feature(lowCircleDiameter, "lowCircleDiameter")
                                  }
            };
        }
    }
}