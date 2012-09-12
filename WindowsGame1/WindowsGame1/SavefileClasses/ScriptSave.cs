using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    public class ScriptSave
    {
        public String name;
        public Boolean active;

        public ScriptSave()
        {
            name = "";
            active = false;
        }

        public ScriptSave(String Name, Boolean Active)
        {
            name = Name;
            active = Active;
        }
    }
}
