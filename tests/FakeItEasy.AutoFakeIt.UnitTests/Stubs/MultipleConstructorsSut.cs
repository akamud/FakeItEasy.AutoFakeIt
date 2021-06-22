namespace FakeItEasy.AutoFakeIt.UnitTests.Stubs
{
    public abstract class DependencyB
    {
        public virtual string Method() => "B";
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
