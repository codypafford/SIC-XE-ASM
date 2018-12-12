using System;
using System.Linq;

namespace ConsoleApp3
{
    public class Instruction
    {
        private string word;
        private int num;
        private string LABEL = "";
        private string OPCODE= null;
        private string OPERAND = "";
        private string COMMENT = "";
        private int BYTESIZE = 0;
        private int ADDRESS;
        private bool unrecognized = false;
        private bool ignore = false;
        private bool hasLabel = false;
        private bool hasOperand= false;
        private bool hasOpCode= false;
        private bool hasComment= false;

        private string numericOpcode = "    ";
        private bool N = false;
        private bool I = false;
        private bool X = false;
        private bool B = false;
        private bool P = true;
        private bool E = false;
        private int PCRelative;
        private bool hasnumericOpcode = false;
        private bool isint;
        private bool characterConstant;
        private bool hexConstant;
        private int codeBlockNum;
        private int RelativeAddress;
        private int codeBlock = 0;
        private string objectCode = null;
        
        
        
        
        public Instruction(String line) {         //HANDLE ALL OTHER INSTRUCTIONS
        String[] operate = line.Trim().Split();
            operate = operate.Where(address=>!string.IsNullOrWhiteSpace(address))
                .ToArray();
        if(operate.Length > 1) {
           // if(operate.)

            if (operate[1].Equals("START")) {
                Program.PROGRAMNAME = operate[0];
                LABEL = operate[0];
                OPCODE = operate[1];
                OPERAND = operate[2];
                Program.STARTADDRESS = Convert.ToInt32(operate[2], 16);
                Program.staticTracker = Convert.ToInt32(operate[2], 16);
                Program.LOCCTR = Convert.ToInt32(operate[2], 16);
                hasLabel = true;
                hasOperand = true;
                hasOpCode = true;
            } else if (operate.Length == 4) {
                LABEL = operate[0];    //label
                OPCODE = operate[1];    //opcode
                OPERAND = operate[2];    //operand
                COMMENT = operate[3];    //comment
                hasLabel = true;
                hasOpCode = true;
                hasOperand = true;
                hasComment = true;

            } else if (operate.Length == 3 && !line.Contains(".")) {
                LABEL = operate[0];
                OPCODE = operate[1];    //operate length changes based on instruction
                OPERAND = operate[2];
                hasLabel = true;
                hasOpCode = true;
                hasOperand = true;

            } else if (operate.Length == 3 && line.Contains(".")) {
                OPCODE = operate[0];
                OPERAND = operate[1];
                COMMENT = operate[2];
                hasOpCode = true;
                hasOperand = true;
                hasComment = true;


            }else if(operate.Length == 2 && !line.Contains(".")){
                OPCODE = operate[0];
                OPERAND = operate[1];
                hasOpCode = true;
                hasOperand = true;
                if(operate[0].Equals("START")){
                    Program.STARTADDRESS = Int32.Parse(operate[1], System.Globalization.NumberStyles.HexNumber);
                    Program.staticTracker = Int32.Parse(operate[1], System.Globalization.NumberStyles.HexNumber);
                }
            }
        }else {
            OPCODE = operate[0];

        }

    }
        
        public Instruction(String[] str) {    //HANDLE COMMENTS
            //turn str to string first



            if(str[1].Equals("START")){
                Program.PROGRAMNAME = str[0];
                LABEL = str[0];
                OPCODE = str[1];
                OPERAND = str[2];
                Program.STARTADDRESS = Int32.Parse(str[2], System.Globalization.NumberStyles.HexNumber);
                Program.staticTracker = Int32.Parse(str[2], System.Globalization.NumberStyles.HexNumber);
                Program.LOCCTR = Int32.Parse(str[2], System.Globalization.NumberStyles.HexNumber);
                hasLabel = true;
                hasOperand = true;

            }else if(str.Length == 3){
                LABEL = str[0];    //label
                OPCODE = str[1];    //opcode
                OPERAND = str[2];    //operand
                hasLabel = true;
                hasOperand = true;
                hasOpCode = true;

            }else if(str.Length == 2){
                OPCODE = str[0];    //operate length changes based on instruction
                OPERAND = str[1];
                hasOperand = true;
                hasOpCode = true;


            }else if(str.Length == 1){   //FIND WAY TO MAKE THIS NOT ALWAYS FALSE
                OPCODE = str[0];
            }else if(str == null){

            }
        }

        public Instruction() {
            LABEL = "";
            OPCODE = "";
            OPERAND = "";
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

        public string Label
        {
            get => LABEL;
            set => LABEL = value;
        }

        public string Opcode
        {
            get => OPCODE;
            set => OPCODE = value;
        }

        public string Operand
        {
            get => OPERAND;
            set => OPERAND = value;
        }

        public string Comment
        {
            get => COMMENT;
            set => COMMENT = value;
        }

        public int Bytesize
        {
            get => BYTESIZE;
            set => BYTESIZE = value;
        }

        public int Address
        {
            get => ADDRESS;
            set => ADDRESS = value;
        }

        public string ObjectCode1
        {
            get => objectCode;
            set => objectCode = value;
        }

        public bool Unrecognized
        {
            get => unrecognized;
            set => unrecognized = value;
        }

        public bool Ignore
        {
            get => ignore;
            set => ignore = value;
        }

        public bool HasLabel
        {
            get => hasLabel;
            set => hasLabel = value;
        }

        public bool HasOperand
        {
            get => hasOperand;
            set => hasOperand = value;
        }

        public bool HasOpCode
        {
            get => hasOpCode;
            set => hasOpCode = value;
        }

        public bool HasComment
        {
            get => hasComment;
            set => hasComment = value;
        }

        public string NumericOpcode
        {
            get =>  numericOpcode;
            set => numericOpcode = value;
        }

        public bool N1
        {
            get => N;
            set => N = value;
        }

        public bool I1
        {
            get => I;
            set => I = value;
        }

        public bool X1
        {
            get => X;
            set => X = value;
        }

        public bool B1
        {
            get => B;
            set => B = value;
        }

        public bool P1
        {
            get => P;
            set => P = value;
        }

        public bool E1
        {
            get => E;
            set => E = value;
        }

        public int PcRelative
        {
            get => PCRelative;
            set => PCRelative = value;
        }

        public bool HasnumericOpcode
        {
            get => hasnumericOpcode;
            set => hasnumericOpcode = value;
        }

        public bool Isint
        {
            get => isint;
            set => isint = value;
        }

        public bool CharacterConstant
        {
            get => characterConstant;
            set => characterConstant = value;
        }

        public bool HexConstant
        {
            get => hexConstant;
            set => hexConstant = value;
        }

        public int CodeBlockNum
        {
            get => codeBlockNum;
            set => codeBlockNum = value;
        }

        public int RelativeAddress1
        {
            get => RelativeAddress;
            set => RelativeAddress = value;
        }

        public int CodeBlock
        {
            get => codeBlock;
            set => codeBlock = value;
        }

        public string ObjectCode
        {
            get => objectCode;
            set => objectCode = value;
        }
        
        public override string ToString()
        {
            String address = Address.ToString("X");
            return String.Format("{0,7}  {1,9} {2, 8} {3,8} {4,20} {5,10}",address, Label, Opcode, Operand, Comment, ObjectCode);
        }

        public bool isInteger(String d)
        {
            int s = Int32.Parse(d);
            if (s % 1 == 0)
            {
                return true;
            }
             return false;
        }
    }
}