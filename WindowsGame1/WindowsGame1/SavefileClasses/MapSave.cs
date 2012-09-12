using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class MapSave
    {
        public String name;
        public Boolean introplayed;
        public List<ObjectSave> objectsaves;

        public MapSave()
        {
            name = "";
            introplayed = false;
            objectsaves = new List<ObjectSave>();
        }

        public MapSave(String Name, Boolean Introplayed, List<Object> Objects)
        {
            name = Name;
            introplayed = Introplayed;
            objectsaves = new List<ObjectSave>();

            foreach (Object obj in Objects)
            {
                objectsaves.Add(new ObjectSave(obj.name, obj.imagenum, obj.visible, obj.walkable, obj.scripts));
            }
        }

    }
}
