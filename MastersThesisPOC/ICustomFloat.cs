namespace MastersThesisPOC
{
    public interface ICustomFloat
    {
        string ExponentAsBitString { get; }
        string MantissaAsBitString { get; }
        string SignAsBitString { get; }

        string ToBitString();
    }
}