using FakeItEasy.AutoFakeIt.UnitTests.Stubs;
using FakeItEasy.Core;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace FakeItEasy.AutoFakeIt.UnitTests.Specs
{
    public class AutoFakeItTestsNonGeneric
    {
        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithoutDependencies()
        {
            var sut = new AutoFakeIt().Generate(typeof(SimpleSut));

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldThrowExceptionWhenClassHasNoConstructorsWithFakeableParameters()
        {
            Action act = () => new AutoFakeIt().Generate(typeof(UnfakeableSut));

            act.Should().Throw<ArgumentException>()
                .WithMessage($"No suitable constructor found for type '{typeof(UnfakeableSut).FullName}'.")
                .WithInnerException<FakeCreationException>();
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithFakedDependencies()
        {
            var sut = (DependenciesSut)new AutoFakeIt().Generate(typeof(DependenciesSut));

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyA.Method()).Returns("Faked");
            sut.DependencyA.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependencies()
        {
            var autoFakeIt = new AutoFakeIt();
            var sut = (SubdependenciesSut)autoFakeIt.Generate(typeof(SubdependenciesSut));

            sut.Should().NotBeNull();
            A.CallTo(() => sut.Dependency.Subdependency.Method()).Returns("Faked");
            sut.Dependency.Subdependency.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithSubdependenciesDespiteItsOrder()
        {
            var autoFakeIt = new AutoFakeIt();
            autoFakeIt.Resolve(typeof(Dependency));
            var sut = autoFakeIt.Generate(typeof(SubdependenciesSut));

            sut.Should().NotBeNull();
        }

        [Test]
        public void GenerateShouldReturnAnInstanceOfClassWithTheMostFakedDependenciesPossible()
        {
            var sut = (MultipleConstructorsSut)new AutoFakeIt().Generate(typeof(MultipleConstructorsSut));

            sut.Should().NotBeNull();
            A.CallTo(() => sut.DependencyB.Method()).Returns("Faked");
            sut.DependencyB.Method().Should().Be("Faked");
        }

        [Test]
        public void GenerateShouldReusePreviouslyResolvedType()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = autoFakeIt.Resolve(typeof(DependencyA));
            var sut = (DependenciesSut)autoFakeIt.Generate(typeof(DependenciesSut));

            sut.DependencyA.Should().Be(depA);
        }

        [Test]
        public void ResolveShouldReturnGeneratedFakeTypes()
        {
            var autoFakeIt = new AutoFakeIt();
            var sut = (DependenciesSut)autoFakeIt.Generate(typeof(DependenciesSut));

            autoFakeIt.Resolve(typeof(DependencyA)).Should().Be(sut.DependencyA);
        }

        [Test]
        public void ResolveShouldGenerateAndReturnAFakeTypeWhenNoneIsAvailable()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = (DependencyA)autoFakeIt.Resolve(typeof(DependencyA));

            depA.Should().NotBeNull();
            A.CallTo(() => depA.Method()).Returns("Faked");
            depA.Method().Should().Be("Faked");
        }

        [Test]
        public void ResolveShouldReturnProvidedDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(depA.GetType(), depA);

            autoFakeIt.Resolve(typeof(DependencyA)).Should().Be(depA);
        }

        [Test]
        public void ResolveShouldReturnProvidedValueTypeDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var structDep = new ValueDependency();
            autoFakeIt.Provide(structDep.GetType(), structDep);

            autoFakeIt.Resolve(typeof(ValueDependency)).Should().Be(structDep);
        }

        [Test]
        public void GenerateShouldReturnProvidedValueTypeDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var structDep = new ValueDependency();
            autoFakeIt.Provide(structDep.GetType(), structDep);

            var generated = (StructDependenciesSut)autoFakeIt.Generate(typeof(StructDependenciesSut));
            generated.ValueDependency.Should().Be(structDep);
        }

        [Test]
        public void ResolveShouldUpdateProvidedDependencyWhenOneIsAlreadyAvailable()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(depA.GetType(), depA);
            var depA2 = new DependencyA();
            autoFakeIt.Provide(depA.GetType(), depA2);

            autoFakeIt.Resolve(typeof(DependencyA)).Should().Be(depA2);
        }

        [Test]
        public void ResolveShouldReturnObjectsRegisteredWithExplicitTypes()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(typeof(IDependencyA), depA);

            autoFakeIt.Resolve(typeof(IDependencyA)).Should().Be(depA);
        }

        [Test]
        public void GenerateShouldUseProvidedDependency()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            autoFakeIt.Provide(typeof(DependencyA), depA);

            var sut = (DependenciesSut)autoFakeIt.Generate(typeof(DependenciesSut));
            sut.DependencyA.Should().Be(depA);
        }

        [Test]
        public void ProvideShouldThrowExceptionWhenDependencyIsNotAssignableToRegisterType()
        {
            var autoFakeIt = new AutoFakeIt();
            var depA = new DependencyA();
            var act = () => autoFakeIt.Provide(typeof(DependencyB), depA);

            act.Should().Throw<ArgumentException>()
             .WithMessage($"Dependency type '{typeof(DependencyB).FullName}' is not assignable to '{typeof(DependencyA).FullName}'.");
        }
    }
}
