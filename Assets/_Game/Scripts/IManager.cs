using UniRx;

namespace Shop
{
    public interface IManager<T,D>
    {
        public IReactiveProperty<D> CallBack { get; }
    }
}