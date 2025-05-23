﻿// Copyright (c) Zhongkai Fu. All rights reserved.
// https://github.com/zhongkaifu/Seq2SeqSharp
//
// This file is part of Seq2SeqSharp.
//
// Seq2SeqSharp is licensed under the BSD-3-Clause license found in the LICENSE file in the root directory of this source tree.
//
// Seq2SeqSharp is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the BSD-3-Clause License for more details.

using ManagedCuda.BasicTypes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml;
using TensorSharp.Cpu;

namespace TensorSharp
{
	public class TensorApplyCPU
	{

        #region Tensor iteration methods
        unsafe public delegate void Apply1KernelFunction(float* x);
		unsafe public delegate void Apply2KernelFunction(float* x, float* y);
		unsafe public delegate void Apply3KernelFunction(float* x, float* y, float* z);
		unsafe public delegate void Apply4KernelFunction(float* x, float* y, float* z, float* k);
		unsafe public delegate void Apply5KernelFunction(float* x, float* y, float* z, float* k, float* l);
		unsafe public delegate void ApplyDim2KernelFuncton(float* x, long sizeX, long stridesX, float* y, long sizeY, long stridesY);
		unsafe public delegate void ApplyDim3KernelFuncton(float* x, long sizeX, long stridesX, float* y, long sizeY, long stridesY, float* z, long sizeZ, long stridesZ);

		unsafe static void Apply1(Tensor tensor1, Apply1KernelFunction func)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);

