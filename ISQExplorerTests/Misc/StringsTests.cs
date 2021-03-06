#nullable enable
using System.Linq;
using ISQExplorer.Misc;
using NUnit.Framework;

namespace ISQExplorerTests
{
    public class StringsTest
    {
        private static object[] _captureTestCases =
        {
            new object?[] {"abcdef", "bcd", 1, "bcd"},
            new object?[] {"abcdef", "a(bcd)e(fg)", 1, null},
            new object?[] {"abcdef", "a(bcd)e", 1, "bcd"},
            new object?[] {"abcdef", "a(bcd)ef", 1, "bcd"},
            new object?[] {"abcdef", "abcd(.*)$", 1, "ef"},
            new object?[] {"abcdef", "(a)", 1, "a"},
            new object?[] {"abcdef", "(ab).*(ef)", 2, "ef"},
            new object?[] {"abcdef", "a(bcd)efg", 1, null},
            new object?[] {"abcdef", "a", 1, "a"},
            new object?[] {"abcdef", "(g)", 1, null},
            new object?[] {"abcdef", "(a)", 2, null},
        };

        [Test]
        [TestCaseSource(nameof(_captureTestCases))]
        public void CaptureTest(string input, string pattern, int number, string? result)
        {
            var res = input.Capture(pattern, number);
            if (result == null)
            {
                Assert.False(res.HasValue);
            }
            else
            {
                Assert.True(res.HasValue);
                Assert.AreEqual(result, res.Value);
            }
        }

        private static object[] _matchesTestCases =
        {
            new object[] {"abcdef", "abcdef", false, true},
            new object[] {"abcdef", "abcdef", true, true},
            new object[] {"abcdef", "abcde", false, false},
            new object[] {"abcdef", "abcde", true, true},
            new object[] {"abcdef", "bc.*$", false, false},
            new object[] {"abcdef", "bc.*$", true, true},
            new object[] {"", ".*", false, true},
            new object[] {"", ".*", true, true},
            new object[] {"", ".+", false, false},
            new object[] {"", ".+", true, false}
        };

        [Test]
        [TestCaseSource(nameof(_matchesTestCases))]
        public void MatchesTest(string input, string pattern, bool substring, bool matches)
        {
            Assert.AreEqual(matches, input.Matches(pattern));
        }

        private static object[] _indexOfAllTestCases =
        {
            new object[] {"abcxdef", "x", new[] {3}},
            new object[] {"xabcdef", "x", new[] {0}},
            new object[] {"abcdef", "x", new int []{}},
            new object[] {"xabcxdef", "x", new[] {0, 4}},
            new object[] {"abcdefx", "x", new[] {6}},
            new object[] {"xx", "x", new[] {0, 1}},
            new object[] {"abcdef", ".+", new[] {0}},
            new object[] {"abcdef", "..", new[] {0, 2, 4}}
        };

        [Test]
        [TestCaseSource(nameof(_indexOfAllTestCases))]
        public void IndexOfAllTest(string input, string pattern, int[] expected)
        {
            var res = input.IndexOfAll(pattern).ToArray();
            Assert.That(res, Is.EquivalentTo(expected));
        }
    }
}