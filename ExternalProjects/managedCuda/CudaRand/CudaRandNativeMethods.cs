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

namespace ManagedCuda.CudaRand
{
	/// <summary>
	/// C# Wrapper-Methods for CuRand functions defined in curand.h
	/// </summary>
	public static class CudaRandNativeMethods
	{
		internal const string CURAND_API_DLL_NAME = "curand64_10";

#if (NETCOREAPP)
		internal const string CURAND_API_DLL_NAME_LINUX = "curand";

		static CudaRandNativeMethods()
		{
			NativeLibrary.SetDllImportResolver(typeof(CudaRandNativeMethods).Assembly, ImportResolver);
		}

		private static IntPtr ImportResolver(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
		{
			IntPtr libHandle = IntPtr.Zero;

			if (libraryName == CURAND_API_DLL_NAME)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					NativeLibrary.TryLoad(CURAND_API_DLL_NAME_LINUX, assembly, DllImportSearchPath.SafeDirectories, out libHandle);
				}
			}
			//On Windows, use the default library name
			return libHandle;
		}
#endif

		/// <summary>
		/// Creates a new random number generator of type rng_type and returns it in ref generator.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandCreateGenerator(ref CurandGenerator generator, GeneratorType rng_type);


		/// <summary>
		/// Creates a new host CPU random number generator of type rng_type and returns it in ref generator.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandCreateGeneratorHost(ref CurandGenerator generator, GeneratorType rng_type);


		/// <summary>
		/// Destroy an existing generator and free all memory associated with its state.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandDestroyGenerator(CurandGenerator generator);


		/// <summary>
		/// Return in version the version number of the dynamically linked CURAND
		/// library.  The format is the same as CUDART_VERSION from the CUDA Runtime.
		/// The only supported configuration is CURAND version equal to CUDA Runtime
		/// version.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGetVersion(ref int version);

		/// <summary>
		/// Set the current stream for CURAND kernel launches.  All library functions
		/// will use this stream until set again.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandSetStream(CurandGenerator generator, CUstream stream);

		/// <summary>
		/// Set the seed value of the pseudorandom number generator.
		/// All values of seed are valid.  Different seeds will produce different sequences.
		/// Different seeds will often not be statistically correlated with each other,
		/// but some pairs of seed values may generate sequences which are statistically correlated.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandSetPseudoRandomGeneratorSeed(CurandGenerator generator, ulong seed);

		/// <summary>
		/// Set the absolute offset of the pseudo or quasirandom number generator.
		/// <para/>
		/// All values of offset are valid.  The offset position is absolute, not 
		/// relative to the current position in the sequence.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandSetGeneratorOffset(CurandGenerator generator, ulong offset);

		/// <summary>
		/// Set the ordering of results of the pseudo or quasirandom number generator.
		/// <para/>
		/// Legal values of order for pseudorandom generators are:<para/>
		/// - <see cref="Ordering.PseudoDefault"/><para/>
		/// - <see cref="Ordering.PseudoBest"/><para/>
		/// - <see cref="Ordering.PseudoSeeded"/><para/>
		/// <para/>
		/// Legal values of order for quasirandom generators are:<para/>
		/// - <see cref="Ordering.QuasiDefault"/>
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandSetGeneratorOrdering(CurandGenerator generator, Ordering order);

		/// <summary>
		/// Set the number of dimensions to be generated by the quasirandom number generator.
		/// <para/>
		/// Legal values for num_dimensions are 1 to 20000.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandSetQuasiRandomGeneratorDimensions(CurandGenerator generator, uint num_dimensions);

