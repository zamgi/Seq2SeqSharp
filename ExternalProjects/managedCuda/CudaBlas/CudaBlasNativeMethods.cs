﻿//	Copyright (c) 2012, Michael Kunz. All rights reserved.
//	http://kunzmi.github.io/managedCuda
//
//	This file is part of ManagedCuda.
//
//	ManagedCuda is free software: you can redistribute it and/or modify
//	it under the terms of the GNU Lesser General Public License as 
//	published by the Free Software Foundation, either version 2.1 of the 
//	License, or (at your option) any later version.
//
//	ManagedCuda is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//	GNU Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
//	MA 02110-1301  USA, http://www.gnu.org/licenses/.


using System;
using System.Runtime.InteropServices;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;

namespace ManagedCuda.CudaBlas
{
    /// <summary>
    /// C# wrapper for cublas_v2.h and cublas_api.h
    /// </summary>
    public static class CudaBlasNativeMethods
    {
        //32bit is no more supported, only 64 bit...
        internal const string CUBLAS_API_DLL_NAME = "cublas64_12";


#if (NETCOREAPP)
        internal const string CUBLAS_API_DLL_NAME_LINUX = "cublas";

        static CudaBlasNativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(CudaBlasNativeMethods).Assembly, ImportResolver);
        }

        private static IntPtr ImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr libHandle = IntPtr.Zero;

            if (libraryName == CUBLAS_API_DLL_NAME)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    NativeLibrary.TryLoad(CUBLAS_API_DLL_NAME_LINUX, assembly, DllImportSearchPath.SafeDirectories, out libHandle);
                }
            }
            //On Windows, use the default library name
            return libHandle;
        }
