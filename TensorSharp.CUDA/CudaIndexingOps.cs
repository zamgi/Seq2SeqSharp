﻿using TensorSharp.CUDA.DeviceCode;

namespace TensorSharp.CUDA
{
    [OpsClass]
    public class CudaIndexingOps
    {
    //    private readonly IndexSelectKernels indexSelect = new IndexSelectKernels();
        private readonly GatherScatterKernels gather = new GatherScatterKernels();


        public CudaIndexingOps()
        {
        }

        //[RegisterOpStorageType("index_select", typeof(CudaStorage))]
        //public Tensor IndexSelect(Tensor result, Tensor src, int dimension, Tensor indices) { return indexSelect.IndexSelect(result, src, dimension, indices); }

        [RegisterOpStorageType("gather", typeof(CudaStorage))]
        public Tensor Gather(Tensor result, Tensor src, int dimension, Tensor indices) { return gather.Gather(result, src, dimension, indices); }

        [RegisterOpStorageType("scatter", typeof(CudaStorage))]
        public Tensor Scatter(Tensor result, Tensor src, int dimension, Tensor indices) { return gather.Scatter(result, src, dimension, indices); }


        [RegisterOpStorageType("scatter_add", typeof(CudaStorage))]
        public Tensor ScatterAdd(Tensor result, Tensor src, int dimension, Tensor indices) { return gather.ScatterAdd(result, src, dimension, indices); }


        [RegisterOpStorageType("scatter_fill", typeof(CudaStorage))]
        public Tensor ScatterFill(Tensor result, float value, int dimension, Tensor indices) { return gather.ScatterFill(result, value, dimension, indices); }
    }
}
