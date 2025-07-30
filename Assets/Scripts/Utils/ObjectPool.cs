using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sorter.Utils
{
    public class ObjectPool<T> : IDisposable
        where T : Component 
    {
        private readonly Queue<T> _pool = new Queue<T>();
        private readonly List<T> _activeObjects = new List<T>();

        private readonly Func<T> _createNewObject;
        private readonly Action<T> _onReturnAction;

        public ObjectPool(Func<T> createNewObject, Action<T> onReturnAction)
        {
            if (createNewObject is null || onReturnAction is null)
                throw new AggregateException("Invalid arguments for ObjectPool");
            
            _createNewObject = createNewObject;
            _onReturnAction = onReturnAction;
        }

        public void Cache(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Return(_createNewObject());
            }
        }

        public T Get()
        {
            var item = _pool.Count > 0 ? _pool.Dequeue() : _createNewObject();
            _activeObjects.Add(item);

            return item;
        }

        public void TryReturn(T item)
        {
            if (_activeObjects.Contains(item) is false)
            {
                Debug.LogError($"Trying to return inactive object", item);
                return;
            }

            Return(item);
        }

        private void Return(T item)
        {
            _onReturnAction(item);
            _pool.Enqueue(item);
            _activeObjects.Remove(item);
        }

        public void ReturnAll()
        {
            ForEachActive(item =>
            {
                _onReturnAction(item);
                _pool.Enqueue(item);
            });
            _activeObjects.Clear();
        }
        
        public void ForEachActive(Action<T> action)
        {
            foreach (T item in _activeObjects)
            {
                action?.Invoke(item);
            }
        }

        public void Dispose()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Dequeue();
                GameObject.Destroy(item);
            }
            _activeObjects.ForEach(GameObject.Destroy);
            _activeObjects.Clear();
        }
    }
}