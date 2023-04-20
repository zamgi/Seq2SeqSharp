﻿// Copyright (c) Zhongkai Fu. All rights reserved.
// https://github.com/zhongkaifu/Seq2SeqSharp
//
// This file is part of Seq2SeqSharp.
//
// Seq2SeqSharp is licensed under the BSD-3-Clause license found in the LICENSE file in the root directory of this source tree.
//
// Seq2SeqSharp is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the BSD-3-Clause License for more details.

using Seq2SeqSharp.Tools;
using System;
using System.Collections.Generic;
using TensorSharp;

namespace Seq2SeqSharp
{
    [Serializable]
    public class LSTMCell
    {
        private readonly IWeightTensor m_Wxh;
        private readonly IWeightTensor m_b;
        private IWeightTensor m_hidden;
        private IWeightTensor m_cell;
        private readonly int m_hdim;
        private readonly int m_deviceId;
        private readonly string m_name;
        private readonly LayerNormalization m_layerNorm1;
        private readonly LayerNormalization m_layerNorm2;

        public IWeightTensor Hidden => m_hidden;

        public LSTMCell(string name, int hdim, int inputDim, int deviceId, bool isTrainable, DType elementType = DType.Float32)
        {
            m_name = name;

            m_Wxh = new WeightTensor(new long[2] { inputDim + hdim, hdim * 4 }, deviceId, normType: NormType.Uniform, name: $"{name}.{nameof(m_Wxh)}", isTrainable: isTrainable, dtype: elementType);
            m_b = new WeightTensor(new long[2] { 1, hdim * 4 }, 0, deviceId, name: $"{name}.{nameof(m_b)}", isTrainable: isTrainable, dtype: elementType);

            m_hdim = hdim;
            m_deviceId = deviceId;

            m_layerNorm1 = new LayerNormalization($"{name}.{nameof(m_layerNorm1)}", hdim * 4, deviceId, isTrainable: isTrainable);
            m_layerNorm2 = new LayerNormalization($"{name}.{nameof(m_layerNorm2)}", hdim, deviceId, isTrainable: isTrainable);
        }

        public IWeightTensor Step(IWeightTensor input, IComputeGraph g)
        {
            using (IComputeGraph innerGraph = g.CreateSubGraph(m_name))
            {
                IWeightTensor hidden_prev = m_hidden;
                IWeightTensor cell_prev = m_cell;

                IWeightTensor inputs = innerGraph.Concate(1, input, hidden_prev);
                IWeightTensor hhSum = innerGraph.Affine(inputs, m_Wxh, m_b);
                IWeightTensor hhSum2 = m_layerNorm1.Norm(hhSum, innerGraph);

                (IWeightTensor gates_raw, IWeightTensor cell_write_raw) = innerGraph.SplitColumns(hhSum2, m_hdim * 3, m_hdim);
                IWeightTensor gates = innerGraph.Sigmoid(gates_raw);
                IWeightTensor cell_write = innerGraph.Tanh(cell_write_raw);

                (IWeightTensor input_gate, IWeightTensor forget_gate, IWeightTensor output_gate) = innerGraph.SplitColumns(gates, m_hdim, m_hdim, m_hdim);

                // compute new cell activation: ct = forget_gate * cell_prev + input_gate * cell_write
                m_cell = g.EltMulMulAdd(forget_gate, cell_prev, input_gate, cell_write);
                IWeightTensor ct2 = m_layerNorm2.Norm(m_cell, innerGraph);

                // compute hidden state as gated, saturated cell activations
                m_hidden = g.EltMul(output_gate, innerGraph.Tanh(ct2));

                return m_hidden;
            }
        }

        public virtual List<IWeightTensor> getParams()
        {
            List<IWeightTensor> response = new List<IWeightTensor>
            {
                m_Wxh,
                m_b
            };

            response.AddRange(m_layerNorm1.GetParams());
            response.AddRange(m_layerNorm2.GetParams());

            return response;
        }

        public void Reset(IWeightFactory weightFactory, int batchSize)
        {
            if (m_hidden != null)
            {
                m_hidden.Dispose();
                m_hidden = null;
            }

            if (m_cell != null)
            {
                m_cell.Dispose();
                m_cell = null;
            }

            m_hidden = weightFactory.CreateWeightTensor(batchSize, m_hdim, m_deviceId, true, name: $"{m_name}.{nameof(m_hidden)}", isTrainable: true);
            m_cell = weightFactory.CreateWeightTensor(batchSize, m_hdim, m_deviceId, true, name: $"{m_name}.{nameof(m_cell)}", isTrainable: true);
        }

        public void Save(IModel stream)
        {
            m_Wxh.Save(stream);
            m_b.Save(stream);

            m_layerNorm1.Save(stream);
            m_layerNorm2.Save(stream);

        }


        public void Load(IModel stream)
        {
            m_Wxh.Load(stream);
            m_b.Load(stream);

            m_layerNorm1.Load(stream);
            m_layerNorm2.Load(stream);
        }
    }

}
