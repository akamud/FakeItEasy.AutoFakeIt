using FakeItEasy;
using FakeItEasy.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFakeItEasy
{
    public class AutoFaker
    {
        private readonly IDictionary<Type, object> _fakedObjects = new Dictionary<Type, object>();

        public T Generate<T>() where T : class
        {
            var constructors = typeof(T).GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Length);

            foreach (var ctor in constructors)
            {
                try
                {
                    var candidateFakeObjects = ctor.GetParameters()
                        .Where(p => !_fakedObjects.ContainsKey(p.ParameterType))
                        .ToDictionary(p => p.ParameterType, p => Create.Fake(p.ParameterType));

                    var generatedObject = (T)ctor.Invoke(candidateFakeObjects.Concat(_fakedObjects)
                        .ToDictionary(x => x.Key, x => x.Value).Values.ToArray());

                    foreach (var temporaryFakedObject in candidateFakeObjects)
                    {
                        _fakedObjects.Add(temporaryFakedObject.Key, temporaryFakedObject.Value);
                    }

                    return generatedObject;
                }
                catch (Exception)
                {
                    // Keep looking for a suitable constructor
                }
            }

            throw new ArgumentException("No suitable constructor found for type.");
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
