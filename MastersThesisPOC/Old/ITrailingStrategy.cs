namespace MastersThesisPOC.Old
{
    public interface ITrailingStrategy
    {
        int CountTrailing(string mantissa);
    }

    public class TrailingZerosStrategy : ITrailingStrategy
    {
        public int CountTrailing(string mantissa)
        {
            int count = 0;
            for (int i = mantissa.Length - 1; i >= 0; i--)
            {
                if (mantissa[i] == '0')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }
    }

    public class TrailingOnesStrategy : ITrailingStrategy
    {
        public int CountTrailing(string mantissa)
        {
            int count = 0;
            for (int i = mantissa.Length - 1; i >= 0; i--)
            {
                if (mantissa[i] == '1')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }
    }
}
