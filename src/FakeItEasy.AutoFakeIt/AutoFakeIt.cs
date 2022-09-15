using FakeItEasy.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FakeItEasy.AutoFakeIt
{
    /// <summary>
    /// The main entrypoint to automatically generate classes with faked dependencies.
    /// </summary>
    public class AutoFakeIt
    {
        private readonly IDictionary<Type, object> _fakedObjects = new Dictionary<Type, object>();

        /// <summary>
        /// Generates a <typeparamref name="T"/> automatically injecting FakeItEasy's fakes for all its dependencies.
        /// </summary>
        /// <typeparam name="T">The class you want to generate, usually your System Under Test.</typeparam>
        /// <returns>A class with all its dependencies faked.</returns>
        /// <exception cref="ArgumentException">Throws an ArgumentException if we can't find a public constructor
        /// with "fakeable" dependencies.</exception>
        public T Generate<T>() where T : class
        {
            var constructors = typeof(T).GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .ToList();

            Exception? lastThrownException = null;
            foreach (var ctor in constructors)
            {
                try
                {
                    var candidateFakeObjects = GenerateCandidateFakeObjects(ctor);

                    var generatedObject = (T)ctor.Invoke(candidateFakeObjects.Values.ToArray());

                    InsertMissingFakedObjects(candidateFakeObjects);

                    return generatedObject;
                }
                catch (Exception ex)
                {
                    lastThrownException = ex;
                    // Keep looking for a suitable constructor
                }
            }

            throw new ArgumentException($"No suitable constructor found for type '{typeof(T)}'.", lastThrownException);
        }

        private Dictionary<Type, object> GenerateCandidateFakeObjects(ConstructorInfo ctor)
        {
            var candidateFakeObjects = new Dictionary<Type, object>();
            foreach (var param in ctor.GetParameters())
            {
                candidateFakeObjects.Add(param.ParameterType, _fakedObjects.ContainsKey(param.ParameterType)
                    ? _fakedObjects[param.ParameterType]
                    : Create.Fake(param.ParameterType));
            }

            return candidateFakeObjects;
        }

        private void InsertMissingFakedObjects(Dictionary<Type, object> candidateFakeObjects)
        {
            foreach (var temporaryFakedObject in candidateFakeObjects)
            {
                if (!_fakedObjects.ContainsKey(temporaryFakedObject.Key))
                    _fakedObjects.Add(temporaryFakedObject.Key, temporaryFakedObject.Value);
            }
        }

        /// <summary>
        /// Returns the object used for <typeparamref name="T"/>. If an object of the given type was not explicitly
        /// provided, it will create a FakeItEasy's fake that will be used for all subsequent calls.
        /// </summary>
        /// <typeparam name="T">The fake object you want to retrieve.</typeparam>
        /// <returns>An object of the given type, either previously provided or a new one just generated with FakeItEasy.</returns>
        public T Resolve<T>() where T : notnull
        {
            if (_fakedObjects.ContainsKey(typeof(T)))
                return (T)_fakedObjects[typeof(T)];
            else
            {
                _fakedObjects[typeof(T)] = A.Fake<T>();
                return (T)_fakedObjects[typeof(T)];
            }
        }

        /// <summary>
        /// Explicitly sets an object to use when someone asks for an object of the given type, either on a
        /// <see cref="Resolve{T}"/> or for a dependency on <see cref="Generate{T}"/>. </summary>
        /// <remarks><para>This is useful for providing your own specific instance of a type, either because you can't
        /// use an automatically generated fake, or because you have a concrete type that you prefer to use.</para>
        /// <para>It will override any previously registered object of the same type of <typeparamref name="T"/>.</para></remarks>
        /// <param name="dependency">The object you want to set.</param>
        /// <typeparam name="T">The type of the object you want to register it as. You can set an object for a specific
        /// type, for example, you can provide an object to be used when someone asks for an interface.</typeparam>
        public void Provide<T>(T dependency) where T : notnull
        {
            if (_fakedObjects.ContainsKey(typeof(T)))
                _fakedObjects[typeof(T)] = dependency;
            else
                _fakedObjects.Add(typeof(T), dependency);
        }
    }
}
