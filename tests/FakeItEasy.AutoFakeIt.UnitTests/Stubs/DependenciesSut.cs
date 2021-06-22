namespace FakeItEasy.AutoFakeIt.UnitTests.Stubs
{
    public class DependencyA : IDependencyA
    {
        public virtual string Method() => "A";
        public string OtherMethod() => "OtherA";
    }

    public interface IDependencyA
    {
        public string OtherMethod();
    }

    public class DependenciesSut
    {
        public DependencyA DependencyA { get; }

        public DependenciesSut(DependencyA dependencyA)
        {
            DependencyA = dependencyA;
        }
    }
}
