namespace MastersThesisPOC.Float
{
    public interface ICustomFloat
    {
        string ExponentAsBitString { get; }
        string MantissaAsBitString { get; }
        string SignAsBitString { get; }

        string ToBitString();
        string GetMantissaAsStringFromUint(uint mantissa);
        uint GetMantissaAsUInt();
    }
}