			TensorIterState tensor1Iter = new TensorIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides);

			do
			{
				for (; !tensor1Iter.ReachedBlockEnd(); tensor1Iter.BlockStep())
				{
					func(tensor1Iter.data);
				}

			} while (tensor1Iter.NextBlock());
		}


		unsafe static void Apply2(Tensor tensor1, Tensor tensor2, Apply2KernelFunction func, int step = 1)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);

			TensorIterState tensor1Iter = new TensorIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides, step);
			TensorIterState tensor2Iter = new TensorIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides, step);

			do
			{
				for (; !tensor1Iter.ReachedBlockEnd() && !tensor2Iter.ReachedBlockEnd(); tensor1Iter.BlockStep(), tensor2Iter.BlockStep())
				{
					func(tensor1Iter.data, tensor2Iter.data);
				}

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock());
		}


		unsafe static void Apply3(Tensor tensor1, Tensor tensor2, Tensor tensor3, Apply3KernelFunction func, int step = 1)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);
			float* buffer3 = (float*)CpuNativeHelpers.GetBufferStart(tensor3);

			TensorIterState tensor1Iter = new TensorIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides, step);
			TensorIterState tensor2Iter = new TensorIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides, step);
			TensorIterState tensor3Iter = new TensorIterState(buffer3, tensor3.DimensionCount, tensor3.Sizes, tensor3.Strides, step);

			do
			{
				for (; !tensor1Iter.ReachedBlockEnd() && !tensor2Iter.ReachedBlockEnd() && !tensor3Iter.ReachedBlockEnd();
						tensor1Iter.BlockStep(), tensor2Iter.BlockStep(), tensor3Iter.BlockStep())
				{
					func(tensor1Iter.data, tensor2Iter.data, tensor3Iter.data);
				}

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock() && tensor3Iter.NextBlock());
		}


		unsafe static void Apply4(Tensor tensor1, Tensor tensor2, Tensor tensor3, Tensor tensor4, Apply4KernelFunction func)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);
			float* buffer3 = (float*)CpuNativeHelpers.GetBufferStart(tensor3);
			float* buffer4 = (float*)CpuNativeHelpers.GetBufferStart(tensor4);

			TensorIterState tensor1Iter = new TensorIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides);
			TensorIterState tensor2Iter = new TensorIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides);
			TensorIterState tensor3Iter = new TensorIterState(buffer3, tensor3.DimensionCount, tensor3.Sizes, tensor3.Strides);
			TensorIterState tensor4Iter = new TensorIterState(buffer4, tensor4.DimensionCount, tensor4.Sizes, tensor4.Strides);

			do
			{
				for (; !tensor1Iter.ReachedBlockEnd() && !tensor2Iter.ReachedBlockEnd() && !tensor3Iter.ReachedBlockEnd() && !tensor4Iter.ReachedBlockEnd();
					tensor1Iter.BlockStep(), tensor2Iter.BlockStep(), tensor3Iter.BlockStep(), tensor4Iter.BlockStep())
				{
					func(tensor1Iter.data, tensor2Iter.data, tensor3Iter.data, tensor4Iter.data);
				}

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock() && tensor3Iter.NextBlock() && tensor4Iter.NextBlock());
		}


		unsafe static void Apply5(Tensor tensor1, Tensor tensor2, Tensor tensor3, Tensor tensor4, Tensor tensor5, Apply5KernelFunction func, int step = 1)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);
			float* buffer3 = (float*)CpuNativeHelpers.GetBufferStart(tensor3);
			float* buffer4 = (float*)CpuNativeHelpers.GetBufferStart(tensor4);
			float* buffer5 = (float*)CpuNativeHelpers.GetBufferStart(tensor5);


			TensorIterState tensor1Iter = new TensorIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides, step);
			TensorIterState tensor2Iter = new TensorIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides, step);
			TensorIterState tensor3Iter = new TensorIterState(buffer3, tensor3.DimensionCount, tensor3.Sizes, tensor3.Strides, step);
			TensorIterState tensor4Iter = new TensorIterState(buffer4, tensor4.DimensionCount, tensor4.Sizes, tensor4.Strides, step);
			TensorIterState tensor5Iter = new TensorIterState(buffer5, tensor5.DimensionCount, tensor5.Sizes, tensor5.Strides, step);

			do
			{
				for (; !tensor1Iter.ReachedBlockEnd() && !tensor2Iter.ReachedBlockEnd() && !tensor3Iter.ReachedBlockEnd() && !tensor4Iter.ReachedBlockEnd() && !tensor5Iter.ReachedBlockEnd();
					tensor1Iter.BlockStep(), tensor2Iter.BlockStep(), tensor3Iter.BlockStep(), tensor4Iter.BlockStep(), tensor5Iter.BlockStep())
				{
					func(tensor1Iter.data, tensor2Iter.data, tensor3Iter.data, tensor4Iter.data, tensor5Iter.data);
				}

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock() && tensor3Iter.NextBlock() && tensor4Iter.NextBlock() && tensor5Iter.NextBlock());
		}


		unsafe static void ApplyDim2(Tensor tensor1, Tensor tensor2, int iterationDim, ApplyDim2KernelFuncton func)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);

			TensorDimIterState tensor1Iter = new TensorDimIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides, iterationDim);
			TensorDimIterState tensor2Iter = new TensorDimIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides, iterationDim);

			do
			{
				func(tensor1Iter.data, tensor1Iter.size, tensor1Iter.stride,
					tensor2Iter.data, tensor2Iter.size, tensor2Iter.stride);

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock());
		}




		unsafe static void ApplyDim3(Tensor tensor1, Tensor tensor2, Tensor tensor3, int iterationDim, ApplyDim3KernelFuncton func)
		{
			float* buffer1 = (float*)CpuNativeHelpers.GetBufferStart(tensor1);
			float* buffer2 = (float*)CpuNativeHelpers.GetBufferStart(tensor2);
			float* buffer3 = (float*)CpuNativeHelpers.GetBufferStart(tensor3);

			TensorDimIterState tensor1Iter = new TensorDimIterState(buffer1, tensor1.DimensionCount, tensor1.Sizes, tensor1.Strides, iterationDim);
			TensorDimIterState tensor2Iter = new TensorDimIterState(buffer2, tensor2.DimensionCount, tensor2.Sizes, tensor2.Strides, iterationDim);
			TensorDimIterState tensor3Iter = new TensorDimIterState(buffer3, tensor3.DimensionCount, tensor3.Sizes, tensor3.Strides, iterationDim);

			do
			{
				func(tensor1Iter.data, tensor1Iter.size, tensor1Iter.stride,
					tensor2Iter.data, tensor2Iter.size, tensor2Iter.stride,
					tensor3Iter.data, tensor3Iter.size, tensor3Iter.stride);

			} while (tensor1Iter.NextBlock() && tensor2Iter.NextBlock() && tensor3Iter.NextBlock());
		}


        #endregion

        unsafe public static void Gather(Tensor result, Tensor src, int dim, Tensor indices)
		{
			unsafe void func(float* rData, long rSize, long rStride,
				float* sData, long sSize, long sStride,
				float* iData, long iSize, long iStride)
			{
				for (int i = 0; i < iSize; ++i)
				{
					long idx = (long)*(iData + i * iStride);
					if (idx < 0 || idx >= sSize) { throw new IndexOutOfRangeException($"Invalid index in gather. Idx = '{idx}', sSize = '{sSize}'"); }

					*(rData + i * rStride) = sData[idx * sStride];
				}
			}

			ApplyDim3(result, src, indices, dim, func);
		}



		unsafe public static void Scatter(Tensor result, Tensor src, int dim, Tensor indices)
		{
			unsafe void func(float* rData, long rSize, long rStride,
				float* sData, long sSize, long sStride,
				float* iData, long iSize, long iStride)
			{

				for (int i = 0; i < iSize; ++i)
				{
					long idx = (long)*(iData + i * iStride);
					if (idx < 0 || idx >= rSize) { throw new IndexOutOfRangeException($"Invalid index in scatter. Idx = '{idx}', rSize = '{rSize}'"); }
				
					rData[idx * rStride] = *(sData + i * sStride);
				}

			}

			ApplyDim3(result, src, indices, dim, func);
		}

		unsafe public static void ScatterAdd(Tensor result, Tensor src, int dim, Tensor indices)
		{
			unsafe void func(float* rData, long rSize, long rStride,
				float* sData, long sSize, long sStride,
				float* iData, long iSize, long iStride)
			{

				for (int i = 0; i < iSize; ++i)
				{
					long idx = (long)*(iData + i * iStride);
					if (idx < 0 || idx >= rSize) { throw new IndexOutOfRangeException($"Invalid index in scatter. Idx = '{idx}', rSize = '{rSize}'"); }

					rData[idx * rStride] += *(sData + i * sStride);
				}

			}

			ApplyDim3(result, src, indices, dim, func);
		}

		unsafe public static void ScatterFill(Tensor result, float value, int dim, Tensor indices)
		{
			unsafe void func(float* rData, long rSize, long rStride, float* iData, long iSize, long iStride)
			{
				for (int i = 0; i < iSize; ++i)
				{
					long idx = (long)*(iData + i * iStride);
					if (idx < 0 || idx >= rSize) { throw new IndexOutOfRangeException($"Invalid index in ScatterFill. Idx = '{idx}', rSize = '{rSize}'"); }

					rData[idx * rStride] = value;
				}

			}

			ApplyDim2(result, indices, dim, func);
		}



		unsafe public static void Fill(Tensor result, float value)
		{
			unsafe void func(float* r)
			{
				*r = value;
			}

			Apply1(result, func);
		}


		unsafe public static void Clamp(Tensor result, Tensor src, float min, float max)
		{
			unsafe void func(float* r, float* s)
			{
				*r = clamp(*s, min, max);
			}
			Apply2(result, src, func);
		}


		unsafe public static void Copy(Tensor result, Tensor src)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);
					vecS.CopyTo(spanR);
				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = *s;
				}
				Apply2(result, src, func);
			}
		}


		unsafe public static void Sum(Tensor result, Tensor src, int dimension)
		{
			unsafe void func(float* r, long rSize, long rStride, float* s, long sSize, long sStride)
			{
				float sum = 0.0f;
				for (long i = 0; i < sSize; ++i)
				{
					sum += s[i * sStride];
				}
				*r = sum;
			}
			ApplyDim2(result, src, dimension, func);
		}


		unsafe public static void Mean(Tensor result, Tensor src, int dimension)
		{
			unsafe void func(float* r, long rSize, long rStride, float* s, long sSize, long sStride)
			{
				float sum = 0.0f;
				for (long i = 0; i < sSize; ++i)
				{
					sum += s[i * sStride];
				}
				*r = sum / sSize;
			}
			ApplyDim2(result, src, dimension, func);
		}


		unsafe public static void Argmax(Tensor resultIndices, Tensor src, int dimension)
		{

			unsafe void func(float* rIndVal, long rIndSize, long rIndStride,
				float* s, long sSize, long sStride)
			{
				float value = s[0];
				float index = 0;
				for (long i = 1; i < sSize; ++i)
				{
					float currentVal = s[i * sStride];
					if (currentVal > value)
					{
						value = currentVal;
						index = (float)i;
					}
				}
				*rIndVal = index;
			}

			ApplyDim2(resultIndices, src, dimension, func);
		}

		unsafe public static void Max(Tensor result, Tensor src, int dimension)
		{
			unsafe void func(float* r, long rSize, long rStride, float* s, long sSize, long sStride)
			{
				float value = s[0];
				for (long i = 1; i < sSize; ++i)
				{
					value = Math.Max(value, s[i * sStride]);
				}
				*r = value;
			}

			ApplyDim2(result, src, dimension, func);
		}


		unsafe public static void Add(Tensor result, Tensor lhs, Tensor rhs)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && lhs.Strides[^1] == 1 && rhs.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* left, float* right)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanLeft = new Span<float>(left, vectorSize);
					Span<float> spanRight = new Span<float>(right, vectorSize);

					Vector<float> vecLeft = new Vector<float>(spanLeft);
					Vector<float> vecRight = new Vector<float>(spanRight);

					Vector<float> vecR = vecLeft + vecRight;
					vecR.CopyTo(spanR);

				}

				Apply3(result, lhs, rhs, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* left, float* right)
				{
					*r = add(*left, *right);
				}

				Apply3(result, lhs, rhs, func);
			}
		}


		unsafe public static void Sub(Tensor result, Tensor lhs, Tensor rhs)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && lhs.Strides[^1] == 1 && rhs.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* left, float* right)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanLeft = new Span<float>(left, vectorSize);
					Span<float> spanRight = new Span<float>(right, vectorSize);

					Vector<float> vecLeft = new Vector<float>(spanLeft);
					Vector<float> vecRight = new Vector<float>(spanRight);

					Vector<float> vecR = vecLeft - vecRight;
					vecR.CopyTo(spanR);

				}

				Apply3(result, lhs, rhs, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* left, float* right)
				{
					*r = *left - *right;
				}

				Apply3(result, lhs, rhs, func);
			}
		}

		unsafe public static void Add(Tensor result, Tensor src, float value)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				Vector<float> vecV = new Vector<float>(value);
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = vecS + vecV;
					vecR.CopyTo(spanR);

				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = add(*s, value);
				}

				Apply2(result, src, func);
			}
		}


		unsafe public static void Pow(Tensor result, Tensor src, float value)
		{
				unsafe void func(float* r, float* s)
				{
					*r = (float)Math.Pow(*s, value);
				}

				Apply2(result, src, func);			
		}


		unsafe public static void RSub(Tensor result, float value, Tensor src)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				Vector<float> vecV = new Vector<float>(value);
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = vecV - vecS;
					vecR.CopyTo(spanR);

				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = value - *s;
				}

				Apply2(result, src, func);
			}
		}



		unsafe public static void Mul(Tensor result, Tensor src, float value)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				Vector<float> vecV = new Vector<float>(value);
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = vecS * vecV;
					vecR.CopyTo(spanR);
				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = mul(*s, value);
				}

				Apply2(result, src, func);
			}
		}


		unsafe public static void Div(Tensor result, Tensor lhs, float rhs)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && lhs.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				Vector<float> vecV = new Vector<float>(rhs);
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = vecS / vecV;
					vecR.CopyTo(spanR);
				}

				Apply2(result, lhs, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = div(*s, rhs);
				}

				Apply2(result, lhs, func);
			}
		}

		unsafe public static void Mul(Tensor result, Tensor lhs, Tensor rhs)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && lhs.Strides[^1] == 1 && rhs.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* left, float* right)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanLeft = new Span<float>(left, vectorSize);
					Span<float> spanRight = new Span<float>(right, vectorSize);

					Vector<float> vecLeft = new Vector<float>(spanLeft);
					Vector<float> vecRight = new Vector<float>(spanRight);

					Vector<float> vecR = vecLeft * vecRight;
					vecR.CopyTo(spanR);

				}

				Apply3(result, lhs, rhs, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* left, float* right)
				{
					*r = mul(*left, *right);
				}

				Apply3(result, lhs, rhs, func);
			}
		}


		unsafe public static void Div(Tensor result, Tensor lhs, Tensor rhs)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && lhs.Strides[^1] == 1 && rhs.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* left, float* right)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanLeft = new Span<float>(left, vectorSize);
					Span<float> spanRight = new Span<float>(right, vectorSize);

					Vector<float> vecLeft = new Vector<float>(spanLeft);
					Vector<float> vecRight = new Vector<float>(spanRight);

					Vector<float> vecR = vecLeft / vecRight;
					vecR.CopyTo(spanR);

				}

				Apply3(result, lhs, rhs, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* left, float* right)
				{
					*r = div(*left, *right);
				}

				Apply3(result, lhs, rhs, func);
			}
		}

		unsafe static public void Relu(Tensor result, Tensor src)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = Vector.Max(vecS, Vector<float>.Zero);
					vecR.CopyTo(spanR);
				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = relu(*s);
				};

				Apply2(result, src, func);
			}
		}


		unsafe static public void Rsqrt(Tensor result, Tensor src)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && src.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* s)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanS = new Span<float>(s, vectorSize);

					Vector<float> vecS = new Vector<float>(spanS);

					Vector<float> vecR = Vector<float>.One / Vector.SquareRoot(vecS);
					vecR.CopyTo(spanR);
				}

				Apply2(result, src, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* s)
				{
					*r = (float)(1.0 / Math.Sqrt(*s));
				};

				Apply2(result, src, func);
			}
		}



		unsafe static public void Sigmoid(Tensor result, Tensor src)
		{
			unsafe void func(float* r, float* s)
			{
				*r = sigmoid(*s);
			};

			Apply2(result, src, func);
		}


		unsafe static public void AddSigmoidD(Tensor result, Tensor t, Tensor resW, Tensor resG)
		{
			unsafe void func(float* r, float* x, float* y, float* z)
			{
				*r = addSigmoidD(*x, *y, *z);
			}

			Apply4(result, t, resW, resG, func);
		}



		unsafe static public void SigmoidD(Tensor result, Tensor resW, Tensor resG)
		{
			unsafe void func(float* r, float* x, float* y)
			{
				*r = sigmoidD(*x, *y);
			}

			Apply3(result, resW, resG, func);
		}



		unsafe static public void Tanh(Tensor result, Tensor src)
		{
			unsafe void func(float* r, float* s)
			{
				*r = (float)Math.Tanh(*s);
			};

			Apply2(result, src, func);
		}


		unsafe static public void Log(Tensor result, Tensor src)
		{
			unsafe void func(float* r, float* s)
			{
				*r = (float)Math.Log(*s);
			};

			Apply2(result, src, func);
		}

        unsafe static public void Exp(Tensor result, Tensor src)
        {
            unsafe void func(float* r, float* s)
            {
                *r = (float)Math.Exp(*s);
            };

            Apply2(result, src, func);
        }

        unsafe static public void TanhD(Tensor result, Tensor resW, Tensor resG)
		{
			unsafe void func(float* r, float* x, float* y)
			{
				*r = tanhD(*x, *y);
			}

			Apply3(result, resW, resG, func);
		}



		unsafe static public void AddTanh(Tensor result, Tensor srcX, Tensor srcY)
		{
			unsafe void func(float* r, float* x, float* y)
			{
				*r = addtanh(*x, *y);
			}

			Apply3(result, srcX, srcY, func);
		}


		unsafe static public void AddTanhD(Tensor result, Tensor srcX, Tensor srcY, Tensor srcZ)
		{
			unsafe void func(float* r, float* x, float* y, float* z)
			{
				*r = addtanhD(*x, *y, *z);

			}
			Apply4(result, srcX, srcY, srcZ, func);
		}

		unsafe static public void ReluD(Tensor result, Tensor srcW, Tensor resG)
		{
			unsafe void func(float* r, float* y, float* x)
			{
				*r = relud(*y, *x);
			}

			Apply3(result, srcW, resG, func);
		}

		unsafe static public void AddReluD(Tensor result, Tensor srcX, Tensor srcW, Tensor srcG)
		{
			unsafe void func(float* r, float*x, float* w, float* g)
			{
				*r = addrelud(*x, *w, *g);
			}

			Apply4(result, srcX, srcW, srcG, func);
		}

		unsafe static public void SiLU(Tensor result, Tensor src)
		{
			unsafe void func(float* r, float* s)
			{
				*r = SiLU(*s);
			};

			Apply2(result, src, func);
		}


		unsafe static public void SiLUD(Tensor result, Tensor srcW, Tensor resG)
		{
			unsafe void func(float* r, float* y, float* x)
			{
				*r = SiLUD(*y, *x);
			}

			Apply3(result, srcW, resG, func);
		}

		unsafe static public void AddSiLUD(Tensor result, Tensor srcG, Tensor srcW, Tensor resG)
		{
			unsafe void func(float* r, float* x, float* w, float* g)
			{
				*r = AddSiLUD(*x, *w, *g);
			}

			Apply4(result, srcG, srcW, resG, func);
		}



        unsafe static public void LeakyReLU(Tensor result, Tensor src)
        {
            unsafe void func(float* r, float* s)
            {
                *r = LeakyReLU(*s);
            };

            Apply2(result, src, func);
        }


        unsafe static public void LeakyReLUD(Tensor result, Tensor srcW, Tensor resG)
        {
            unsafe void func(float* r, float* y, float* x)
            {
                *r = LeakyReLUD(*y, *x);
            }

            Apply3(result, srcW, resG, func);
        }

        unsafe static public void AddLeakyReLUD(Tensor result, Tensor srcG, Tensor srcW, Tensor resG)
        {
            unsafe void func(float* r, float* x, float* w, float* g)
            {
                *r = AddLeakyReLUD(*x, *w, *g);
            }

            Apply4(result, srcG, srcW, resG, func);
        }


        unsafe static public void AddMulV(Tensor result, Tensor srcX, Tensor srcY, float val)
		{
			unsafe void func(float* r, float* x, float* y)
			{
				*r = *x + (*y * val);
			}

			Apply3(result, srcX, srcY, func);
		}


		unsafe static public void MulMulAdd(Tensor result, Tensor srcX, Tensor srcY, Tensor srcZ, Tensor srcW)
		{
			int vectorSize = Vector<float>.Count;
			if (result.Strides[^1] == 1 && srcX.Strides[^1] == 1 && srcY.Strides[^1] == 1 && srcZ.Strides[^1] == 1 && srcW.Strides[^1] == 1 && result.Sizes[^1] % vectorSize == 0)
			{
				unsafe void funcVec(float* r, float* x, float* y, float* z, float* w)
				{
					Span<float> spanR = new Span<float>(r, vectorSize);
					Span<float> spanX = new Span<float>(x, vectorSize);
					Span<float> spanY = new Span<float>(y, vectorSize);
					Span<float> spanZ = new Span<float>(z, vectorSize);
					Span<float> spanW = new Span<float>(w, vectorSize);

					Vector<float> vecX = new Vector<float>(spanX);
					Vector<float> vecY = new Vector<float>(spanY);
					Vector<float> vecZ = new Vector<float>(spanZ);
					Vector<float> vecW = new Vector<float>(spanW);

					Vector<float> vecR = vecX * vecY + vecZ * vecW;
					vecR.CopyTo(spanR);
				}

				Apply5(result, srcX, srcY, srcZ, srcW, funcVec, vectorSize);
			}
			else
			{
				unsafe void func(float* r, float* x, float* y, float* z, float* w)
				{
					*r = mulmuladd(*x, *y, *z, *w);
				}

				Apply5(result, srcX, srcY, srcZ, srcW, func);
			}
		}




		unsafe static public void AddMul(Tensor result, Tensor srcX, Tensor srcY, Tensor srcZ)
		{
			unsafe void func(float* r, float* x, float* y, float* z)
			{
				*r = addmul(*x, *y, *z);
			}

			Apply4(result, srcX, srcY, srcZ, func);
		}


		unsafe static public void AddDiv(Tensor result, Tensor srcX, Tensor srcY, Tensor srcZ)
		{
			unsafe void func(float* r, float* x, float* y, float* z)
			{
				*r = adddiv(*x, *y, *z);
			}

			Apply4(result, srcX, srcY, srcZ, func);
		}


		unsafe static public void BuildSelfMask(Tensor result, Tensor originalLengths, int rows, int cols, int paddedSeqLen, float value, float maskedValue)
		{
			float* ptResult = (float*)CpuNativeHelpers.GetBufferStart(result);
			float* ptOriginalLengths = (float*)CpuNativeHelpers.GetBufferStart(originalLengths);

			for (int j = 0; j < rows; j++)
			{
				float* resultRow = ptResult + j * cols;
				int batchIdx = j / paddedSeqLen;
				int seqIdxInBatch = j % paddedSeqLen;

				for (int id = 0; id < cols; id++)
				{
					int originalLength = (int)ptOriginalLengths[batchIdx];
					if (id < originalLength && seqIdxInBatch < originalLength)
					{
						resultRow[id] = value;
					}
					else
					{
						resultRow[id] = maskedValue;
					}
				}
			}
		}



		unsafe static public void BuildTriMask(Tensor result, int rows, int cols, float value, float maskedValue)
		{
			float* ptResult = (float*)CpuNativeHelpers.GetBufferStart(result);

			for (int j = 0; j < rows; j++)
			{
				float* resultRow = ptResult + j * cols;
				for (int id = 0; id < cols; id++)
				{

					if (id <= j)
					{
						resultRow[id] = value;
					}
					else
					{
						resultRow[id] = maskedValue;
					}
				}
			}
		}



		unsafe static public void BuildSelfTriMask(Tensor result, Tensor originalLengths, int rows, int cols, int paddedSeqLen, float value, float maskedValue)
		{
			float* ptResult = (float*)CpuNativeHelpers.GetBufferStart(result);
			float* ptOriginalLengths = (float*)CpuNativeHelpers.GetBufferStart(originalLengths);

			for (int j = 0; j < rows; j++)
			{
				float* resultRow = ptResult + j * cols;
				int batchIdx = j / paddedSeqLen;
				int seqIdxInBatch = j % paddedSeqLen;

				for (int id = 0; id < cols; id++)
				{
					int originalLength = (int)ptOriginalLengths[batchIdx];
					if (id < originalLength && seqIdxInBatch < originalLength && id <= seqIdxInBatch)
					{
						resultRow[id] = value;
					}
					else
					{
						resultRow[id] = maskedValue;
					}

				}
			}
		}



		unsafe static public void BuildSrcTgtMask(Tensor result, Tensor srcOriginalLengths, Tensor tgtOriginalLengths, int rows, int cols, int tgtPaddedSeqLen, float value, float maskedValue)
		{
			float* ptResult = (float*)CpuNativeHelpers.GetBufferStart(result);
			float* ptSrcOriginalLengths = (float*)CpuNativeHelpers.GetBufferStart(srcOriginalLengths);
			float* ptTgtOriginalLengths = (float*)CpuNativeHelpers.GetBufferStart(tgtOriginalLengths);

			for (int j = 0; j < rows; j++)
			{
				float* resultRow = ptResult + j * cols;
				int batchIdx = j / tgtPaddedSeqLen;
				int seqIdxInBatch = j % tgtPaddedSeqLen;

				for (int id = 0; id < cols; id++)
				{
					int srcOriginalLength = (int)ptSrcOriginalLengths[batchIdx];
					int tgtOriginalLength = (int)ptTgtOriginalLengths[batchIdx];

					if (id < srcOriginalLength && seqIdxInBatch < tgtOriginalLength)
					{
						resultRow[id] = value;
					}
					else
					{
						resultRow[id] = maskedValue;
					}
				}
			}
		}


		unsafe static public void replace_smaller(float* array, float* arrayIdx, int k, float data, float idx)
		{
			if (data < array[k - 1])
				return;
			for (int j = k - 2; j >= 0; j--)
			{
				if (data > array[j])
				{
					array[j + 1] = array[j];
					arrayIdx[j + 1] = arrayIdx[j];
				}
				else
				{
					array[j + 1] = data;
					arrayIdx[j + 1] = idx;
					return;
				}
			}
			array[0] = data;
			arrayIdx[0] = idx;
		}

		unsafe static public void TopK(Tensor outVal, Tensor outIdx, Tensor inVal, int k, int rows, int cols)
		{
			float* pOutVal = (float*)CpuNativeHelpers.GetBufferStart(outVal);
			float* pOutIdx = (float*)CpuNativeHelpers.GetBufferStart(outIdx);
			float* pInVal = (float*)CpuNativeHelpers.GetBufferStart(inVal);


			for (int j = 0; j < rows; ++j)
			{
				float* outputRow = pOutVal + j * k;
				float* outputIdxRow = pOutIdx + j * k;
				float* inputRow = pInVal + j * cols;

				for (int i = 0; i < k; ++i)
				{
					outputRow[i] = -1.70141e+38f;
					outputIdxRow[i] = -1.70141e+38f;

				}

				for (int i = 0; i < cols; i++)
				{
					replace_smaller(outputRow, outputIdxRow, k, inputRow[i], i);
				}
			}
		}

		unsafe static public void RoPE(Tensor tOut, Tensor tIn, int rows, int cols, int seqLen, int rowOffset)
		{
			float* result = (float*)CpuNativeHelpers.GetBufferStart(tOut);
			float* src = (float*)CpuNativeHelpers.GetBufferStart(tIn);

			for (int j = 0; j < rows; ++j)
			{
				float* resultRow = result + j * cols;
				float* srcRow = src + j * cols;
				int m = (j % seqLen) + rowOffset;

				for (int id = 0; id < cols; id++)
				{
					int i = id / 2;
					float theta = (float)Math.Pow(500000.0, -2.0 * i / cols);
					float theta_m = theta * m;
					float cos_theta_m = (float)Math.Cos(theta_m);
					float sin_theta_m = (float)Math.Sin(theta_m);

					if (id % 2 == 0)
					{
						resultRow[id] = srcRow[id] * cos_theta_m - srcRow[id + 1] * sin_theta_m;
					}
					else
					{
						resultRow[id] = srcRow[id] * cos_theta_m + srcRow[id - 1] * sin_theta_m;
					}

				}
			}
		}


		unsafe static public void RoPEGrad(Tensor tOut, Tensor tIn, int rows, int cols, int seqLen, int rowOffset)
		{
			float* grad = (float*)CpuNativeHelpers.GetBufferStart(tOut);
			float* adj = (float*)CpuNativeHelpers.GetBufferStart(tIn);

			for (int j = 0; j < rows; j++)
			{
				float* gradRow = grad + j * cols;
				float* adjRow = adj + j * cols;
				int m = (j % seqLen) + rowOffset;

				for (int id = 0; id < cols; id++)
				{
					int i = id / 2;
					float theta = (float)Math.Pow(500000.0, -2.0 * i / cols);
					float theta_m = theta * m;
					float cos_theta_m = (float)Math.Cos(theta_m);
					float sin_theta_m = (float)Math.Sin(theta_m);

					if (id % 2 == 0)
					{
						gradRow[id] += (adjRow[id] * cos_theta_m + adjRow[id + 1] * sin_theta_m);
					}
					else
					{
						gradRow[id] += (adjRow[id] * cos_theta_m - adjRow[id - 1] * sin_theta_m);
					}
				}
			}
		}

		unsafe static public bool IsCorrupted(Tensor tIn, int rows, int cols)
		{
            float* pIn = (float*)CpuNativeHelpers.GetBufferStart(tIn);

            for (int j = 0; j < rows; ++j)
            {
                float* sp = pIn + j * cols;
                for (int i = 0; i < cols; ++i)
                {
					if (float.IsFinite(sp[i]) == false)
					{
						return true;
					}
                }
            }
			return false;
        }

        unsafe static public void Softmax(Tensor tOut, Tensor tIn, int rows, int cols)
		{
			float* pOut = (float*)CpuNativeHelpers.GetBufferStart(tOut);
			float* pIn = (float*)CpuNativeHelpers.GetBufferStart(tIn);

			for (int j = 0; j < rows; ++j)
			{
				float* so = pOut + j * cols;
				float* sp = pIn + j * cols;

				float max = sp[0];
				for (int i = 1; i < cols; ++i)
					max = Math.Max(max, sp[i]);

				float sum = 0.0f;
				for (int i = 0; i < cols; ++i)
				{
					float ex = (float)Math.Exp(sp[i] - max);
					so[i] = ex;
					sum += ex;
				}

				Span<float> spanSO = new Span<float>(so, cols);
				int vectorSize = Vector<float>.Count;
				int k = 0;
				Vector<float> vecSum = new Vector<float>(sum);
				for (k = 0; k < cols - vectorSize; k += vectorSize)
				{
					Vector<float> vecSO = new Vector<float>(spanSO.Slice(k));
					vecSO /= vecSum;

					vecSO.CopyTo(spanSO.Slice(k));
				}
				for (; k < cols; k++)
				{
					so[k] /= sum;
				}
			}
		}


		unsafe static public void SoftmaxGrad(Tensor grad_, Tensor adj_, Tensor val_, int rows, int cols, bool addGrad)
		{

			float* grad = (float*)CpuNativeHelpers.GetBufferStart(grad_);
			float* adj = (float*)CpuNativeHelpers.GetBufferStart(adj_);
			float* val = (float*)CpuNativeHelpers.GetBufferStart(val_);

			for (int j = 0; j < rows; ++j)
			{
				float* gradRow = grad + j * cols;
				float* adjRow = adj + j * cols;
				float* valRow = val + j * cols;

				float sum = 0.0f;
				for (int i = 0; i < cols; ++i)
				{
					sum += valRow[i] * adjRow[i];
				}

				for (int i = 0; i < cols; ++i)
				{
					if (addGrad)
					{
						gradRow[i] += valRow[i] * (adjRow[i] - sum);
					}
					else
					{
						gradRow[i] = valRow[i] * (adjRow[i] - sum);
					}
				}
			}
		}



		unsafe static public void IndexSelect(Tensor result_, Tensor src_, Tensor indice_, int rows, int cols, bool isAdd)
		{
			float* result = (float*)CpuNativeHelpers.GetBufferStart(result_);
			float* src = (float*)CpuNativeHelpers.GetBufferStart(src_);
			float* indice = (float*)CpuNativeHelpers.GetBufferStart(indice_);

			for (int j = 0; j < rows; j++)
			{
				int srcIdx = (int)indice[j];
				if (srcIdx >= 0)
				{
					float* resultRow = result + j * cols;
					float* srcRow = src + srcIdx * cols;

					for (int i = 0; i < cols; ++i)
					{
						if (isAdd == false)
						{
							resultRow[i] = srcRow[i];
						}
						else
						{
							resultRow[i] += srcRow[i];
						}
					}
				}
			}
		}


		unsafe static public void IndexSelectGrad(Tensor grad_, Tensor adj_, Tensor indice_, int rows, int cols)
		{
			float* grad = (float*)CpuNativeHelpers.GetBufferStart(grad_);
			float* adj = (float*)CpuNativeHelpers.GetBufferStart(adj_);
			float* indice = (float*)CpuNativeHelpers.GetBufferStart(indice_);

			for (int j = 0; j < rows; j++)
			{
				int gradIdx = (int)indice[j];
				if (gradIdx >= 0)
				{
					float* adjRow = adj + j * cols;
					float* gradRow = grad + gradIdx * cols;

					for (int i = 0; i < cols; ++i)
					{
						gradRow[i] += adjRow[i];
					}
				}
			}
		}



		unsafe static public void LayerNorm(Tensor out_,
			Tensor in_,
			Tensor gamma_,
			Tensor beta_,
			float eps,
			int rows,
			int cols)
		{
			float* outPtr = (float*)CpuNativeHelpers.GetBufferStart(out_);
			float* inPtr = (float*)CpuNativeHelpers.GetBufferStart(in_);
			float* alpha = (float*)CpuNativeHelpers.GetBufferStart(gamma_);
			float* beta = (beta_ != null) ? (float*)CpuNativeHelpers.GetBufferStart(beta_) : null;

			for (int j = 0; j < rows; ++j)
			{
				float* so = outPtr + j * cols;
				float* sp = inPtr + j * cols;

				Span<float> spanSP = new Span<float>(sp, cols);

				float sum = 0.0f;
				int vectorSize = Vector<float>.Count;
				Vector<float> vecAdded = Vector<float>.Zero;
				int i = 0;

				for (i = 0; i < cols - vectorSize; i += vectorSize)
				{
					Vector<float> vecSp = new Vector<float>(spanSP.Slice(i));
					vecAdded += vecSp;
				}
				sum = Vector.Dot(vecAdded, Vector<float>.One);
				for (; i < cols; i++)
				{
					sum += sp[i];
				}

				float mean = sum / cols;
				float sqSum = 0.0f;

				Vector<float> vecMean = new Vector<float>(mean);
				for (i = 0; i < cols - vectorSize; i += vectorSize)
				{
					Vector<float> vecSp = new Vector<float>(spanSP.Slice(i));
					Vector<float> vecEx = vecSp - vecMean;
					sqSum += Vector.Dot(vecEx, vecEx);
				}
				for (; i < cols; ++i)
				{
					float ex = sp[i] - mean;
					sqSum += ex * ex;
				}

				float sigma = (float)Math.Sqrt(eps + sqSum / cols);

				Span<float> spanSO = new Span<float>(so, cols);
				Span<float> spanAlpha = new Span<float>(alpha, cols);
				Span<float> spanBeta = (beta != null) ? new Span<float>(beta, cols) : null;
				Vector<float> vecSigma = new Vector<float>(sigma);

				for (i = 0; i < cols - vectorSize; i += vectorSize)
				{
					Vector<float> vecSp = new Vector<float>(spanSP.Slice(i));
					Vector<float> vecAlpha = new Vector<float>(spanAlpha.Slice(i));

					Vector<float> vecT = vecAlpha * ((vecSp - vecMean) / vecSigma);

					if (spanBeta != null)
					{
						Vector<float> vecBeta = new Vector<float>(spanBeta.Slice(i));
						vecT += vecBeta;
					}

					vecT.CopyTo(spanSO.Slice(i));
				}
				for (; i < cols; ++i)
				{
					float t = alpha[i] * ((sp[i] - mean) / sigma);
					if (beta != null)
					{
						t += beta[i];
					}

					so[i] = t;
				}


			}
		}



		unsafe static public void RMSNorm(Tensor out_,
			Tensor in_,
			Tensor gamma_,
			Tensor beta_,
			float eps,
			int rows,
			int cols)
		{
			float* outPtr = (float*)CpuNativeHelpers.GetBufferStart(out_);
			float* inPtr = (float*)CpuNativeHelpers.GetBufferStart(in_);
			float* gamma = (float*)CpuNativeHelpers.GetBufferStart(gamma_);
			float* beta = (beta_ != null) ? (float*)CpuNativeHelpers.GetBufferStart(beta_) : null;
			bool bias = (beta_ != null);

            float N = cols;
			for (int j = 0; j < rows; j++)
			{
				float* yRow = outPtr + j * cols;
				float* xRow = inPtr + j * cols;

				float _sqSum = 0;

				for (int id = 0; id < cols; id++)
				{
					float xv = (float)xRow[id];

					_sqSum += xv * xv;

				}

				float rms = (float)Math.Sqrt(_sqSum / N + eps); 
				for (int id = 0; id < cols; id++)
				{

					float gammav = gamma[id];
					float xv = xRow[id];
					float betav = bias ? beta[id] : 0.0f;
					float rmsNorm = xv / rms;
					float y = gammav * rmsNorm + betav;
					yRow[id] = y;

				}
			}


		}

        unsafe static public void LayerNormGrad(Tensor gradX_,
			Tensor gradGamma_,
			Tensor gradBeta_,
			Tensor adj_,
			Tensor y_,
			Tensor x_,
			Tensor gamma_,
			Tensor beta_,
			int rows,
			int cols,
			float eps)
		{
			float* gradX = (float*)CpuNativeHelpers.GetBufferStart(gradX_);
			float* gradGamma = (float*)CpuNativeHelpers.GetBufferStart(gradGamma_);
			float* gradBeta = gradBeta_ != null ? (float*)CpuNativeHelpers.GetBufferStart(gradBeta_) : null;
			float* adj = (float*)CpuNativeHelpers.GetBufferStart(adj_);
			float* y = (float*)CpuNativeHelpers.GetBufferStart(y_);
			float* x = (float*)CpuNativeHelpers.GetBufferStart(x_);
			float* gamma = (float*)CpuNativeHelpers.GetBufferStart(gamma_);
			float* beta = beta_ != null ? (float*)CpuNativeHelpers.GetBufferStart(beta_) : null;

			if (beta != null)
			{
				for (int j = 0; j < rows; ++j)
				{
					float* xRow = x + j * cols;
					float* yRow = y + j * cols;
					float* adjRow = adj + j * cols;
					float* gradXRow = gradX + j * cols;

					float sum_x = 0.0f;
					float sum_adj = 0.0f;
					float sum_adj_x = 0.0f;
					float sum_sqr = 0.0f;

					for (int i = 0; i < cols; ++i)
					{
						sum_x += xRow[i];
						sum_adj_x += adjRow[i] * (yRow[i] - (beta != null ? beta[i] : 0.0f)) / gamma[i];
						sum_adj += adjRow[i];
					}

					float mean = sum_x / cols;
					for (int i = 0; i < cols; ++i)
					{
						float ex = xRow[i] - mean;
						sum_sqr += ex * ex;
					}

					float sigma = (float)Math.Sqrt(eps + sum_sqr / cols);
					for (int i = 0; i < cols; ++i)
					{
						float grad_x = 0.0f;
						float x_hat = (yRow[i] - beta[i]) / gamma[i];
						grad_x += cols * adjRow[i];
						grad_x -= sum_adj;
						grad_x -= sum_adj_x * x_hat;
						grad_x /= cols * sigma;

						gradXRow[i] += gamma[i] * grad_x;
						gradGamma[i] += adjRow[i] * x_hat;
						gradBeta[i] += adjRow[i];
					}
				}
			}
			else
			{
				for (int j = 0; j < rows; ++j)
				{
					float* xRow = x + j * cols;
					float* yRow = y + j * cols;
					float* adjRow = adj + j * cols;
					float* gradXRow = gradX + j * cols;

					float sum_x = 0.0f;
					float sum_adj = 0.0f;
					float sum_adj_x = 0.0f;
					float sum_sqr = 0.0f;

					for (int i = 0; i < cols; ++i)
					{
						sum_x += xRow[i];
						sum_adj_x += adjRow[i] * (yRow[i] - (beta != null ? beta[i] : 0.0f)) / gamma[i];
						sum_adj += adjRow[i];
					}

					float mean = sum_x / cols;

					for (int i = 0; i < cols; ++i)
					{
						float ex = xRow[i] - mean;
						sum_sqr += ex * ex;
					}

					float sigma = (float)Math.Sqrt(eps + sum_sqr / cols);

					for (int i = 0; i < cols; ++i)
					{
						float grad_x = 0.0f;
						float x_hat = yRow[i] / gamma[i];
						grad_x += cols * adjRow[i];
						grad_x -= sum_adj;
						grad_x -= sum_adj_x * x_hat;
						grad_x /= cols * sigma;

						gradXRow[i] += gamma[i] * grad_x;
						gradGamma[i] += adjRow[i] * x_hat;
					}
				}
			}
		}


		unsafe static public void RMSNormGrad(Tensor gradX_,
			Tensor gradGamma_,
			Tensor gradBeta_,
			Tensor adj_,
			Tensor y_,
			Tensor x_,
			Tensor gamma_,
			Tensor beta_,
			int rows,
			int cols,
			float eps)
		{
			float* gradX = (float*)CpuNativeHelpers.GetBufferStart(gradX_);
			float* gradGamma = (float*)CpuNativeHelpers.GetBufferStart(gradGamma_);
			float* gradBeta = (gradBeta_ != null) ? (float*)CpuNativeHelpers.GetBufferStart(gradBeta_) : null;
            float* adj = (float*)CpuNativeHelpers.GetBufferStart(adj_);
			float* y = (float*)CpuNativeHelpers.GetBufferStart(y_);
			float* x = (float*)CpuNativeHelpers.GetBufferStart(x_);
			float* gamma = (float*)CpuNativeHelpers.GetBufferStart(gamma_);
            float* beta = (beta_ != null) ? (float*)CpuNativeHelpers.GetBufferStart(beta_) : null;
			bool bias = (beta_ != null);

            float N = cols;
			for (int j = 0; j < rows; j++)
			{
				float* xRow = x + j * cols;
				float* yRow = y + j * cols;
				float* adjRow = adj + j * cols;

				float sum_adj_r = (float)0.0f;
				float sum_sqr = (float)0.0f;

				for (int id = 0; id < cols; id++)
				{

					float xv = xRow[id];
					float yv = yRow[id];
					float betav = bias ? beta[id] : 0.0f;
					float gammav = (float)gamma[id];
					float adjv = adjRow[id];
					float rv = (yv - betav) / gammav; // go back to RMSNorm(x) from scaled and shifted version for accumulation

					sum_adj_r += adjv * rv;
					sum_sqr += xv * xv;

				}

				float rms = (float)Math.Sqrt(sum_sqr / N + eps);

				// Jacobian of RMS norm
				// J = [ \frac{1}{N * rms} (N\delta_{ij} - RN_i RN_j) ]_{ij}
				// J * a = dC/dx_i = ( N a_i - RN_i \sum_j RN_j a_j ) / (N * rms)

				for (int id = 0; id < cols; id++)
				{

					float xv = xRow[id];
					float gammav = (float)gamma[id];
					float adjv = adjRow[id];
					float rmsNorm = xv / rms;

					float gradNorm = N * adjv - rmsNorm * sum_adj_r;
					gradNorm /= N * rms;

					float gradXv = gammav * gradNorm;

					// Keep RMSN gradient between [-1000, 1000] for TensorOps, this currently used for making values fit into fp16. This wil also clip inf. 
					// @TODO: to be fixed and removed.
					float sign = Math.Sign(gradXv); //functional::Ops<AccType>::sgn(gradXv);
					float cutoff = (float)1000.0f; // @TODO: expose this somehow as an option? or better: make obsolete.
					gradXv = Math.Abs(gradXv) > cutoff ? sign * cutoff : gradXv; // if gradXv is NaN the value return is NaN too because NaN > value is false.

					// @TODO: frankly, this is embarrasing and should rather be removed or optional? It does help for low precision computation though. Maybe turn into option?
					gradXv = float.IsNaN(gradXv) ? 0.0f : gradXv; // turn NaN into 0.

					float* gradXRow = gradX + j * cols;
					gradXRow[id] += (float)(gradXv);

					gradGamma[id] += (float)(adjv * rmsNorm);
					if (bias)
					{
						gradBeta[id] += adjRow[id];
					}
                }
			}
		}


        unsafe static public void Adam(Tensor tw, Tensor tg, Tensor tv, Tensor tm, int rows, int cols, float gradNormFactor, float step_size, float clipval, float regc, float decay_rate_v, float decay_rate_m, int iter, float eps)
		{
			float* w = (float*)CpuNativeHelpers.GetBufferStart(tw);
			float* g = (float*)CpuNativeHelpers.GetBufferStart(tg);
			float* v = (float*)CpuNativeHelpers.GetBufferStart(tv);
			float* m = (float*)CpuNativeHelpers.GetBufferStart(tm);

			for (int j = 0; j < rows; j++)
			{
				float* sw = w + j * cols;
				float* sg = g + j * cols;
				float* sv = v + j * cols;
				float* sm = m + j * cols;

				for (int i = 0; i < cols; i++)
				{
					if (sg[i] != 0.0)
					{
						float g2 = sg[i] / gradNormFactor;

						if (g2 > clipval)
						{
							g2 = clipval;
						}
						if (g2 < -clipval)
						{
							g2 = -clipval;
						}

						sm[i] = sm[i] * decay_rate_m + (1.0f - decay_rate_m) * g2;
						sv[i] = sv[i] * decay_rate_v + (1.0f - decay_rate_v) * g2 * g2;

						double m_cap = sm[i] / (1.0 - Math.Pow(decay_rate_m, iter));
						double v_cap = sv[i] / (1.0 - Math.Pow(decay_rate_v, iter));

						sw[i] -= (float)(step_size * m_cap / (Math.Sqrt(v_cap) + eps));

						sg[i] = 0;
					}
				}
			}
		}


		#region Internal operations


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float relu(float w)
		{
			if (w < 0.0f)
				return 0.0f;
			return w;

		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float relud(float w, float g)
		{
			if (w > 0.0f)
				return g;
			return 0.0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float addrelud(float t, float w, float g)
		{
			if (w > 0.0f)
				return t + g;
			return t;
		}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float LeakyReLU(float w)
        {
            if (w < 0.0f)
                return 0.01f * w;
            return w;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float LeakyReLUD(float w, float g)
        {
            if (w >= 0.0f)
                return g;
            return 0.01f * g;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float AddLeakyReLUD(float t, float w, float g)
        {
            if (w >= 0.0f)
                return t + g;
            return t + 0.01f * g;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float SiLU(float w)
		{
			return w / (1.0f + (float)Math.Exp(-w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float SiLUD(float w, float resG)
		{
			float sig = 1.0f / (1.0f + (float)Math.Exp(-w));
			float grad = sig * (1.0f + w * (1.0f - sig));
			return resG * grad;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float AddSiLUD(float t, float w, float resG)
		{
			float sig = 1.0f / (1.0f + (float)Math.Exp(-w));
			float grad = sig * (1.0f + w * (1.0f - sig));
			return t + resG * grad;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float add(float x, float y)
		{
			return x + y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float mul(float x, float y)
		{
			return x * y;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float div(float x, float y)
		{
			return x / y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float sigmoid(float x)
		{
			return (float)(1.0 / (1.0 + Math.Exp(-x)));
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float sigmoidD(float resW, float resG)
		{
			return resW * (1.0f - resW) * resG;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float addSigmoidD(float t, float resW, float resG)
		{
			return t + resW * (1.0f - resW) * resG;
		}



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float mulmuladd(float x, float y, float z, float w)
		{
			return x * y + z * w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float clamp(float val, float min, float max)
		{
			if (val < min)
				return min;
			if (val > max)
				return max;
			return val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float addmul(float x, float y, float z)
		{
			return x + y * z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float adddiv(float x, float y, float z)
		{
			return x + y / z;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float addtanh(float x, float y)
		{
			return (float)Math.Tanh(x + y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float addtanhD(float t, float resW, float resG)
		{
			return t + (1.0f - resW * resW) * resG;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static float tanhD(float resW, float resG)
		{
			return (1.0f - resW * resW) * resG;
		}

		#endregion
	}
}
