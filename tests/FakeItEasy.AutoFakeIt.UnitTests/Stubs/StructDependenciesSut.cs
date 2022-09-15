namespace FakeItEasy.AutoFakeIt.UnitTests.Stubs
{
    public struct ValueDependency
    {
        public string Value { get; }
        public ValueDependency()
        {
            Value = System.Guid.NewGuid().ToString();
        }
    }

    public class StructDependenciesSut
    {
        public ValueDependency ValueDependency { get; }
        public DependencyA DependencyA { get; }

        public StructDependenciesSut(DependencyA dependencyA, ValueDependency valueDependency)
        {
            ValueDependency = valueDependency;
            DependencyA = dependencyA;
        }
    }
}
