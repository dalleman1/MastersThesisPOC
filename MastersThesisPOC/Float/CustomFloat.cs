namespace MastersThesisPOC.Float
{
    public class CustomFloat : ICustomFloat
    {
        private readonly uint _bits;

        public CustomFloat(float value)
        {
            uint bits = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            _bits = bits;
        }

        public string MantissaAsBitString
        {
            get { return Convert.ToString(_bits & 0x7FFFFF, 2).PadLeft(23, '0'); }
        }

        public uint GetMantissaAsUInt()
        {
            int bits = BitConverter.ToInt32(BitConverter.GetBytes(_bits), 0);
            uint mantissa = (uint)(bits & 0x7FFFFF);
            return mantissa;
        }


        public string GetMantissaAsStringFromUint(uint mantissa)
        {
            string binaryString = Convert.ToString(mantissa, 2).PadLeft(23, '0');
            return binaryString;
        }

        public string ExponentAsBitString
        {
            get { return Convert.ToString(_bits >> 23 & 0xFF, 2).PadLeft(8, '0'); }
        }

        public string SignAsBitString
        {
            get { return Convert.ToString(_bits >> 31 & 0x1, 2); }
        }
        public string ToBitString()
        {
            return Convert.ToString(_bits, 2).PadLeft(32, '0');
        }
    }

}
