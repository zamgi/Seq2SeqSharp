﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TensorSharp.Core;

namespace TensorSharp
{
    [Serializable]
    public class Tensor : IDisposable
    {
        private long[] sizes;
        private long[] strides;
        private readonly Storage storage;
        private readonly long storageOffset;

        private bool isDisposed = false;


        /// <summary>
        /// Construct a new tensor, using the given allocator to construct a storage. The new tensor
        /// will be contiguous in memory. The tensor's elements will not be initialized.
        /// </summary>
        /// <param name="allocator"></param>
        /// <param name="elementType"></param>
        /// <param name="sizes"></param>
        public Tensor(IAllocator allocator, DType elementType, params long[] sizes)
            : this(allocator, elementType, sizes, TensorDimensionHelpers.GetContiguousStride(sizes))
        {
        }

        public Tensor(IAllocator allocator, DType elementType, long[] sizes, long[] strides)
        {
            this.sizes = sizes;
            this.strides = strides;
            storageOffset = 0;
            storage = allocator.Allocate(elementType, TensorDimensionHelpers.GetStorageSize(sizes, strides));
        }

        public Tensor(long[] sizes, long[] strides, Storage storage, long storageOffset)
        {
            this.sizes = sizes;
            this.strides = strides;
            this.storage = storage;
            this.storageOffset = storageOffset;

            this.storage.AddRef();
        }

        //~Tensor()
        //{
        //    if (!isDisposed)
        //    {
        //        Dispose();
        //    }
        //}

        public override string ToString()
        {
            return TensorFormatting.FormatTensorTypeAndSize(this);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                storage.Release();
            }
            else
            {
                throw new ObjectDisposedException("Tensor");
            }
        }

        public override bool Equals(object obj)
        {
            Tensor o = obj as Tensor;
            if (o == null)
            {
                return false;
            }

            return
                Object.ReferenceEquals(storage, o.storage) &&
                storageOffset == o.storageOffset &&
                TensorResultBuilder.ArrayEqual(sizes, o.sizes) &&
                TensorResultBuilder.ArrayEqual(strides, o.strides);
        }

        public override int GetHashCode()
        {
            return
                storage.GetHashCode() ^
                storageOffset.GetHashCode() ^
                sizes.Aggregate(0, (acc, item) => acc ^ item.GetHashCode()) ^
                strides.Aggregate(0, (acc, item) => acc ^ item.GetHashCode());
        }

        public DType ElementType => storage.ElementType;
        public long[] Sizes => sizes;
        public long[] Strides => strides;
        public Storage Storage => storage;
        public long StorageOffset => storageOffset;
        public IAllocator Allocator => storage.Allocator;

        public int DimensionCount => sizes.Length;



        public long GetStorageSize()
        {
            return TensorDimensionHelpers.GetStorageSize(sizes, strides);
        }

        /// <summary>
        /// Returns a new Tensor object which points to the same storage as this,
        /// incrementing the refcount of the storage object.
        /// </summary>
        public Tensor CopyRef()
        {
            return new Tensor(sizes, strides, storage, storageOffset);
        }

        public bool IsOwnerExclusive()
        {
            return storage.IsOwnerExclusive();
        }

        public string Format()
        {
            return TensorFormatting.Format(this);
        }

        private long? elementCount = null;
        public long ElementCount()
        {
            if (elementCount.HasValue)
            {
                return elementCount.Value;
            }

            elementCount = TensorDimensionHelpers.ElementCount(sizes);
            return elementCount.Value;
        }

