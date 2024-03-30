﻿// Copyright (c) Zhongkai Fu. All rights reserved.
// https://github.com/zhongkaifu/Seq2SeqSharp
//
// This file is part of Seq2SeqSharp.
//
// Seq2SeqSharp is licensed under the BSD-3-Clause license found in the LICENSE file in the root directory of this source tree.
//
// Seq2SeqSharp is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the BSD-3-Clause License for more details.

using AdvUtils;
using System;
using TensorSharp.Core;
using TensorSharp.CUDA.DeviceCode;
using TensorSharp.CUDA.KernelOps;
using TensorSharp.CUDA.MatrixMul;

namespace TensorSharp.CUDA
{
    [OpsClass]
    public class CudaBasicOps
    {
        private readonly CopyOps copyOps;

        private readonly ElementwiseKernels elementwiseKernels = new ElementwiseKernels();
        private readonly ElementwiseOpKernels elementwiseOpKernels = new ElementwiseOpKernels();
        private readonly ElementwiseTriKernels elementwiseTriKernels = new ElementwiseTriKernels();
        private readonly ElementwiseActKernels elementwiseActKernels = new ElementwiseActKernels();

        private readonly FillCopyKernels fillCopyKernels = new FillCopyKernels();

        private readonly CudaReduceKernels cudaReduceKernels = new CudaReduceKernels();
        private readonly CudaReduceAllKernels cudaReduceAllKernels = new CudaReduceAllKernels();

        private readonly VarStdKernels varStdKernels = new VarStdKernels();
        private readonly ReduceDimIndexKernels reduceDimIndexKernels = new ReduceDimIndexKernels();

        private readonly AdvFuncKernels advFuncKernels = new AdvFuncKernels();

        public CudaBasicOps()
        {
            copyOps = new CopyOps(fillCopyKernels);
        }


        
        //public Tensor NewContiguous(Tensor src)
        //{
        //    var result = new Tensor(src.Allocator, src.ElementType, (long[])src.Sizes.Clone());
        //    Copy(result, src);
        //    return result;
        //}

        //public Tensor AsContiguous(Tensor src)
        //{
        //    if (src.IsContiguous())
        //        return src.CopyRef();
        //    else
        //        return NewContiguous(src);
        //}

        public static Tensor Concat(Tensor result, int dimension, params Tensor[] inputs)
        {
            return TensorConcatenation.Concat(result, dimension, inputs);
        }


        //public float SumAll(Tensor src) { using (var resultTensor = SumAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }
        //public float ProdAll(Tensor src) { using (var resultTensor = ProdAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }
        //public float MinAll(Tensor src) { using (var resultTensor = MinAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }
        //public float MaxAll(Tensor src) { using (var resultTensor = MaxAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }

        //public float MeanAll(Tensor src) { using (var resultTensor = MeanAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }
        //public float VarAll(Tensor src) { using (var resultTensor = VarAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }
        //public float StdAll(Tensor src) { using (var resultTensor = StdAll(null, src)) { return resultTensor.GetElementAsFloat(0); } }

        //[RegisterOpStorageType("normall", typeof(CudaStorage))]
        //public float NormAll(Tensor src, float value) { using (var resultTensor = NormAll(null, src, value)) { return resultTensor.GetElementAsFloat(0); } }

      
        [RegisterOpArgCount("copy")]
        public void CopyGpu(
            [OpArgStorageType(typeof(CudaStorage))] Tensor result,
            [OpArgStorageType(typeof(CudaStorage))] Tensor src)
        {
            try
            {
                long totalElements = result.ElementCount();
                if (totalElements != src.ElementCount())
                {
                    throw new InvalidOperationException("Tensors must have equal numbers of elements");
                }

                if (src.DimensionCount == 0)
                {
                    return;
                }

                copyOps.CopyGpu(result, src, totalElements);
            }
            catch (Exception err)
            {
                Logger.WriteLine(Logger.Level.err, $"Error Message = '{err.Message}'.");
                Logger.WriteLine(Logger.Level.debug, $"Call Stack = '{err.StackTrace}'");
                throw;
            }
        }

        [RegisterOpArgCount("copy")]
        public void CopyCpuToGpu(
            [OpArgStorageType(typeof(CudaStorage))] Tensor result,
            [OpArgStorageType(typeof(Cpu.CpuStorage))] Tensor src)
        {
            try
            {
                long totalElements = result.ElementCount();
                if (totalElements != src.ElementCount())
                {
                    throw new InvalidOperationException("Tensors must have equal numbers of elements");
                }

                if (src.DimensionCount == 0)
                {
                    return;
                }

                copyOps.CopyCpuToGpu(result, src, totalElements);
            }
            catch (Exception err)
            {
                Logger.WriteLine(Logger.Level.err, $"Error Message = '{err.Message}'.");
                Logger.WriteLine(Logger.Level.debug, $"Call Stack = '{err.StackTrace}'");
                throw;
            }
        }

