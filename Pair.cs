using System;
using System.Linq;

namespace ConsoleApp3
{
    public class Pair
    { 
        private String mnemonic;
        private int opcode;
        private int byteSize;
        private bool unrecognized = false;
        
        public Pair(String line)
        {
            String[] mnemonicNum = line.Trim().Split(' ');  
            mnemonicNum = mnemonicNum.Where(address=>!string.IsNullOrWhiteSpace(address))
                .ToArray();
            mnemonic = mnemonicNum[0];
            if (mnemonicNum.Length > 1)
            {
                try
                {
                  //  Console.WriteLine("here is mnemoninNum[1] " + mnemonicNum[1]);
                    opcode = Convert.ToInt32(mnemonicNum[1], 16);
                  //  Console.WriteLine("{0:X}", opcode );
                    if(mnemonicNum.Length > 2){  //this should work. this wont assign a byte size to my sic file. only the sicops file
                        int.TryParse(mnemonicNum[2], out var b);
                      //  Console.WriteLine("this is the bs " + b);
                        byteSize = b;


                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid entry: " + line + " is an unrecognized input");
                    unrecognized = true;
                    throw;
                }
            }
            else
            {
                opcode = 0000;
            }

        }

        public string Mnemonic
        {
            get => mnemonic;
            set => mnemonic = value;
        }

        public int Opcode
        {
            get => opcode;
            set => opcode = value;
        }

        public int ByteSize
        {
            get => byteSize;
            set => byteSize = value;
        }

    }
}