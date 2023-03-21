/*
 * Source code copied from UnityEngine.UI.ObjectPool:
 * https://bitbucket.org/Unity-Technologies/ui/src/ccb946ecc23815d1a7099aee0ed77b0cde7ff278/UnityEngine.UI/UI/Core/Utility/ObjectPool.cs?at=5.1
 */

//#define OBJECTPOOL_DEBUG

using System;
using System.Collections.Generic;
using UnityEngine.Events;
#if OBJECTPOOL_DEBUG
using UnityEngine;
#endif

namespace HB.Utilities
{
    /// <exclude />
    public class ObjectPool<T> where T : class
    {
        public delegate T0 UnityFunc<T0>();

        private readonly Stack<T> _stack;
        private readonly UnityAction<T> _onGet;
        private readonly UnityAction<T> _onRelease;
        private readonly UnityFunc<T> _onNew;

        public string Name { get; set; }

        public int CountAll { get; private set; }

        public int CountActive => CountAll - CountInactive;

        public int CountInactive => _stack.Count;

        public ObjectPool(int capacity, UnityFunc<T> actionNew, UnityAction<T> actionOnGet = null,
            UnityAction<T> actionOnRelease = null, string name = null)
        {
            _stack = new Stack<T>(capacity);
            _onNew = actionNew ?? throw new ArgumentException("New action can't be null!");
            _onGet = actionOnGet;
            _onRelease = actionOnRelease;
            Name = name;
        }

        public void WarmUp(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T element = _onNew();
                CountAll++;
                _stack.Push(element);
            }
        }

        public T Get()
        {
#if OBJECTPOOL_DEBUG
            var created = false;
#endif
            T element;
            if (_stack.Count == 0)
            {
#if OBJECTPOOL_DEBUG
                created = true;
                logWarning("Created an object.");
#endif
                element = _onNew();
                CountAll++;
            }
            else
            {
                element = _stack.Pop();
            }

            if (_onGet != null) _onGet(element);
#if OBJECTPOOL_DEBUG
            log(string.Format("Getting object from pool. New: {0}, count: {1}, left: {2}", created, CountAll, stack.Count));
#endif
            return element;
        }

        public void Release(T element)
        {
#if OBJECTPOOL_DEBUG
            if (stack.Count > 0 && ReferenceEquals(stack.Peek(), element))
                logError("Internal error. Trying to destroy object that is already released to pool.");
#endif
            _onRelease?.Invoke(element);
            _stack.Push(element);
#if OBJECTPOOL_DEBUG
            log(string.Format("Returned object to pool. Left: {0}", stack.Count));
#endif
        }

        public void Release(object element)
        {
            T obj = (T)element;
            if (obj == null) return;
            Release(obj);
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < _stack.Count; i++)
            {
                var element = _stack.Pop();
                Release(element);
            }
        }

#if OBJECTPOOL_DEBUG
        private void log(string message)
        {
            if (string.IsNullOrEmpty(Info)) return;
            UnityEngine.Debug.LogFormat("[{0}] ObjectPool ({1}): {2}", DateTime.Now.ToString("hh:mm:ss.fff"), Info, message);
        }

        private void logWarning(string message)
        {
            if (string.IsNullOrEmpty(Info)) return;
            UnityEngine.Debug.LogWarningFormat("[{0}] ObjectPool ({1}): {2}", DateTime.Now.ToString("hh:mm:ss.fff"), Info, message);
        }

        private void logError(string message)
        {
            if (string.IsNullOrEmpty(Info)) return;
            UnityEngine.Debug.LogErrorFormat("[{0}] ObjectPool ({1}): {2}", DateTime.Now.ToString("hh:mm:ss.fff"), Info, message);
        }
#endif
    }
}