        [RegisterOpArgCount("copy")]
        public void CopyGpuToCpu(
            [OpArgStorageType(typeof(Cpu.CpuStorage))] Tensor result,
            [OpArgStorageType(typeof(CudaStorage))] Tensor src)
        {
            try
            {
                long totalElements = result.ElementCount();
                if (totalElements != src.ElementCount())
                {
                    throw new InvalidOperationException("Tensors must have equal numbers of elements");
                }

                if (src.DimensionCount == 0)
                {
                    return;
                }

                copyOps.CopyGpuToCpu(result, src, totalElements);
            }
            catch (Exception err)
            {
                Logger.WriteLine(Logger.Level.err, $"Error Message = '{err.Message}'.");
                Logger.WriteLine(Logger.Level.debug, $"Call Stack = '{err.StackTrace}'");
                throw;
            }
        }


        [RegisterOpStorageType("fill", typeof(CudaStorage))]
        public void Fill(Tensor result, float value)
        {
            FillOp.Invoke(fillCopyKernels, result, value);
        }


        [RegisterOpStorageType("dot", typeof(CudaStorage))]
        public static Tensor Dot(Tensor result, Tensor lhs, Tensor rhs)
        {
            TSCudaContext context = CudaHelpers.TSContextForTensor(lhs);
            if (lhs.DimensionCount == 1 && rhs.DimensionCount == 1)
            {
                return CudaMatrixMulDot.Dot(context, result, lhs, rhs);
            }
            else if (lhs.DimensionCount == 2 && rhs.DimensionCount == 1)
            {
                return CudaMatrixMulMV.Mul_M_V(context, result, lhs, rhs);
            }
            else if (lhs.DimensionCount == 2 && rhs.DimensionCount == 2)
            {
                return CudaMatrixMulMM.Mul_M_M(context, result, lhs, rhs);
            }
            else
            {
                throw new NotSupportedException(message: string.Format("Multiplication of {0}D with {1}D tensor is not supported"));
            }
        }

        [RegisterOpStorageType("addmm", typeof(CudaStorage))]
        public static Tensor Addmm(Tensor result, float beta, Tensor src, float alpha, Tensor m1, Tensor m2)
        {
            try
            {
                TSCudaContext context = CudaHelpers.TSContextForTensor(src);
                if (src.ElementType != m1.ElementType || src.ElementType != m2.ElementType || (result != null && result.ElementType != src.ElementType))
                {
                    throw new InvalidOperationException($"All tensors must have the same element type. src = '{src.ElementType}', m1 = '{m1.ElementType}', m2 = '{m2.ElementType}' result = '{result.ElementType}'");
                }

                if (result != null && !(result.Storage is CudaStorage))
                {
                    throw new ArgumentException("result must be a CUDA tensor", nameof(result));
                }

                if (!(m1.Storage is CudaStorage))
                {
                    throw new ArgumentException("m1 must be a CUDA tensor", nameof(m1));
                }

                if (!(m2.Storage is CudaStorage))
                {
                    throw new ArgumentException("m2 must be a CUDA tensor", nameof(m2));
                }

                if (src.DimensionCount != 2)
                {
                    throw new ArgumentException("src must be a matrix", nameof(src));
                }

                if (m1.DimensionCount != 2)
                {
                    throw new ArgumentException("m1 must be a matrix", nameof(m1));
                }

                if (m2.DimensionCount != 2)
                {
                    throw new ArgumentException("m2 must be a matrix", nameof(m2));
                }

                if (src.Sizes[0] != m1.Sizes[0] || src.Sizes[1] != m2.Sizes[1] || m1.Sizes[1] != m2.Sizes[0])
                {
                    throw new InvalidOperationException($"Size mismatch, srcSize0 = {src.Sizes[0]}, m1Size0 = {m1.Sizes[0]}, srcSize1 = {src.Sizes[1]}, m2Size1 = {m2.Sizes[1]}, m1Size1 = '{m1.Sizes[1]}', m2Size0 = '{m2.Sizes[0]}'");
                }

                Tensor writeTarget = TensorResultBuilder.GetWriteTarget(result, src, false, src.Sizes);

                if (writeTarget != src)
                {
                    Ops.Copy(writeTarget, src);
                }

                CudaMatrixMulMM.Gemm(context, alpha, m1, m2, beta, writeTarget);


                return writeTarget;
            }
            catch (Exception err)
            {
                Logger.WriteLine($"Exception in Addmm: '{err.Message}'");
                Logger.WriteLine($"Call stack: '{err.StackTrace}'");

                throw;
            }
        }



