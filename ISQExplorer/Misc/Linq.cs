using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISQExplorer.Functional;

namespace ISQExplorer.Misc
{
    public static class Linq
    {
        /// <summary>
        /// Returns true if at least a certain number of the elements of an enumerable satisfy a predicate, false if not.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="number">The number of elements from the enumerable that must satisfy the predicate.</param>
        /// <param name="predicate">A function taking an element and returning true or false.</param>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <returns>True if the predicate returned true for at least the given number of elements, false if not.</returns>
        public static bool AtLeast<T>(this IEnumerable<T> enumerable, int number, Func<T, bool> predicate)
        {
            if (number < 0)
            {
                return true;
            }

            var total = 0;
            foreach (var elem in enumerable)
            {
                if (predicate(elem))
                {
                    total++;
                }

                if (total >= number)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if at least a certain percentage of the elements of an enumerable satisfy a predicate, false if not.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="percent">The proportion of elements that must satisfy the predicate. This is a value from 0.0 to 1.0.</param>
        /// <param name="predicate">A function taking an element and returning true or false.</param>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <returns>True if the predicate returned true for at least the given number of elements, false if not.</returns>
        public static bool AtLeastPercent<T>(this IEnumerable<T> enumerable, double proportion, Func<T, bool> predicate)
        {
            if (proportion < 0 || proportion > 1)
            {
                throw new ArgumentException("Percent must be between 0 and 1.");
            }

            var accepted = 0;
            var total = 0;

            foreach (var elem in enumerable)
            {
                total++;
                if (predicate(elem))
                {
                    accepted++;
                }
            }

            return (double) accepted / total >= proportion;
        }

        /// <summary>
        /// Enumerates through each element of the input with both the element and the 0-based index.
        /// Equivalent to Python's enumerate().
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <typeparam name="T">The type of the enumerable's elements.</typeparam>
        /// <returns>An enumerable containing tuples of (element, index).</returns>
        public static IEnumerable<(T elem, int index)> Enumerate<T>(this IEnumerable<T> enumerable)
        {
            var i = 0;
            foreach (var y in enumerable)
            {
                yield return (elem: y, index: i);
                i++;
            }
        }

        /// <summary>
        /// Executes the given action for each element of the input.
        /// </summary>
        /// <param name="enumerable">The input enumerable.</param>
        /// <param name="action">The action to execute.</param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var y in enumerable)
            {
                action(y);
            }
        }

        /// <summary>
        /// Returns each line of the input reader as an enumerable.
        /// Useful for reading lines from a file.
        /// </summary>
        /// <param name="reader">The input reader.</param>
        /// <returns>An enumerable of lines from that reader.</returns>
        public static IEnumerable<string> Lines(this StreamReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Returns true of none of the elements return true for the given function. Opposite of Any().
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="predicate">The function to check each element against.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>True if none of the elements satisfy the condition. False if not.</returns>
        public static bool None<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) =>
            !enumerable.Any(predicate);

        /// <summary>
        /// Returns true if the enumerable contains no elements. Opposite of Any().
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>True if the enumerable contains no elements. False if not.</returns>
        public static bool None<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        /// <summary>
        /// Returns an enumerable of integers in a certain range. Equivalent to Python's range().
        /// </summary>
        /// <example>
        /// Range(5)        -> 0, 1, 2, 3, 4
        /// Range(2, 5)     -> 2, 3, 4
        /// Range(5, 2)     -> 5, 4, 3
        /// Range(2, 5, 2)  -> 2, 4
        /// Range(0, 5, 2)  -> 0, 2, 4
        /// Range(5, 0, -1) -> 5, 4, 3, 2, 1
        /// Range(0)        -> nothing
        /// Range(1, 1)     -> nothing
        /// Range(5, 2, 1)  -> nothing
        /// Range(2, 5, -1) -> nothing
        /// </example>
        /// The number to stop the range at.
        /// This number is not yielded, but all before it are.
        /// If this number is 0 or less, no integers are yielded.
        /// <returns></returns>
        public static IEnumerable<int> Range(int stop)
        {
            for (var i = 0; i < stop; ++i)
            {
                yield return i;
            }            
        }

        /// <summary>
        /// Returns an enumerable of integers in a certain range. Equivalent to Python's range().
        /// </summary>
        /// <example>
        /// Range(5)        -> 0, 1, 2, 3, 4
        /// Range(2, 5)     -> 2, 3, 4
        /// Range(5, 2)     -> 5, 4, 3
        /// Range(2, 5, 2)  -> 2, 4
        /// Range(0, 5, 2)  -> 0, 2, 4
        /// Range(5, 0, -1) -> 5, 4, 3, 2, 1
        /// Range(0)        -> nothing
        /// Range(1, 1)     -> nothing
        /// Range(5, 2, 1)  -> nothing
        /// Range(2, 5, -1) -> nothing
        /// </example>
        /// <param name="start">
        /// The number to start the range at. By default this is 0.
        /// </param>
        /// <param name="stop">
        /// The number to stop the range at. If this is equal to start, no integers are yielded.
        /// </param>
        /// <param name="step">
        /// The amount to advance each number in the range by.
        /// By default this is 1 if start is greater than stop, otherwise it is -1.
        /// If this value is positive and start is greater than stop, this will yield no numbers.
        /// This value can be negative, which will yield descending numbers if start is greater than stop, otherwise it will yield no numbers.
        /// If this value is set to 0, the default step is used.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<int> Range(int start, int stop, int step = 0)
        {
            if (step == 0)
            {
                step = stop > start ? 1 : -1;
            }

            for (var i = start; step > 0 && i < stop || step < 0 && i > stop; i += step)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Turns an enumerable into a randomly shuffled list.
        /// The order of the list is not cryptographically random.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="seed">The seed to use for the random number generator, or null to use a time-based seed.</param>
        /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
        /// <returns>A list containing the elements of the enumerable in shuffled order.</returns>
        public static IList<T> ToShuffledList<T>(this IEnumerable<T> enumerable, int? seed = null)
        {
            var list = enumerable.ToList();
            var rng = new Random((int)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds);

            for (var i = list.Count - 1; i > 0; --i)
            {
                var randIndex = rng.Next(0, i);
                var temp = list[i];
                list[i] = list[randIndex];
                list[randIndex] = temp;
            }
            return list;
        }

        /// <summary>
        /// Executes a function for each element of the input enumerable, indicating if any of them threw an exception.
        /// This fails fast if any of the instances throw.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The function.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>An Optional[Exception] containing the first exception thrown.</returns>
        public static async Task<Optional<Exception>> TryAllParallel<T>(this IEnumerable<T> enumerable,
            Func<T, Task<Optional<Exception>>> action)
        {
            try
            {
                var res = await Task.WhenAll(enumerable.AsParallel().Select(action));
                return res.Any(x => x.HasValue) ? res.First(x => x.HasValue) : new Optional<Exception>();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Executes a function for each element of the input enumerable, indicating if any of them threw an exception.
        /// This fails fast if any of the instances throw.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The function.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>An Optional[Exception] containing the first exception thrown.</returns>
        public static async Task<Optional<Exception>> TryAllParallel<T>(this IEnumerable<T> enumerable,
            Func<T, Task> action)
        {
            try
            {
                await Task.WhenAll(enumerable.AsParallel().Select(action));
                return new Optional<Exception>();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Executes a function for each element of the input enumerable, indicating if any of them threw an exception.
        /// This fails fast if any of the instances throw.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The function.</param>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <returns>An Optional[Exception] containing the first exception thrown.</returns>
        public static async Task<Optional<Exception>> TryAllParallel<T>(this IEnumerable<T> enumerable,
            Action<T> action)
        {
            return await enumerable.TryAllParallel(y => Task.Run(() => action(y)));
        }
    }
}