		/// <summary>
		/// Use generator to generate num 32-bit results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <see cref="curandSetStream(CurandGenerator, CUstream)"/>, or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit values with every bit random.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerate(CurandGenerator generator, [Out] uint[] outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num 32-bit results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <see cref="curandSetStream(CurandGenerator, CUstream)"/>, or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit values with every bit random.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerate(CurandGenerator generator, CUdeviceptr outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num 64-bit results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <see cref="curandSetStream(CurandGenerator, CUstream)"/>, or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit values with every bit random.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLongLong(CurandGenerator generator, [Out] ulong[] outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num 64-bit results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <see cref="curandSetStream(CurandGenerator, CUstream)"/>, or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit values with every bit random.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLongLong(CurandGenerator generator, CUdeviceptr outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values between 0.0f and 1.0f,
		/// excluding 0.0f and including 1.0f.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateUniform(CurandGenerator generator, [Out] float[] outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values between 0.0f and 1.0f,
		/// excluding 0.0f and including 1.0f.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateUniform(CurandGenerator generator, CUdeviceptr outputPtr, SizeT num);
		

		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit double precision floating point values between 
		/// 0.0 and 1.0, excluding 0.0 and including 1.0.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateUniformDouble(CurandGenerator generator, [Out] double[] outputPtr, SizeT num);

		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit double precision floating point values between 
		/// 0.0 and 1.0, excluding 0.0 and including 1.0.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateUniformDouble(CurandGenerator generator, CUdeviceptr outputPtr, SizeT num);

		
		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values with mean  mean and standard
		/// deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// curandCreateGeneratorHost(). These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateNormal(CurandGenerator generator, [Out] float[] outputPtr, SizeT n, float mean, float stddev);

		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results. Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values with mean  mean and standard
		/// deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// curandCreateGeneratorHost(). These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateNormal(CurandGenerator generator, CUdeviceptr outputPtr, SizeT n, float mean, float stddev);
		
		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit floating point values with mean mean and standard
		/// deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// curandCreateGeneratorHost().  These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateNormalDouble(CurandGenerator generator, [Out] double[] outputPtr, SizeT n, double mean, double stddev);

		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr. The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit floating point values with mean mean and standard
		/// deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// curandCreateGeneratorHost().  These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateNormalDouble(CurandGenerator generator, CUdeviceptr outputPtr, SizeT n, double mean, double stddev);

		
		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using ::curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values with log-normal distribution based on
		/// an associated normal distribution with mean mean and standard deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality. <para/>
		/// The normally distributed results are transformed into log-normal distribution.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with ::curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// ::curandCreateGeneratorHost(). These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLogNormal(CurandGenerator generator, [Out] float[] outputPtr,  SizeT n, float mean, float stddev);

		/// <summary>
		/// Use generator to generate num float results into the device memory at
		/// outputPtr.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using ::curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 32-bit floating point values with log-normal distribution based on
		/// an associated normal distribution with mean mean and standard deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality. <para/>
		/// The normally distributed results are transformed into log-normal distribution.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with ::curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// ::curandCreateGeneratorHost(). These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLogNormal(CurandGenerator generator, CUdeviceptr outputPtr, SizeT n, float mean, float stddev);

		
		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit floating point values with log-normal distribution based on
		/// an associated normal distribution with mean mean and standard deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// The normally distributed results are transformed into log-normal distribution.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with ::curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// ::curandCreateGeneratorHost().  These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLogNormalDouble(CurandGenerator generator, double[] outputPtr, SizeT n, double mean, double stddev);

