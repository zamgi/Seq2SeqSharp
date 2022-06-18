﻿// Copyright (c) Zhongkai Fu. All rights reserved.
// https://github.com/zhongkaifu/Seq2SeqSharp
//
// This file is part of Seq2SeqSharp.
//
// Seq2SeqSharp is licensed under the BSD-3-Clause license found in the LICENSE file in the root directory of this source tree.
//
// Seq2SeqSharp is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the BSD-3-Clause License for more details.

using System;
using System.Collections.Generic;

namespace TensorSharp.CUDA.Util
{
    public class PooledObject<T> : IDisposable
    {
        private readonly Action<PooledObject<T>> onDispose;
        private readonly T value;

        private bool disposed = false;

        public PooledObject(T value, Action<PooledObject<T>> onDispose)
        {
            if (onDispose == null)
            {
                throw new ArgumentNullException("onDispose");
            }

            this.onDispose = onDispose;
            this.value = value;
        }

        public T Value
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(ToString());
                }

                return value;
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                onDispose(this);
                disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }
    }

    public class ObjectPool<T> : IDisposable
    {
        private readonly Func<T> constructor;
        private readonly Action<T> destructor;
        private readonly Stack<T> freeList = new Stack<T>();
        private bool disposed = false;
        private static object locker = new object();

        public ObjectPool(int initialSize, Func<T> constructor, Action<T> destructor)
        {
            if (constructor == null)
            {
                throw new ArgumentNullException("constructor");
            }

            if (destructor == null)
            {
                throw new ArgumentNullException("destructor");
            }

            this.constructor = constructor;
            this.destructor = destructor;

            lock (locker)
            {
                for (int i = 0; i < initialSize; ++i)
                {
                    freeList.Push(constructor());
                }
            }
        }

        public void Dispose()
        {
            lock (locker)
            {
                if (!disposed)
                {
                    disposed = true;
                    foreach (T item in freeList)
                    {
                        destructor(item);
                    }
                    freeList.Clear();
                }
            }
        }

        public PooledObject<T> Get()
        {
            lock (locker)
            {
                T value = freeList.Count > 0 ? freeList.Pop() : constructor();
                return new PooledObject<T>(value, Release);
            }
        }

        private void Release(PooledObject<T> handle)
        {
            lock (locker)
            {
                freeList.Push(handle.Value);
            }
        }
    }
}