#endif

        #region Basics
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCreate_v2(ref CudaBlasHandle handle);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDestroy_v2(CudaBlasHandle handle);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetVersion_v2(CudaBlasHandle handle, ref int version);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetWorkspace_v2(CudaBlasHandle handle, CUdeviceptr workspace, SizeT workspaceSizeInBytes);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetStream_v2(CudaBlasHandle handle, CUstream streamId);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetStream_v2(CudaBlasHandle handle, ref CUstream streamId);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetPointerMode_v2(CudaBlasHandle handle, ref PointerMode mode);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetPointerMode_v2(CudaBlasHandle handle, PointerMode mode);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetAtomicsMode(CudaBlasHandle handle, ref AtomicsMode mode);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetAtomicsMode(CudaBlasHandle handle, AtomicsMode mode);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetMathMode(CudaBlasHandle handle, ref Math mode);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetMathMode(CudaBlasHandle handle, Math mode);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetSmCountTarget(CudaBlasHandle handle, ref int smCountTarget);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetSmCountTarget(CudaBlasHandle handle, int smCountTarget);

        #endregion

        #region Set and Get

        /// <summary>
        /// copies n elements from a vector x in CPU memory space to a vector y 
        /// in GPU memory space. Elements in both vectors are assumed to have a 
        /// size of elemSize bytes. Storage spacing between consecutive elements
        /// is incx for the source vector x and incy for the destination vector
        /// y. In general, y points to an object, or part of an object, allocated
        /// via cublasAlloc(). Column major format for two-dimensional matrices
        /// is assumed throughout CUBLAS. Therefore, if the increment for a vector 
        /// is equal to 1, this access a column vector while using an increment 
        /// equal to the leading dimension of the respective matrix accesses a 
        /// row vector.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetVector(int n, int elemSize, [In] IntPtr x, int incx, CUdeviceptr devicePtr, int incy);

        /// <summary>
        /// copies n elements from a vector x in GPU memory space to a vector y 
        /// in CPU memory space. Elements in both vectors are assumed to have a 
        /// size of elemSize bytes. Storage spacing between consecutive elements
        /// is incx for the source vector x and incy for the destination vector
        /// y. In general, x points to an object, or part of an object, allocated
        /// via cublasAlloc(). Column major format for two-dimensional matrices
        /// is assumed throughout CUBLAS. Therefore, if the increment for a vector 
        /// is equal to 1, this access a column vector while using an increment 
        /// equal to the leading dimension of the respective matrix accesses a 
        /// row vector.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetVector(int n, int elemSize, [In] CUdeviceptr x, int incx, IntPtr y, int incy);

        /// <summary>
        /// copies a tile of rows x cols elements from a matrix A in CPU memory
        /// space to a matrix B in GPU memory space. Each element requires storage
        /// of elemSize bytes. Both matrices are assumed to be stored in column 
        /// major format, with the leading dimension (i.e. number of rows) of 
        /// source matrix A provided in lda, and the leading dimension of matrix B
        /// provided in ldb. In general, B points to an object, or part of an 
        /// object, that was allocated via cublasAlloc().
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetMatrix(int rows, int cols, int elemSize, [In] IntPtr A, int lda, CUdeviceptr B, int ldb);

        /// <summary>
        /// copies a tile of rows x cols elements from a matrix A in GPU memory
        /// space to a matrix B in CPU memory space. Each element requires storage
        /// of elemSize bytes. Both matrices are assumed to be stored in column 
        /// major format, with the leading dimension (i.e. number of rows) of 
        /// source matrix A provided in lda, and the leading dimension of matrix B
        /// provided in ldb. In general, A points to an object, or part of an 
        /// object, that was allocated via cublasAlloc().
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetMatrix(int rows, int cols, int elemSize, [In] CUdeviceptr A, int lda, IntPtr B, int ldb);

        /// <summary>
        /// cublasSetVectorAsync has the same functionnality as cublasSetVector
        /// but the transfer is done asynchronously within the CUDA stream passed
        /// in parameter.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetVectorAsync(int n, int elemSize, [In] IntPtr hostPtr, int incx, CUdeviceptr devicePtr, int incy, CUstream stream);
        /// <summary>
        /// cublasGetVectorAsync has the same functionnality as cublasGetVector
        /// but the transfer is done asynchronously within the CUDA stream passed
        /// in parameter.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetVectorAsync(int n, int elemSize, [In] CUdeviceptr devicePtr, int incx, IntPtr hostPtr, int incy, CUstream stream);

        /// <summary>
        /// cublasSetMatrixAsync has the same functionnality as cublasSetMatrix
        /// but the transfer is done asynchronously within the CUDA stream passed
        /// in parameter.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetMatrixAsync(int rows, int cols, int elemSize, [In] IntPtr A, int lda, CUdeviceptr B, int ldb, CUstream stream);

        /// <summary>
        /// cublasGetMatrixAsync has the same functionnality as cublasGetMatrix
        /// but the transfer is done asynchronously within the CUDA stream passed
        /// in parameter.
        /// </summary>
        /// <returns>
        /// CudaBlas Error Codes: <see cref="CublasStatus.Success"/>, <see cref="CublasStatus.InvalidValue"/>,
        /// <see cref="CublasStatus.MappingError"/>, <see cref="CublasStatus.NotInitialized"/>.
        /// </returns>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetMatrixAsync(int rows, int cols, int elemSize, [In] CUdeviceptr A, int lda, IntPtr B, int ldb, CUstream stream);

        #endregion

        #region BLAS1
        #region host/device independent
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCopyEx(CudaBlasHandle handle,
                                                      int n,
                                                      [In] CUdeviceptr x,
                                                      cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr y,
                                                      cudaDataType yType,
                                                      int incy);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScopy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDcopy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCcopy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZcopy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSswap_v2(CudaBlasHandle handle,
                                         int n,
                                         CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDswap_v2(CudaBlasHandle handle,
                                         int n,
                                         CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCswap_v2(CudaBlasHandle handle,
                                         int n,
                                         CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZswap_v2(CudaBlasHandle handle,
                                         int n,
                                         CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSwapEx(CudaBlasHandle handle,
                                                      int n,
                                                      CUdeviceptr x,
                                                      cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr y,
                                                      cudaDataType yType,
                                                      int incy);

        #endregion

        #region Host pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasNrm2Ex(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     IntPtr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType); /* host or device pointer */

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDotEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     IntPtr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDotcEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     IntPtr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSnrm2_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        ref float result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDnrm2_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        ref double result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScnrm2_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref float result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDznrm2_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref double result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSdot_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        ref float result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDdot_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        ref double result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCdotu_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         ref cuFloatComplex result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCdotc_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         ref cuFloatComplex result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdotu_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         ref cuDoubleComplex result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdotc_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         ref cuDoubleComplex result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScalEx(CudaBlasHandle handle,
                                                     int n,
                                                     IntPtr alpha,  /* host or device pointer */
                                                     cudaDataType alphaType,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     cudaDataType executionType);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] ref float alpha,  // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] ref double alpha,  // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] ref cuFloatComplex alpha, // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsscal_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref float alpha, // host or device pointer
                                         CUdeviceptr x,
                                         int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] ref cuDoubleComplex alpha, // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdscal_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         CUdeviceptr x,
                                         int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasAxpyEx(CudaBlasHandle handle,
                                                      int n,
                                                      IntPtr alpha, /* host or device pointer */
                                                      cudaDataType alphaType,
                                                      CUdeviceptr x,
                                                      cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr y,
                                                      cudaDataType yType,
                                                      int incy,
                                                      cudaDataType executiontype);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref float alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIsamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIdamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIcamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIzamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIamaxEx(CudaBlasHandle handle,
                                                      int n,
                                                      [In] CUdeviceptr x, cudaDataType xType,
                                                      int incx,
                                                      ref int result  /* host or device pointer */
                                                    );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIsamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIdamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIcamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIzamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref int result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIaminEx(CudaBlasHandle handle,
                                                      int n,
                                                      [In] CUdeviceptr x, cudaDataType xType,
                                                      int incx,
                                                      ref int result /* host or device pointer */
                                                    );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasAsumEx(CudaBlasHandle handle,
                                                     int n,

                                                     [In] CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     IntPtr result,
                                                     cudaDataType resultType, /* host or device pointer */
                                                     cudaDataType executiontype
                                                  );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSasum_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        ref float result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDasum_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        ref double result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScasum_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref float result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDzasum_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         ref double result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref float c,  // host or device pointer
                                        [In] ref float s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref double c,  // host or device pointer
                                        [In] ref double s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref float c,      // host or device pointer
                                        [In] ref cuFloatComplex s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref float c,  // host or device pointer
                                        [In] ref float s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref double c,            // host or device pointer
                                        [In] ref cuDoubleComplex s);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] ref double c,  // host or device pointer
                                        [In] ref double s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     IntPtr c,  /* host or device pointer */
                                                     IntPtr s,
                                                     cudaDataType csType,
                                                     cudaDataType executiontype);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotg_v2(CudaBlasHandle handle,
                                        ref float a,   // host or device pointer
                                        ref float b,   // host or device pointer
                                        ref float c,   // host or device pointer
                                        ref float s);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotg_v2(CudaBlasHandle handle,
                                        ref double a,  // host or device pointer
                                        ref double b,  // host or device pointer
                                        ref double c,  // host or device pointer
                                        ref double s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCrotg_v2(CudaBlasHandle handle,
                                        ref cuFloatComplex a,  // host or device pointer
                                        ref cuFloatComplex b,  // host or device pointer
                                        ref float c,      // host or device pointer
                                        ref cuFloatComplex s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZrotg_v2(CudaBlasHandle handle,
                                        ref cuDoubleComplex a,  // host or device pointer
                                        ref cuDoubleComplex b,  // host or device pointer
                                        ref double c,           // host or device pointer
                                        ref cuDoubleComplex s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotgEx(CudaBlasHandle handle,
                                                     IntPtr a,   /* host or device pointer */
                                                     IntPtr b,   /* host or device pointer */
                                                     cudaDataType abType,
                                                     IntPtr c,   /* host or device pointer */
                                                     IntPtr s,   /* host or device pointer */
                                                     cudaDataType csType,
                                                     cudaDataType executiontype);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotm_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In, MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] float[] param);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotm_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In, MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] double[] param);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotmEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     IntPtr param, /* host or device pointer */
                                                     cudaDataType paramType,
                                                     cudaDataType executiontype);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotmg_v2(CudaBlasHandle handle,
                                         ref float d1,        // host or device pointer
                                         ref float d2,        // host or device pointer
                                         ref float x1,        // host or device pointer
                                         [In] ref float y1,  // host or device pointer
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] float[] param);    // host or device pointer

        /// <summary>
        /// </summary> 
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotmg_v2(CudaBlasHandle handle,
                                         ref double d1,        // host or device pointer  
                                         ref double d2,        // host or device pointer  
                                         ref double x1,        // host or device pointer  
                                         [In] ref double y1,  // host or device pointer  
                                         [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] double[] param);    // host or device pointer  

        /// <summary>
        /// </summary> 
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotmgEx(CudaBlasHandle handle,
                                                      IntPtr d1,        /* host or device pointer */
                                                      cudaDataType d1Type,
                                                      IntPtr d2,        /* host or device pointer */
                                                      cudaDataType d2Type,
                                                      IntPtr x1,        /* host or device pointer */
                                                      cudaDataType x1Type,
                                                      IntPtr y1,  /* host or device pointer */
                                                      cudaDataType y1Type,
                                                      IntPtr param,     /* host or device pointer */
                                                      cudaDataType paramType,
                                                      cudaDataType executiontype
                                                      );
        #endregion

        #region Device pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasNrm2Ex(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType); /* host or device pointer */

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDotEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     CUdeviceptr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDotcEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     CUdeviceptr result,
                                                     cudaDataType resultType,
                                                     cudaDataType executionType);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSnrm2_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDnrm2_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScnrm2_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDznrm2_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSdot_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDdot_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCdotu_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr result);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCdotc_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdotu_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdotc_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScalEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr alpha,  /* host or device pointer */
                                                     cudaDataType alphaType,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     cudaDataType executionType);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr alpha,  // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr alpha,  // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsscal_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         CUdeviceptr x,
                                         int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZscal_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        CUdeviceptr x,
                                        int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdscal_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         CUdeviceptr x,
                                         int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasAxpyEx(CudaBlasHandle handle,
                                                      int n,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      cudaDataType alphaType,
                                                      CUdeviceptr x,
                                                      cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr y,
                                                      cudaDataType yType,
                                                      int incy,
                                                      cudaDataType executiontype);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZaxpy_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIsamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIdamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIcamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIzamax_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIamaxEx(CudaBlasHandle handle,
                                                      int n,
                                                      [In] CUdeviceptr x, cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr result  /* host or device pointer */
                                                    );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIsamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIdamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIcamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIzamin_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasIaminEx(CudaBlasHandle handle,
                                                      int n,
                                                      [In] CUdeviceptr x, cudaDataType xType,
                                                      int incx,
                                                      CUdeviceptr result /* host or device pointer */
                                                    );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasAsumEx(CudaBlasHandle handle,
                                                     int n,

                                                     [In] CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr result,
                                                     cudaDataType resultType, /* host or device pointer */
                                                     cudaDataType executiontype
                                                  );

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSasum_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDasum_v2(CudaBlasHandle handle,
                                        int n,
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasScasum_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDzasum_v2(CudaBlasHandle handle,
                                         int n,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         CUdeviceptr result); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,  // host or device pointer
                                        [In] CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,  // host or device pointer
                                        [In] CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,      // host or device pointer
                                        [In] CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,  // host or device pointer
                                        [In] CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,            // host or device pointer
                                        [In] CUdeviceptr s);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdrot_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr c,  // host or device pointer
                                        [In] CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     CUdeviceptr c,  /* host or device pointer */
                                                     CUdeviceptr s,
                                                     cudaDataType csType,
                                                     cudaDataType executiontype);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotg_v2(CudaBlasHandle handle,
                                        CUdeviceptr a,   // host or device pointer
                                        CUdeviceptr b,   // host or device pointer
                                        CUdeviceptr c,   // host or device pointer
                                        CUdeviceptr s);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotg_v2(CudaBlasHandle handle,
                                        CUdeviceptr a,  // host or device pointer
                                        CUdeviceptr b,  // host or device pointer
                                        CUdeviceptr c,  // host or device pointer
                                        CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCrotg_v2(CudaBlasHandle handle,
                                        CUdeviceptr a,  // host or device pointer
                                        CUdeviceptr b,  // host or device pointer
                                        CUdeviceptr c,      // host or device pointer
                                        CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZrotg_v2(CudaBlasHandle handle,
                                        CUdeviceptr a,  // host or device pointer
                                        CUdeviceptr b,  // host or device pointer
                                        CUdeviceptr c,           // host or device pointer
                                        CUdeviceptr s); // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotgEx(CudaBlasHandle handle,
                                                     CUdeviceptr a,   /* host or device pointer */
                                                     CUdeviceptr b,   /* host or device pointer */
                                                     cudaDataType abType,
                                                     CUdeviceptr c,   /* host or device pointer */
                                                     CUdeviceptr s,   /* host or device pointer */
                                                     cudaDataType csType,
                                                     cudaDataType executiontype);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotm_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr param);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotm_v2(CudaBlasHandle handle,
                                        int n,
                                        CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr y,
                                        int incy,
                                        [In] CUdeviceptr param);  // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotmEx(CudaBlasHandle handle,
                                                     int n,
                                                     CUdeviceptr x,
                                                     cudaDataType xType,
                                                     int incx,
                                                     CUdeviceptr y,
                                                     cudaDataType yType,
                                                     int incy,
                                                     CUdeviceptr param, /* host or device pointer */
                                                     cudaDataType paramType,
                                                     cudaDataType executiontype);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSrotmg_v2(CudaBlasHandle handle,
                                         CUdeviceptr d1,        // host or device pointer
                                         CUdeviceptr d2,        // host or device pointer
                                         CUdeviceptr x1,        // host or device pointer
                                         [In] CUdeviceptr y1,  // host or device pointer
                                         CUdeviceptr param);    // host or device pointer

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDrotmg_v2(CudaBlasHandle handle,
                                         CUdeviceptr d1,        // host or device pointer
                                         CUdeviceptr d2,        // host or device pointer
                                         CUdeviceptr x1,        // host or device pointer
                                         [In] CUdeviceptr y1,  // host or device pointer
                                         CUdeviceptr param);    // host or device pointer  

        /// <summary>
        /// </summary> 
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasRotmgEx(CudaBlasHandle handle,
                                                      CUdeviceptr d1,        /* host or device pointer */
                                                      cudaDataType d1Type,
                                                      CUdeviceptr d2,        /* host or device pointer */
                                                      cudaDataType d2Type,
                                                      CUdeviceptr x1,        /* host or device pointer */
                                                      cudaDataType x1Type,
                                                      [In] CUdeviceptr y1,  /* host or device pointer */
                                                      cudaDataType y1Type,
                                                      CUdeviceptr param,     /* host or device pointer */
                                                      cudaDataType paramType,
                                                      cudaDataType executiontype
                                                      );
        #endregion
        #endregion

        #region BLAS2
        #region host/device independent
        #region TRMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);
        #endregion
        #region TBMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStbmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtbmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtbmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtbmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A, int lda,
                                         CUdeviceptr x, int incx);
        #endregion
        #region TPMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStpmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtpmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtpmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtpmv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP,
                                         CUdeviceptr x, int incx);
        #endregion
        #region TRSV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr A, int lda,
                                         CUdeviceptr x, int incx);
        #endregion
        #region TPSV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStpsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP,
                                         CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtpsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtpsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtpsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, [In] CUdeviceptr AP,
                                         CUdeviceptr x, int incx);
        #endregion
        #region TBSV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStbsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A,
                                         int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtbsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A,
                                         int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtbsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A,
                                         int lda, CUdeviceptr x, int incx);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtbsv_v2(CudaBlasHandle handle, FillMode uplo, Operation trans,
                                         DiagType diag, int n, int k, [In] CUdeviceptr A,
                                         int lda, CUdeviceptr x, int incx);
        #endregion
        #endregion

        #region host pointer
        #region GEMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] ref float alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref float beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref double beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region GBMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] ref float alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref float beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref double beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SYMV/HEMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref float alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref float beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref double beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChemv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhemv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha,  // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SBMV/HBMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] ref float alpha,   // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref float beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] ref double alpha,   // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref double beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SPMV/HPMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref float alpha,  // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref float beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref double beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuFloatComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] ref cuDoubleComplex beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region GER
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSger_v2(CudaBlasHandle handle,
                                        int m,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDger_v2(CudaBlasHandle handle,
                                        int m,
                                        int n,
                                        [In] ref double alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgeru_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgerc_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgeru_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgerc_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);
        #endregion
        #region SYR/HER
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref double alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref cuFloatComplex alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref cuDoubleComplex alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref double alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);
        #endregion
        #region SPR/HPR
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref double alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref double alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);
        #endregion
        #region SYR2/HER2
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr2_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref float alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref double alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr2_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] ref cuFloatComplex alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher2_v2(CudaBlasHandle handle,
                                         FillMode uplo, int n,
                                         [In] ref cuFloatComplex alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        #endregion
        #region SPR2/HPR2
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref float alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref double alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuFloatComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);
        #endregion
        #endregion

        #region device pointer
        #region GEMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region GBMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgbmv_v2(CudaBlasHandle handle,
                                         Operation trans,
                                         int m,
                                         int n,
                                         int kl,
                                         int ku,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SYMV/HEMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsymv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChemv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhemv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SBMV/HBMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha,   // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha,   // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhbmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region SPMV/HPMV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,   // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta,  // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpmv_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr AP,
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr beta, // host or device pointer
                                         CUdeviceptr y,
                                         int incy);
        #endregion
        #region GER
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSger_v2(CudaBlasHandle handle,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDger_v2(CudaBlasHandle handle,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgeru_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgerc_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgeru_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgerc_v2(CudaBlasHandle handle,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);
        #endregion
        #region SYR/HER
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr A,
                                        int lda);
        #endregion
        #region SPR/HPR
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpr_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        CUdeviceptr AP);
        #endregion
        #region SYR2/HER2
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr2_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr2_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        int n,
                                        [In] CUdeviceptr alpha, // host or device pointer
                                        [In] CUdeviceptr x,
                                        int incx,
                                        [In] CUdeviceptr y,
                                        int incy,
                                        CUdeviceptr A,
                                        int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher2_v2(CudaBlasHandle handle,
                                         FillMode uplo, int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr A,
                                         int lda);

        #endregion
        #region SPR2/HPR2
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSspr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDspr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha,  // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChpr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhpr2_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         int n,
                                         [In] CUdeviceptr alpha, // host or device pointer
                                         [In] CUdeviceptr x,
                                         int incx,
                                         [In] CUdeviceptr y,
                                         int incy,
                                         CUdeviceptr AP);
        #endregion
        #endregion
        #endregion

        #region BLAS3
        #region host pointer
        #region GEMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] ref float alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] ref float beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] ref double alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] ref double beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] ref cuFloatComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] ref cuFloatComplex beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3m(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      ref cuFloatComplex alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      ref cuFloatComplex beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3m(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemm3m(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      ref cuDoubleComplex alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      ref cuDoubleComplex beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemm3m(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3mEx(CudaBlasHandle handle,
                                                     Operation transa, Operation transb,
                                                     int m, int n, int k,
                                                     ref cuFloatComplex alpha,
                                                     CUdeviceptr A,
                                                     cudaDataType Atype,
                                                     int lda,
                                                     CUdeviceptr B,
                                                     cudaDataType Btype,
                                                     int ldb,
                                                     ref cuFloatComplex beta,
                                                     CUdeviceptr C,
                                                     cudaDataType Ctype,
                                                     int ldc);




        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] ref cuDoubleComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] ref cuDoubleComplex beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemm(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      ref half alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      ref half beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemm(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemmBatched(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      ref half alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      ref half beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc,
                                                      int batchCount);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemmBatched(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      int ldc,
                                                      int batchCount);

        /* IO in FP16/FP32, computation in float */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmEx(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      ref float alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      DataType Atype,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      DataType Btype,
                                                      int ldb,
                                                      ref float beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      DataType Ctype,
                                                      int ldc);



        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmEx(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      IntPtr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      cudaDataType Atype,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      cudaDataType Btype,
                                                      int ldb,
                                                      IntPtr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      cudaDataType Ctype,
                                                      int ldc,
                                                      ComputeType computeType,
                                                      GemmAlgo algo);
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmEx(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      cudaDataType Atype,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      cudaDataType Btype,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      cudaDataType Ctype,
                                                      int ldc,
                                                      ComputeType computeType,
                                                      GemmAlgo algo);

        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmEx(CudaBlasHandle handle,
                                                     Operation transa, Operation transb,
                                                     int m, int n, int k,
                                                     ref cuFloatComplex alpha,
                                                     CUdeviceptr A,
                                                     cudaDataType Atype,
                                                     int lda,
                                                     CUdeviceptr B,
                                                     cudaDataType Btype,
                                                     int ldb,
                                                     ref cuFloatComplex beta,
                                                     CUdeviceptr C,
                                                     cudaDataType Ctype,
                                                     int ldc);

        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmEx(CudaBlasHandle handle,
                                                     Operation transa, Operation transb,
                                                     int m, int n, int k,
                                                     CUdeviceptr alpha,
                                                     CUdeviceptr A,
                                                     cudaDataType Atype,
                                                     int lda,
                                                     CUdeviceptr B,
                                                     cudaDataType Btype,
                                                     int ldb,
                                                     CUdeviceptr beta,
                                                     CUdeviceptr C,
                                                     cudaDataType Ctype,
                                                     int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasUint8gemmBias(CudaBlasHandle handle,
                                                           Operation transa, Operation transb, Operation transc,
                                                           int m, int n, int k,
                                                           CUdeviceptr A, int A_bias, int lda,
                                                           CUdeviceptr B, int B_bias, int ldb,
                                                           CUdeviceptr C, int C_bias, int ldc,
                                                           int C_mult, int C_shift);

        /* SYRK */


        /* IO in FP16/FP32, computation in float */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmEx(CudaBlasHandle handle,
                                                      Operation transa,
                                                      Operation transb,
                                                      int m,
                                                      int n,
                                                      int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr A,
                                                      DataType Atype,
                                                      int lda,
                                                      CUdeviceptr B,
                                                      DataType Btype,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr C,
                                                      DataType Ctype,
                                                      int ldc);



        #endregion
        #region SYRK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyrk_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        Operation trans,
                                        int n,
                                        int k,
                                        [In] ref float alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] ref float beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] ref double alpha,  //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] ref double beta,  //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] ref cuFloatComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] ref cuFloatComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] ref cuDoubleComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] ref cuDoubleComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);


        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrkEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              ref cuFloatComplex alpha, /* host or device pointer */
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              ref cuFloatComplex beta, /* host or device pointer */
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);


        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrkEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              CUdeviceptr alpha, /* host or device pointer */
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              CUdeviceptr beta, /* host or device pointer */
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);

        /* IO in Int8 complex/cuComplex, computation in cuComplex, Gaussian math */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrk3mEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              ref cuFloatComplex alpha,
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              ref cuFloatComplex beta,
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);

        /* IO in Int8 complex/cuComplex, computation in cuComplex, Gaussian math */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrk3mEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              CUdeviceptr lpha,
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              CUdeviceptr beta,
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);

        #endregion
        #region HERK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] ref float alpha,  //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] ref float beta,   //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZherk_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        Operation trans,
                                        int n,
                                        int k,
                                        [In] ref double alpha,  //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] ref double beta,  //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        #endregion
        #region SYR2K
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref float alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref float beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref double alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref double beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref cuFloatComplex alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref cuFloatComplex beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref cuDoubleComplex alpha,  //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref cuDoubleComplex beta,  //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);
        #endregion
        #region HER2K
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref cuFloatComplex alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref float beta,   //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] ref cuDoubleComplex alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] ref double beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);


        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherkEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              ref float alpha,  /* host or device pointer */
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              ref float beta,   /* host or device pointer */
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);

        /* IO in Int8 complex/cuComplex, computation in cuComplex, Gaussian math */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherk3mEx(CudaBlasHandle handle,
                                                               FillMode uplo,
                                                               Operation trans,
                                                               int n,
                                                               int k,
                                                               ref float alpha,
                                                               CUdeviceptr A, cudaDataType Atype,
                                                               int lda,
                                                               ref float beta,
                                                               CUdeviceptr C,
                                                               cudaDataType Ctype,
                                                               int ldc);


        /* IO in Int8 complex/cuComplex, computation in cuComplex */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherkEx(CudaBlasHandle handle,
                                                              FillMode uplo,
                                                              Operation trans,
                                                              int n,
                                                              int k,
                                                              CUdeviceptr alpha,  /* host or device pointer */
                                                              CUdeviceptr A,
                                                              cudaDataType Atype,
                                                              int lda,
                                                              CUdeviceptr beta,   /* host or device pointer */
                                                              CUdeviceptr C,
                                                              cudaDataType Ctype,
                                                              int ldc);

        /* IO in Int8 complex/cuComplex, computation in cuComplex, Gaussian math */
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherk3mEx(CudaBlasHandle handle,
                                                               FillMode uplo,
                                                               Operation trans,
                                                               int n,
                                                               int k,
                                                               CUdeviceptr alpha,
                                                               CUdeviceptr A, cudaDataType Atype,
                                                               int lda,
                                                               CUdeviceptr beta,
                                                               CUdeviceptr C,
                                                               cudaDataType Ctype,
                                                               int ldc);

        /* SYR2K */

        #endregion
        #region SYRKX : eXtended SYRK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref float alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref float beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref double alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref double beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref cuFloatComplex alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref cuFloatComplex beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref cuDoubleComplex alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref cuDoubleComplex beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);
        #endregion

        #region HERKX : eXtended HERK         
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref cuFloatComplex alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref float beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZherkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] ref cuDoubleComplex alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] ref double beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);
        #endregion

        #region SYMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref float alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref float beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref double alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref double beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref cuFloatComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref cuFloatComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref cuDoubleComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        #endregion
        #region HEMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChemm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref cuFloatComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref cuFloatComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhemm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] ref cuDoubleComplex alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] ref cuDoubleComplex beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        #endregion
        #region TRSM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrsm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] ref float alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         CUdeviceptr B,
                                         int ldb);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrsm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] ref double alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         CUdeviceptr B,
                                         int ldb);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrsm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] ref cuFloatComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        CUdeviceptr B,
                                        int ldb);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrsm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] ref cuDoubleComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        CUdeviceptr B,
                                        int ldb);

        #endregion
        #region TRMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrmm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] ref float alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrmm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] ref double alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrmm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] ref cuFloatComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrmm_v2(CudaBlasHandle handle, SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] ref cuDoubleComplex alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        CUdeviceptr C,
                                        int ldc);
        #endregion
        #endregion

        #region device pointer
        #region GEMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] CUdeviceptr beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] CUdeviceptr beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] CUdeviceptr beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemm_v2(CudaBlasHandle handle,
                                        Operation transa,
                                        Operation transb,
                                        int m,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        [In] CUdeviceptr beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);
        #endregion
        #region SYRK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyrk_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        Operation trans,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr beta, //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha,  //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr beta,  //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyrk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);
        #endregion
        #region HERK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherk_v2(CudaBlasHandle handle,
                                         FillMode uplo,
                                         Operation trans,
                                         int n,
                                         int k,
                                         [In] CUdeviceptr alpha,  //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr beta,   //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZherk_v2(CudaBlasHandle handle,
                                        FillMode uplo,
                                        Operation trans,
                                        int n,
                                        int k,
                                        [In] CUdeviceptr alpha,  //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr beta,  //host or device pointer  
                                        CUdeviceptr C,
                                        int ldc);

        #endregion
        #region SYR2K
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyr2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha,  //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta,  //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);
        #endregion
        #region HER2K
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCher2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta,   //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZher2k_v2(CudaBlasHandle handle,
                                          FillMode uplo,
                                          Operation trans,
                                          int n,
                                          int k,
                                          [In] CUdeviceptr alpha, //host or device pointer  
                                          [In] CUdeviceptr A,
                                          int lda,
                                          [In] CUdeviceptr B,
                                          int ldb,
                                          [In] CUdeviceptr beta, //host or device pointer  
                                          CUdeviceptr C,
                                          int ldc);

        #endregion
        #region SYRKX : eXtended SYRK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsyrkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);
        #endregion

        #region HERKX : eXtended HERK
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCherkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZherkx(CudaBlasHandle handle,
                                                    FillMode uplo,
                                                    Operation trans,
                                                    int n,
                                                    int k,
                                                    [In] CUdeviceptr alpha, /* host or device pointer */
                                                    [In] CUdeviceptr A,
                                                    int lda,
                                                    [In] CUdeviceptr B,
                                                    int ldb,
                                                    [In] CUdeviceptr beta, /* host or device pointer */
                                                    [In] CUdeviceptr C,
                                                    int ldc);
        #endregion
        #region SYMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZsymm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        #endregion
        #region HEMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasChemm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZhemm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         [In] CUdeviceptr beta, //host or device pointer  
                                         CUdeviceptr C,
                                         int ldc);

        #endregion
        #region TRSM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrsm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         CUdeviceptr B,
                                         int ldb);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrsm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         CUdeviceptr B,
                                         int ldb);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrsm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        CUdeviceptr B,
                                        int ldb);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrsm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        CUdeviceptr B,
                                        int ldb);

        #endregion
        #region TRMM
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrmm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrmm_v2(CudaBlasHandle handle,
                                         SideMode side,
                                         FillMode uplo,
                                         Operation trans,
                                         DiagType diag,
                                         int m,
                                         int n,
                                         [In] CUdeviceptr alpha, //host or device pointer  
                                         [In] CUdeviceptr A,
                                         int lda,
                                         [In] CUdeviceptr B,
                                         int ldb,
                                         CUdeviceptr C,
                                         int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrmm_v2(CudaBlasHandle handle,
                                        SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        CUdeviceptr C,
                                        int ldc);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrmm_v2(CudaBlasHandle handle, SideMode side,
                                        FillMode uplo,
                                        Operation trans,
                                        DiagType diag,
                                        int m,
                                        int n,
                                        [In] CUdeviceptr alpha, //host or device pointer  
                                        [In] CUdeviceptr A,
                                        int lda,
                                        [In] CUdeviceptr B,
                                        int ldb,
                                        CUdeviceptr C,
                                        int ldc);
        #endregion
        #endregion
        #endregion

        #region CUBLAS BLAS-like extension
        #region GEAM
        #region device ptr
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);
        #endregion
        #region host ptr
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  ref float alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  ref float beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  ref double alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  ref double beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  ref cuFloatComplex alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  ref cuFloatComplex beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgeam(CudaBlasHandle handle,
                                                  Operation transa,
                                                  Operation transb,
                                                  int m,
                                                  int n,
                                                  ref cuDoubleComplex alpha, /* host or device pointer */
                                                  CUdeviceptr A,
                                                  int lda,
                                                  ref cuDoubleComplex beta, /* host or device pointer */
                                                  CUdeviceptr B,
                                                  int ldb,
                                                  CUdeviceptr C,
                                                  int ldc);
        #endregion
        #endregion

        #region Batched - MATINV
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSmatinvBatched(CudaBlasHandle handle,
                                                          int n,
                                                          CUdeviceptr A,                  /*Device pointer*/
                                                          int lda,
                                                          CUdeviceptr Ainv,               /*Device pointer*/
                                                          int lda_inv,
                                                          CUdeviceptr INFO,                   /*Device Pointer*/
                                                          int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDmatinvBatched(CudaBlasHandle handle,
                                                          int n,
                                                          CUdeviceptr A,                 /*Device pointer*/
                                                          int lda,
                                                          CUdeviceptr Ainv,              /*Device pointer*/
                                                          int lda_inv,
                                                          CUdeviceptr INFO,                   /*Device Pointer*/
                                                          int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCmatinvBatched(CudaBlasHandle handle,
                                                          int n,
                                                          CUdeviceptr A,              /*Device pointer*/
                                                          int lda,
                                                          CUdeviceptr Ainv,           /*Device pointer*/
                                                          int lda_inv,
                                                          CUdeviceptr INFO,                   /*Device Pointer*/
                                                          int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZmatinvBatched(CudaBlasHandle handle,
                                                          int n,
                                                          CUdeviceptr A,        /*Device pointer*/
                                                          int lda,
                                                          CUdeviceptr Ainv,     /*Device pointer*/
                                                          int lda_inv,
                                                          CUdeviceptr INFO,                   /*Device Pointer*/
                                                          int batchSize);

        #endregion

        #region DGMM

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSdgmm(CudaBlasHandle handle,
                                                  SideMode mode,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr x,
                                                  int incx,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDdgmm(CudaBlasHandle handle,
                                                  SideMode mode,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr x,
                                                  int incx,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCdgmm(CudaBlasHandle handle,
                                                  SideMode mode,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr x,
                                                  int incx,
                                                  CUdeviceptr C,
                                                  int ldc);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZdgmm(CudaBlasHandle handle,
                                                  SideMode mode,
                                                  int m,
                                                  int n,
                                                  CUdeviceptr A,
                                                  int lda,
                                                  CUdeviceptr x,
                                                  int incx,
                                                  CUdeviceptr C,
                                                  int ldc);
        #endregion
        #endregion
        //Ab hier NEU

        #region BATCH GEMM
        #region device pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   CUdeviceptr alpha,  /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   CUdeviceptr beta,   /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   CUdeviceptr alpha,  /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   CUdeviceptr beta,  /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   CUdeviceptr alpha, /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   CUdeviceptr beta, /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   CUdeviceptr alpha, /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   CUdeviceptr beta, /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        //Missing before:
        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3mBatched(CudaBlasHandle handle,
                                                          Operation transa,
                                                          Operation transb,
                                                          int m,
                                                          int n,
                                                          int k,
                                                          ref cuFloatComplex alpha, /* host or device pointer */
                                                          CUdeviceptr Aarray,
                                                          int lda,
                                                          CUdeviceptr Barray,
                                                          int ldb,
                                                          ref cuFloatComplex beta, /* host or device pointer */
                                                          CUdeviceptr Carray,
                                                          int ldc,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3mBatched(CudaBlasHandle handle,
                                                          Operation transa,
                                                          Operation transb,
                                                          int m,
                                                          int n,
                                                          int k,
                                                          CUdeviceptr alpha, /* host or device pointer */
                                                          CUdeviceptr Aarray,
                                                          int lda,
                                                          CUdeviceptr Barray,
                                                          int ldb,
                                                          CUdeviceptr beta, /* host or device pointer */
                                                          CUdeviceptr Carray,
                                                          int ldc,
                                                          int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3mStridedBatched(CudaBlasHandle handle,
                                                                         Operation transa,
                                                                         Operation transb,
                                                                         int m,
                                                                         int n,
                                                                         int k,
                                                                 ref cuFloatComplex alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref cuFloatComplex beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemm3mStridedBatched(CudaBlasHandle handle,
                                                                         Operation transa,
                                                                         Operation transb,
                                                                         int m,
                                                                         int n,
                                                                         int k,
                                                                 CUdeviceptr alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);




        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmBatchedEx(CudaBlasHandle handle,
                                                              Operation transa,
                                                              Operation transb,
                                                              int m,
                                                              int n,
                                                              int k,
                                                      CUdeviceptr alpha, /* host or device pointer */
                                                      CUdeviceptr Aarray,
                                                      cudaDataType Atype,
                                                      int lda,
                                                      CUdeviceptr Barray,
                                                      cudaDataType Btype,
                                                      int ldb,
                                                      CUdeviceptr beta, /* host or device pointer */
                                                      CUdeviceptr Carray,
                                                      cudaDataType Ctype,
                                                      int ldc,
                                                      int batchCount,
                                                      ComputeType computeType,
                                                      GemmAlgo algo);

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmStridedBatchedEx(CudaBlasHandle handle,
                                                                         Operation transa,
                                                                         Operation transb,
                                                                         int m,
                                                                         int n,
                                                                         int k,
                                                                 CUdeviceptr alpha,  /* host or device pointer */
                                                                 CUdeviceptr A,
                                                                 cudaDataType Atype,
                                                                 int lda,
                                                                 long strideA,   /* purposely signed */
                                                                 CUdeviceptr B,
                                                                 cudaDataType Btype,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   /* host or device pointer */
                                                                 CUdeviceptr C,
                                                                 cudaDataType Ctype,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount,
                                                                 ComputeType computeType,
                                                                 GemmAlgo algo);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 CUdeviceptr alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device pointer   
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 CUdeviceptr alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 CUdeviceptr alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 CUdeviceptr alpha,  // host or device poi$
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device poi$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 CUdeviceptr alpha,  // host or device poi$
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 CUdeviceptr beta,   // host or device poi$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);


        #endregion
        #region host pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   ref float alpha,  /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   ref float beta,   /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   ref double alpha,  /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   ref double beta,  /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   ref cuFloatComplex alpha, /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   ref cuFloatComplex beta, /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemmBatched(CudaBlasHandle handle,
                                   Operation transa,
                                   Operation transb,
                                   int m,
                                   int n,
                                   int k,
                                   ref cuDoubleComplex alpha, /* host or device pointer */
                                   CUdeviceptr Aarray,
                                   int lda,
                                   CUdeviceptr Barray,
                                   int ldb,
                                   ref cuDoubleComplex beta, /* host or device pointer */
                                   CUdeviceptr Carray,
                                   int ldc,
                                   int batchCount);

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmBatchedEx(CudaBlasHandle handle,
                                                              Operation transa,
                                                              Operation transb,
                                                              int m,
                                                              int n,
                                                              int k,
                                                      IntPtr alpha, /* host or device pointer */
                                                      CUdeviceptr Aarray,
                                                      cudaDataType Atype,
                                                      int lda,
                                                      CUdeviceptr Barray,
                                                      cudaDataType Btype,
                                                      int ldb,
                                                      IntPtr beta, /* host or device pointer */
                                                      CUdeviceptr Carray,
                                                      cudaDataType Ctype,
                                                      int ldc,
                                                      int batchCount,
                                                      cudaDataType computeType,
                                                      GemmAlgo algo);

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGemmStridedBatchedEx(CudaBlasHandle handle,
                                                                         Operation transa,
                                                                         Operation transb,
                                                                         int m,
                                                                         int n,
                                                                         int k,
                                                                 IntPtr alpha,  /* host or device pointer */
                                                                 CUdeviceptr A,
                                                                 cudaDataType Atype,
                                                                 int lda,
                                                                 long strideA,   /* purposely signed */
                                                                 CUdeviceptr B,
                                                                 cudaDataType Btype,
                                                                 int ldb,
                                                                 long strideB,
                                                                 IntPtr beta,   /* host or device pointer */
                                                                 CUdeviceptr C,
                                                                 cudaDataType Ctype,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount,
                                                                 cudaDataType computeType,
                                                                 GemmAlgo algo);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 ref float alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref float beta,   // host or device pointer   
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 ref double alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref double beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 ref cuFloatComplex alpha,  // host or device pointer
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref cuFloatComplex beta,   // host or device pointer$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 ref cuDoubleComplex alpha,  // host or device poi$
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref cuDoubleComplex beta,   // host or device poi$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasHgemmStridedBatched(CudaBlasHandle handle,
                                                                 Operation transa,
                                                                 Operation transb,
                                                                 int m,
                                                                 int n,
                                                                 int k,
                                                                 ref half alpha,  // host or device poi$
                                                                 CUdeviceptr A,
                                                                 int lda,
                                                                 long strideA,   // purposely signed 
                                                                 CUdeviceptr B,
                                                                 int ldb,
                                                                 long strideB,
                                                                 ref half beta,   // host or device poi$
                                                                 CUdeviceptr C,
                                                                 int ldc,
                                                                 long strideC,
                                                                 int batchCount);

        #endregion
        #endregion

        #region Batched LU - GETRF
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgetrfBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                      /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                          /*Device Pointer*/
                                                  CUdeviceptr INFO,                       /*Device Pointer*/
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgetrfBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                     /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                          /*Device Pointer*/
                                                  CUdeviceptr INFO,                       /*Device Pointer*/
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgetrfBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                 /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device Pointer*/
                                                  CUdeviceptr INFO,                      /*Device Pointer*/
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgetrfBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,           /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device Pointer*/
                                                  CUdeviceptr INFO,                      /*Device Pointer*/
                                                  int batchSize);

        #endregion

        #region Batched inversion based on LU factorization from getrf
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgetriBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                     /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device pointer*/
                                                  CUdeviceptr C,                     /*Device pointer*/
                                                  int ldc,
                                                  CUdeviceptr INFO,
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgetriBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                    /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device pointer*/
                                                  CUdeviceptr C,                    /*Device pointer*/
                                                  int ldc,
                                                  CUdeviceptr INFO,
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgetriBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,                 /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device pointer*/
                                                  CUdeviceptr C,                 /*Device pointer*/
                                                  int ldc,
                                                  CUdeviceptr INFO,
                                                  int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgetriBatched(CudaBlasHandle handle,
                                                  int n,
                                                  CUdeviceptr A,           /*Device pointer*/
                                                  int lda,
                                                  CUdeviceptr P,                         /*Device pointer*/
                                                  CUdeviceptr C,           /*Device pointer*/
                                                  int ldc,
                                                  CUdeviceptr INFO,
                                                  int batchSize);

        #endregion

        #region TRSM - Batched Triangular Solver
        #region device pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          CUdeviceptr alpha,           /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          CUdeviceptr alpha,          /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          CUdeviceptr alpha,       /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          CUdeviceptr salpha, /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);


        #endregion
        #region host pointer
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          ref float alpha,           /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          ref double alpha,          /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          ref cuFloatComplex alpha,       /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrsmBatched(CudaBlasHandle handle,
                                                          SideMode side,
                                                          FillMode uplo,
                                                          Operation trans,
                                                          DiagType diag,
                                                          int m,
                                                          int n,
                                                          ref cuDoubleComplex alpha, /*Host or Device Pointer*/
                                                          CUdeviceptr A,
                                                          int lda,
                                                          CUdeviceptr B,
                                                          int ldb,
                                                          int batchCount);


        #endregion
        #endregion


        #region TPTTR : Triangular Pack format to Triangular format
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStpttr(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr AP,
                                                     CUdeviceptr A,
                                                     int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtpttr(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr AP,
                                                     CUdeviceptr A,
                                                     int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtpttr(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr AP,
                                                     CUdeviceptr A,
                                                     int lda);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtpttr(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr AP,
                                                     CUdeviceptr A,
                                                     int lda);
        #endregion

        #region TRTTP : Triangular format to Triangular Pack format 
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasStrttp(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr A,
                                                     int lda,
                                                     CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDtrttp(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr A,
                                                     int lda,
                                                     CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCtrttp(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr A,
                                                     int lda,
                                                     CUdeviceptr AP);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZtrttp(CudaBlasHandle handle,
                                                     FillMode uplo,
                                                     int n,
                                                     CUdeviceptr A,
                                                     int lda,
                                                     CUdeviceptr AP);
        #endregion

        #region Batch QR Factorization

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgeqrfBatched(CudaBlasHandle handle,
                                                           int m,
                                                           int n,
                                                           CUdeviceptr Aarray,           /*Device pointer*/
                                                           int lda,
                                                           CUdeviceptr TauArray,        /* Device pointer*/
                                                           ref int info,
                                                           int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgeqrfBatched(CudaBlasHandle handle,
                                                            int m,
                                                            int n,
                                                            CUdeviceptr Aarray,           /*Device pointer*/
                                                            int lda,
                                                            CUdeviceptr TauArray,        /* Device pointer*/
                                                            ref int info,
                                                            int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgeqrfBatched(CudaBlasHandle handle,
                                                            int m,
                                                            int n,
                                                            CUdeviceptr Aarray,           /*Device pointer*/
                                                            int lda,
                                                            CUdeviceptr TauArray,        /* Device pointer*/
                                                            ref int info,
                                                            int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgeqrfBatched(CudaBlasHandle handle,
                                                            int m,
                                                            int n,
                                                            CUdeviceptr Aarray,           /*Device pointer*/
                                                            int lda,
                                                            CUdeviceptr TauArray,        /* Device pointer*/
                                                            ref int info,
                                                            int batchSize);
        #endregion

        #region Least Square Min only m >= n and Non-transpose supported
        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgelsBatched(CudaBlasHandle handle,
                                                           Operation trans,
                                                           int m,
                                                           int n,
                                                           int nrhs,
                                                           CUdeviceptr Aarray, /*Device pointer*/
                                                           int lda,
                                                           CUdeviceptr Carray, /* Device pointer*/
                                                           int ldc,
                                                           ref int info,
                                                           CUdeviceptr devInfoArray, /* Device pointer*/
                                                           int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgelsBatched(CudaBlasHandle handle,
                                                           Operation trans,
                                                           int m,
                                                           int n,
                                                           int nrhs,
                                                           CUdeviceptr Aarray, /*Device pointer*/
                                                           int lda,
                                                           CUdeviceptr Carray, /* Device pointer*/
                                                           int ldc,
                                                           ref int info,
                                                           CUdeviceptr devInfoArray, /* Device pointer*/
                                                           int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgelsBatched(CudaBlasHandle handle,
                                                           Operation trans,
                                                           int m,
                                                           int n,
                                                           int nrhs,
                                                           CUdeviceptr Aarray, /*Device pointer*/
                                                           int lda,
                                                           CUdeviceptr Carray, /* Device pointer*/
                                                           int ldc,
                                                           ref int info,
                                                           CUdeviceptr devInfoArray,
                                                           int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgelsBatched(CudaBlasHandle handle,
                                                           Operation trans,
                                                           int m,
                                                           int n,
                                                           int nrhs,
                                                           CUdeviceptr Aarray, /*Device pointer*/
                                                           int lda,
                                                           CUdeviceptr Carray, /* Device pointer*/
                                                           int ldc,
                                                           ref int info,
                                                           CUdeviceptr devInfoArray,
                                                           int batchSize);
        #endregion

        //New in Cuda 7.0

        #region Batched solver based on LU factorization from getrf

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSgetrsBatched(CudaBlasHandle handle,
                                                            Operation trans,
                                                            int n,
                                                            int nrhs,
                                                            CUdeviceptr Aarray,
                                                            int lda,
                                                            CUdeviceptr devIpiv,
                                                            CUdeviceptr Barray,
                                                            int ldb,
                                                            ref int info,
                                                            int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasDgetrsBatched(CudaBlasHandle handle,
                                                           Operation trans,
                                                           int n,
                                                           int nrhs,
                                                           CUdeviceptr Aarray,
                                                           int lda,
                                                           CUdeviceptr devIpiv,
                                                           CUdeviceptr Barray,
                                                           int ldb,
                                                           ref int info,
                                                           int batchSize);

        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasCgetrsBatched(CudaBlasHandle handle,
                                                            Operation trans,
                                                            int n,
                                                            int nrhs,
                                                            CUdeviceptr Aarray,
                                                            int lda,
                                                            CUdeviceptr devIpiv,
                                                            CUdeviceptr Barray,
                                                            int ldb,
                                                            ref int info,
                                                            int batchSize);


        /// <summary>
        /// </summary>
        [DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasZgetrsBatched(CudaBlasHandle handle,
                                                            Operation trans,
                                                            int n,
                                                            int nrhs,
                                                            CUdeviceptr Aarray,
                                                            int lda,
                                                            CUdeviceptr devIpiv,
                                                            CUdeviceptr Barray,
                                                            int ldb,
                                                            ref int info,
                                                            int batchSize);
        #endregion


        //New in Cuda 9.2

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasLoggerConfigure(int logIsOn, int logToStdOut, int logToStdErr, [MarshalAs(UnmanagedType.LPStr)] string logFileName);

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasSetLoggerCallback(cublasLogCallback userCallback);

        /// <summary>
		/// </summary>
		[DllImport(CUBLAS_API_DLL_NAME)]
        public static extern CublasStatus cublasGetLoggerCallback(ref cublasLogCallback userCallback);
    }
}
