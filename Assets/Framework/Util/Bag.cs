#region File description
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bag.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion File description

using System;

namespace Assets.Framework.Util
{
    public class Bag<T>
    {
        private T[] contents;

        public Bag(int capacity = 16)
        {
            contents = new T[capacity];
            Count = 0;
        }

        public int Capacity
        {
            get { return contents.Length; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public int Count { get; private set; }

        public T this[int index]
        {
            get { return Get(index); }

            set
            {
                if (index >= contents.Length)
                {
                    Grow(index*2);
                    Count = index + 1;
                }
                else if (index >= Count)
                {
                    Count = index + 1;
                }

                contents[index] = value;
            }
        }

        public void Add(T element)
        {
            if (Count == contents.Length)
            {
                Grow();
            }

            contents[Count] = element;
            ++Count;
        }

        public void AddRange(Bag<T> rangeOfElements)
        {
            for (int index = 0, count = rangeOfElements.Count; count > index; ++index)
            {
                Add(rangeOfElements.Get(index));
            }
        }

        public void Clear()
        {
            for (var index = Count - 1; index >= 0; --index)
            {
                contents[index] = default(T);
            }

            Count = 0;
        }

        public bool Contains(T element)
        {
            for (var index = Count - 1; index >= 0; --index)
            {
                if (element.Equals(contents[index]))
                {
                    return true;
                }
            }

            return false;
        }

        public T Get(int index)
        {
            //Not confident this is the right solution. 
            if (index >= contents.Length)
            {
                Grow(index*2);
            }

            return contents[index];
        }

        public T Remove(int index)
        {
            var result = contents[index];
            --Count;

            contents[index] = contents[Count];

            contents[Count] = default(T);
            return result;
        }

        public bool Remove(T element)
        {
            for (var index = Count - 1; index >= 0; --index)
            {
                if (element.Equals(contents[index]))
                {
                    --Count;

                    contents[index] = contents[Count];
                    contents[Count] = default(T);

                    return true;
                }
            }

            return false;
        }

        public bool RemoveAll(Bag<T> bag)
        {
            var isResult = false;
            for (var index = bag.Count - 1; index >= 0; --index)
            {
                if (Remove(bag.Get(index)))
                {
                    isResult = true;
                }
            }

            return isResult;
        }

        public T RemoveLast()
        {
            if (Count > 0)
            {
                --Count;
                var result = contents[Count];

                // default(T) if class = null.
                contents[Count] = default(T);
                return result;
            }

            return default(T);
        }

        public void Set(int index, T element)
        {
            if (index >= contents.Length)
            {
                Grow(index*2);
                Count = index + 1;
            }
            else if (index >= Count)
            {
                Count = index + 1;
            }

            contents[index] = element;
        }

        private void Grow()
        {
            Grow((int) (contents.Length*1.5) + 1);
        }

        private void Grow(int newCapacity)
        {
            var oldElements = contents;
            contents = new T[newCapacity];
            Array.Copy(oldElements, 0, contents, 0, oldElements.Length);
        }
    }
}