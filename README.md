# FakeItEasy.AutoFakeIt

![Build](https://github.com/akamud/FakeItEasy.AutoFakeIt/workflows/Build/badge.svg)
[![codecov](https://codecov.io/gh/akamud/FakeItEasy.AutoFakeIt/branch/main/graph/badge.svg?token=6V0YZL6II9)](https://codecov.io/gh/akamud/FakeItEasy.AutoFakeIt)
[![Nuget](https://img.shields.io/nuget/v/FakeItEasy.AutoFakeIt.svg?style=flat)](https://www.nuget.org/packages/FakeItEasy.AutoFakeIt)

A very simple, yet flexible, "AutoFaker" for [FakeItEasy](https://fakeiteasy.github.io/) to easily auto generate classes with faked dependencies.

## Getting started

[NuGet package](https://www.nuget.org/packages/FakeItEasy.AutoFakeIt) available:
```
PM> Install-Package FakeItEasy.AutoFakeIt
```

I focused on providing a familiar and very simple-to-use API, without any unnecessary limitations, so you can automatically generate your [System Under Test (SUT)](https://en.wikipedia.org/wiki/System_under_test) with all the dependencies faked, wether they are classes or interfaces. You can also customize and retrieve any dependency you want.

```csharp
using FakeItEasy;
using FakeItEasy.AutoFakeIt;
using NUnit.Framework;

namespace UnitTests
{
    public interface IMyDependency
    {
        string MyMethod();
    }

    public class MyDependency : IMyDependency
    {
        public string MyMethod() => "Hello";
    }

    public class MyOtherDependency
    {
        public virtual string MyMethod() => "World";
    }

    public class MySut
    {
        public IMyDependency MyDependency { get; }
        public MyOtherDependency MyOtherDependency { get; }

        public MySut(IMyDependency myDependency, MyOtherDependency myOtherDependency)
        {
            MyDependency = myDependency;
            MyOtherDependency = myOtherDependency;
        }
    }

    public class SampleTest
    {
        [Test]
        public void MyTest()
        {
            // the main entrypoint, you can put this on a base class, as long
            // as you always create a new one for each test to make sure that
            // your tests are idempotent and isolated
            var autoFakeIt = new AutoFakeIt();

            // you can optionally provide a dependency for any type
            autoFakeIt.Provide<IMyDependency>(new MyDependency());

            // it will respect any objects you've provided before
            // you can also just let the Generate provide FakeItEasy's fakes
            // for all missing dependencies
            var mySut = autoFakeIt.Generate<MySut>();

            // you can get any dependency used/generated
            var theAutoFakedDependency = autoFakeIt.Resolve<MyOtherDependency>();

            // you can setup or assert the generated fakes since it 
            // just an ordinary FakeItEasy's fake object
            A.CallTo(() => theAutoFakedDependency.MyMethod()).Returns("Faked");
        }
    }
}

```

That's all you probably need from an autofaker. If you want, you can look at this project's [tests](https://github.com/akamud/FakeItEasy.AutoFakeIt/blob/main/tests/FakeItEasy.AutoFakeIt.UnitTests/Specs/AutoFakeItTests.cs) to see all the possibilites.

## Other options

There are many other alternatives to this package, why did I make yet another autofaker for FakeItEasy? While all the other alternatives worked, many of them had a few shortcomings related to how **I** expected it to work. I grew accustomed to the automocker from Moq that had a very similar API to this package.  
That doesn't mean that this is the best implementation available, it just means that this is the implementation that was the most familiar to **me**. Hopefully some of you will find it familiar too. 

My goal with this package was to provide all the features I wanted, that were missing in all the other alternatives I've tested:

* Have a way of providing a concrete class instead of relying on a faked dependency
* Automatically provide a fake for a concrete class (with virtual methods) instead of requiring an interface
* Allow changing a dependency after instantiating the `AutoFakeIt` object
* Allow passing dependencies or requesting generated dependencies any time

All the other packages missed some of the above.

### Available alternatives
- [Autofac.Extras.FakeItEasy](https://github.com/autofac/Autofac.Extras.FakeItEasy)
- [FakeItEasy.Auto](https://github.com/jamiehumphries/FakeItEasy.Auto)
- [FakeItEasy.AutoFake](https://github.com/fkthat/FakeItEasy.AutoFake)

## Contributing

If you have a scenario that isn't supported, please open an issue so we can discuss it :)
