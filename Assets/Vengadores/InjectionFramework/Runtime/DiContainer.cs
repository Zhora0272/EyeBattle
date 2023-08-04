using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Vengadores.Utility.LogWrapper;

namespace Vengadores.InjectionFramework
{
    /**
     * Global container for managing the instances.
     */
    public class DiContainer
    {
        private Dictionary<Type, List<object>> _map;
        private Dictionary<Type, Dictionary<string, object>> _mapWithIds;

        private ReflectionCache _reflectionCache;

        public DiContainer()
        {
            _map = new Dictionary<Type, List<object>>();
            _mapWithIds = new Dictionary<Type, Dictionary<string, object>>();

            _reflectionCache = new ReflectionCache();

            RegisterInstance(new ContainerRegistry(this));
        }

        [PublicAPI] public T Get<T>(string id = null)
        {
            return (T)Get(typeof(T), id);
        }

        private object Get(Type objType, string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                if (_map.TryGetValue(objType, out var value) && value.Count > 0)
                {
                    return value[0];
                }
                throw new NullReferenceException(
                    GameLog.GetTagText("Injection") +
                    GameLog.GetColoredText(Color.red, objType.Name) + " not found in DiContainer");
            }
            else
            {
                if (_mapWithIds.TryGetValue(objType, out var idDictionary))
                {
                    if (idDictionary.TryGetValue(id, out var value))
                    {
                        return value;
                    }
                    throw new NullReferenceException(
                        GameLog.GetTagText("Injection") + 
                        GameLog.GetColoredText(Color.red, objType.Name) + " with id:" + 
                        GameLog.GetColoredText(Color.red, id) + " not found in DiContainer");
                }
                throw new NullReferenceException(
                    GameLog.GetTagText("Injection") + 
                    GameLog.GetColoredText(Color.red, objType.Name) + " not found in DiContainer");
            }
        }
    
        // Called via reflection for array injections
        [PublicAPI] public T[] GetAll<T>()
        {
            var result = new List<T>();
        
            if (_map.TryGetValue(typeof(T), out var value))
            {
                result.AddRange(value.Cast<T>());
            }
        
            if (_mapWithIds.TryGetValue(typeof(T), out var idDictionary))
            {
                result.AddRange(idDictionary.Values.Cast<T>());
            }

            if (result.Count == 0)
            {
                throw new NullReferenceException(
                    GameLog.GetTagText("Injection") + 
                    GameLog.GetColoredText(Color.red, typeof(T).Name) + " not found in DiContainer");
            }

            return result.ToArray();
        }

        internal HashSet<object> GetAllInstances()
        {
            var result = new HashSet<object>();
            foreach (var valueList in _map.Values)
            {
                foreach (var value in valueList)
                {
                    result.Add(value);
                }
            }
            foreach (var valueDict in _mapWithIds.Values)
            {
                foreach (var value in valueDict.Values)
                {
                    result.Add(value);
                }
            }
            return result;
        }

        [PublicAPI] public void Inject(object obj)
        {
            var type = obj.GetType();
            
            if(!_reflectionCache.ShouldTryToInject(type)) return;
            
            bool anyFieldsInjected = false;
            foreach (var fieldInfo in _reflectionCache.GetFieldInfos(type))
            {
                var attribute = _reflectionCache.GetInjectAttribute(fieldInfo);
                if (attribute != null)
                {
                    var fieldType = fieldInfo.FieldType;
                    var id = attribute.Id;

                    
                    if (fieldType.IsArray)
                    {
                        var elementType = fieldType.GetElementType();
                        var constructedMethod = typeof(DiContainer).GetMethod("GetAll")?.MakeGenericMethod(elementType);
                        object instances;
                        try
                        {
                            instances = constructedMethod?.Invoke(this, new object[] {});
                        }
                        catch
                        {
                            GameLog.LogError(
                                "Injection", 
                                "Injection error in " + GameLog.GetColoredText(new Color(1f, 165f/255f, 0f), type.Name));
                            throw;
                        }
                        fieldInfo.SetValue(obj, instances);
                    }
                    else
                    {
                        object instance;
                        try
                        {
                            instance = Get(fieldType, id);
                        }
                        catch (NullReferenceException)
                        {
                            GameLog.LogError(
                                "Injection",
                                "Injection error in " + GameLog.GetColoredText(new Color(1f, 165f/255f, 0f), type.Name));
                            throw;
                        }
                        fieldInfo.SetValue(obj, instance);
                    }

                    anyFieldsInjected = true;
                }
            }

            if (!anyFieldsInjected)
            {
                _reflectionCache.CacheNotInjectableType(type);
            }
        }

