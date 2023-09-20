using System;
using UniRx;

namespace Shop
{
    public interface IManager<T,D>
    {
        public ReactiveProperty<D> CallBack { get; set; }
    }
}