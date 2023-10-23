using System.Collections;

namespace AmazingCalculator.Tests.Unit;

public class AddData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { -5, 5, 0 };
        yield return new object[] { -5, -5, -10 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