        internal void InjectAllMonoBehavioursOn(GameObject gameObject)
        {
            var monoBehaviours = new List<MonoBehaviour>();
            monoBehaviours.AddRange(gameObject.GetComponents<MonoBehaviour>());
            monoBehaviours.AddRange(gameObject.GetComponentsInChildren<MonoBehaviour>(true));
            
            foreach (var component in monoBehaviours)
            {
                Inject(component);
            }
        }

        [PublicAPI] public void RegisterInstance(object obj, string id = null)
        {
            var registry = new ContainerRegistry(obj, id);
            RegisterInstance(registry);
        }
        
        internal void RegisterInstance(ContainerRegistry containerRegistry)
        {
            AddObjectToMap(containerRegistry);
        
            foreach (var interfaceOfType in containerRegistry.Type.GetInterfaces())
            {
                AddObjectToMap(new ContainerRegistry(interfaceOfType, containerRegistry.Object, containerRegistry.Id));
            }
        }

        [PublicAPI] public T CreateFromNew<T>(string id = null)
        {
            var obj = Activator.CreateInstance<T>();
            var bindingInfo = new ContainerRegistry(obj, id);
            RegisterInstance(bindingInfo);
            Inject(obj);
            return obj;
        }

        [PublicAPI] public T CreateFromNewComponent<T>(GameObject root, string id = null) where T : Component
        {
            var component = root.AddComponent<T>();
            var bindingInfo = new ContainerRegistry(component, id);
            RegisterInstance(bindingInfo);
            Inject(component);
            return component;
        }
    
        private void AddObjectToMap(ContainerRegistry containerRegistry)
        {
            if (!containerRegistry.HasId())
            {
                if (!_map.ContainsKey(containerRegistry.Type))
                {
                    _map.Add(containerRegistry.Type, new List<object>());
                }
            
                _map[containerRegistry.Type].Add(containerRegistry.Object);
            }
            else
            {
                if (!_mapWithIds.ContainsKey(containerRegistry.Type))
                {
                    _mapWithIds.Add(containerRegistry.Type, new Dictionary<string, object>());
                }
            
                _mapWithIds[containerRegistry.Type].Add(containerRegistry.Id, containerRegistry.Object);
            }
        }

        [PublicAPI] public void UnregisterInstance(object obj, string id = null)
        {
            var registry = new ContainerRegistry(obj, id);
            UnregisterInstance(registry);
        }
        
        internal void UnregisterInstance(ContainerRegistry containerRegistry)
        {
            RemoveObjectFromMap(containerRegistry);
        
            foreach (var interfaceOfType in containerRegistry.Type.GetInterfaces())
            {
                RemoveObjectFromMap(new ContainerRegistry(interfaceOfType, containerRegistry.Object, containerRegistry.Id));
            }
        }
    
        private void RemoveObjectFromMap(ContainerRegistry containerRegistry)
        {
            if (!containerRegistry.HasId())
            {
                if (_map.TryGetValue(containerRegistry.Type, out var value))
                {
                    value.Remove(containerRegistry.Object);

                    if (value.Count == 0)
                    {
                        _map.Remove(containerRegistry.Type);
                    }
                }
            }
            else
            {
                if (_mapWithIds.TryGetValue(containerRegistry.Type, out var idDictionary))
                {
                    if (idDictionary.ContainsKey(containerRegistry.Id))
                    {
                        idDictionary.Remove(containerRegistry.Id);

                        if (idDictionary.Count == 0)
                        {
                            _map.Remove(containerRegistry.Type);
                        }
                    }
                }
            }
        }
    }
    
    internal class ContainerRegistry
    {
        public readonly Type Type;
        public readonly object Object;
        public readonly string Id;
        
        public ContainerRegistry(object o, string id = null)
        {
            Object = o;
            Id = id;
            Type = Object.GetType();
        }

        public ContainerRegistry(Type type, object o, string id = null)
        {
            Type = type;
            Object = o;
            Id = id;
        }

        public bool HasId()
        {
            return !string.IsNullOrEmpty(Id);
        }
    }
}
