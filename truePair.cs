using System;

namespace ConsoleApp3
{
    public class truePair
    {
        private string word;
        private int num;
        public bool unrecognized = false;


        public truePair(string line) {
            string[] wordNum = line.Split(' ', '\t');
            word = wordNum[0];

            if (wordNum.Length > 1) {
                try {
                    int.TryParse(wordNum[1], out num);
                }
                catch (Exception e) {
                    Program.errorMessages.Add("ERROR: Invalid entry: " + line + " is an unrecognized input");
                    unrecognized = true;
                   // return;
                }
            }

            else {
                num = 0000;
            }

        }

        public string Word
        {
            get => word;
            set => word = value;
        }

        public int Num
        {
            get => num;
            set => num = value;
        }

        public bool Unrecognized
        {
            get => unrecognized;
            set => unrecognized = value;
        }
    }
}