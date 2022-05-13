﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StringAlgorithms.SuffixTries.Tests
{
    [TestClass]
    public class SuffixTrieNodeTests
    {
        [TestMethod]
        public void Indexer_RetrievesChild()
        {
            var root = BuildSuffixTrieExample();
            Assert.AreEqual(root.Children[new(0)], root[new(0)]);
            Assert.AreEqual(root.Children[new(3)], root[new(3)]);
        }

        [TestMethod]
        public void Children_ImmutabilityOnGet()
        {
            var root = BuildSuffixTrieExample();
            Assert.ThrowsException<NotSupportedException>(
                () => root.Children.Clear());
            Assert.ThrowsException<NotSupportedException>(
                () => root.Children[root.Children.First().Key] = new SuffixTrieNode(0));
            Assert.ThrowsException<NotSupportedException>(
                () => root.Children.Remove(root.Children.First().Key));
        }

        [TestMethod]
        public void Children_Immutability_FromCtorParam()
        {
            var rootChildren = new Dictionary<SuffixTrieEdge, SuffixTrieNode> { };
            var root = new SuffixTrieNode(rootChildren);
            Assert.IsTrue(root.IsLeaf);

            rootChildren.Add(new(0), new SuffixTrieNode(0));
            Assert.IsTrue(root.IsLeaf);
        }

        [TestMethod]
        public void GetAllNodeToLeafPaths_Correctness()
        {
            var root = BuildSuffixTrieExample();
            var rootToLeafPaths = root.GetAllNodeToLeafPaths().ToList();

            Assert.AreEqual(4, rootToLeafPaths.Count);

            Assert.AreEqual(1, CountOccurrencesByEdges(rootToLeafPaths, new(0), new(1), new(2), new(3)));
            Assert.AreEqual(1, CountOccurrencesByEdges(rootToLeafPaths, new(0), new(1), new(3)));
            Assert.AreEqual(1, CountOccurrencesByEdges(rootToLeafPaths, new(0), new(3)));
            Assert.AreEqual(1, CountOccurrencesByEdges(rootToLeafPaths, new SuffixTrieEdge(3)));
        }

        [TestMethod]
        public void GetAllSuffixesFor_Correctness()
        {
            var text = new TextWithTerminator("abc");
            var root = BuildSuffixTrieExample();
            var suffixes = root.GetAllSuffixesFor(text);

            var t = text.Terminator;
            Assert.IsTrue(suffixes.OrderBy(s => s).SequenceEqual(
                new List<string> { $"abc{t}", $"ab{t}", $"a{t}", $"{t}" }.OrderBy(s => s)));
        }

        /// <remarks>
        /// Built from "aaa".
        /// </remarks>
        private static SuffixTrieNode BuildSuffixTrieExample()
        {
            return new SuffixTrieNode(new Dictionary<SuffixTrieEdge, SuffixTrieNode>
            {
                [new(0)] = new SuffixTrieNode(new Dictionary<SuffixTrieEdge, SuffixTrieNode>
                {
                    [new(1)] = new SuffixTrieNode(new Dictionary<SuffixTrieEdge, SuffixTrieNode>
                    {
                        [new(2)] = new SuffixTrieNode(new Dictionary<SuffixTrieEdge, SuffixTrieNode>
                        {
                            [new(3)] = new SuffixTrieNode(0)
                        }),
                        [new(3)] = new SuffixTrieNode(1),
                    }),
                    [new(3)] = new SuffixTrieNode(2),
                }),
                [new(3)] = new SuffixTrieNode(3),
            });
        }

        private static int CountOccurrencesByEdges(
            IEnumerable<SuffixTriePath> paths,
            params SuffixTrieEdge[] pathToFind) => (
                from path in paths
                let pathKeys = path.PathNodes.Select(kvp => kvp.Key)
                where pathKeys.SequenceEqual(pathToFind)
                select path)
                .Count();
    }
}
