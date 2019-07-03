using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSMetaphorWorld
{
    public class Notepad
    {
        List<string> contents; //first list element = top of page, last = bottom      a list might not be the way to go

        public Notepad()
        {
            contents = new List<string>();
        }

        public void writeString(string newValue)
        {
            contents.Add(newValue);
        }

        public string tearOffLastValue()
        {
            string lastValue = contents[contents.Count];
            contents.RemoveAt(contents.Count - 1);
            return lastValue;
        }
    }
}