        [RegisterOpStorageType("addmmbatch", typeof(CudaStorage))]
        public static Tensor AddmmBatch(Tensor result, float beta, Tensor src, float alpha, Tensor m1, Tensor m2)
        {
            try
            {
                TSCudaContext context = CudaHelpers.TSContextForTensor(src);
                if (src.ElementType != m1.ElementType || src.ElementType != m2.ElementType || (result != null && result.ElementType != src.ElementType))
                {
                    throw new InvalidOperationException($"All tensors must have the same element type src = '{src.ElementType}', m1 = '{m1.ElementType}', m2 = '{m2.ElementType}' result = '{result.ElementType}'");
                }

                if (result != null && !(result.Storage is CudaStorage))
                {
                    throw new ArgumentException("result must be a CUDA tensor", nameof(result));
                }

                if (!(m1.Storage is CudaStorage))
                {
                    throw new ArgumentException("m1 must be a CUDA tensor", nameof(m1));
                }

                if (!(m2.Storage is CudaStorage))
                {
                    throw new ArgumentException("m2 must be a CUDA tensor", nameof(m2));
                }

                if (src.DimensionCount != 3)
                {
                    throw new ArgumentException("src must be a matrix", nameof(src));
                }

                if (m1.DimensionCount != 3)
                {
                    throw new ArgumentException("m1 must be a matrix", nameof(m1));
                }

                if (m2.DimensionCount != 3)
                {
                    throw new ArgumentException("m2 must be a matrix", nameof(m2));
                }

                if (src.Sizes[1] != m1.Sizes[1] || src.Sizes[2] != m2.Sizes[2] || m1.Sizes[2] != m2.Sizes[1])
                {
                    throw new InvalidOperationException($"Size mismatch, srcSize0 = {src.Sizes[0]}, m1Size0 = {m1.Sizes[0]}, srcSize1 = {src.Sizes[1]}, m2Size1 = {m2.Sizes[1]}, m1Size1 = '{m1.Sizes[1]}', m2Size0 = '{m2.Sizes[0]}'");
                }

                Tensor writeTarget = TensorResultBuilder.GetWriteTarget(result, src, true, src.Sizes);

                if (writeTarget != src)
                {
                    Ops.Copy(writeTarget, src);
                }

                CudaMatrixMulMM.GemmBatch(context, alpha, m1, m2, beta, writeTarget);


                return writeTarget;
            }
            catch (Exception err)
            {
                Logger.WriteLine(Logger.Level.err, $"Failed to run AddmmBatch on GPU. Error message = '{err.Message}'.");
                Logger.WriteLine(Logger.Level.debug, $"Call stack = '{err.StackTrace}'");
                throw;
            }
        }

