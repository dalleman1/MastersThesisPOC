namespace MastersThesisPOC
{
    public interface IMetrics
    {
        float CalculatePercentDifference(float value1, float value2);
        float CalculateAverageErrorPercentDifference(List<float> originalValues, List<float> newValues, float M);
        (float list1Average, float list2Average) CalculateAverages(List<float> list1, List<float> list2);
    }

    public class Metrics : IMetrics
    {
        public float CalculatePercentDifference(float value1, float value2)
        {
            return Math.Abs((value1 - value2) / ((value1 + value2) / 2)) * 100;
        }

        public float CalculateAverageErrorPercentDifference(List<float> originalValues, List<float> newValues, float M)
        {
            if (originalValues.Count != newValues.Count)
            {
                throw new ArgumentException("Both lists must have the same number of elements.");
            }

            float totalDifference = 0;

            for (int i = 0; i < originalValues.Count; i++)
            {
                var res = CalculatePercentDifference(originalValues[i], (newValues[i]/M));
                totalDifference += res;
            }

            return totalDifference / originalValues.Count;
        }

        public (float list1Average, float list2Average) CalculateAverages(List<float> list1, List<float> list2)
        {
            float list1Average = list1.Average();
            float list2Average = list2.Average();

            return (list1Average, list2Average);
        }
    }
}
