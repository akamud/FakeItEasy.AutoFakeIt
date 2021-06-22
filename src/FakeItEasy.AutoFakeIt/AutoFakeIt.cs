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

        public T Generate<T>() where T : class
        {
            var constructors = typeof(T).GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length)
                .ToList();

            Exception? lastThrownException = null;
            for (var i = 0; i < constructors.Count; i++)
            {
                try
                {
                    var candidateFakeObjects = GenerateCandidateFakeObjects(constructors[i]);

                    var generatedObject = (T)constructors[i].Invoke(candidateFakeObjects.Values.ToArray());

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

        public T Resolve<T>() where T : class
        {
            if (_fakedObjects.ContainsKey(typeof(T)))
                return (T)_fakedObjects[typeof(T)];
            else
            {
                _fakedObjects[typeof(T)] = A.Fake<T>();
                return (T)_fakedObjects[typeof(T)];
            }
        }

        public void Provide<T>(T dependency) where T : class
        {
            if (_fakedObjects.ContainsKey(typeof(T)))
                _fakedObjects[typeof(T)] = dependency;
            else
                _fakedObjects.Add(typeof(T), dependency);
        }
    }
}
