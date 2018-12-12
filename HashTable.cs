using System;
using System.Collections;

namespace ConsoleApp3
{
    public class HashTable
    {
        private String[] hashArray;

        public HashTable(int primeSize)
        {
            hashArray = new String[primeSize];
        }


        public void createHashArray(int hashValue, String line) {  //The Insertion
            int probe;
            ArrayList collisionTracker = new ArrayList();

            if (hashArray[hashValue] == null) { //if location is empty

                hashArray[hashValue] = line;


                probe = -1;

            }  else {

                if (hashValue == (hashArray.Length - 1)){    // Checks if it is the end of the array

                    probe = 0;     // moves the probing index to the beginning

                } else{
                    probe = hashValue + 1;   // If not the end of array, add one because its Linear Probing!
                }

            }

            while ( (probe != -1) && (probe != hashValue) ) {    // probe cannot equal hashValue because of next else
                //statement
                if (hashArray[probe] == null) {

                    hashArray[probe] = line;

                    probe = -1;


                } else {

                    if (probe == (hashArray.Length - 1)) {     // Checks if it is the end of the array

                        probe = 0;

                    } else {
                        collisionTracker.Add(probe);
                        probe++;    // increments index because Linear Probing!
                    }

                }
            }
        }

        public int? searchByteSize(int hashValue, Instruction instruction, int prime){
            while (hashArray[hashValue] != null)
            {
                Pair p = new Pair(hashArray[hashValue]);
             //   Console.WriteLine("byte" + p.ByteSize);

                if (p.Mnemonic.Equals(instruction.Opcode)) {

                    instruction.NumericOpcode = p.Opcode.ToString();
                    instruction.HasnumericOpcode = true;
                  //  Console.WriteLine("this is the bytesize" + p.ByteSize);
                    return p.ByteSize;

                }

                hashValue = (hashValue + 1) % (prime);

            }

            return null;
        }
        
        void SearchLinearProbe(int hashValue, truePair pair, int prime) {    //The Search
            // pair = The Pair made from each new Line
            while (hashArray[hashValue] != null)
            {
                truePair p = new truePair(hashArray[hashValue]);

                if (p.Word.Equals(pair.Word) && p.Num != 0 && p.Word != null && p.Num != null) {
                    String address = String.Format("%02X", p.Num & 0xFFFFF);
                    Console.WriteLine(String.Format("%9s\t%12s\t%9s", hashValue,pair.Word,address));
                    return;
                }
                hashValue = (hashValue + 1) % (prime);

            }

        }
        
        public void searchDuplicates(String label) {

            for(int i = 0; i < hashArray.Length; i++){
                if(hashArray[i] != null) {
                    truePair p = new truePair(hashArray[i]);
                    if(p.Word.Trim().Equals(label)){
                        if(!Program.errors.Contains("ERROR: DUPLICATE LABEL FOUND AT ADDRESS " + label)) {
                            Program.errors.Add("ERROR: DUPLICATE LABEL FOUND AT ADDRESS " + String.Format("%02X", p.Num & 0xFFFFF));
                        }
                    }
                }
            }
        }

        public int SearchAddressofLabel(int hashValue, String instruction, int prime) {    //The Search
            // pair = The Pair made from each new Line
            while (hashArray[hashValue] != null)
            {
                truePair p = new truePair(hashArray[hashValue]);

                if (p.Word.Equals(instruction) && p.Num != 0) {

                    return p.Num;
                }
                hashValue = (hashValue + 1) % (prime);
            }
            return 0000;
        }

        public void printArray(){
                foreach ( Object obj in hashArray )
                    Console.Write( "   {0}", obj );
                Console.WriteLine();
           
        }
    }
}