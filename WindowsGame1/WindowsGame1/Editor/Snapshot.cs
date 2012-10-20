using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class Snapshot
    {
        public String Mapname;
        public DateTime Timestamp;

        public List<String> GVName;   // GameVariables Name
        public List<String> GVValue;  // GameVariables Value

        public Snapshot(List<String[]> gamevariables, Maps map)
        {
            Mapname = map.name;

            GVName = new List<string>();
            GVValue = new List<string>();

            foreach (String[] var in gamevariables)
            {
                GVName.Add(var[0]);
                GVValue.Add(var[1]);
            }

            Timestamp = DateTime.Now;
        }
    }
}
