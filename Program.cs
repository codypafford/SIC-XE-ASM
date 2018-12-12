using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ConsoleApp3;

class Program
    {
        public static int EndAddress;
        public static ArrayList INSTRUCTIONLst = new ArrayList();
        public static int STARTADDRESS = 0; 
        public static int staticTracker = 0;
        public static int LOCCTR = 0;
        public static String PROGRAMNAME = "";
        public static int BASEADDRESS;

        static int size = 150; //gets the number size of the pairArray based on how many lines are in file
        static int prime = findPrime(2 * size);  //Finds prime number larger than 2 * Size
        static HashTable SICOPStable = new HashTable(prime); //create the HashTable to store String and Value
        public static ArrayList errorMessages = new ArrayList();
        public static ArrayList errors = new ArrayList();
        static HashTable LabelTable = new HashTable(prime);
        static HashTable SYMTAB = new HashTable(prime);

        static void Main(string[] args)
        {
            
            
            createSICOPS(args);
            passOne(args);
            
        }
        
        private static void passOne(String[] args)
        {
            var counter = 0;
            string line;
            string[] findComment;

            System.IO.StreamReader file =   
                new System.IO.StreamReader(args[0]);  
            while((line = file.ReadLine()) != null)  
            {  
                       if (line.StartsWith(".")) {           //IF INSTRUCTION ONLY HAS A COMMENT-----------------------
                    Instruction instruction = new Instruction();
                    instruction.Comment = (line.Trim());
                    INSTRUCTIONLst.Add(instruction);
                    Console.WriteLine(instruction);

                } else {
                    if (line.Contains(".") && line[0] != '.') {          //IF INSTRUCTION HAS COMMENT----------------------
                        findComment = line.Trim().Split("\\.");
                        String[] str = findComment[0].Trim().Split("\\s+"); //retrieve the string from this part
                        if (str.Length == 1) {
                            Instruction instruction = new Instruction();
                            instruction.Opcode = findComment[0].Trim();
                            instruction.Comment = "." + findComment[1];
                            try {
                                var hashValue = findHashVal(instruction.Opcode, prime);
                                var bz = SICOPStable.searchByteSize(hashValue, instruction, prime);
                                instruction.Bytesize = (int) bz;
                            } catch (Exception e) {
                                checkValidOpcode(instruction);
                            }

                            Console.WriteLine("fkfkfkf" + instruction);
                            INSTRUCTIONLst.Add(instruction);

                        } else {
                            Instruction instruction = new Instruction(str);
                            instruction.Comment = "." + findComment[1];

                            try {
                                var hashValue = findHashVal(instruction.Opcode, prime);
                                var bz = SICOPStable.searchByteSize(hashValue, instruction, prime);
                                instruction.Bytesize = (int) bz;
                            } catch (Exception e) {
                                checkValidOpcode(instruction);
                            }

                            Console.WriteLine("fkfkfkf" + instruction);
                            INSTRUCTIONLst.Add(instruction);
                        }

                    } else {                        //THIS IS IF INSTRUCTION HAS NO COMMENT--------------------
                        Instruction instruction = new Instruction(line);
                        try {
                            var hashValue = findHashVal(instruction.Opcode, prime);
                            var bz = SICOPStable.searchByteSize(hashValue, instruction, prime);
                            instruction.Bytesize = (int) (bz);
                        } catch (Exception e) {
                            checkValidOpcode(instruction);

                        }
                        INSTRUCTIONLst.Add(instruction);
                        //Console.WriteLine("Address =" + instruction.Address + " " + "Label =" + instruction.Label + " " + "Opcode =" + instruction.Opcode 
                                      //    + " " + "Operand =" + instruction.Operand);
                    }
                }
            }  
            file.Close();  
            Console.WriteLine("There were {0} lines.", counter);  
//          Suspend the screen.  
            
           // createLTORG();
            calculateByteSize();
            calculateAddress();
            createFlags();
           // getRelativeAddresses();     //Actual addresses if no "USE"
            makeLableTable();           //PRINTS OUT SYMBOL TABLE
          //  sortErrorMessagesandPrint();
            getPCandE();
            createObjectCode();
            printFinalInstructions();
           // printLabelTable();
            SYMTAB.printArray();
        }
        
  /*      private static void createLTORG() {
            Stack LTORGS = new Stack();
            for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
                Instruction ins = (Instruction)INSTRUCTIONLst[i];
                if (ins.Opcode.Contains("=")) {
                    if (ins.Operand.Length >= 9) {
                        String s = ins.Operand.Substring(0, 10);         //manages the size of the LTORG (7 digits)
                        String line = s + " " + "BYTE" + " " + ins.Operand.Substring(1);
                        Instruction instruction = new Instruction(line);
                        LTORGS.Push(instruction);

                    } else {
                        String line = ins.Operand + " " + "BYTE" + " " + ins.Operand.Substring(1);
                        Instruction instruction = new Instruction(line);
                        LTORGS.Push(instruction);
                    }
                }
                if (ins.Opcode.Equals("LTORG")) {
                    while (LTORGS.Count > 0) {
                        Object instruction = LTORGS.Pop();
                        i++;
                        INSTRUCTIONLst.Add((Instruction) instruction);
                    }
                }
            }
        }*/
        
        private static void printFinalInstructions() {
            Console.WriteLine("\n\n");
            //  System.out.println(String.format("%6s\t%8s\t%7s\t%10s\t%15s", "ADDRESS", "LABEL", "OPCODE", "OPERAND", "COMMENT"));

            for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
                Instruction instruction = (Instruction)INSTRUCTIONLst[i];

                if (instruction.Address == 0) {
                    //instruction.Address = ("----");
                }
                if (instruction.Opcode.Equals("END") || instruction.Opcode.Equals("LTORG")) {
                    instruction.ObjectCode = "NULL";
                }
                Console.WriteLine(instruction);


                if (instruction.PcRelative != null) {
                    //System.out.println("THIS IS THE PC RELATIVE ADDRESS USED FOR OPERAND: " + INSTRUCTIONLst.get(i).getOPERAND() + " -> " + INSTRUCTIONLst.get(i).getPCRelative());
                }
                if (instruction.Address == EndAddress) {
                    // return;
                }
                //System.out.println(instruction.getPCRelative());
                // System.out.println(instruction.getObjectCode());

            }
        }
        
        private static void createObjectCode() {
        String sOne = null;    //first part
        String sTwo = null;    //second part
        String TA = 0.ToString();     //third part
        String s;
        for (int i = 0; i < INSTRUCTIONLst.Count-1; i++) {
            Instruction instruction = (Instruction) INSTRUCTIONLst[i];
            Instruction instruction2 = (Instruction) INSTRUCTIONLst[i + 1];
            try
            {
                
                if (instruction.P1 && ((instruction.PcRelative - instruction2.Address >= 2048) || instruction.PcRelative - instruction2.Address <= -2047)) {
                    instruction.P1 = false;
                    instruction.B1 = true;
                }
                if(instruction.Operand.Equals("")){
                    instruction.X1 = false;
                    instruction.B1 = false;
                    instruction.P1 = false;
                    instruction.E1 = false;
                    sTwo = 0.ToString();
                    TA = 0.ToString();
                }
            }catch(Exception e){

            }
            ///FIRST PART     FIRST PART     FIRST PART    FIRST PART    FIRST PART
            //   3 AND 4 BYTE INSTRUCTIONS      FOR 3 AND 4 BYTE INSTRUCTIONS
            
            if ((!instruction.Opcode.Equals("WORD") &&
                    !instruction.Opcode.Equals("RESW") && !instruction.Opcode.Equals("BYTE") && !instruction.Opcode.Equals("RESB"))) {


                if (instruction.Bytesize == 3 || instruction.Bytesize == 4) {
                    try {
                        if (instruction.N1 && instruction.I1)
                        {
                            int opcode = Convert.ToInt32(instruction.NumericOpcode);
                            sOne = opcode.ToString() + 3;
                           // Console.WriteLine("first part=" + String.Format("%02X", sOne & 0xFFFFF));
                        } else if (instruction.N1 && !instruction.I1) {
                            int opcode =  Convert.ToInt32(instruction.NumericOpcode);
                            sOne = opcode.ToString() + 2;
                            //System.out.println("first part=" + String.format("%02X", sOne & 0xFFFFF));

                        } else if (!instruction.N1 && instruction.I1) {
                            int opcode =  Convert.ToInt32(instruction.NumericOpcode);
                            sOne = opcode.ToString() + 1;
                           // System.out.println("first part=" + String.format("%02X", sOne & 0xFF));

                        } else if (!instruction.N1 && !instruction.N1) {
                            int opcode = Convert.ToInt32(instruction.NumericOpcode);
                            sOne = opcode.ToString();
                            //System.out.println("first part=" + sOne);

                        }
                    } catch (Exception e) {
                        // System.out.println("should be okay");
                    }
                } else if (instruction.Bytesize == 2) {
                    try {
                        int opcode = Convert.ToInt32(instruction.NumericOpcode);
                        sOne = opcode.ToString();
                    } catch (Exception e) {

                    }
                }

                //-------------------------------------------------------------------------------------------------------------------
                //   FOR 3 AND 4 BYTE INSTRUCTIONS     FOR 3 AND 4 BYTE INSTRUCTIONS       FOR 3 AND 4 BYTE INSTRUCTIONS
                // SECOND AND THIRD PART     SECOND AND THIRD PART      SECOND AND THIRD PART       SECOND AND THIRD PART
                if (instruction.Bytesize == 3 || instruction.Bytesize == 4 && instruction.Bytesize != 2) {
                    String nixbpe = instruction.X1 + "-" + instruction.B1 + "-" + instruction.P1 + "-" + instruction.E1;
                    try {
                        switch (nixbpe) {
                            case "true-false-false-false":
                                s = 8.ToString();
                                sTwo = Convert.ToInt32(s, 16).ToString();
                               // String.Format("%02X", sTwo & 0xFFF);
                                Console.WriteLine("second part=" + sTwo);
                                break;
                            case "true-true-false-false":              //GO TO CREATE FLAGS METHOD AND MAKE IT POSSIBLE TO USE BASE IF PC IS OUT OF RANGE-> CHECKRANGE()
                                s = "C";      // THIS
                                sTwo = Convert.ToInt32(s, 16).ToString();
                              //  System.out.println("second part=" + String.Format("%01X", sTwo & 0xFFF));
                                TA = (instruction.PcRelative - BASEADDRESS).ToString();
                               // System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                break;
                            case "true-false-true-false":
                                s = "A";     //THIS
                                sTwo = Convert.ToInt32(s, 16).ToString();
                              //  System.out.println("second part=" + String.format("%01X", sTwo & 0xFFF));
                                if (instruction.isInteger(instruction.Operand.Substring(1)))
                                {
                                    TA = instruction.Operand;
                                    // System.out.println("last part=" + TA);
                                } else {
                                    TA = (instruction.PcRelative - instruction2.Address).ToString();
                                    //System.out.println(String.format("%03X", TA & 0xFFF));
                                }
                                break;
                            case "true-false-false-true":
                                s = 9.ToString();
                                sTwo = Convert.ToInt32(s, 16).ToString();
                               // System.out.println("second part=" + sTwo);
                                TA = instruction.PcRelative.ToString();
                               // System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                break;
                            case "false-true-false-false":
                                s = 4.ToString();   //THIS
                                sTwo = Convert.ToInt32(s, 16).ToString();
                               // System.out.println("second part=" + sTwo);
                                TA =(instruction.PcRelative - BASEADDRESS).ToString();
                               // System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                break;
                            case "false-false-true-false":
                                s = 2.ToString();
                                sTwo = Convert.ToInt32(s, 16).ToString();
                              //  System.out.println("second part=" + sTwo);
                                TA = (instruction.PcRelative - instruction2.Address).ToString();
                               // System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                break;
                            case "false-false-false-true":
                                s = 1.ToString();
                                sTwo = Convert.ToInt32(s, 16).ToString();
                              //  System.out.println("second part=" + sTwo);
                                if (instruction.isInteger(instruction.Operand.Substring(1))) {
                                    TA = instruction.Operand.Substring(1);
                                } else
                                {
                                    TA = instruction.PcRelative.ToString();
                                    // System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                }
                                break;
                            case "false-false-false-false":
                                s = 0.ToString();
                                sTwo = Convert.ToInt32(s, 16).ToString();
                               // System.out.println("second part=" + sTwo);
                                TA = instruction.Operand.Substring(1);
                                //System.out.println("last part=" + String.format("%03X", TA & 0xFFF));
                                break;

                        }

                    } catch (Exception e) {

                    }
                    //-------------------------------------------------------------------------------------------------------------------
                    ////     SECOND PART FOR 2 BYTE INSTRUCTIONS
                } else if (instruction.Bytesize == 2) {
                    String[] strings = instruction.Operand.Split(",");
                    String register1 = strings[0].Trim();
                    String register2 = strings[1].Trim();
                    String r1 = "";
                    String r2 = "";
                    switch (register1) {
                        case "A":
                            r1 = 0.ToString();
                            break;
                        case "X":
                            r1 = 1.ToString();
                            break;
                        case "L":
                            r1 = 2.ToString();
                            break;
                        case "PC":
                            r1 = 8.ToString();
                            break;
                        case "SW":
                            r1 = 9.ToString();
                            break;
                        case "B":
                            r1 = 3.ToString();
                            break;
                        case "S":
                            r1 = 4.ToString();
                            break;
                        case "T":
                            r1 = 5.ToString();
                            break;
                        case "F":
                            r1 = 6.ToString();
                            break;
                    }
                    switch (register2) {
                        case "A":
                            r2 = 0.ToString();
                            break;
                        case "X":
                            r2 = 1.ToString();
                            break;
                        case "L":
                            r2 = 2.ToString();
                            break;
                        case "PC":
                            r2 = 8.ToString();
                            break;
                        case "SW":
                            r2 = 9.ToString();
                            break;
                        case "B":
                            r2 = 3.ToString();
                            break;
                        case "S":
                            r2 = 4.ToString();
                            break;
                        case "T":
                            r2 = 5.ToString();
                            break;
                        case "F":
                            r2 = 6.ToString();
                            break;
                    }
                  //  System.out.println("r1 is " + r1);
                  //  System.out.println("r2 is " + r2);

                    TA = r1 + r2;
                   // System.out.println("last part is " + TA);

                }
                try {
                    StringBuilder sb = new StringBuilder();
                    String part1 = sOne;
                    sb.Append(part1);
                    if (instruction.Bytesize != 2) {
                        String part2 = sTwo;
                        sb.Append(part2);
                    }
                    if (instruction.Bytesize == 3 || instruction.Bytesize == 4) {
                        if (instruction.Bytesize == 4) {
                            String part3 = "00" + TA;
                            sb.Append(part3);
                        } else {
                            String part3 = TA;
                            sb.Append(part3);
                        }
                    } else
                        sb.Append(TA);

                    instruction.ObjectCode1 = sb.ToString();
                    // sOne = null;
                    //  sTwo = null;
                    //  TA = 0;

                } catch (Exception e) {

                }
                //-------------------------------------------------------------------------------------------------------------------
                //     IF OPCODE IS WORD/RESB/RESW/BYTE     IF OPCODE IS WORD/RESB/RESW/BYTE
                //CREATES OBJECT CODE FOR WORDS AND RESW'S
            } else if (instruction.Opcode.Equals("WORD") ||
                       instruction.Opcode.Equals("RESW") || instruction.Opcode.Equals("BYTE")
                    || instruction.Opcode.Equals("RESB")) {

                if (instruction.Opcode.Trim().Equals("RESW")) {                  //RESW
                    int n = 3 * Convert.ToInt32(instruction.Operand);
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int zz = 0; zz <= n; zz++) {
                        stringBuilder.Append("F");
                    }
                    instruction.ObjectCode1 = stringBuilder.ToString();
                } else if (instruction.Opcode.Trim().Equals("WORD")) {                  //WORD
                    String hex = instruction.Operand;
                    String objCode = hex;
                    instruction.ObjectCode1 = objCode;
                } else if (instruction.Opcode.Trim().Equals("RESB")) {                   //RESB
                    StringBuilder stringBuilder = new StringBuilder();
                    int n = Convert.ToInt32(instruction.Operand);
                    for (int zz = 0; zz <= n; zz++) {
                        stringBuilder.Append("F");
                    }
                    instruction.ObjectCode1 = stringBuilder.ToString();             //BYTE
                    
                } else if (instruction.Opcode.Trim().Equals("BYTE") && !instruction.Operand.Contains("=")) {
                    if (instruction.Operand.Contains("C'")) {
                        String str = instruction.Operand.Substring(2, instruction.Operand.Length - 1);
                        char[] charArray = str.ToCharArray();
                       // System.out.println("str is " + str);
                       // System.out.println(charArray);
                        StringBuilder builders = new StringBuilder();
                        foreach (char c in charArray) {
                            int ii = c;
                            // Step-3 Convert integer value to hex using toHexString() method.
                            builders.Append(ii.ToString());
                        }
                        instruction.ObjectCode1 = builders.ToString();
                    } else if (instruction.Operand.Contains("X'"))
                    {
                        int end = instruction.Operand.Length - 1;
                        int position = instruction.Operand.LastIndexOf("'");
                        instruction.ObjectCode1 = instruction.Operand.Substring(2, end - 2);
                    }
                } else if (instruction.Opcode.Trim().Equals("BYTE") && instruction.Operand.Contains("=")) {

                }
            }
        }
    }
        
        private static void getPCandE() {       //USE THE OPERAND TO FIND THE ADDRESS OF LABELS
        for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
            Instruction ins = (Instruction)INSTRUCTIONLst[i];
            try {
                if (ins.Operand.StartsWith("#") || ins.Operand.StartsWith("@")) {
                    String str = ins.Operand.Substring(1);
                    int hv = findHashVal(str, prime);
                    int pc = LabelTable.SearchAddressofLabel(hv, str, prime);  //CORRECTLY FIND ADDRESSES
                    String PC = String.Format("%02X", pc & 0xFFFFF);
                   // Console.WriteLine("PC relative address of " + ins.Opcode + " is " + PC);
                    ins.PcRelative = pc;
                } else if (ins.Operand.Contains(",X")) {
                    String[] str = ins.Operand.Split(",");
                    String s = str[0];
                    int hv = findHashVal(s, prime);
                    int pc = LabelTable.SearchAddressofLabel(hv, s, prime);  //CORRECTLY FIND ADDRESSES
                    String PC = String.Format("%02X", pc & 0xFFFFF);
                    ins.PcRelative = pc;
                 //   Console.WriteLine("PC relative address of " + ins.Opcode + " is " + PC);

                } else {
                    int hv = findHashVal(ins.Operand, prime);
                    int pc = LabelTable.SearchAddressofLabel(hv, ins.Operand, prime);  //CORRECTLY FIND ADDRESSES
                    String PC = String.Format("%03X", pc & 0xFFFFF);
                    if (ins.Opcode.Equals("BASE")) {
                        BASEADDRESS = pc;
                    }
                  //  Console.WriteLine("PC relative address of " + ins.Opcode + " is " + PC);
                    ins.PcRelative = pc;
                }
            } catch (Exception e) {
                //System.out.println("threw exception" + INSTRUCTIONLst.get(i).getOPERAND());
                try {
                    int check; 
                    int.TryParse(ins.Operand, out check);
                } catch (Exception e1) {
                    if (ins.HasOperand && !ins.isInteger(ins.Operand.Substring(1))
                            && !ins.Operand.Contains(",X") && ins.Address != 0) {
                        //Main.errorMessages.add("ERROR: LABEL UNDEFINED AT ADDRESS " + String.format("%02X", INSTRUCTIONLst.get(i).getADDRESS()  & 0xFFFFF));
                    }
                }
            }
        }
    }

 
        
        private static void makeLableTable() {     // I SHOULD RENAME
            for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
                Instruction ins = (Instruction)INSTRUCTIONLst[i];
                if (ins.Address != 0) {
                    if (ins.HasLabel) {        //IF INSTRUCTION HAS A LABEL
                        int hv = findHashVal(ins.Label, prime);
                        String lblAddress = ins.Label + " " + ins.Address;    //PUT INTO ITS OWN METHOD TO PRINT THE SYMBOLE-LABEL TABLE

                        LabelTable.searchDuplicates(ins.Label.Trim());
                        createLabelTable(lblAddress, hv);

                    }
                }
            }
            LabelTable.printArray();
        }
        
        private static void createLabelTable(String lblAddress, int hv) {
            LabelTable.createHashArray(hv, lblAddress);

        }
        
        private static void createFlags() {
            for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
                Instruction instruction = (Instruction)INSTRUCTIONLst[i];

                if (instruction.Opcode.Trim().Contains("+")) {
                    instruction.E1 = true;
                    instruction.P1 = false;
                    // System.out.println("JDJDJDJD");
                }
                if (instruction.Operand.StartsWith("#")) {
                    instruction.I1 = true;
                    if (instruction.isInteger(instruction.Operand.Substring(1))) {
                        instruction.P1 = false;           //SOMETIMES P CAN BE TRUE WHILE IT IS IMMEDIATE THOUGH?  #ZZZ
                    }
                }
                if (instruction.Operand.StartsWith("@")) {
                    instruction.N1 = (true);
                }
                if (instruction.Operand.Contains(",X")) {
                    instruction.X1 = true;
                }
                if (instruction.Operand.Equals("BASE")) {
                    instruction.B1 = true;       //NOT ACTUALLY THE RIGHT THING TO DO
                }
                if (!instruction.I1 && !instruction.N1) {
                    instruction.I1 = true;
                    instruction.N1 = true;

                }
            }

        }
        
        private static void calculateAddress() {
            for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
                Instruction instruction = (Instruction)INSTRUCTIONLst[i];

                if (!instruction.Ignore) {
                    instruction.Address = staticTracker;
                    staticTracker += instruction.Bytesize;
                }
                if (instruction.Opcode.Equals("END")) {
                    EndAddress = instruction.Address;
                }
                if (!instruction.HasLabel) {
                    errorMessages.Add("ERROR: NO LABEL FOUND AT ADDRESS " + String.Format("%02X", instruction.Address & 0xFFFFF));
                }
            }

        }
        
        private static void calculateByteSize() {
        for (int i = 0; i < INSTRUCTIONLst.Count; i++) {
            Instruction instruction = (Instruction)INSTRUCTIONLst[i];
            try {
                if (instruction.Opcode.Equals("RESB") || instruction.Opcode.Equals("RESW") || instruction.Opcode.Equals("BYTE")
                        || instruction.Opcode.Equals("WORD")) {

                    if (instruction.Opcode.Equals("WORD")) {
                        instruction.Bytesize = 3;
                    } else if (instruction.Opcode.Equals("RESB"))
                    {
                        int.TryParse(instruction.Operand, out var bytesize);
                        instruction.Bytesize = bytesize;
                    } else if (instruction.Opcode.Equals("RESW")) {
                        int.TryParse(instruction.Operand, out var bytesize);
                        instruction.Bytesize = 3 * bytesize;
                    } else if (instruction.Opcode.Equals("BYTE")) {
                        if (instruction.Operand.Contains("C'")) {
                            String str = instruction.Operand.Substring(1);
                            int size = str.Length - 2;    //TO GET RID OF " ' "
                            instruction.Bytesize = size;
                            instruction.CharacterConstant = true;
                        } else if (instruction.Operand.Contains("X'") && !instruction.Operand.Contains("=")) {
                            String str = instruction.Operand.Substring(1);
                            int size = str.Length - 1;
                            if (size % 2 != 0) {
                                Program.errorMessages.Add("ERROR: ODD NUMBER OF HEX VALUES AT ADDRESS " + String.Format("%02X", staticTracker & 0xFFFFF));
                            }
                            size = size / 2;
                            instruction.Bytesize = size;
                            instruction.HexConstant = true;
                        } else
                            Program.errorMessages.Add("ERROR: NO QUOTES FOUND IN THE OPERAND FIELD AT ADDRESS " + String.Format("%02X", staticTracker & 0xFFFFF));
                    }
                    if (!instruction.HasLabel) {
                        //Main.errorMessages.add("ERROR: MISSING LABEL AT ADDRESS " + String.format("%02X", staticTracker & 0xFFFFF));
                    }
                }
            } catch (Exception e) {

            }
        }

    }
        
        private static void checkValidOpcode(Instruction instruction) {
            if (!instruction.Opcode.Equals("START") && !instruction.Opcode.Equals("END") && !instruction.Opcode.Equals("RESW")
                && !instruction.Opcode.Equals("RESB") && !instruction.Opcode.Equals("BYTE") && !instruction.Opcode.Equals("BASE") && !instruction.Opcode.Equals("WORD")
                && !instruction.Opcode.Equals("LTORG") && !instruction.Opcode.Equals("") && !instruction.Opcode.Equals("EQU")
                && !instruction.Opcode.Equals("CSECT") && !instruction.Opcode.Equals("USE")) {

                errorMessages.Add("ERROR: INVALID MNEMONIC " + instruction.Opcode + " WILL BE IGNORED ");

                instruction.Ignore = (true);     //WILL HIDE INVALID INSTRUCTIONS IF OPCODE IS INVALID
            }
        }
        
        private static int findPrime(int p) {
            if(p==0)

                return 2;

            if(p%2==0)

                p++;

            else

                p+=2;

            while(true)

            {

                if(isNoPrime(p)==1)

                {

                    return p;

                }

                p+=2;

            }
        }
        
        private static int isNoPrime(int p) {
            for(var aa=3;aa<=Math.Sqrt(p);aa+=2)

                if(p%aa==0)

                    return 0;

            return 1;
        }

        private static int findHashVal(string key, int p)   // Get Hash Value of each String

        {

            var hashVal=0;
            var s = new string(key.ToCharArray());


            for(var j=0; j < key.Length; j++)

            {
                var Val = s[j];    //Using ASCII Values
                hashVal=(hashVal*26+Val)%p;

            }

            return hashVal;

        }
        private static void createSICOPS(String[] args) {

            try {
                System.IO.StreamReader file =   
                    new System.IO.StreamReader(args[1]);
                string line;
                while ((line = file.ReadLine()) != null) {
                   // Console.WriteLine("this is my line " + line);
                    Pair pair = new Pair(line);
                   // Console.WriteLine("pairs mnemonic is " + pair.Mnemonic);
                   // Console.WriteLine("pairs opcode is " + pair.Opcode);
                    var hashValue = findHashVal(pair.Mnemonic, prime);
                    SICOPStable.createHashArray(hashValue, line);
                }
                file.Close();
            } catch (Exception e) {
                Console.WriteLine(e);
            }


        }
    }





