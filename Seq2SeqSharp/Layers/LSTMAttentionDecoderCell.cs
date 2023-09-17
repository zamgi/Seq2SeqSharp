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
using System;
using System.Collections.Generic;
using TensorSharp;

namespace Seq2SeqSharp
{
    [Serializable]
    public class LSTMAttentionDecoderCell
    {
        public IWeightTensor Hidden { get; set; }
        public IWeightTensor Cell { get; set; }

        private readonly int m_hiddenDim;
        private readonly int m_inputDim;
        private readonly int m_deviceId;
        private readonly string m_name;
        private readonly IWeightTensor m_Wxhc;
        private readonly IWeightTensor m_b;
        private readonly LayerNormalization m_layerNorm1;
        private readonly LayerNormalization m_layerNorm2;

        public LSTMAttentionDecoderCell(string name, int hiddenDim, int inputDim, int contextDim, int deviceId, bool isTrainable, DType elementType = DType.Float32)
        {
            m_name = name;
            m_hiddenDim = hiddenDim;
            m_inputDim = inputDim;
            m_deviceId = deviceId;

            Logger.WriteLine($"Create LSTM attention decoder cell '{name}' HiddemDim = '{hiddenDim}', InputDim = '{inputDim}', ContextDim = '{contextDim}', DeviceId = '{deviceId}'");

            m_Wxhc = new WeightTensor(new long[2] { inputDim + hiddenDim + contextDim, hiddenDim * 4 }, deviceId, initType: RandomInitType.Uniform, name: $"{name}.{nameof(m_Wxhc)}", isTrainable: isTrainable, dtype: elementType);
            m_b = new WeightTensor(new long[2] { 1, hiddenDim * 4 }, 0, deviceId, name: $"{name}.{nameof(m_b)}", isTrainable: isTrainable, dtype: elementType);

            m_layerNorm1 = new LayerNormalization($"{name}.{nameof(m_layerNorm1)}", hiddenDim * 4, deviceId, isTrainable, elementType: elementType);
            m_layerNorm2 = new LayerNormalization($"{name}.{nameof(m_layerNorm2)}", hiddenDim, deviceId, isTrainable, elementType: elementType);
        }

        /// <summary>
        /// Update LSTM-Attention cells according to given weights
        /// </summary>
        /// <param name="context">The context weights for attention</param>
        /// <param name="input">The input weights</param>
        /// <param name="computeGraph">The compute graph to build workflow</param>
        /// <returns>Update hidden weights</returns>
        public IWeightTensor Step(IWeightTensor context, IWeightTensor input, IComputeGraph g)
        {
            using (IComputeGraph computeGraph = g.CreateSubGraph(m_name))
            {
                IWeightTensor cell_prev = Cell;
                IWeightTensor hidden_prev = Hidden;

                IWeightTensor hxhc = computeGraph.Concate(1, input, hidden_prev, context);
                IWeightTensor hhSum = computeGraph.Affine(hxhc, m_Wxhc, m_b);
                IWeightTensor hhSum2 = m_layerNorm1.Norm(hhSum, computeGraph);

                (IWeightTensor gates_raw, IWeightTensor cell_write_raw) = computeGraph.SplitColumns(hhSum2, m_hiddenDim * 3, m_hiddenDim);
                IWeightTensor gates = computeGraph.Sigmoid(gates_raw);
                IWeightTensor cell_write = computeGraph.Tanh(cell_write_raw);

                (IWeightTensor input_gate, IWeightTensor forget_gate, IWeightTensor output_gate) = computeGraph.SplitColumns(gates, m_hiddenDim, m_hiddenDim, m_hiddenDim);

                // compute new cell activation: ct = forget_gate * cell_prev + input_gate * cell_write
                Cell = g.EltMulMulAdd(forget_gate, cell_prev, input_gate, cell_write);
                IWeightTensor ct2 = m_layerNorm2.Norm(Cell, computeGraph);

                Hidden = g.EltMul(output_gate, computeGraph.Tanh(ct2));


                return Hidden;
            }
        }

        public List<IWeightTensor> getParams()
        {
            List<IWeightTensor> response = new List<IWeightTensor>
            {
                m_Wxhc,
                m_b
            };

            response.AddRange(m_layerNorm1.GetParams());
            response.AddRange(m_layerNorm2.GetParams());

            return response;
        }

        public void Reset(IWeightFactory weightFactory, int batchSize)
        {
            Hidden = weightFactory.CreateWeightTensor(batchSize, m_hiddenDim, m_deviceId, true, name: $"{m_name}.{nameof(Hidden)}", isTrainable: true);
            Cell = weightFactory.CreateWeightTensor(batchSize, m_hiddenDim, m_deviceId, true, name: $"{m_name}.{nameof(Cell)}", isTrainable: true);
        }

        public void Save(IModel stream)
        {
            m_Wxhc.Save(stream);
            m_b.Save(stream);

            m_layerNorm1.Save(stream);
            m_layerNorm2.Save(stream);
        }


        public void Load(IModel stream)
        {
            m_Wxhc.Load(stream);
            m_b.Load(stream);

            m_layerNorm1.Load(stream);
            m_layerNorm2.Load(stream);
        }
    }
}


