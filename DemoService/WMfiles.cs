using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BasaService2
{
    public class WMfiles
    {
        public void WMWriteToFile(int greenLevelIn, int yellowSpaceLevelIn, int massIn, int volumeIn,
           int distanceBetweenLinesIn, int lowCircleDiameterIn, int exampleTypeIn)
        {
            string startupPath = System.IO.Directory.GetCurrentDirectory();
            
            return;
            /*string startupPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "abc.txt");
            using (System.IO.StreamWriter file =
            new StreamWriter(startupPath, true))
            {
                file.WriteLine("Fourth line");
            }*/
        }
    }
}