using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityFigmaMCP.Editor
{
    internal static class EditorMainThreadQueue
    {
        private static readonly Queue<Action> _queue = new();
        private static bool _initialized;

        public static void Enqueue(Action action)
        {
            lock (_queue)
                _queue.Enqueue(action);

            EnsureInitialized();
        }

        private static void EnsureInitialized()
        {
            if (_initialized)
                return;

            _initialized = true;
            EditorApplication.update += Flush;
        }

        private static void Flush()
        {
            const int maxPerTick = 64;
            
            for (var i = 0; i < maxPerTick; i++)
            {
                Action action;
                lock (_queue)
                {
                    if (_queue.Count == 0)
                        return;
                    
                    action = _queue.Dequeue();
                }

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
    }
}
