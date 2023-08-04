using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Vengadores.InjectionFramework
{
    /**
     * Lookup cache for expensive reflection operations. 
     */
    public class ReflectionCache
    {
        private Dictionary<FieldInfo, InjectAttribute> _attributeCache;
        private Dictionary<Type, FieldInfo[]> _fieldInfoCache;
        private HashSet<Type> _notInjectableTypes;
        
        public ReflectionCache()
        {
            _attributeCache = new Dictionary<FieldInfo, InjectAttribute>();
            _fieldInfoCache = new Dictionary<Type, FieldInfo[]>();
            _notInjectableTypes = new HashSet<Type>();
        }

        public FieldInfo[] GetFieldInfos(Type type)
        {
            FieldInfo[] fieldInfos;
            if (_fieldInfoCache.TryGetValue(type, out var cacheFieldInfos))
            {
                fieldInfos = cacheFieldInfos;
            }
            else
            {
                // Iterate self and base classes
                var infos = new List<FieldInfo>();
                var pivotType = type;
                while (pivotType != null && pivotType != typeof(MonoBehaviour))
                {
                    infos.AddRange(pivotType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                    pivotType = pivotType.BaseType;
                }
                fieldInfos = infos.ToArray();
                _fieldInfoCache.Add(type, fieldInfos);
            }

            return fieldInfos;
        }

        public InjectAttribute GetInjectAttribute(FieldInfo fieldInfo)
        {
            InjectAttribute attribute;
            if (_attributeCache.TryGetValue(fieldInfo, out var cachedAttribute))
            {
                attribute = cachedAttribute;
            }
            else
            {
                attribute = fieldInfo.GetCustomAttribute<InjectAttribute>();
                _attributeCache.Add(fieldInfo, attribute);
            }

            return attribute;
        }

        public bool ShouldTryToInject(Type type)
        {
            return !_notInjectableTypes.Contains(type);
        }

        public void CacheNotInjectableType(Type type)
        {
            _notInjectableTypes.Add(type);
        }
    }
}