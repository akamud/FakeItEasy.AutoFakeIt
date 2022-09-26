using FakeItEasy.AutoFakeIt.UnitTests.Stubs;
using FakeItEasy.Core;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace FakeItEasy.AutoFakeIt.UnitTests.Specs
{
    public class AutoFakeItTests
    {
        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithoutDependencies()
        {
            var sut = new AutoFakeIt().Generate<SimpleSut>();

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldThrowExceptionWhenClassHasNoConstructorsWithFakeableParameters()
        {
            Action act = () => new AutoFakeIt().Generate<UnfakeableSut>();

            act.Should().Throw<ArgumentException>()
                .WithMessage($"No suitable constructor found for type '{typeof(UnfakeableSut).FullName}'.")
                .WithInnerException<FakeCreationException>();
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithFakedDependencies()
        {
            var sut = new AutoFakeIt().Generate<DependenciesSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyA.Method()).Returns("Faked");
            sut.DependencyA.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependencies()
        {
            var autoFakeIt = new AutoFakeIt();
            var sut = autoFakeIt.Generate<SubdependenciesSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.Dependency.Subdependency.Method()).Returns("Faked");
            sut.Dependency.Subdependency.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependenciesDespiteItsOrder()
        {
            var autoFakeIt = new AutoFakeIt();
            autoFakeIt.Resolve<Dependency>();
            var sut = autoFakeIt.Generate<SubdependenciesSut>();

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithTheMostFakedDependenciesPossible()
        {
            var sut = new AutoFakeIt().Generate<MultipleConstructorsSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyB.Method()).Returns("Faked");
            sut.DependencyB.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReusePreviouslyResolvedType()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = autoFakeIt.Resolve<DependencyA>();
            var sut = autoFakeIt.Generate<DependenciesSut>();

            sut.DependencyA.Should().Be(depA);
        }

        [Test]
        public void ResolveShouldReturnGeneratedFakeTypes()
        {
            var autoFakeIt = new AutoFakeIt();
            var sut = autoFakeIt.Generate<DependenciesSut>();

            autoFakeIt.Resolve<DependencyA>().Should().Be(sut.DependencyA);
        }

        [Test]
        public void ResolveShouldGenerateAndReturnAFakeTypeWhenNoneIsAvailable()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = autoFakeIt.Resolve<DependencyA>();

            depA.Should().NotBeNull();
            A.CallTo(() => depA.Method()).Returns("Faked");
            depA.Method().Should().Be("Faked");
        }

        [Test]
        public void ResolveShouldReturnProvidedDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(depA);

            autoFakeIt.Resolve<DependencyA>().Should().Be(depA);
        }

        [Test]
        public void ResolveShouldReturnProvidedValueTypeDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var structDep = new ValueDependency();
            autoFakeIt.Provide(structDep);

            autoFakeIt.Resolve<ValueDependency>().Should().Be(structDep);
        }

        [Test]
        public void GenerateShouldReturnProvidedValueTypeDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var structDep = new ValueDependency();
            autoFakeIt.Provide(structDep);

            autoFakeIt.Generate<StructDependenciesSut>().ValueDependency.Should().Be(structDep);
        }

        [Test]
        public void ResolveShouldUpdateProvidedDependencyWhenOneIsAlreadyAvailable()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(depA);
            var depA2 = new DependencyA();
            autoFakeIt.Provide(depA2);

            autoFakeIt.Resolve<DependencyA>().Should().Be(depA2);
        }

        [Test]
        public void ResolveShouldReturnObjectsRegisteredWithExplicitTypes()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide<IDependencyA>(depA);

            autoFakeIt.Resolve<IDependencyA>().Should().Be(depA);
        }

        [Test]
        public void GenerateShouldUseProvidedDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(depA);

            var sut = autoFakeIt.Generate<DependenciesSut>();
            sut.DependencyA.Should().Be(depA);
        }

        [Test]
        public void GenericResolveShouldWorkWithNonGenericProvide()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(typeof(DependencyA), depA);

            var resolvedDepA = autoFakeIt.Resolve<DependencyA>();
            resolvedDepA.Should().NotBeNull();
        }
    }
}
