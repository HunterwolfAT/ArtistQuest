using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace WindowsGame1
{
    public class Itemhandler
    {
        public List<Item> items;
        
        public Itemhandler()
        {
            items = new List<Item>();
        }

        public void LoadContent(ContentManager myContentManager)
        {
            foreach (Item item in items)
            {
                item.LoadContent(myContentManager);
            }
        }

        public void AddItem(String name, String picture)
        {
            Item newitem = new Item(name, picture);
            newitem.scripts.Add(new Script("look"));
            newitem.scripts.Add(new Script("defaultUse"));
            items.Add(newitem);
        }

        public void DeleteItem(int index)
        {
            items.RemoveAt(index);
        }

        public Item ReturnItem(int index)
        {
            return items[index];
        }

        public Item FindItem(String name)
        {
            foreach (Item item in items)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        public List<Item> ReturnItemList()
        {
            return items;
        }
    }
}
