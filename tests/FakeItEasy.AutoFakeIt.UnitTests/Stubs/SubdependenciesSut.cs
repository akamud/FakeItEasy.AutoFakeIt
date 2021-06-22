namespace FakeItEasy.AutoFakeIt.UnitTests.Stubs
{
    public class Dependency
    {
        public Subdependency Subdependency { get; }

        public Dependency(Subdependency subdependency)
        {
            Subdependency = subdependency;
        }
    }

    public class AnotherDependency
    {
        public Subdependency Subdependency { get; }

        public AnotherDependency(Subdependency subdependency)
        {
            Subdependency = subdependency;
        }
    }


    public class Subdependency
    {
        public virtual string Method() => "Sub";
    }

    public class SubdependenciesSut
    {
        public Dependency Dependency { get; }

        public SubdependenciesSut(Dependency dependency, AnotherDependency anotherDependency)
        {
            Dependency = dependency;
        }
    }
}
