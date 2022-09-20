namespace MaJiCSoft.ChatOverlay
{
    using System;

    public interface ITypedFactory<T>
    {
        T Create();
    }

    public class TypedFactory<T> : ITypedFactory<T>
    {
        private readonly Func<T> initFunc;
        public TypedFactory(Func<T> initFunc)
        {
            this.initFunc = initFunc;
        }

        public T Create()
        {
            return initFunc();
        }
    }
}
