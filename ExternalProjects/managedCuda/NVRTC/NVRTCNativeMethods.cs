﻿//	Copyright (c) 2014, Michael Kunz. All rights reserved.
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

namespace ManagedCuda.NVRTC
{
    /// <summary/>
    public static class NVRTCNativeMethods
    {
        internal const string NVRTC_API_DLL_NAME = "nvrtc64_112_0";

#if (NETCOREAPP)
        internal const string NVRTC_API_DLL_NAME_LINUX = "nvrtc";

        static NVRTCNativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(NVRTCNativeMethods).Assembly, ImportResolver);
        }

        private static IntPtr ImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr libHandle = IntPtr.Zero;

            if (libraryName == NVRTC_API_DLL_NAME)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    NativeLibrary.TryLoad(NVRTC_API_DLL_NAME_LINUX, assembly, DllImportSearchPath.SafeDirectories, out libHandle);
                }
            }
            //On Windows, use the default library name
            return libHandle;
        }
#endif

        [DllImport(NVRTC_API_DLL_NAME, EntryPoint = "nvrtcGetErrorString")]
        internal static extern IntPtr nvrtcGetErrorStringInternal(nvrtcResult result);

        /// <summary>
        /// helper function that stringifies the given #nvrtcResult code, e.g., NVRTC_SUCCESS to
        /// "NVRTC_SUCCESS". For unrecognized enumeration values, it returns "NVRTC_ERROR unknown"
        /// </summary>
        /// <param name="result">CUDA Runtime Compiler API result code.</param>
        /// <returns>Message string for the given nvrtcResult code.</returns>
        public static string nvrtcGetErrorString(nvrtcResult result)
        {
            IntPtr ptr = nvrtcGetErrorStringInternal(result);
            return Marshal.PtrToStringAnsi(ptr);
        }


        /// <summary>
        /// sets the output parameters \p major and \p minor
        /// with the CUDA Runtime Compiler version number.
        /// </summary>
        /// <param name="major">CUDA Runtime Compiler major version number.</param>
        /// <param name="minor">CUDA Runtime Compiler minor version number.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcVersion(ref int major, ref int minor);


        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetNumSupportedArchs(ref int numArchs);
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetSupportedArchs(int[] supportedArchs);

        /// <summary>
        /// creates an instance of ::nvrtcProgram with the
        /// given input parameters, and sets the output parameter \p prog with it.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="src">CUDA program source.</param>
        /// <param name="name">CUDA program name.<para/>
        /// name can be NULL; "default_program" is used when name is NULL.</param>
        /// <param name="numHeaders">Number of headers used.<para/>
        /// numHeaders must be greater than or equal to 0.</param>
        /// <param name="headers">Sources of the headers.<para/>
        /// headers can be NULL when numHeaders is 0.</param>
        /// <param name="includeNames">Name of each header by which they can be included in the CUDA program source.<para/>
        /// includeNames can be NULL when numHeaders is 0.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcCreateProgram(ref nvrtcProgram prog,
                               [MarshalAs(UnmanagedType.LPStr)] string src,
                               [MarshalAs(UnmanagedType.LPStr)] string name,
                               int numHeaders,
                               IntPtr[] headers,
                               IntPtr[] includeNames);




        /// <summary>
        /// destroys the given program.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcDestroyProgram(ref nvrtcProgram prog);

        /// <summary>
        /// compiles the given program.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="numOptions">Number of compiler options passed.</param>
        /// <param name="options">Compiler options in the form of C string array.<para/>
        /// options can be NULL when numOptions is 0.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcCompileProgram(nvrtcProgram prog, int numOptions, IntPtr[] options);

        /// <summary>
        /// sets \p ptxSizeRet with the size of the PTX generated by the previous compilation of prog (including the trailing NULL).
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="ptxSizeRet">Size of the generated PTX (including the trailing NULL).</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetPTXSize(nvrtcProgram prog, ref SizeT ptxSizeRet);

        /// <summary>
        /// stores the PTX generated by the previous compilation
        /// of prog in the memory pointed by ptx.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="ptx">Compiled result.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetPTX(nvrtcProgram prog, byte[] ptx);

        /// <summary>
        /// nvrtcGetCUBINSize sets \p cubinSizeRet with the size of the cubin
        /// generated by the previous compilation of \p prog.The value of
        /// cubinSizeRet is set to 0 if the value specified to \c -arch is a
        /// virtual architecture instead of an actual architecture.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compilation program.</param>
        /// <param name="cubinSizeRet">Size of the generated cubin.</param>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetCUBINSize(nvrtcProgram prog, ref SizeT cubinSizeRet);

        /// <summary>
        /// nvrtcGetCUBIN stores the cubin generated by the previous compilation
        /// of \p prog in the memory pointed by \p cubin.No cubin is available
        /// if the value specified to \c -arch is a virtual architecture instead
        /// of an actual architecture.
        /// </summary>
        /// <param name="prog">prog CUDA Runtime Compilation program.</param>
        /// <param name="cubin">cubin  Compiled and assembled result.</param>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetCUBIN(nvrtcProgram prog, byte[] cubin);

        /// <summary>
        /// nvrtcGetNVVMSize sets \p nvvmSizeRet with the size of the NVVM
        /// generated by the previous compilation of \p prog.The value of
        /// nvvmSizeRet is set to 0 if the program was not compiled with 
        /// -dlto.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compilation program.</param>
        /// <param name="nvvmSizeRet">Size of the generated NVVM.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetNVVMSize(nvrtcProgram prog, ref SizeT nvvmSizeRet);

        /// <summary>
        /// nvrtcGetNVVM stores the NVVM generated by the previous compilation
        /// of \p prog in the memory pointed by \p nvvm.
        /// The program must have been compiled with -dlto,
        /// otherwise will return an error.
        /// </summary>
        /// <param name="prog">prog CUDA Runtime Compilation program.</param>
        /// <param name="nvvm">nvvm Compiled result.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetNVVM(nvrtcProgram prog, byte[] nvvm);

        /// <summary>
        /// sets logSizeRet with the size of the log generated by the previous compilation of prog (including the trailing NULL).
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="logSizeRet">Size of the compilation log (including the trailing NULL).</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetProgramLogSize(nvrtcProgram prog, ref SizeT logSizeRet);

        /// <summary>
        /// stores the log generated by the previous compilation of prog in the memory pointed by log.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compiler program.</param>
        /// <param name="log">Compilation log.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetProgramLog(nvrtcProgram prog, byte[] log);


        /// <summary>
        /// nvrtcAddNameExpression notes the given name expression
        /// denoting a __global__ function or function template
        /// instantiation.<para/>
        /// The identical name expression string must be provided on a subsequent
        /// call to nvrtcGetLoweredName to extract the lowered name.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compilation program.</param>
        /// <param name="name_expression">constant expression denoting a __global__ function or function template instantiation.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcAddNameExpression(nvrtcProgram prog, [MarshalAs(UnmanagedType.LPStr)] string name_expression);


        /// <summary>
        /// nvrtcGetLoweredName extracts the lowered (mangled) name
        /// for a __global__ function or function template instantiation,
        /// and updates *lowered_name to point to it. The memory containing
        /// the name is released when the NVRTC program is destroyed by 
        /// nvrtcDestroyProgram.<para/>
        /// The identical name expression must have been previously
        /// provided to nvrtcAddNameExpression.
        /// </summary>
        /// <param name="prog">CUDA Runtime Compilation program.</param>
        /// <param name="name_expression">constant expression denoting a __global__ function or function template instantiation.</param>
        /// <param name="lowered_name">initialized by the function to point to a C string containing the lowered (mangled) name corresponding to the provided name expression.</param>
        /// <returns></returns>
        [DllImport(NVRTC_API_DLL_NAME)]
        public static extern nvrtcResult nvrtcGetLoweredName(nvrtcProgram prog,
                                        [MarshalAs(UnmanagedType.LPStr)] string name_expression, ref IntPtr lowered_name);

    }
}
