using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Psswdgen
{
    class Program
    {
        static readonly string Character_set = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
        static readonly string Number_set = Character_set + "1234567890";
        static readonly string Expanded_set = Number_set + "!@#$%^&*()-=_+[]{}?/.,<>";

        static void Main(string[] args)
        {
            Style mode = Style.Everything;
            bool exit = false;
            do
            {
                Console.Clear();
                Utils.Write("Password Mode: " + mode.ToString() + "\n");
                Console.WriteLine("Press ENTER to generate password; ESC to exit; or M to change mode...");
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape) exit = true;
                else if (key == ConsoleKey.M)
                    mode = (Style)Utils.Menu(3, "Choose Mode", new string[] { "Character Only", "Character & Number", "Character, Number, & Symbol" });
                else
                {
                    int len = ReadInt("How Long? ");
                    if (len == -1) continue;
                    Console.Clear();
                    Utils.Write("Your Password Is:\n");
                    Password(len, mode);
                    Console.WriteLine("\nPress ENTER to Continue...");
                    Console.ReadLine();
                }
            } while (!exit);

        }
        static void Password(int len, Style s)
        {
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            string currset;
            switch (s)
            {
                case Style.CharacterOnly:
                    currset = Character_set;
                    break;
                case Style.CharacterNumber:
                    currset = Number_set;
                    break;
                default:
                    currset = Expanded_set;
                    break;
            }
            byte setlen = (byte)currset.Length;

            for (int i = 0; i < len; ++i)
            {
                Utils.Write(currset[Roll(setlen, rngCsp)]);
            }

            rngCsp.Dispose();
        }

        static int Roll(byte len, RNGCryptoServiceProvider csp)
        {
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                csp.GetBytes(randomNumber);
            }
            while (!IsFair(randomNumber[0], len));
            return (randomNumber[0] % len);
        }
        
        static int ReadInt(string message)
        {
            int val;
            string s;
            int times = 0;
            do
            {
                if (times > 0)
                {
                    Console.WriteLine("Sorry, try again.");
                }
                Console.Write(message);
                s = Console.ReadLine();
                times++;
            } while (!int.TryParse(s, out val));
            return val;
        }

        static bool IsFair(byte val, byte len)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / len;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return val < len * fullSetsOfValues;
        }
    }

    public class Utils
    {
        public static void Write(object o, ConsoleColor foreground = ConsoleColor.Gray, ConsoleColor background = ConsoleColor.Black)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.Write(o);
        }

        public static int Menu(int numchoices, string title, string[] options)
        {
            int currchoice = 0;
            int maxchoice = numchoices - 1;
            for (; ; )
            {
                Console.Clear();
                Write(title + "\n", ConsoleColor.Black, ConsoleColor.Gray);
                for (int i = 0; i < numchoices; i++)
                {
                    if (i == currchoice)
                    {
                        Write((i + 1) + ". " + options[i] + "\n", ConsoleColor.Black, ConsoleColor.Gray);
                    }
                    else Write((i + 1) + ". " + options[i] + "\n");
                }

                ConsoleKey k = Console.ReadKey(true).Key;
                int val = (int)k - 49;
                if (val <= maxchoice && val >= 0)
                {
                    currchoice = val;
                }
                else switch (k)
                {
                    case ConsoleKey.DownArrow:
                        if (currchoice < maxchoice) currchoice++;
                        break;

                    case ConsoleKey.UpArrow:
                        if (currchoice > 0) currchoice--;
                        break;

                    case ConsoleKey.Enter:
                        return currchoice;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    enum Style
    {
        CharacterOnly = 0,
        CharacterNumber = 1,
        Everything = 2
    }
}
