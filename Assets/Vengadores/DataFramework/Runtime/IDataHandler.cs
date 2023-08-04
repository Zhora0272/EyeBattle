using System;

namespace Vengadores.DataFramework
{
    public interface IDataHandler
    {
        void Load(BaseData data, Action onComplete);

        void Save(BaseData data, Action onComplete);
    }
}