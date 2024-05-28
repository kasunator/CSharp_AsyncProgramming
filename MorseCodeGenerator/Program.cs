using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MorseCodeGenerator
{
    internal class Program
    {
        static char[] charArray = { ' ','A','B','C','D','E','F','G','H','I','J','K','L','M',
                        'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                        '1','2','3','4','5','6','7','8','9','0','?','!','.',',',
                        ';', ':', '+', '-', '/', '=', ')', '(','"'};

        static string[] morse_array = { " ", ".-", "-...", "-.-.", "-..", ".",
                            "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--",
                            "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-",
                            "...-", ".--", "-..-", "-.--", "--..",
                            ".----", "..---", "...--", "...-", ".....",
                            "-....", "--...", "---..", "----.", "-----",
                            "..--..", "-.-.--", ".-.-.-", "--..--", "-.-.-.",
                            "---...", ".-.-.", "-....-", "-..-.", "-...-",
                            "-.--.-", "-.--.", ".-..-."};
        static string morse_string = ".-..-. -. --- .-- / .. ... / - .... . / - .. -- . / ..-. --- .-. / .- .-.. .-.. /" +
            " --. --- --- -.. / -- . -. / - --- / -.-. --- -- . / - --- / - .... . / .- .. -.. / --- ..-. / - .... . .. .-. / -.-. --- ..- -. - .-. -.-- -.-.-- .-..-. / -.--. .--. .-.. . .- ... . / -... . /" +
            " .- -.. ...- .. ... . -.. / - .... .- - / -. --- - / .- .-.. .-.. / -- --- .-. ... . / " +
            "-.-. --- -.. . / -.-. --- -. ...- . .-. - . .-. ... / .- .-. . / . --.- ..- .- .-.. -.--.- .-.-.-";
        private static string toMorse(string input)
        {
            /*There is a space between each letter and;  "/" between each word.*/
            string hc_input = input.ToUpper(); /*convert to higher case*/
            string morse_string = "";

            foreach (char ch in hc_input)
            {

                for (int i = 0; i < charArray.Length; i++)
                {
                    if (ch == charArray[i])
                    {
                        if (ch == ' ') /*space found means end of word*/
                        {   /* "/" between words */
                            morse_string += morse_array[i] + "/ ";
                        }
                        else
                        {
                            /*else  we just add a space after morse code*/
                            morse_string += morse_array[i] + " ";
                        }
                        break;
                    }
                }
            }

            return morse_string;

        }

        private static string FromMorse(string input)
        {
            string output = "";
            int start_index = 0;
            char[] delim = { '/' };
            string[] tokens = input.Split(delim);
            int index = 0;
            foreach (string token in tokens)
            {
                string[] morse_chars = token.Split(null); //split into each morese character
                foreach (string morse in morse_chars)
                {
                    index = 0;
                    foreach (string morse_character in morse_array) // go through each  morese character in array
                    {
                        if (morse == morse_character)
                        {
                            output += charArray[index];
                            break;
                        }
                        index++;
                    }
                }
                output += " ";

            }
            return output;
        }


        static void Main(string[] args)
        {
            string the_word = "\"Now is the time for all good men to come to the aid of their country!\"" +
                                "(Please be advised that not all morse code converters are equal).";
            string kasun = "kasun";
            string result = toMorse(the_word);
            Console.WriteLine("to Morese output:" + result);

            string morseTo_word = FromMorse(morse_string);
            Console.WriteLine("From Morese output:" + morseTo_word);

            Console.ReadKey();
        }
    }
}
