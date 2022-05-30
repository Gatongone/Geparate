using System;
namespace Geparate.Pool
{
    public interface IPool<T>
    {
        /// <summary>
        /// Create or get a object from pool
        /// </summary>
        T Spawn();
        /// <summary>
        /// Create or get a object from pool in an asynchronous way
        /// </summary>
        void SpawnAsync(Action<T> callback);
        /// <summary>
        /// Push a object into pool
        /// </summary>
        void Despawn(T target);
        /// <summary>
        /// Create some object when pool initializes
        /// </summary>
        void Prewarm(int count);
        /// <summary>
        /// Clear pool
        /// </summary>
        void Clear();
    }
}
