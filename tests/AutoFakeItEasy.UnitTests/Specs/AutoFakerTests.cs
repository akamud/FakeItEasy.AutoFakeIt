using AutoFakeItEasy.UnitTests.Stubs;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace AutoFakeItEasy.UnitTests.Specs
{
    public class AutoFakerTests
    {
        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithoutDependencies()
        {
            var sut = new AutoFaker().Generate<SimpleSut>();

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldThrowExceptionWhenClassHasNoConstructorsWithFakeableParameters()
        {
            Action act = () => new AutoFaker().Generate<UnfakeableSut>();

            act.Should().Throw<ArgumentException>().WithMessage("No suitable constructor found for type.");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithFakedDependencies()
        {
            var sut = new AutoFaker().Generate<DependenciesSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyA.Method()).Returns("Faked");
            sut.DependencyA.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependencies()
        {
            var autoFaker = new AutoFaker();
            var sut = autoFaker.Generate<SubdependenciesSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.Dependency.Subdependency.Method()).Returns("Faked");
            sut.Dependency.Subdependency.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependenciesDespiteItsOrder()
        {
            var autoFaker = new AutoFaker();
            autoFaker.Resolve<Dependency>();
            var sut = autoFaker.Generate<SubdependenciesSut>();

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithTheMostFakedDependenciesPossible()
        {
            var sut = new AutoFaker().Generate<MultipleConstructorsSut>();

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyB.Method()).Returns("Faked");
            sut.DependencyB.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReusePreviouslyResolvedType()
        {
            var autoFaker = new AutoFaker();
            var depA = autoFaker.Resolve<DependencyA>();
            var sut = autoFaker.Generate<DependenciesSut>();

            sut.DependencyA.Should().Be(depA);
        }

        [Test]
        public void ResolveShouldReturnGeneratedFakeTypes()
        {
            var autoFaker = new AutoFaker();
            var sut = autoFaker.Generate<DependenciesSut>();

            autoFaker.Resolve<DependencyA>().Should().Be(sut.DependencyA);
        }

        [Test]
        public void ResolveShouldGenerateAndReturnAFakeTypeWhenNoneIsAvailable()
        {
            var autoFaker = new AutoFaker();
            var depA = autoFaker.Resolve<DependencyA>();

            depA.Should().NotBeNull();
            A.CallTo(() => depA.Method()).Returns("Faked");
            depA.Method().Should().Be("Faked");
        }

        [Test]
        public void ResolveShouldReturnProvidedDependency()
        {
            var autoFaker = new AutoFaker();
            var depA = new DependencyA();
            autoFaker.Provide(depA);

            autoFaker.Resolve<DependencyA>().Should().Be(depA);
        }

        [Test]
        public void ResolveShouldUpdateProvidedDependencyWhenOneIsAlreadyAvailable()
        {
            var autoFaker = new AutoFaker();
            var depA = new DependencyA();
            autoFaker.Provide(depA);
            var depA2 = new DependencyA();
            autoFaker.Provide(depA2);

            autoFaker.Resolve<DependencyA>().Should().Be(depA2);
        }

        [Test]
        public void ResolveShouldReturnObjectsRegisteredWithExplicitTypes()
        {
            var autoFaker = new AutoFaker();
            var depA = new DependencyA();
            autoFaker.Provide<IDependencyA>(depA);

            autoFaker.Resolve<IDependencyA>().Should().Be(depA);
        }

        [Test]
        public void GenerateShouldUseProvidedDependency()
        {
            var autoFaker = new AutoFaker();
            var depA = new DependencyA();
            autoFaker.Provide(depA);

            var sut = autoFaker.Generate<DependenciesSut>();
            sut.DependencyA.Should().Be(depA);
        }
    }
}
