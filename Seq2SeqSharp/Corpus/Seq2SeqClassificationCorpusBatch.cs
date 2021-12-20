﻿using AdvUtils;
using Seq2SeqSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seq2SeqSharp.Corpus
{
    /// <summary>
    /// Data Format
    /// Source side: [Text Sequence] \t [Contextual Feature1] \t [Contextual Feature2] \t ... \t [Contextual FeatureN]
    /// Target side: [Category Tag] \t [Model Generated Text]
    /// </summary>
    public class Seq2SeqClassificationCorpusBatch : CorpusBatch
    {

        public override void CreateBatch(List<SntPair> sntPairs)
        {
            base.CreateBatch(sntPairs);

            TryAddPrefix(SrcTknsGroups[0], BuildInTokens.CLS);


            if (TgtTknsGroups.Count != 2)
            {
                throw new DataMisalignedException($"The group size in target sentence is '{TgtTknsGroups.Count}', but it should be 2.");
            }

            TryAddPrefix(TgtTknsGroups[1], BuildInTokens.BOS);
            TryAddSuffix(TgtTknsGroups[1], BuildInTokens.EOS);
        }


        public override void CreateBatch(List<List<List<string>>> srcTokensGroups, List<List<List<string>>> tgtTokensGroups)
        {
            SrcTknsGroups = srcTokensGroups;

            if (tgtTokensGroups != null)
            {
                TgtTknsGroups = tgtTokensGroups;
                TryAddPrefix(TgtTknsGroups[0], BuildInTokens.BOS);
            }
            else
            {
                TgtTknsGroups = new List<List<List<string>>>
                {
                    new List<List<string>>(),
                    InitializeHypTokens(BuildInTokens.BOS)
                };
            }


            TryAddPrefix(SrcTknsGroups[0], BuildInTokens.CLS);
        }


        public override ISntPairBatch CloneSrcTokens()
        {
            Seq2SeqClassificationCorpusBatch spb = new Seq2SeqClassificationCorpusBatch
            {
                SrcTknsGroups = SrcTknsGroups,
                TgtTknsGroups = new List<List<List<string>>>
            {
                new List<List<string>>(),
                InitializeHypTokens(BuildInTokens.BOS)
            }
            };

            return spb;
        }



    }
}
