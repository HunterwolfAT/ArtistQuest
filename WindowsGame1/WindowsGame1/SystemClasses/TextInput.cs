using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class TextInput
    {
        String oldinput = null;
        int counter = 0;

        public void Update()
        {
            counter++;
        }

        public String getInput(KeyboardState KnewState, String text)
        {
            
            Keys[] keys = KnewState.GetPressedKeys();
            if (keys.Count() > 0)
            {
                Console.WriteLine("First Press: " + keys[0].ToString());
                if (keys[0].ToString() == "Back")
                {
                    if (text.Length > 0 && counter > 3)
                    {
                        counter = 0;
                        return text.Substring(0, text.Length - 1);
                    }
                }
                else if (keys[0].ToString() != "Enter" && keys[0].ToString() != oldinput && keys[0].ToString() != "LeftShift")
                {
                    oldinput = keys[0].ToString();
                    counter = 0;
                    if (keys[0].ToString() == "Space")
                        return text + " ";
                    else if (keys[0].ToString() == "OemComma")
                        return text + ",";
                    //else if (keys[0].ToString() == "OemSemicolon")
                    //    return text + "ü";
                    //else if (keys[0].ToString() == "OemTilde")
                    //    return text + "ö";
                    //else if (keys[0].ToString() == "OemQuotes")
                    //    return text + "ä";
                    else if (keys[0].ToString() == "OemPeriod")
                        return text + ".";
                    else if (keys[0].ToString() == "OemMinus")
                        return text + "-";
                    else if (keys[0].ToString() == "OemComma")
                        return text + ",";
                    else if (keys[0].ToString() == "D1")
                        return text + "1";
                    else if (keys[0].ToString() == "D2")
                        return text + "2";
                    else if (keys[0].ToString() == "D3")
                        return text + "3";
                    else if (keys[0].ToString() == "D4")
                        return text + "4";
                    else if (keys[0].ToString() == "D5")
                        return text + "5";
                    else if (keys[0].ToString() == "D6")
                        return text + "6";
                    else if (keys[0].ToString() == "D7")
                        return text + "7";
                    else if (keys[0].ToString() == "D8")
                        return text + "8";
                    else if (keys[0].ToString() == "D9")
                        return text + "9";
                    else if (keys[0].ToString() == "D0")
                        return text + "0";
                    else
                    {
                        if (keys.Count() > 1)
                        {
                            if (keys[1].ToString() == "LeftShift")
                                return text + keys[0].ToString().ToUpper();
                            else
                                return text + keys[0].ToString().ToLower();
                        }
                        return text + keys[0].ToString().ToLower();
                    }

                }

                counter++;
                Console.WriteLine(counter.ToString());
                if (counter > 6)
                    oldinput = null;
                else
                    oldinput = keys[0].ToString();
            }
            return null;
        }
    }
}