		/// <summary>
		/// Use generator to generate num double results into the device memory at
		/// outputPtr.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using curandSetStream(), or the null stream if no stream has been set.
		/// <para/>
		/// Results are 64-bit floating point values with log-normal distribution based on
		/// an associated normal distribution with mean mean and standard deviation stddev.
		/// <para/>
		/// Normally distributed results are generated from pseudorandom generators
		/// with a Box-Muller transform, and so require num to be even.
		/// Quasirandom generators use an inverse cumulative distribution 
		/// function to preserve dimensionality.
		/// The normally distributed results are transformed into log-normal distribution.
		/// <para/>
		/// There may be slight numerical differences between results generated
		/// on the GPU with generators created with ::curandCreateGenerator()
		/// and results calculated on the CPU with generators created with
		/// ::curandCreateGeneratorHost().  These differences arise because of
		/// differences in results for transcendental functions.  In addition,
		/// future versions of CURAND may use newer versions of the CUDA math
		/// library, so different versions of CURAND may give slightly different
		/// numerical values.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateLogNormalDouble(CurandGenerator generator, CUdeviceptr outputPtr, SizeT n, double mean, double stddev);

		
		/// <summary>
		/// Generate the starting state of the generator.  This function is
		/// automatically called by generation functions such as
		/// ::curandGenerate() and ::curandGenerateUniform().
		/// It can be called manually for performance testing reasons to separate
		/// timings for starting state generation and random number generation.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGenerateSeeds(CurandGenerator generator);

		
		/// <summary>
		/// Get a pointer to an array of direction vectors that can be used
		/// for quasirandom number generation.  The resulting pointer will
		/// reference an array of direction vectors in host memory.
		/// <para/>
		/// The array contains vectors for many dimensions.  Each dimension
		/// has 32 vectors.  Each individual vector is an unsigned int.
		/// <para/>
		/// Legal values for set are:
		/// - <see cref="DirectionVectorSet.JoeKuo6_32"/> (20,000 dimensions)
		/// - <see cref="DirectionVectorSet.ScrambledJoeKuo6_32"/> (20,000 dimensions)
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGetDirectionVectors32(out IntPtr vectors, DirectionVectorSet set);

		
		/// <summary>
		/// Get a pointer to an array of scramble constants that can be used
		/// for quasirandom number generation.  The resulting pointer will
		/// reference an array of unsinged ints in host memory.
		/// <para/>
		/// The array contains constants for many dimensions.  Each dimension
		/// has a single unsigned int constant.
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGetScrambleConstants32(out IntPtr constants);

	
		/// <summary>
		/// Get a pointer to an array of direction vectors that can be used
		/// for quasirandom number generation. The resulting pointer will
		/// reference an array of direction vectors in host memory.
		/// <para/>
		/// The array contains vectors for many dimensions. Each dimension
		/// has 64 vectors. Each individual vector is an unsigned long long.
		/// <para/>
		/// Legal values for set are:
		/// - <see cref="DirectionVectorSet.JoeKuo6_64"/> (20,000 dimensions)
		/// - <see cref="DirectionVectorSet.ScrambledJoeKuo6_64"/> (20,000 dimensions)
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGetDirectionVectors64(out IntPtr vectors, DirectionVectorSet set);

		
		/// <summary>
		/// Get a pointer to an array of scramble constants that can be used
		/// for quasirandom number generation. The resulting pointer will
		/// reference an array of unsinged long longs in host memory.
		/// <para/>
		/// The array contains constants for many dimensions. Each dimension
		/// has a single unsigned long long constant. 
		/// </summary>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGetScrambleConstants64(out IntPtr constants);

		
		/// <summary>
		/// Construct histogram array for poisson distribution.<para/>
		/// Construct histogram array for poisson distribution with lambda <c>lambda</c>.
		/// For lambda greater than 2000 optimization with normal distribution is used.
		/// </summary>
		/// <param name="lambda">lambda for poisson distribution</param>
		/// <param name="discrete_distribution">pointer to mapped memory to store histogram</param>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandCreatePoissonDistribution(double lambda, ref DiscreteDistribution discrete_distribution);

		/// <summary>
		/// Destroy histogram array for discrete distribution.<para/>
		/// Destroy histogram array for discrete distribution created by curandCreatePoissonDistribution.
		/// </summary>
		/// <param name="discrete_distribution">pointer to mapped memory where histogram is stored</param>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandDestroyDistribution(DiscreteDistribution discrete_distribution);


		/// <summary>
		/// Generate Poisson-distributed unsigned ints.<para/>
		/// Use <c>generator</c> to generate <c>num</c> unsigned int results into the device memory at
		/// <c>outputPtr</c>.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <c>curandSetStream()</c>, or the null stream if no stream has been set.
		/// Results are 32-bit unsigned int point values with poisson distribution based on
		/// an associated poisson distribution with lambda <c>lambda</c>.
		/// </summary>
		/// <param name="generator">Generator to use</param>
		/// <param name="outputPtr">Pointer to device memory to store CUDA-generated results, or Pointer to host memory to store CPU-generated results</param>
		/// <param name="n">Number of unsigned ints to generate</param>
		/// <param name="lambda">lambda for poisson distribution</param>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGeneratePoisson(CurandGenerator generator, CUdeviceptr outputPtr, SizeT n, double lambda);


		/// <summary>
		/// Generate Poisson-distributed unsigned ints.<para/>
		/// Use <c>generator</c> to generate <c>num</c> unsigned int results into the device memory at
		/// <c>outputPtr</c>.  The device memory must have been previously allocated and be
		/// large enough to hold all the results.  Launches are done with the stream
		/// set using <c>curandSetStream()</c>, or the null stream if no stream has been set.
		/// Results are 32-bit unsigned int point values with poisson distribution based on
		/// an associated poisson distribution with lambda <c>lambda</c>.
		/// </summary>
		/// <param name="generator">Generator to use</param>
		/// <param name="outputPtr">Pointer to device memory to store CUDA-generated results, or Pointer to host memory to store CPU-generated results</param>
		/// <param name="n">Number of unsigned ints to generate</param>
		/// <param name="lambda">lambda for poisson distribution</param>
		/// <returns>Status</returns>
		[DllImport(CURAND_API_DLL_NAME)]
		public static extern CurandStatus curandGeneratePoisson(CurandGenerator generator, uint[] outputPtr, SizeT n, double lambda);
	}
}

