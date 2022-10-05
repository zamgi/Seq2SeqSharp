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
using Seq2SeqSharp.Tools;
using Seq2SeqSharp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seq2SeqSharp.Layers
{
    internal class MoEFeedForward : IFeedForwardLayer
    {
        private readonly LayerNormalization layerNorm;

        private readonly IWeightTensor m_Whd1;
        private readonly IWeightTensor m_Whd2;

        private readonly IWeightTensor m_Router;
        private readonly IWeightTensor m_RouterBias;

        private readonly string m_name;
        private readonly int m_expertNum;
        private readonly int m_hiddenDim;
        private readonly int m_expertsPerTokenFactor;

        private ActivateFuncEnums m_activateFunc;

        public MoEFeedForward(string name, int expertNum, int hiddenDim, float dropoutRatio, int deviceId, bool isTrainable, float learningRateFactor = 1.0f, ActivateFuncEnums activateFunc = ActivateFuncEnums.Relu, int expertsPerTokenFactor = 1)
        {
            m_name = name;
            m_activateFunc = activateFunc;
            m_expertNum = expertNum;
            m_hiddenDim = hiddenDim;
            m_expertsPerTokenFactor = expertsPerTokenFactor;

            Logger.WriteLine($"Creating MoE feed forward layer. Name = '{name}', ExpertNum = '{expertNum}', ExpertsPerToken = '{expertsPerTokenFactor}', HiddenDim = '{hiddenDim}', DeviceId = '{deviceId}', Dropout ratio = '{dropoutRatio}', IsTrainable = '{isTrainable}', Learning rate factor = '{learningRateFactor}', Activate Function = '{activateFunc}'");

            layerNorm = new LayerNormalization($"{name}.{nameof(layerNorm)}", hiddenDim, deviceId, isTrainable, learningRateFactor: learningRateFactor);

            m_Whd1 = new WeightTensor(new long[3] { expertNum, hiddenDim, hiddenDim * 4 }, deviceId, name: $"{name}.{nameof(m_Whd1)}", normType: NormType.Uniform, isTrainable: isTrainable, learningRateFactor: learningRateFactor);
            m_Whd2 = new WeightTensor(new long[3] { expertNum, hiddenDim * 4, hiddenDim }, deviceId, name: $"{name}.{nameof(m_Whd2)}", normType: NormType.Uniform, isTrainable: isTrainable, learningRateFactor: learningRateFactor);

            m_Router = new WeightTensor(new long[2] { hiddenDim, expertNum }, deviceId, name: $"{name}.{nameof(m_Router)}", normType: NormType.Uniform, isTrainable: isTrainable, learningRateFactor: learningRateFactor);
            m_RouterBias = new WeightTensor(new long[2] { 1, expertNum }, 0, deviceId, name: $"{name}.{nameof(m_RouterBias)}", isTrainable: isTrainable, learningRateFactor: learningRateFactor);

        }

        public INeuralUnit CloneToDeviceAt(int deviceId)
        {
            throw new NotImplementedException();
        }

        public int GetDeviceId()
        {
            throw new NotImplementedException();
        }

        public IWeightTensor Process(IWeightTensor input, int batchSize, IComputeGraph graph)
        {
            //Computing routing result
            using var g = graph.CreateSubGraph($"{m_name}_MoEFeedForward");
            var inputNorm = layerNorm.Norm(input, g);
            var inputRouterDense = g.Affine(inputNorm, m_Router, m_RouterBias); // [batchSize * seqLen, expertNum]
            var inputRouter = g.Softmax(inputRouterDense); // [batchSize * seqLen, expertNum]


            if (Logger.Verbose == Logger.LogVerbose.Debug)
            {
                var routerArray = inputRouter.ToWeightArray();
                for (int i = 0; i < input.Rows; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < m_expertNum; j++)
                    {
                        sb.Append(routerArray[i * m_expertNum + j]);
                        sb.Append(" ");
                    }

                    Logger.WriteLine($"Token '{i}': '{sb.ToString()}'");
                }

            }

            (var topValue, var topIndex) = g.TopK(inputRouter, m_expertsPerTokenFactor); // [batchSize * seqLen, m_expertsPerTokenFactor]

            //if (g.NeedsBackprop)
            //{
            //    //Z-loss
            //    //var zLoss = g.Exp(inputRouterDense); // [batchSize * seqLen, expertNum]
            //    //zLoss = g.Sum(zLoss, 1); // [batchSize * seqLen, 1]
            //    //zLoss = g.Log(zLoss); // [batchSize * seqLen, 1]
            //    //zLoss = g.EltMul(zLoss, zLoss); // [batchSize * seqLen, 1]
            //    //zLoss = g.Mean(zLoss, 0); // [1,1]
            //    //zLoss = g.Mul(zLoss, 0.001f);
            //    //zLoss.FillGradient(1.0f);



            //    // Loss for load balance
            //    var routerLoss = g.Mean(inputRouter, 0); // [1, expertNum]
            //    var topKScatter = g.Scatter(topIndex, 1, 1, runGradient: false, shape: inputRouter.Sizes); // [batchSize * seqLen, expertNum]
            //    topKScatter = g.Mean(topKScatter, 0); // [1, expertNum]

            //    routerLoss = g.EltMul(routerLoss, topKScatter); // [1, expertNum]
            //    routerLoss = g.Mean(routerLoss, 1); // [1, 1]
            //    routerLoss.FillGradient((float)(m_expertNum * m_expertNum) * 0.01f);
            //}

            var topIndexArray = topIndex.ToWeightArray();
            List<float>[] indexs = new List<float>[m_expertNum]; // [expertNum, token_offsets]
            for (int i = 0; i < indexs.Length; i++)
            {
                indexs[i] = new List<float>();
            }

            for (int i = 0; i < input.Rows; i++)
            {
                for (int j = 0; j < m_expertsPerTokenFactor; j++)
                {
                    int expertIdx = (int)topIndexArray[i * m_expertsPerTokenFactor + j];
                    indexs[expertIdx].Add(i);
                }
            }

            for (int i = 0; i < m_expertNum; i++)
            {
                if (Logger.Verbose == Logger.LogVerbose.Debug)
                {
                    Logger.WriteLine($"Expert '{i}' process '{indexs[i].Count}' tokens.");
                }


                if (indexs[i].Count > 0)
                {
                    var scores_eI = g.AsContiguous(g.Peek(inputRouter, 1, i)); // [batchSize * seqLen, 1]
                    var tokenIdx_eI = g.CreateTensorWeights(new long[] { indexs[i].Count, 1 }, indexs[i].ToArray());

                    var topValue_eI = g.IndexSelect(scores_eI, tokenIdx_eI); // [indexs[i].Count, 1]
                    topValue_eI = g.AsContiguous(g.Expand(topValue_eI, dims: new long[] { indexs[i].Count, inputNorm.Sizes[^1] }));

                    var tokenEmbs = g.IndexSelect(inputNorm, tokenIdx_eI);
                    var m_Whd1_i = g.AsContiguous(g.Select(m_Whd1, 0, i));
                    var m_Whd2_i = g.AsContiguous(g.Select(m_Whd2, 0, i));


                    tokenEmbs = g.Mul(tokenEmbs, m_Whd1_i);
                    tokenEmbs = ((m_activateFunc == ActivateFuncEnums.Swish) ? g.Swish(tokenEmbs, inPlace: true) : g.Relu(tokenEmbs, inPlace: true));
                    tokenEmbs = g.Mul(tokenEmbs, m_Whd2_i);
                    tokenEmbs = g.EltMul(tokenEmbs, topValue_eI);

                    var resultEmbs = g.IndexUpdate(inputNorm.Sizes, tokenEmbs, tokenIdx_eI, true);
                    input = g.Add(input, resultEmbs);

                }
            }

            input.UnbindFromComputeGraph();

            return input;
        }
       
        public virtual List<IWeightTensor> GetParams()
        {
            List<IWeightTensor> response = new List<IWeightTensor>();

            response.AddRange(layerNorm.GetParams());

            response.AddRange(m_Whd1.GetParams());
            response.AddRange(m_Whd2.GetParams());

            response.AddRange(m_Router.GetParams());
            response.AddRange(m_RouterBias.GetParams());

            return response;
        }


        public void Save(IModel stream)
        {
            layerNorm.Save(stream);

            m_Whd1.Save(stream);
            m_Whd2.Save(stream);

            m_Router.Save(stream);
            m_RouterBias.Save(stream);

            stream.AddWeights($"{m_name}.ActivateFunc", new float[1] { (float)m_activateFunc });
        }


        public void Load(IModel stream)
        {
            layerNorm.Load(stream);

            m_Whd1.Load(stream);
            m_Whd2.Load(stream);

            m_Router.Load(stream);
            m_RouterBias.Load(stream);

            m_activateFunc = (ActivateFuncEnums)stream.GetWeights($"{m_name}.ActivateFunc")[0];
            Logger.WriteLine($"Loading '{m_name}' activate function setting '{m_activateFunc}'");

        }
    }
}