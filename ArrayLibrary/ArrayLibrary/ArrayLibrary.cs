namespace ArrayLibrary;
public static class ArrayMinimum
{
    public static IEnumerable<T> FindLocalMinima<T>(IEnumerable<T> collection) where T : IComparable<T>
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        var list = collection.ToList();

        if (list.Count < 3)
            throw new InvalidOperationException("В колекции должно быть больше трех элементов.");

        for (int i = 0; i < list.Count; i++)
        {
            if (i == 0)
            {
                if (list[i].CompareTo(list[i + 1]) < 0)
                {
                    yield return list[i];
                }
            }
            else if (i == list.Count - 1)
            {
                if (list[i].CompareTo(list[i - 1]) < 0)
                {
                    yield return list[i];
                }
            }
            else
            {
                if (list[i].CompareTo(list[i - 1]) < 0 && list[i].CompareTo(list[i + 1]) < 0)
                {
                    yield return list[i];
                }
            }
        }
    }
}
