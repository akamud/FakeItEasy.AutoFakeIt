namespace FakeItEasy.AutoFakeIt.UnitTests.Stubs
{
    public class DependencyB
    {
        public virtual string Method() => "A";
    }

    public class MultipleConstructorsSut
    {
        public DependencyB DependencyB { get; }

        public MultipleConstructorsSut(DependencyB dependencyB)
        {
            DependencyB = dependencyB;
        }

        public MultipleConstructorsSut(DependencyB dependencyB, int number)
        {
            DependencyB = dependencyB;
        }
    }
}
