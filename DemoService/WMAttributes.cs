using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BasaService2
{
    public class WMAttributes
    {
        //defines , enums , default values ...
        enum WMType {Learn , Exam, NoAction};
        private int defaultvalue = -1;


        //attributes
        public int greenLevel;   //range: 
        public int yellowSpaceLevel;    //range: 
        public int mass;    //range: 
        public int volume;  //range: 
        public int distanceBetweenLines;    //range: 
        public int lowCircleDiameter;   //range: 

        //attributes (not included in the ID3 tree)
        public int exampleType;
        public int WMid;

        //constructor (defaultValue)
        public WMAttributes()
        {
            greenLevel=defaultvalue;
            yellowSpaceLevel = defaultvalue;
            mass = defaultvalue;
            volume = defaultvalue;
            distanceBetweenLines = defaultvalue;
            lowCircleDiameter = defaultvalue;
            exampleType = (int) WMType.NoAction;
        }

        //constructor
        public WMAttributes(int greenLevelIn,int yellowSpaceLevelIn, int massIn, int volumeIn,
           int distanceBetweenLinesIn, int lowCircleDiameterIn,int exampleTypeIn)
        {
            greenLevel = greenLevelIn;
            yellowSpaceLevel = yellowSpaceLevelIn;
            mass = massIn;
            volume = volumeIn;
            distanceBetweenLines = distanceBetweenLinesIn;
            lowCircleDiameter = lowCircleDiameterIn;
        }

        public int WMgetId()
        {
            return WMid;
        }

        public int WMgetGreenLevel()
        {
            return greenLevel;
        }

        public int WMgetYellowSpaceLevel()
        {
            return yellowSpaceLevel;
        }

        public int WMgetMass()
        {
            return mass;
        }

        public int WMgetVolume()
        {
            return volume;
        }

        public int WMgetDistanceBetweenLines()
        {
            return distanceBetweenLines;
        }

        public int WMgetLowCircleDiameter()
        {
            return lowCircleDiameter;
        }
    }
}