        [RegisterOpStorageType("abs", typeof(CudaStorage))]
        public Tensor Abs(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "abs", result, src); }
        [RegisterOpStorageType("neg", typeof(CudaStorage))]
        public Tensor Neg(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "neg", result, src); }
        [RegisterOpStorageType("sign", typeof(CudaStorage))]
        public Tensor Sign(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "sign", result, src); }

        [RegisterOpStorageType("sqrt", typeof(CudaStorage))]
        public Tensor Sqrt(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "sqrt", result, src); }




        [RegisterOpStorageType("float2half", typeof(CudaStorage))]
        public Tensor Float2Half(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "float2half", result, src); }

        [RegisterOpStorageType("half2float", typeof(CudaStorage))]
        public Tensor Half2Float(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "half2float", result, src); }



        [RegisterOpStorageType("rsqrt", typeof(CudaStorage))]
        public Tensor Rsqrt(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "rsqrt", result, src); }


        [RegisterOpStorageType("exp", typeof(CudaStorage))]
        public Tensor Exp(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "exp", result, src); }
        [RegisterOpStorageType("log", typeof(CudaStorage))]
        public Tensor Log(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "log", result, src); }
        [RegisterOpStorageType("log1p", typeof(CudaStorage))]
        public Tensor Log1p(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "log1p", result, src); }
        [RegisterOpStorageType("floor", typeof(CudaStorage))]
        public Tensor Floor(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "floor", result, src); }
        [RegisterOpStorageType("ceil", typeof(CudaStorage))]
        public Tensor Ceil(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "ceil", result, src); }
        [RegisterOpStorageType("round", typeof(CudaStorage))]
        public Tensor Round(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "round", result, src); }
        [RegisterOpStorageType("trunc", typeof(CudaStorage))]
        public Tensor Trunc(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "trunc", result, src); }
        [RegisterOpStorageType("frac", typeof(CudaStorage))]
        public Tensor Frac(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseKernels, "frac", result, src); }

        [RegisterOpStorageType("sin", typeof(CudaStorage))]
        public Tensor Sin(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "sin", result, src); }
        [RegisterOpStorageType("cos", typeof(CudaStorage))]
        public Tensor Cos(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "cos", result, src); }
        [RegisterOpStorageType("tan", typeof(CudaStorage))]
        public Tensor Tan(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "tan", result, src); }

        [RegisterOpStorageType("asin", typeof(CudaStorage))]
        public Tensor Asin(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "asin", result, src); }
        [RegisterOpStorageType("acos", typeof(CudaStorage))]
        public Tensor Acos(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "acos", result, src); }
        [RegisterOpStorageType("atan", typeof(CudaStorage))]
        public Tensor Atan(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "atan", result, src); }

        [RegisterOpStorageType("sinh", typeof(CudaStorage))]
        public Tensor Sinh(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "sinh", result, src); }
        [RegisterOpStorageType("cosh", typeof(CudaStorage))]
        public Tensor Cosh(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "cosh", result, src); }
        [RegisterOpStorageType("tanh", typeof(CudaStorage))]
        public Tensor Tanh(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseTriKernels, "tanh", result, src); }

        [RegisterOpStorageType("addtanhD", typeof(CudaStorage))]
        public Tensor AddTanhD(Tensor result, Tensor t, Tensor resW, Tensor resG) { return ElementwiseTTTTOp.Invoke(elementwiseTriKernels, "addtanhD", result, t, resW, resG); }

        [RegisterOpStorageType("tanhD", typeof(CudaStorage))]
        public Tensor TanhD(Tensor result, Tensor resW, Tensor resG) { return ElementwiseTTTOp.Invoke(elementwiseTriKernels, "tanhD", result, resW, resG); }


        [RegisterOpStorageType("addtanh", typeof(CudaStorage))]
        public Tensor AddTanh(Tensor result, Tensor x, Tensor y) { return ElementwiseTTTOp.Invoke(elementwiseTriKernels, "addtanh", result, x, y); }


        [RegisterOpStorageType("addtanh3", typeof(CudaStorage))]
        public Tensor AddTanh3(Tensor result, Tensor x, Tensor y, Tensor z) { return ElementwiseTTTTOp.Invoke(elementwiseTriKernels, "addtanh3", result, x, y, z); }

        [RegisterOpStorageType("sigmoidD", typeof(CudaStorage))]
        public Tensor SigmoidD(Tensor result, Tensor resW, Tensor resG) { return ElementwiseTTTOp.Invoke(elementwiseActKernels, "sigmoidD", result, resW, resG); }

        [RegisterOpStorageType("sigmoid", typeof(CudaStorage))]
        public Tensor Sigmoid(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseActKernels, "sigmoid", result, src); }

        [RegisterOpStorageType("addsigmoidD", typeof(CudaStorage))]
        public Tensor AddSigmoidD(Tensor result, Tensor t, Tensor resW, Tensor resG) { return ElementwiseTTTTOp.Invoke(elementwiseActKernels, "addsigmoidD", result, t, resW, resG); }

        [RegisterOpStorageType("relu", typeof(CudaStorage))]
        public Tensor Relu(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseActKernels, "relu", result, src); }

        [RegisterOpStorageType("relud", typeof(CudaStorage))]
        public Tensor ReluD(Tensor result, Tensor w, Tensor g) { return ElementwiseTTTOp.Invoke(elementwiseActKernels, "relud", result, w, g); }

        [RegisterOpStorageType("addrelud", typeof(CudaStorage))]
        public Tensor AddReluD(Tensor result, Tensor t, Tensor w, Tensor g) { return ElementwiseTTTTOp.Invoke(elementwiseActKernels, "addrelud", result, t, w, g); }





        [RegisterOpStorageType("LeakyReLU", typeof(CudaStorage))]
        public Tensor LeakyReLU(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseActKernels, "LeakyReLU", result, src); }

        [RegisterOpStorageType("LeakyReLUD", typeof(CudaStorage))]
        public Tensor LeakyReLUD(Tensor result, Tensor w, Tensor g) { return ElementwiseTTTOp.Invoke(elementwiseActKernels, "LeakyReLUD", result, w, g); }

        [RegisterOpStorageType("AddLeakyReLUD", typeof(CudaStorage))]
        public Tensor AddLeakyReLUD(Tensor result, Tensor t, Tensor w, Tensor g) { return ElementwiseTTTTOp.Invoke(elementwiseActKernels, "AddLeakyReLUD", result, t, w, g); }






        [RegisterOpStorageType("SiLU", typeof(CudaStorage))]
        public Tensor SiLU(Tensor result, Tensor src) { return ElementwiseTTOp.Invoke(elementwiseActKernels, "SiLU", result, src); }

        [RegisterOpStorageType("SiLUD", typeof(CudaStorage))]
        public Tensor SiLUD(Tensor result, Tensor srcW, Tensor resG) { return ElementwiseTTTOp.Invoke(elementwiseActKernels, "SiLUD", result, srcW, resG); }

        [RegisterOpStorageType("AddSiLUD", typeof(CudaStorage))]
        public Tensor AddSiLUD(Tensor result, Tensor srcG, Tensor srcW, Tensor resG) { return ElementwiseTTTTOp.Invoke(elementwiseActKernels, "AddSiLUD", result, srcG, srcW, resG); }





        [RegisterOpStorageType("mulmuladd", typeof(CudaStorage))]
        public Tensor MulMulAdd(Tensor result, Tensor x, Tensor y, Tensor z, Tensor w) { return ElementwiseTTTTTOp.Invoke(elementwiseKernels, "mulmuladd", result, x, y, z, w); }

        [RegisterOpStorageType("addmul", typeof(CudaStorage))]
        public Tensor AddMul(Tensor result, Tensor x, Tensor y, Tensor z) { return ElementwiseTTTTOp.Invoke(elementwiseKernels, "addmul", result, x, y, z); }
        [RegisterOpStorageType("addmulv", typeof(CudaStorage))]
        public Tensor AddMulV(Tensor result, Tensor x, Tensor y, float z) { return ElementwiseTTTSOp.Invoke(elementwiseKernels, "addmulv", result, x, y, z); }


        [RegisterOpStorageType("adddiv", typeof(CudaStorage))]
        public Tensor AddDiv(Tensor result, Tensor x, Tensor y, Tensor z) { return ElementwiseTTTTOp.Invoke(elementwiseKernels, "adddiv", result, x, y, z); }


        [RegisterOpStorageType("maskfill", typeof(CudaStorage))]
        public Tensor MaskFill(Tensor result, Tensor t, Tensor mask, float defValue) { return ElementwiseTTTSOp.Invoke(elementwiseKernels, "maskfill", result, t, mask, defValue); }



        [RegisterOpStorageType("atan2", typeof(CudaStorage))]
        public Tensor Atan2(Tensor result, Tensor srcY, Tensor srcX) { return Atan2Op.Invoke(elementwiseTriKernels, result, srcY, srcX); }
        [RegisterOpStorageType("pow", typeof(CudaStorage))]
        public Tensor Pow(Tensor result, Tensor src, float value) { return ElementwiseTTSOp.Invoke(elementwiseKernels, "pow", result, src, value); }
        [RegisterOpStorageType("tpow", typeof(CudaStorage))]
        public Tensor Tpow(Tensor result, float value, Tensor src) { return ElementwiseTTSOp.Invoke(elementwiseKernels, "tpow", result, src, value); }
        [RegisterOpStorageType("lerp", typeof(CudaStorage))]
        public Tensor Lerp(Tensor result, Tensor srcA, Tensor srcB, float weight) { return LerpOp.Invoke(elementwiseKernels, result, srcA, srcB, weight); }
        [RegisterOpStorageType("clamp", typeof(CudaStorage))]
        public Tensor Clamp(Tensor result, Tensor src, float min, float max) { return ClampOp.Invoke(elementwiseKernels, result, src, min, max); }

        [RegisterOpStorageType("addv", typeof(CudaStorage))]
        public Tensor Add(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "add", result, rhs, lhs); }
        [RegisterOpStorageType("subv", typeof(CudaStorage))]
        public Tensor Sub(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "sub", result, rhs, lhs); }
        [RegisterOpStorageType("rsubv", typeof(CudaStorage))]
        public Tensor Sub(Tensor result, float rhs, Tensor lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "rsub", result, lhs, rhs); }
        [RegisterOpStorageType("mulv", typeof(CudaStorage))]
        public Tensor Mul(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "mul", result, rhs, lhs); }
        [RegisterOpStorageType("divv", typeof(CudaStorage))]
        public Tensor Div(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "div", result, rhs, lhs); }
        [RegisterOpStorageType("rdivv", typeof(CudaStorage))]
        public Tensor Div(Tensor result, float rhs, Tensor lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "rdiv", result, lhs, rhs); }
        [RegisterOpStorageType("modv", typeof(CudaStorage))]
        public Tensor Mod(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "mod", result, rhs, lhs); }

        [RegisterOpStorageType("gtValue", typeof(CudaStorage))]
        public Tensor GreaterThan(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "gt", result, rhs, lhs); }
        [RegisterOpStorageType("ltValue", typeof(CudaStorage))]
        public Tensor LessThan(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "lt", result, rhs, lhs); }
        [RegisterOpStorageType("geValue", typeof(CudaStorage))]
        public Tensor GreaterOrEqual(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "ge", result, rhs, lhs); }
        [RegisterOpStorageType("leValue", typeof(CudaStorage))]
        public Tensor LessOrEqual(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "le", result, rhs, lhs); }
        [RegisterOpStorageType("eqValue", typeof(CudaStorage))]
        public Tensor EqualTo(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "eq", result, rhs, lhs); }
        [RegisterOpStorageType("neValue", typeof(CudaStorage))]
        public Tensor NotEqual(Tensor result, Tensor rhs, float lhs) { return ElementwiseTTSOp.Invoke(elementwiseOpKernels, "ne", result, rhs, lhs); }


        [RegisterOpStorageType("addt", typeof(CudaStorage))]
        public Tensor Add(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cadd", result, rhs, lhs); }


        [RegisterOpStorageType("atomicadd", typeof(CudaStorage))]
        public Tensor AtomicAdd(Tensor result, Tensor rhs) { return ElementwiseAtomicAddOp.Invoke(elementwiseOpKernels, result, rhs); }


        [RegisterOpStorageType("subt", typeof(CudaStorage))]
        public Tensor Sub(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "csub", result, rhs, lhs); }
        
        [RegisterOpStorageType("mult", typeof(CudaStorage))]
        public Tensor Mul(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cmul", result, rhs, lhs); }
        
        [RegisterOpStorageType("divt", typeof(CudaStorage))]
        public Tensor Div(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cdiv", result, rhs, lhs); }
        
        [RegisterOpStorageType("modt", typeof(CudaStorage))]
        public Tensor Mod(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cmod", result, rhs, lhs); }

        [RegisterOpStorageType("gtTensor", typeof(CudaStorage))]
        public Tensor GreaterThan(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cgt", result, rhs, lhs); }
        [RegisterOpStorageType("ltTensor", typeof(CudaStorage))]
        public Tensor LessThan(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "clt", result, rhs, lhs); }
        [RegisterOpStorageType("geTensor", typeof(CudaStorage))]
        public Tensor GreaterOrEqual(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cge", result, rhs, lhs); }
        [RegisterOpStorageType("leTensor", typeof(CudaStorage))]
        public Tensor LessOrEqual(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cle", result, rhs, lhs); }
        [RegisterOpStorageType("eqTensor", typeof(CudaStorage))]
        public Tensor EqualTo(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "ceq", result, rhs, lhs); }
        [RegisterOpStorageType("neTensor", typeof(CudaStorage))]
        public Tensor NotEqual(Tensor result, Tensor rhs, Tensor lhs) { return ElementwiseTTTOp.Invoke(elementwiseOpKernels, "cne", result, rhs, lhs); }


        [RegisterOpStorageType("sum", typeof(CudaStorage))]
        public Tensor Sum(Tensor result, Tensor src, int dimension) { return ReductionOp.Invoke(cudaReduceKernels, "sum", 0.0f, ReduceInitType.GivenValue, result, src, dimension); }
        [RegisterOpStorageType("prod", typeof(CudaStorage))]
        public Tensor Prod(Tensor result, Tensor src, int dimension) { return ReductionOp.Invoke(cudaReduceKernels, "prod", 1.0f, ReduceInitType.GivenValue, result, src, dimension); }
        [RegisterOpStorageType("min", typeof(CudaStorage))]
        public Tensor Min(Tensor result, Tensor src, int dimension) { return ReductionOp.Invoke(cudaReduceKernels, "min", 0.0f, ReduceInitType.MaxValue, result, src, dimension); }
        [RegisterOpStorageType("max", typeof(CudaStorage))]
        public Tensor Max(Tensor result, Tensor src, int dimension) { return ReductionOp.Invoke(cudaReduceKernels, "max", 0.0f, ReduceInitType.MinValue, result, src, dimension); }

        [RegisterOpStorageType("argmin", typeof(CudaStorage))]
        public Tensor Argmin(Tensor result, Tensor src, int dimension) { return reduceDimIndexKernels.ArgMin(result, src, dimension); }

        [RegisterOpStorageType("argmax", typeof(CudaStorage))]
        public Tensor Argmax(Tensor result, Tensor src, int dimension) { return reduceDimIndexKernels.ArgMax(result, src, dimension); }


        [RegisterOpStorageType("mean", typeof(CudaStorage))]
        public Tensor Mean(Tensor result, Tensor src, int dimension)
        {
            long[] requiredOutputSize = (long[])src.Sizes.Clone();
            requiredOutputSize[dimension] = 1;
            Tensor writeTarget = TensorResultBuilder.GetWriteTarget(result, src, false, requiredOutputSize);

            Sum(writeTarget, src, dimension);
            Div(writeTarget, writeTarget, src.Sizes[dimension]);
            return writeTarget;
        }

        [RegisterOpStorageType("norm", typeof(CudaStorage))]
        public Tensor Norm(Tensor result, Tensor src, int dimension, float value)
        {
            if (value == 0)
            {
                return ReductionOp.Invoke(cudaReduceKernels, "e0_norm", 0.0f, ReduceInitType.GivenValue, result, src, dimension);
            }
            else if (value == 1)
            {
                return ReductionOp.Invoke(cudaReduceKernels, "e1_norm", 0.0f, ReduceInitType.GivenValue, result, src, dimension);
            }
            else if (value == 2)
            {
                Tensor writeTarget = ReductionOp.Invoke(cudaReduceKernels, "e2_norm", 0.0f, ReduceInitType.GivenValue, result, src, dimension);
                Pow(writeTarget, writeTarget, 0.5f);
                return writeTarget;
            }
            else
            {
                Tensor writeTarget = ReductionOp.Invoke(cudaReduceKernels, "en_norm", 0.0f, ReduceInitType.GivenValue, result, src, dimension, value);
                Pow(writeTarget, writeTarget, 1.0f / value);
                return writeTarget;
            }
        }

        [RegisterOpStorageType("std", typeof(CudaStorage))]
        public Tensor Std(Tensor result, Tensor src, int dimension, bool normByN) { return varStdKernels.Std(result, src, dimension, normByN); }
        [RegisterOpStorageType("var", typeof(CudaStorage))]
        public Tensor Var(Tensor result, Tensor src, int dimension, bool normByN) { return varStdKernels.Var(result, src, dimension, normByN); }



        [RegisterOpStorageType("indexselect", typeof(CudaStorage))]
        public Tensor IndexSelect(Tensor result, Tensor src, Tensor indice, bool isAdd) { return advFuncKernels.IndexSelect(result, src, indice, isAdd); }

        [RegisterOpStorageType("indexselectgrad", typeof(CudaStorage))]
        public Tensor IndexSelectGrad(Tensor grad, Tensor adj, Tensor indice) { return advFuncKernels.IndexSelectGrad(grad, adj, indice); }



        [RegisterOpStorageType("rope", typeof(CudaStorage))]
        public Tensor RoPE(Tensor result, Tensor src, int seqLen) { return advFuncKernels.RoPE(result, src, seqLen); }

        [RegisterOpStorageType("ropegrad", typeof(CudaStorage))]
        public Tensor RoPEGrad(Tensor grad, Tensor adj, int seqLen) { return advFuncKernels.RoPEGrad(grad, adj, seqLen); }




        [RegisterOpStorageType("buildsrctgtmask", typeof(CudaStorage))]
        public Tensor BuildSrcTgtMask(Tensor result, Tensor srcOriginalLengths, Tensor tgtOriginalLengths, int srcPaddedSeqLength, int tgtPaddedSeqLength, float value, float maskedValue)
        {
            return advFuncKernels.BuildSrcTgtMask(result, srcOriginalLengths, tgtOriginalLengths, srcPaddedSeqLength, tgtPaddedSeqLength, value, maskedValue);
        }


        [RegisterOpStorageType("buildselfmask", typeof(CudaStorage))]
        public Tensor BuildSelfMask(Tensor result, Tensor originalLengths, int paddedSeqLength, float value, float maskedValue)
        {
            return advFuncKernels.BuildSelfMask(result, originalLengths, paddedSeqLength, value, maskedValue);
        }


        [RegisterOpStorageType("buildselftrimask", typeof(CudaStorage))]
        public Tensor BuildSelfTriMask(Tensor result, Tensor originalLengths, int paddedSeqLength, float value, float maskedValue)
        {
            return advFuncKernels.BuildSelfTriMask(result, originalLengths, paddedSeqLength, value, maskedValue);
        }

        [RegisterOpStorageType("buildtrimask", typeof(CudaStorage))]
        public Tensor BuildTriMask(Tensor result, float value, float maskedValue)
        {
            return advFuncKernels.BuildTriMask(result, value, maskedValue);
        }


        [RegisterOpStorageType("iscorrupted", typeof(CudaStorage))]
        public bool IsCorrupted(Tensor src) { return advFuncKernels.IsCorrupted(src); }

        [RegisterOpStorageType("softmax", typeof(CudaStorage))]
        public Tensor Softmax(Tensor result, Tensor src) { return advFuncKernels.Softmax(result, src); }

        [RegisterOpStorageType("softmaxgrad", typeof(CudaStorage))]
        public Tensor SoftmaxGrad(Tensor grad, Tensor adj, Tensor val, bool addGrad = true) { return advFuncKernels.SoftmaxGrad(grad, adj, val, addGrad); }


        [RegisterOpStorageType("topK", typeof(CudaStorage))]
        public Tensor TopK(Tensor outVal, Tensor outIdx, Tensor inVal, int k) { return advFuncKernels.TopK(outVal, outIdx, inVal, k); }


        [RegisterOpStorageType("layernorm", typeof(CudaStorage))]
        public Tensor LayerNorm(Tensor result, Tensor src, Tensor alpha, Tensor beta, float eps = 1e-09f) { return advFuncKernels.LayerNorm(result, src, alpha, beta, eps); }
        [RegisterOpStorageType("layernormgrad", typeof(CudaStorage))]
        public Tensor LayerNormGrad(Tensor outGrad, Tensor alphaGrad, Tensor betaGrad, Tensor inGrad, Tensor y, Tensor x, Tensor alpha, Tensor beta, float eps = 1e-09f) { return advFuncKernels.LayerNormGrad(outGrad, alphaGrad, betaGrad, inGrad, y, x, alpha, beta, eps); }



        [RegisterOpStorageType("rmsnorm", typeof(CudaStorage))]
        public Tensor RMSNorm(Tensor result, Tensor src, Tensor alpha, Tensor beta, float eps = 1e-09f) { return advFuncKernels.RMSNorm(result, src, alpha, beta, eps); }
        [RegisterOpStorageType("rmsnormgrad", typeof(CudaStorage))]
        public Tensor RMSNormGrad(Tensor outGrad, Tensor alphaGrad, Tensor betaGrad, Tensor inGrad, Tensor y, Tensor x, Tensor alpha, Tensor beta, float eps = 1e-09f) { return advFuncKernels.RMSNormGrad(outGrad, alphaGrad, betaGrad, inGrad, y, x, alpha, beta, eps); }




        [RegisterOpStorageType("addlayernorm", typeof(CudaStorage))]
        public Tensor AddLayerNorm(Tensor result, Tensor src1, Tensor src2, Tensor alpha, Tensor beta, float eps = 1e-09f) { return advFuncKernels.AddLayerNorm(result, src1, src2, alpha, beta, eps); }
        [RegisterOpStorageType("addlayernormgrad", typeof(CudaStorage))]
        public void AddLayerNormGrad(Tensor out1Grad, Tensor out2Grad, Tensor alphaGrad, Tensor betaGrad, Tensor inGrad, Tensor y, Tensor x1, Tensor x2, Tensor alpha, Tensor beta, float eps = 1e-09f) { advFuncKernels.AddLayerNormGrad(out1Grad, out2Grad, alphaGrad, betaGrad, inGrad, y, x1, x2, alpha, beta, eps); }

        [RegisterOpStorageType("adam", typeof(CudaStorage))]
        public Tensor Adam(Tensor weight, Tensor gradient, Tensor v, Tensor m, float gradNormFactor, float step_size, float clipval, float regc, float decay_rate_v, float decay_rate_m, int iter, float eps)
        {
            return advFuncKernels.Adam(weight, gradient, v, m, gradNormFactor, step_size, clipval, regc, decay_rate_v, decay_rate_m, iter, eps);
        }


        [RegisterOpStorageType("rmsprop", typeof(CudaStorage))]
        public Tensor RMSProp(Tensor weight, Tensor gradient, Tensor cache, float gradNormFactor, float step_size, float clipval, float regc, float decay_rate, float eps)
        {
            return advFuncKernels.RMSProp(weight, gradient, cache, gradNormFactor, step_size, clipval, regc, decay_rate, eps);
        }


        [RegisterOpStorageType("sumall", typeof(CudaStorage))]
        public Tensor SumAll(Tensor result, Tensor src)
        {
            return ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "sumAll", result, src);
        }

        [RegisterOpStorageType("prodall", typeof(CudaStorage))]
        public Tensor ProdAll(Tensor result, Tensor src)
        {
            return ReduceAllOp.Invoke(cudaReduceAllKernels, 1.0f, ReduceInitType.GivenValue, "prodAll", result, src);
        }

        [RegisterOpStorageType("minall", typeof(CudaStorage))]
        public Tensor MinAll(Tensor result, Tensor src)
        {
            return ReduceAllOp.Invoke(cudaReduceAllKernels, 0, ReduceInitType.MaxValue, "minAll", result, src);
        }

        [RegisterOpStorageType("maxall", typeof(CudaStorage))]
        public Tensor MaxAll(Tensor result, Tensor src)
        {
            return ReduceAllOp.Invoke(cudaReduceAllKernels, 0, ReduceInitType.MinValue, "maxAll", result, src);
        }


        [RegisterOpStorageType("meanall", typeof(CudaStorage))]
        public Tensor MeanAll(Tensor result, Tensor src)
        {
            if (src.DimensionCount == 0 || src.ElementCount() == 0)
            {
                throw new ArgumentException("src must be a non-empty tensor");
            }

            Tensor writeTarget = TensorResultBuilder.GetWriteTarget(result, src, false, 1);
            SumAll(writeTarget, src);
            Div(writeTarget, writeTarget, src.ElementCount());
            return writeTarget;
        }

        [RegisterOpStorageType("normall", typeof(CudaStorage))]
        public Tensor NormAll(Tensor result, Tensor src, float value)
        {
            if (value == 0)
            {
                return ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "e0_normAll", result, src);
            }
            else if (value == 1)
            {
                return ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "e1_normAll", result, src);
            }
            else if (value == 2)
            {

                Tensor writeTarget = ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "e2_normAll", result, src);
                Pow(writeTarget, writeTarget, 0.5f);
                return writeTarget;
            }
            else
            {
                Tensor writeTarget = ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "en_normAll", result, src, value);
                Pow(writeTarget, writeTarget, 1.0f / value);
                return writeTarget;
            }
        }


        [RegisterOpStorageType("varall", typeof(CudaStorage))]
        public Tensor VarAll(Tensor result, Tensor src)
        {
            if (src.DimensionCount == 0 || src.ElementCount() == 0)
            {
                throw new ArgumentException("src must be a non-empty tensor");
            }

            float mean = Ops.MeanAll(src);
            Tensor writeTarget = ReduceAllOp.Invoke(cudaReduceAllKernels, 0.0f, ReduceInitType.GivenValue, "en_norm", result, src, mean);
            Div(writeTarget, writeTarget, src.ElementCount() - 1);
            return writeTarget;
        }

        [RegisterOpStorageType("stdall", typeof(CudaStorage))]
        public Tensor StdAll(Tensor result, Tensor src)
        {
            Tensor writeTarget = VarAll(result, src);
            Pow(writeTarget, writeTarget, 0.5f);
            return writeTarget;
        }

    }
}
