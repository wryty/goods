using ArrayLibrary;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void FindMinValid()
        {
            var numbers = new List<int> { 1,5,6,2,3 };

            var localMinima = ArrayMinimum.FindLocalMinima(numbers);

            CollectionAssert.AreEquivalent(new[] { 1,2 }, localMinima);
        }
        [Test]
        public void FindMinEmpty()
        {
            var equalNumbers = new List<int> { 2, 2, 2, 2, 2 };

            var localMinima = ArrayMinimum.FindLocalMinima(equalNumbers);

            CollectionAssert.IsEmpty(localMinima);
        }

        [Test]
        public void FindMinFirst()
        {
            var bigCollection = new List<int> { 7,7,7,7,7,7,7,7,7,7,7,7,7,1,7,7,7,7,7,7,7,7,7,7 };

            var localMinima = ArrayMinimum.FindLocalMinima(bigCollection);

            CollectionAssert.AreEqual(new[] { 1 }, localMinima);
        }

        [Test]
        public void FinMinBigNumbers()
        {
            var testDataBigNum = new List<int> { 1012312,34534,12312334,1231231243,0,5 };

            var localMinima = ArrayMinimum.FindLocalMinima(testDataBigNum);

            CollectionAssert.AreEqual(new[] { 34534,0 }, localMinima);
        }

        [Test]
        public void FindMinNegative()
        {
            var testDataNegative = new List<char> { 'a', 'b', 'c'};

            var localMinima = ArrayMinimum.FindLocalMinima(testDataNegative);

            CollectionAssert.AreEqual(new[] { 'a'}, localMinima);
        }
    }
}
