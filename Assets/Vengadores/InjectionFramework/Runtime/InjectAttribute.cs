using System;
using JetBrains.Annotations;

namespace Vengadores.InjectionFramework
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field)]  
    public class InjectAttribute : Attribute  
    {
        public string Id { get; private set; }

        public InjectAttribute() { }

        public InjectAttribute(string id)
        {
            Id = id;
        }  
    }
}  
