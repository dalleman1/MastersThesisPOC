namespace MastersThesisPOC.Helpers
{
    public class ListWithDuplicates : List<KeyValuePair<float, float>>
    {
        public void Add(float key, float value)
        {
            var element = new KeyValuePair<float, float>(key, value);
            this.Add(element);
        }
    }

}
