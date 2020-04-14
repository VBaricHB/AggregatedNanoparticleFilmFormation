using ANPaX.AggregateFormation.interfaces;
using NLog;
using System;

namespace ANPaX.AggregateFormation.interfaces
{
    public interface IParticleFactory<T>
    {
        public T Build(int size);
    }
}