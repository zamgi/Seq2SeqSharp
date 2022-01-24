﻿using AdvUtils;
using Seq2SeqSharp.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Seq2SeqSharp.Utils
{
    public class PositionEmbedding
    {

        public static IWeightTensor AddPositionEmbedding(IComputeGraph g, IWeightTensor posEmbedding, int batchSize, IWeightTensor inputEmbs, float dropoutRatio)
        {
            var Column = posEmbedding.Columns;
            int seqLen = inputEmbs.Rows / batchSize;

            using (var posEmbeddingPeek = g.Peek(posEmbedding, 0, 0, seqLen))
            {
                using (var posEmbeddingPeekView = g.View(posEmbeddingPeek, dims: new long[] { 1, seqLen, Column }))
                {
                    using (var posEmbeddingPeekViewExp = g.Expand(posEmbeddingPeekView, dims: new long[] { batchSize, seqLen, Column }))
                    {
                        inputEmbs = g.View(inputEmbs, dims: new long[] { batchSize, seqLen, Column });
                        inputEmbs = g.Add(inputEmbs, posEmbeddingPeekViewExp, inPlace: true);
                        inputEmbs = g.View(inputEmbs, dims: new long[] { batchSize * seqLen, Column });
                    }
                }
            }

            inputEmbs = g.Dropout(inputEmbs, batchSize, dropoutRatio, inPlace: true);

            return inputEmbs;
        }

        public static WeightTensor BuildPositionWeightTensor(int row, int column, int deviceId, string name = "", bool isTrainable = false)
        {
            Logger.WriteLine($"Building position weights tensor. Row = '{row}', Column = '{column}', DeviceId = '{deviceId}', Name = '{name}', Trainable = '{isTrainable}'");

            WeightTensor t = new WeightTensor(new long[2] { row, column }, deviceId, name: name, isTrainable: isTrainable, needGradient: isTrainable);
            float[] posWeights = new float[row * column];

            float numTimescales = (float)column / 2;
            float logTimescaleIncrement = (float)(Math.Log(10000.0f) / (numTimescales - 1.0f));

            for (int p = 0; p < row; ++p)
            {
                for (int i = 0; i < numTimescales; i++)
                {
                    float v = (float)(p * Math.Exp(i * -logTimescaleIncrement));

                    posWeights[p * column + i] = (float)Math.Sin(v);
                    posWeights[p * column + (int)numTimescales + i] = (float)Math.Cos(v);
                }
            }

            t.TWeight.CopyFrom(posWeights);

            return t;
        }
    }
}
