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
using Seq2SeqSharp.Corpus;
using Seq2SeqSharp.Tools;
using Seq2SeqSharp.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;

namespace Seq2SeqSharp.Corpus
{

    /// <summary>
    /// Vision-Text Corpus
    /// Src side are picture file paths
    /// Tgt side are the aligned text
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VisionTextCorpus<T> : ICorpus<T> where T : IVisionSntPairBatch, new()
    {
        internal int m_maxSrcTokenSize = 32;
        internal int m_maxTgtTokenSize = 32;
        internal int m_maxTokenSizePerBatch = 1;
        internal List<string> m_srcFileList;
        internal List<string> m_tgtFileList;
        internal PaddingEnums m_paddingEnums;

        private bool m_showTokenDist = true;

        private readonly Random rnd = new Random(DateTime.Now.Millisecond);

        public string CorpusName { get; set; }

        private TooLongSequence m_tooLongSequence = TooLongSequence.Ignore;

        private string m_sortedIndexedDataSetFilePath = "";
        private int m_batchNumInTotal = 0;

        public List<Dictionary<string, long>> CountTokenFreqs()
        {
            List<Dictionary<string, long>> td = new List<Dictionary<string, long>>();

            for (int i = 0; i < m_tgtFileList.Count; i++)
            {
                Logger.WriteLine(Logger.Level.debug, $"Start to count token frequency in '{m_tgtFileList[i]}'.");

                StreamReader srTgt = new StreamReader(m_tgtFileList[i]);

                while (true)
                {
                    if (srTgt.EndOfStream)
                    {
                        break;
                    }

                    string tgtLine = srTgt.ReadLine();
                    if (tgtLine.IsNullOrEmpty())
                    {
                        break;
                    }

                    string[] tgtGroups = tgtLine.Split('\t');

                    if (td.Count == 0)
                    {
                        for (int j = 0; j < tgtGroups.Length; j++)
                        {
                            td.Add(new Dictionary<string, long>());
                        }
                    }

                    for (int j = 0; j < tgtGroups.Length; j++)
                    {
                        string[] tgtTokens = tgtGroups[j].Split(' ');
                        foreach (var tgtToken in tgtTokens)
                        {
                            if (td[j].ContainsKey(tgtToken) == false)
                            {
                                td[j].Add(tgtToken, 0);
                            }
                            td[j][tgtToken]++;
                        }

                    }
                }
            }

#if DEBUG
            for (int j = 0; j < td.Count; j++)
            {
                Logger.WriteLine(Logger.Level.debug, $"Original token size at group '{j}' target = '{td[j].Count}'");
            }
#endif

            return td;
        }


        private (Dictionary<long, LinkedList<long>>, Dictionary<long, long>, string) BuildIndex()
        {
            Logger.WriteLine(Logger.Level.debug, $"Start to build index for data set.");

            SortedDictionary<int, int> dictTgtLenDist = new SortedDictionary<int, int>();
            int corpusSize = 0;
            int tooLongTgtSntCnt = 0;
            string randomFileName = Path.GetRandomFileName();
            Logger.WriteLine($"Loading and shuffling corpus from '{m_srcFileList.Count}' files.");

            string binaryDataSetFilePath = randomFileName + ".tmp";
            BinaryWriter bw = new BinaryWriter(new FileStream(binaryDataSetFilePath, FileMode.Create));

            Dictionary<long, LinkedList<long>> len2offsets = new Dictionary<long, LinkedList<long>>();
            Dictionary<long, long> len2lengths = new Dictionary<long, long>();

            for (int i = 0; i < m_srcFileList.Count; i++)
            {
                StreamReader srSrc = new StreamReader(m_srcFileList[i]);
                StreamReader srTgt = new StreamReader(m_tgtFileList[i]);

                while (true)
                {
                    if (srSrc.EndOfStream && srTgt.EndOfStream)
                    {
                        break;
                    }

                    RawSntPair rawSntPair = new RawSntPair(srSrc.ReadLine(), srTgt.ReadLine(), m_maxSrcTokenSize, m_maxTgtTokenSize, m_tooLongSequence == TooLongSequence.Truncation);
                    if (rawSntPair.IsEmptyPair())
                    {
                        break;
                    }

                    if (String.IsNullOrEmpty(rawSntPair.SrcSnt))
                    {
                        throw new InvalidDataException($"Source Line is empty. The data set is corrupted. SourceLine = '{rawSntPair.SrcSnt}', TargetLine = '{rawSntPair.TgtSnt}'");
                    }

                    if (String.IsNullOrEmpty(rawSntPair.TgtSnt))
                    {
                        throw new InvalidDataException($"Target Line is empty. The data set is corrupted. SourceLine = '{rawSntPair.SrcSnt}', TargetLine = '{rawSntPair.TgtSnt}'");
                    }

                    if (m_showTokenDist)
                    {
                        if (dictTgtLenDist.ContainsKey(rawSntPair.TgtTokenSize / 100) == false)
                        {
                            dictTgtLenDist.Add(rawSntPair.TgtTokenSize / 100, 0);
                        }
                        dictTgtLenDist[rawSntPair.TgtTokenSize / 100]++;
                    }

                    bool hasTooLongSent = false;
                    if (rawSntPair.TgtTokenSize > m_maxTgtTokenSize)
                    {
                        Interlocked.Increment(ref tooLongTgtSntCnt);
                        hasTooLongSent = true;
                    }

                    if (hasTooLongSent)
                    {
                        continue;
                    }

                    long offset = bw.BaseStream.Position;
                    bw.Write(String.Join("\n", new string[] { rawSntPair.SrcSnt, rawSntPair.TgtSnt }));

                    long length = rawSntPair.TgtGroupLenId;                   
                    if (len2offsets.ContainsKey(length) == false)
                    {
                        len2offsets.Add(length, new LinkedList<long>());
                        len2lengths.Add(length, 0);
                    }
                    len2offsets[length].AddLast(offset);
                    len2lengths[length]++;

                    Interlocked.Increment(ref corpusSize);
                }

                srSrc.Close();
                srTgt.Close();
            }

            bw.Close();

            Logger.WriteLine(Logger.Level.debug, $"Shuffled '{corpusSize}' sentence pairs.");

            if (tooLongTgtSntCnt > 0)
            {
                Logger.WriteLine(Logger.Level.warn, ConsoleColor.Yellow, $"Found {tooLongTgtSntCnt} target sentences are longer than '{m_maxTgtTokenSize}' tokens, ignore them.");
            }

            if (m_showTokenDist)
            {
                Logger.WriteLine(Logger.Level.debug, $"AggregateSrcLength = '{m_paddingEnums}'");
                Logger.WriteLine(Logger.Level.debug, $"Tgt token length distribution");

                int tgtTotalNum = 0;
                foreach (var pair in dictTgtLenDist)
                {
                    tgtTotalNum += pair.Value;
                }

                int tgtAccNum = 0;

                foreach (var pair in dictTgtLenDist)
                {
                    tgtAccNum += pair.Value;

                    Logger.WriteLine(Logger.Level.debug, $"{pair.Key * 100} ~ {(pair.Key + 1) * 100}: {pair.Value}  (acc: {100.0f * (float)tgtAccNum / (float)tgtTotalNum:F}%)");
                }

                m_showTokenDist = false;
            }

            Logger.WriteLine(Logger.Level.debug, $"Finished to build index for data set.");

            return (len2offsets, len2lengths, binaryDataSetFilePath);
        }


        public long GetNextLength(Dictionary<long, LinkedList<long>> len2offsets, Dictionary<long, long> len2counts)
        {
            long totalItems = 0;
            foreach (var pair in len2offsets)
            {
                totalItems += len2counts[pair.Key];
            }

            int rndItems = rnd.Next((int)totalItems);
            totalItems = 0;
            foreach (var pair in len2offsets)
            {
                long length = len2counts[pair.Key];
                if (totalItems <= rndItems && totalItems + length >= rndItems)
                {
                    return pair.Key;
                }
                totalItems += length;
            }

            return -1;
        }

        public void PrepareDataSet()
        {
            try
            {
                m_batchNumInTotal = 0;
                (var length2offsets, var length2counts, string tmpDataSetFilePath) = BuildIndex();

                Logger.WriteLine(Logger.Level.debug, $"Start to sort and shuffle data set by length.");

                m_sortedIndexedDataSetFilePath = tmpDataSetFilePath + ".sorted";
                using (BinaryWriter bw = new BinaryWriter(new FileStream(m_sortedIndexedDataSetFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 40960000)))
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(tmpDataSetFilePath))
                using (MemoryMappedViewStream mms = mmf.CreateViewStream())
                {
                    using (BinaryReader br = new BinaryReader(mms))
                    {
                        while (length2offsets.Count > 0)
                        {
                            long length = GetNextLength(length2offsets, length2counts);
                            LinkedList<long> offsets = length2offsets[length];

                            int totalTgtTokenSize = 0;
                            int sentSize = 0;
                            List<string> srcLines = new List<string>();
                            List<string> tgtLines = new List<string>();
                            while (totalTgtTokenSize < m_maxTokenSizePerBatch && offsets.Any())
                            {
                                long offset = offsets.First.Value;
                                offsets.RemoveFirst();
                                length2counts[length]--;

                                br.BaseStream.Seek(offset, SeekOrigin.Begin);

                                string[] srcTgtLine = br.ReadString().Split("\n");
                                string srcLine = srcTgtLine[0];
                                string tgtLine = srcTgtLine[1];

                                totalTgtTokenSize += tgtLine.Split(' ').Length;

                                srcLines.Add(srcLine);
                                tgtLines.Add(tgtLine);


                                sentSize++;
                            }

                            bw.Write(sentSize);
                            bw.Write(String.Join("\n", srcLines));
                            bw.Write(String.Join("\n", tgtLines));

                            m_batchNumInTotal++;
                            if (m_batchNumInTotal % 10000 == 0)
                            {
                                Logger.WriteLine(Logger.Level.debug, $"Batch '{m_batchNumInTotal}' has been processed.");
                            }


                            if (offsets.Any() == false)
                            {
                                length2offsets.Remove(length);
                                length2counts.Remove(length);
                            }
                        }

                        bw.Write(-1);
                    }
                }

                File.Delete(tmpDataSetFilePath);

                Logger.WriteLine($"Finished to sort and shuffle data set by length. Total batch size = '{m_batchNumInTotal}'");
            }
            catch (Exception err)
            {
                Logger.WriteLine(Logger.Level.err, $"Failed to prepare data set: '{err.Message}'.");
                Logger.WriteLine(Logger.Level.debug, $"Call Stack = '{err.StackTrace}'");
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (String.IsNullOrEmpty(m_sortedIndexedDataSetFilePath) || File.Exists(m_sortedIndexedDataSetFilePath) == false)
            {
                PrepareDataSet();
            }
            else
            {
                Logger.WriteLine(Logger.Level.debug, $"Use existing sorted indexed data set file '{m_sortedIndexedDataSetFilePath}'");
            }

            int batchIdx = 0;
            int currentBatchPercent = 0;

            using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(m_sortedIndexedDataSetFilePath))
            using (MemoryMappedViewStream mms = mmf.CreateViewStream())
            {
                using (BinaryReader br = new BinaryReader(mms))
                {
                    while (true)
                    {
                        int sizeInBatch = br.ReadInt32();
                        if (sizeInBatch < 0)
                        {
                            break;
                        }

                        List<IPair> outputs = new List<IPair>();

                        string[] srcLines = br.ReadString().Split("\n"); // List of picture file path
                        string[] tgtLines = br.ReadString().Split("\n");// List of text
                        batchIdx++;

                        for (int i = 0; i < sizeInBatch; i++)
                        {
                            var srcLine = srcLines[i];
                            var tgtLine = tgtLines[i];

                            if (m_batchNumInTotal > 0)
                            {
                                if ((100 * batchIdx / m_batchNumInTotal) > currentBatchPercent)
                                {
                                    Logger.WriteLine($"Processing batch '{batchIdx}/{m_batchNumInTotal}'."); // The '{i}th' record in this batch is: Source = '{srcLine}' Target = '{tgtLine}'");
                                    currentBatchPercent++;
                                }
                            }

                            IPair sntPair = new VisionSntPair(srcLine, tgtLine);
                            outputs.Add(sntPair);
                        }

                        var batch = new T();
                        batch.CreateBatch(outputs);
                        yield return batch;
                    }
                }
            }

            File.Delete(m_sortedIndexedDataSetFilePath);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public VisionTextCorpus()
        {

        }

        /// <summary>
        /// Build vocabulary from training corpus
        /// </summary>
        /// <param name="vocabSize"></param>
        public Vocab BuildVocabs(int tgtVocabSize = 45000, int minFreq = 1)
        {
            CorpusBatch.t_ds = CountTokenFreqs();

            (var srcVocabs, var tgtVocabs) = CorpusBatch.GenerateVocabs(0, tgtVocabSize, minFreq);
            return tgtVocabs[0];
        }

        public VisionTextCorpus(string corpusFilePath, string srcLangName, string tgtLangName, int maxTokenSizePerBatch, int maxSrcSentLength = 32, int maxTgtSentLength = 32, PaddingEnums paddingEnums = PaddingEnums.AllowPadding, TooLongSequence tooLongSequence = TooLongSequence.Ignore, string indexedFilePath = null)
        {
            Logger.WriteLine($"Loading parallel corpus from '{corpusFilePath}' for source side '{srcLangName}' and target side '{tgtLangName}' MaxSrcSentLength = '{maxSrcSentLength}',  MaxTgtSentLength = '{maxTgtSentLength}', Token Padding Type = '{paddingEnums}', TooLongSequence = '{tooLongSequence}'");
            m_maxTokenSizePerBatch = maxTokenSizePerBatch;
            m_maxSrcTokenSize = maxSrcSentLength;
            m_maxTgtTokenSize = maxTgtSentLength;

            m_tooLongSequence = tooLongSequence;

            m_paddingEnums = paddingEnums;
            CorpusName = corpusFilePath;
            m_sortedIndexedDataSetFilePath = indexedFilePath;

            m_srcFileList = new List<string>();
            m_tgtFileList = new List<string>();
            string[] files = Directory.GetFiles(corpusFilePath, $"*.*", SearchOption.TopDirectoryOnly);

            Dictionary<string, string> srcKey2FileName = new Dictionary<string, string>();
            Dictionary<string, string> tgtKey2FileName = new Dictionary<string, string>();

            string srcSuffix = $".{srcLangName}.snt";
            string tgtSuffix = $".{tgtLangName}.snt";

            foreach (string file in files)
            {
                if (file.EndsWith(srcSuffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    string srcKey = file.Substring(0, file.Length - srcSuffix.Length);
                    srcKey2FileName.Add(srcKey, file);

                    Logger.WriteLine($"Add source file '{file}' to key '{srcKey}'");
                }

                if (file.EndsWith(tgtSuffix, StringComparison.InvariantCultureIgnoreCase))
                {
                    string tgtKey = file.Substring(0, file.Length - tgtSuffix.Length);
                    tgtKey2FileName.Add(tgtKey, file);


                    Logger.WriteLine($"Add target file '{file}' to key '{tgtKey}'");
                }
            }

            foreach (var pair in srcKey2FileName)
            {
                m_srcFileList.Add(pair.Value);
                m_tgtFileList.Add(tgtKey2FileName[pair.Key]);
            }

        }
    }
}