        public bool IsContiguous()
        {
            long z = 1;
            for (int d = sizes.Length - 1; d >= 0; d--)
            {
                if (sizes[d] != 1)
                {
                    if (strides[d] == z)
                    {
                        z *= sizes[d];
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        public bool IsSameSizeAs(Tensor other)
        {
            return Core.TensorResultBuilder.ArrayEqual(sizes, other.sizes);
        }

        /// <summary>
        /// Note: this does not check whether indices are in range
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public float GetElementAsFloat(params long[] indices)
        {
            if (indices.Length != DimensionCount)
            {
                throw new ArgumentException($"Number of indices must equal number of tensor dimensions. Tensor dim = '{DimensionCount}' and input indices length is '{indices.Length}'");
            }

            for (int i = 0; i < indices.Length; ++i)
            {
                if (indices[i] < 0 || indices[i] >= Sizes[i])
                {
                    throw new ArgumentException("Index " + i + " with value " + indices[i] + " is out of range");
                }
            }

            long offset = 0;
            for (int i = 0; i < indices.Length; ++i)
            {
                offset += indices[i] * strides[i];
            }

            return storage.GetElementAsFloat(storageOffset + offset);
        }

        public float[] GetElementsAsFloat(int length)
        {
            return storage.GetElementsAsFloat(storageOffset, length);
        }

        /// <summary>
        /// Note: this does not check whether indices are in range
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        public void SetElementAsFloat(float value, params long[] indices)
        {
            if (indices.Length != DimensionCount)
            {
                throw new ArgumentException("Number of indices must equal number of tensor dimensions");
            }

            for (int i = 0; i < indices.Length; ++i)
            {
                if (indices[i] < 0 || indices[i] >= Sizes[i])
                {
                    throw new ArgumentException("Index " + i + " with value " + indices[i] + " is out of range");
                }
            }

            long offset = 0;
            for (int i = 0; i < indices.Length; ++i)
            {
                offset += indices[i] * strides[i];
            }

            storage.SetElementAsFloat(storageOffset + offset, value);
        }

        public void SetElementsAsFloat(float[] value)
        {
            storage.SetElementsAsFloat(storageOffset, value);
        }

        public void SetElementsAsFloat(float[] value, params long[] indices)
        {
            if (indices.Length != DimensionCount)
            {
                throw new ArgumentException("Number of indices must equal number of tensor dimensions");
            }

            for (int i = 0; i < indices.Length; ++i)
            {
                if (indices[i] < 0 || indices[i] >= Sizes[i])
                {
                    throw new ArgumentException("Index " + i + " with value " + indices[i] + " is out of range");
                }
            }

            long offset = 0;
            for (int i = 0; i < indices.Length; ++i)
            {
                offset += indices[i] * strides[i];
            }

            storage.SetElementsAsFloat(storageOffset + offset, value);
        }


        private string PrintSizes(long[] sizes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (long size in sizes)
            {
                sb.Append(size);
                sb.Append(" ");
            }

            return sb.ToString();
        }

        public int[] GetElementsAsInt(int length)
        {
            return storage.GetElementsAsInt(storageOffset, length);
        }

        public void SetElementsAsInt(int[] value)
        {
            storage.SetElementsAsInt(storageOffset, value);
        }

        public Tensor View(params long[] sizes)
        {
            if (!IsContiguous())
            {
                throw new InvalidOperationException("Cannot use View on a non-contiguous tensor");
            }

            if (ElementCount() != TensorDimensionHelpers.ElementCount(sizes))
            {
                throw new InvalidOperationException($"Output tensor must have the same number of elements as the input. Size = {PrintSizes(this.sizes)}, New Size = {PrintSizes(sizes)}");
            }

            return new Tensor(sizes, TensorDimensionHelpers.GetContiguousStride(sizes), storage, storageOffset);
        }

        public Tensor Narrow(int dimension, long startIndex, long size)
        {
            if (dimension < 0 || dimension >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension");
            }

            if (startIndex < 0 || startIndex >= sizes[dimension])
            {
                throw new ArgumentOutOfRangeException("startIndex", $"startIndex = '{startIndex}', sizes[dimension] = '{sizes[dimension]}', dimension = '{dimension}', size = '{size}'");
            }

            if (size <= 0 || startIndex + size > sizes[dimension])
            {
                throw new ArgumentOutOfRangeException("size", $"startIndex = '{startIndex}', sizes[dimension] = '{sizes[dimension]}', dimension = '{dimension}', size = '{size}'");
            }

            long newOffset = storageOffset + startIndex * strides[dimension];
            long[] newSizes = (long[])sizes.Clone();
            newSizes[dimension] = size;

            return new Tensor(newSizes, strides, storage, newOffset);
        }

        public Tensor Select(int dimension, long index)
        {
            if (DimensionCount == 1)
            {
                throw new InvalidOperationException("Select requires 2 or more dimensions");
            }

            if (dimension < 0 || dimension >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension");
            }

            if (index < 0 || index >= sizes[dimension])
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Tensor result = Narrow(dimension, index, 1);
            result.sizes = ArrayRemove(sizes, dimension);
            result.strides = ArrayRemove(strides, dimension);

            return result;
        }


        public Tensor Transpose()
        {
            if (DimensionCount != 2)
            {
                throw new InvalidOperationException("Parameterless Transpose is only valid on 2d tensors");
            }

            return Transpose(0, 1);
        }

        public Tensor Transpose(int dimension1, int dimension2)
        {
            if (dimension1 < 0 || dimension1 >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension1");
            }

            if (dimension2 < 0 || dimension2 >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension2");
            }

            //if (dimension1 == dimension2)
            //{
            //    storage.AddRef();
            //    return this;
            //}

            long[] newSizes = (long[])sizes.Clone();
            long[] newStrides = (long[])strides.Clone();
            ArraySwap(newSizes, dimension1, dimension2);
            ArraySwap(newStrides, dimension1, dimension2);
            return new Tensor(newSizes, newStrides, storage, storageOffset);
        }

        public Tensor Permute(params int[] dims)
        {
            if (dims.Length != DimensionCount)
            {
                throw new InvalidOperationException("The number of permutation indices must equal the number of tensor dimensions");
            }

            Tensor result = CopyRef();
            foreach (Tuple<int, int> swap in SwapsForPermutation(dims))
            {
                Tensor resultOld = result;
                result = result.Transpose(swap.Item1, swap.Item2);
                resultOld.Dispose();
            }

            return result;
        }

        /// <summary>
        /// Expand one or more singleton dimensions (dimensions with size 1) by using a stride of 0
        /// </summary>
        /// <param name="tensor"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public Tensor Expand(params long[] newSizes)
        {
            if (newSizes.Length != DimensionCount)
            {
                throw new InvalidOperationException($"number of elements of newSizes must match the dimension count of tensor. New dimension = '{newSizes.Length}', Current dimension = '{DimensionCount}', New tensor shape = '{string.Join(" ", newSizes)}', current tensor shape = '{string.Join(" ", Sizes)}'");
            }

            long[] newStrides = (long[])strides.Clone();
            for (int i = 0; i < newSizes.Length; ++i)
            {
                if (newSizes[i] != Sizes[i])
                {
                    if (Sizes[i] != 1)
                    {
                        throw new InvalidOperationException("Can only expand singleton dimensions (dimensions of size 1)");
                    }

                    newStrides[i] = 0;
                }
            }

            return new Tensor(newSizes, newStrides, storage, storageOffset);
        }


        /// <summary>
        /// Return a new tensor where **all** singleton dimensions have been removed
        /// </summary>
        /// <param name="tensor"></param>
        /// <returns></returns>
        public Tensor Squeeze()
        {
            Tuple<long, long>[] newSizeStrides = sizes.Zip(strides, Tuple.Create)
                .Where(x => x.Item1 != 1)
                .ToArray();

            long[] newSizes = newSizeStrides.Select(x => x.Item1).ToArray();
            long[] newStrides = newSizeStrides.Select(x => x.Item2).ToArray();

            return new Tensor(newSizes, newStrides, storage, storageOffset);
        }


        /// <summary>
        /// Return a new tensor where the given singleton dimension has been removed
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        public Tensor Squeeze(int dimension)
        {
            if (DimensionCount == 1)
            {
                throw new InvalidOperationException("Squeeze requires 2 or more dimensions");
            }

            if (dimension < 0 || dimension >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension");
            }

            long[] newSizes = ArrayRemove(sizes, dimension);
            long[] newStrides = ArrayRemove(strides, dimension);

            return new Tensor(newSizes, newStrides, storage, storageOffset);
        }




        /// <summary>
        /// Returns a tensor which contains all slices of size size in the given dimension. The step between two slices is given by step.
        /// The result tensor has an additional dimension of size size.
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="size"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Tensor Unfold(int dimension, long size, long step)
        {
            if (DimensionCount == 0)
            {
                throw new InvalidOperationException("Cannot unfold an empty tensor");
            }

            if (dimension < 0 || dimension >= DimensionCount)
            {
                throw new ArgumentOutOfRangeException("dimension is out of range", "dimension");
            }

            if (size > sizes[dimension])
            {
                throw new ArgumentOutOfRangeException("size cannot be larger than the size of dimension", "size");
            }

            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("step must be at least 1", "step");
            }

            long[] newSize = new long[DimensionCount + 1];
            long[] newStrides = new long[DimensionCount + 1];
            Array.Copy(sizes, newSize, DimensionCount);
            Array.Copy(strides, newStrides, DimensionCount);

            newSize[DimensionCount] = size;
            newStrides[DimensionCount] = strides[dimension];

            newSize[dimension] = (sizes[dimension] - size) / step + 1;
            newStrides[dimension] = step * strides[dimension];

            return new Tensor(newSize, newStrides, Storage, StorageOffset);
        }


        // Pad array by prepending with 1 until its length equals newSize
        private static long[] Pad1Prepend(long[] array, int newSize)
        {
            long[] result = new long[newSize];

            // Fill new extra elements with 1
            for (int i = 0; i < newSize - array.Length; ++i)
            {
                result[i] = 1;
            }

            // Copy array to the last array.Length elements of result
            Array.Copy(array, 0, result, newSize - array.Length, array.Length);

            return result;
        }

        // Prepend singleton dimensions until DimensionCount equals newDimCount
        private Tensor PadToDimCount(int newDimCount)
        {
            long[] newSizes = Pad1Prepend(sizes, newDimCount);

            long[] newStrides = TensorDimensionHelpers.GetContiguousStride(newSizes);
            Array.Copy(strides, 0, newStrides, newStrides.Length - strides.Length, strides.Length);

            return new Tensor(newSizes, newStrides, storage, storageOffset);
        }

        public Tensor RepeatTensor(params long[] repetitions)
        {
            if (repetitions.Length < DimensionCount)
            {
                throw new InvalidOperationException("repetitions must be at least the same length as the number of tensor dimensions");
            }

            if (repetitions.Any(x => x < 1))
            {
                throw new InvalidOperationException("All dimensions must be repeated at least once");
            }

            Tensor paddedSrc = PadToDimCount(repetitions.Length);
            long[] resultSize = paddedSrc.Sizes.Zip(repetitions, (s, r) => s * r).ToArray();

            Tensor result = new Tensor(Allocator, ElementType, resultSize);

            Tensor urTensor = result.CopyRef();
            for (int i = 0; i < paddedSrc.DimensionCount; ++i)
            {
                Tensor oldUrTensor = urTensor;
                urTensor = urTensor.Unfold(i, paddedSrc.Sizes[i], paddedSrc.Sizes[i]);
                oldUrTensor.Dispose();
            }

            Tensor paddedSrc2 = paddedSrc.PadToDimCount(urTensor.DimensionCount);
            Tensor expandedSrc = paddedSrc2.Expand(urTensor.Sizes);
            Ops.Copy(urTensor, expandedSrc);

            paddedSrc.Dispose();
            paddedSrc2.Dispose();
            urTensor.Dispose();
            expandedSrc.Dispose();

            /*
            var sizesWritten = (long[])this.sizes.Clone();
            using (var subResult = result.GetRegion(Enumerable.Repeat((long)0, DimensionCount).ToArray(), sizesWritten))
            {
                Ops.Copy(subResult, this);
            }

            for (int i = 0; i < repetitions.Length; ++i)
            {
                if (repetitions[i] == 1) continue;

                sizesWritten[i] *= repetitions[i];
                using (var subResultSrc = result.GetRegion(Enumerable.Repeat((long)0, DimensionCount).ToArray(), this.sizes))
                using (var subResultTgt = result.GetRegion(Enumerable.Repeat((long)0, DimensionCount).ToArray(), this.sizes))
                {
                    Ops.Copy(subResultTgt, subResultSrc);
                }
            }*/

            return result;
        }

        /*
            private Tensor GetRegion(long[] dimensionStarts, long[] dimensionSizes)
        {
            var result = this.CopyRef();
            for (int i = 0; i < dimensionStarts.Length; ++i)
            {
                var resultOld = result;
                result.Narrow(i, dimensionStarts[i], dimensionSizes[i]);
                resultOld.Dispose();
            }
            return result;
        }*/


        public void CopyFrom(Array array)
        {
            DType elementType = DTypeBuilder.FromCLRType(array.GetType().GetElementType());

            if (!IsContiguous())
            {
                throw new InvalidOperationException("Tensor must be contiguous to copy from CLR array");
            }

            if (ElementCount() != array.LongLength)
            {
                throw new InvalidOperationException("Tensor and array must have the same number of elements");
            }

            if (ElementType != elementType)
            {
                throw new InvalidOperationException("Tensor and array must have the same element types");
            }

            GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            try
            {
                int length = Buffer.ByteLength(array);
                Storage.CopyToStorage(StorageOffset, handle.AddrOfPinnedObject(), length);
            }
            finally
            {
                handle.Free();
            }
        }


        public void CopyToArray(Array array)
        {
            DType elementType = DTypeBuilder.FromCLRType(array.GetType().GetElementType());

            if (!IsContiguous())
            {
                throw new InvalidOperationException("Tensor must be contiguous to copy from CLR array");
            }

            if (ElementCount() != array.LongLength)
            {
                throw new InvalidOperationException("Tensor and array must have the same number of elements");
            }

            if (ElementType != elementType)
            {
                throw new InvalidOperationException("Tensor and array must have the same element types");
            }

            GCHandle handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            try
            {
                int length = Buffer.ByteLength(array);
                Storage.CopyFromStorage(handle.AddrOfPinnedObject(), StorageOffset, length);
            }
            finally
            {
                handle.Free();
            }
        }


        public static Tensor FromArray(IAllocator allocator, Array array)
        {
            // From the CLI spec(section 8.9.1):
            // Array elements shall be laid out within the array object in row - major order
            // (i.e., the elements associated with the rightmost array dimension shall be laid out contiguously from lowest to highest index).
            // The actual storage allocated for each array element can include platform - specific padding.

            // This is already in the order we want - and here we will (potentially incorrectly) assume that there is no
            // 'platform-specific padding'. This appears to be a reasonable assumption on both CLR and Mono.
            // Assuming no platform-specific padding allows us to use memcpy instead of iterating and copying each element

            DType elementType = DTypeBuilder.FromCLRType(array.GetType().GetElementType());

            long[] dimSizes =
                Enumerable.Range(0, array.Rank)
                .Select(x => (long)array.GetLength(x))
                .ToArray();

            Tensor result = new Tensor(allocator, elementType, dimSizes);
            result.CopyFrom(array);
            return result;
        }


        private static void ArraySwap<T>(T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }

        // Return a copy of an array, but with the item at index removed
        private static T[] ArrayRemove<T>(T[] source, long index)
        {
            T[] result = new T[source.Length - 1];
            for (int i = 0; i < result.Length; ++i)
            {
                if (i < index)
                {
                    result[i] = source[i];
                }
                else
                {
                    result[i] = source[i + 1];
                }
            }
            return result;
        }

        // Convert a permutation into a sequence of swap operations.
        // perm must contain a permuation of the indices [0, perm.Length)
        // The returned tuples indicate pairs of indices that should be swapped. The swaps
        // must be performed in the given order.
        private static IEnumerable<Tuple<int, int>> SwapsForPermutation(int[] perm)
        {
            int j;
            for (int i = 0; i < perm.Length; ++i)
            {
                int p = perm[i];
                if (p != i && p != -1)
                {
                    j = i;
                    do
                    {
                        if (perm[j] < 0 || perm[j] >= perm.Length)
                        {
                            throw new InvalidOperationException("Invalid permutation");
                        }

                        yield return Tuple.Create(j, perm[j]);


                        int jOld = j;
                        j = perm[j];
                        perm[jOld] = -1;
                    } while (perm[j] != i);
                    perm[j] = j;
                }
            }
        }


        public void Serialize(System.IO.Stream stream)
        {
            TensorSerialization.Serialize(this, stream);
        }

        public static Tensor Deserialize(IAllocator allocator, System.IO.Stream stream)
        {
            return TensorSerialization.Deserialize(allocator, stream);
        }
    }
}
