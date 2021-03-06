#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISQExplorer.Functional
{
    public static class Try
    {
        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <typeparam name="T">The type of the Try.</typeparam>
        /// <returns>A Try of the same type as the return value of the function.</returns>
        public static Try<T> Of<T>(Func<T> func) => new Try<T>(func);

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <typeparam name="T">The type of the Try.</typeparam>
        /// <returns>A Try of the same type as the return value of the function.</returns>
        public static Try<T> OfEither<T>(Func<Either<T, Exception>> func) => new Try<T>(func);

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <typeparam name="T">The type of the Try.</typeparam>
        /// <typeparam name="TException">The type of the Try's exception.</typeparam>
        /// <returns>A Try of the same type as the return value of the function.</returns>
        public static Try<T> OfEither<T, TException>(Func<Either<T, TException>> func) where TException : Exception
            => new Try<T, TException>(func);

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        /// <typeparam name="T">The type of the Try.</typeparam>
        /// <typeparam name="TException">The type of the Try's exception.</typeparam>
        /// <returns>A Try of the same type as the return value of the function.</returns>
        public static Try<T> Of<T, TException>(Func<T> func) where TException : Exception
            => new Try<T, TException>(func);

        /// <summary>
        /// Constructs a Try out of the given value.
        /// </summary>
        /// <param name="val">The value to construct it out of.</param>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <returns>A Try{T} constructed from the given value.</returns>
        public static Try<T> Of<T>(T val) => new Try<T>(val);

        /// <summary>
        /// Constructs a Try out of an async function, containing either the value returned by the Task, or the exception it may throw.
        /// This function itself will not throw any exceptions the function passed to it may throw, but said exceptions will be present in the resulting Try.
        /// </summary>
        /// <param name="func">The function returning a Task returning the desired value.</param>
        /// <typeparam name="T">The type of the desired value.</typeparam>
        /// <returns>A Task of the given Try.</returns>
        public static async Task<Try<T>> OfAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Constructs a Try out of an async function, containing either the value returned by the Task, or the exception it may throw.
        /// This function itself will not throw any exceptions the function passed to it may throw, but said exceptions will be present in the resulting Try.
        /// </summary>
        /// <param name="func">The function returning a Task returning the desired value.</param>
        /// <typeparam name="T">The type of the desired value.</typeparam>
        /// <typeparam name="TException">The type of the exception to catch.</typeparam>
        /// <returns>A Task of the given Try.</returns>
        public static async Task<Try<T, TException>> OfAsync<T, TException>(Func<Task<T>> func)
            where TException : Exception
        {
            try
            {
                return await func();
            }
            catch (TException e)
            {
                return e;
            }
        }

        /// <summary>
        /// Constructs a Try out of an async function, containing either the value returned by the Task, or the exception it may throw.
        /// This function itself will not throw any exceptions the function passed to it may throw, but said exceptions will be present in the resulting Try.
        /// </summary>
        /// <param name="func">The function returning a Task returning the desired value.</param>
        /// <typeparam name="T">The type of the desired value.</typeparam>
        /// <returns>A Task of the given Try.</returns>
        public static async Task<Try<T>> OfEitherAsync<T>(Func<Task<Either<T, Exception>>> func)
        {
            try
            {
                return (await func()).Match(
                    val => new Try<T>(val),
                    ex => new Try<T>(ex)
                );
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Constructs a Try out of an async function, containing either the value returned by the Task, or the exception it may throw.
        /// This function itself will not throw any exceptions the function passed to it may throw, but said exceptions will be present in the resulting Try.
        /// </summary>
        /// <param name="func">The function returning a Task returning the desired value.</param>
        /// <typeparam name="T">The type of the desired value.</typeparam>
        /// <typeparam name="TException">The type of the desired exception.</typeparam>
        /// <returns>A Task of the given Try.</returns>
        public static async Task<Try<T, TException>> OfEitherAsync<T, TException>(
            Func<Task<Either<T, TException>>> func)
            where TException : Exception
        {
            try
            {
                return (await func()).Match(
                    val => new Try<T, TException>(val),
                    ex => new Try<T, TException>(ex)
                );
            }
            catch (TException e)
            {
                return e;
            }
        }

        /// <summary>
        /// Converts the exception to a different type if necessary.
        /// </summary>
        /// <param name="e">An exception.</param>
        /// <param name="message">A message to use if the exception is casted.</param>
        /// <typeparam name="TException">The type of the exception to cast to.</typeparam>
        /// <returns>A new exception if the input exception is not the desired type, or the input exception if it is.</returns>
        public static TException Cast<TException>(this Exception e, string message = "") where TException : Exception
        {
            if (e is TException te)
            {
                return te;
            }

            var res = typeof(TException).GetConstructor(new[] {typeof(string), typeof(Exception)})
                ?.Invoke(new object[] {message, e});

            if (res == null || !(res is TException te2))
            {
                throw new InvalidOperationException(
                    $"Exception '{typeof(TException)}' does not have a (string, Exception) constructor.");
            }

            return te2;
        }

        public static IEnumerable<Exception> Exceptions<T>(this IEnumerable<Try<T>> enumerable) =>
            enumerable.Where(x => !x.HasValue).Select(x => x.Exception);

        public static IEnumerable<TException>
            Exceptions<T, TException>(this IEnumerable<Try<T, TException>> enumerable) where TException : Exception =>
            enumerable.Where(x => !x.HasValue).Select(x => x.Exception);

        public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> enumerable) =>
            enumerable.Where(x => x.HasValue).Select(x => x.Value);

        public static IEnumerable<T> Values<T, TException>(this IEnumerable<Try<T, TException>> enumerable)
            where TException : Exception =>
            enumerable.Where(x => x.HasValue).Select(x => x.Value);
    }

    /// <summary>
    /// Contains a value, or an exception detailing why said value is not present.
    /// </summary>
    /// <typeparam name="T">The underlying value.</typeparam>
    public class Try<T> : IEquatable<Try<T>>
    {
        private readonly T _value;
        private readonly Exception _ex;
        private readonly string _stackTrace;
        public bool HasValue { get; }

        /// <summary>
        /// Constructs a Try out of the given value.
        /// </summary>
        /// <param name="val">The value.</param>
        public Try(T val)
        {
            (_value, _ex, HasValue) = (val, default!, true);
            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Constructs a Try out of the given exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public Try(Exception ex)
        {
            (_value, _ex, HasValue) = (default!, ex, false);
            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public Try(Func<Either<T, Exception>> func)
        {
            try
            {
                var val = func();

                if (val.HasLeft)
                {
                    (_value, _ex, HasValue) = (val.Left, default!, true);
                }
                else
                {
                    (_value, _ex, HasValue) = (default!, val.Right, false);
                }
            }
            catch (Exception ex)
            {
                (_value, _ex, HasValue) = (default!, ex, false);
            }

            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public Try(Func<T> func)
        {
            try
            {
                var val = func();
                (_value, _ex, HasValue) = (val, default!, true);
            }
            catch (Exception ex)
            {
                (_value, _ex, HasValue) = (default!, ex, false);
            }

            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Copy constructor for Try[T].
        /// </summary>
        /// <param name="other">The other Try.</param>
        public Try(Try<T> other)
        {
            (_value, _ex, HasValue) = (other.HasValue ? other.Value : default!,
                !other.HasValue ? other.Exception : default!, other.HasValue);
            _stackTrace = Environment.StackTrace;
        }

        public T Value
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }

                throw new InvalidOperationException(
                    $"This Try has an exception, not a value.\nStack trace of original Try: {_stackTrace}.", _ex);
            }
        }

        public T ValueOrThrow
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }

                throw _ex;
            }
        }

        public Exception Exception
        {
            get
            {
                if (!HasValue)
                {
                    return _ex;
                }

                throw new InvalidOperationException($"This Try has a value, not an exception. Value: '{_value}'.");
            }
        }

        /// <summary>
        /// Executes one of two functions depending on if a value or an exception is present in this Try.
        /// </summary>
        /// <param name="val">The function to execute if a value is present.</param>
        /// <param name="ex">The function to execute if a value is not present.</param>
        /// <typeparam name="TRes">The value to return. Both functions must have the same return type.</typeparam>
        /// <returns>The return value of the function executed.</returns>
        public TRes Match<TRes>(Func<T, TRes> val, Func<Exception, TRes> ex) => HasValue ? val(Value) : ex(Exception);

        /// <summary>
        /// Executes one of two functions depending on if a value or an exception is present in this Try.
        /// </summary>
        /// <param name="val">The function to execute if a value is present.</param>
        /// <param name="ex">The function to execute if a value is not present.</param>
        public void Match(Action<T> val, Action<Exception> ex)
        {
            if (HasValue)
            {
                val(Value);
            }
            else
            {
                ex(Exception);
            }
        }

        /// <summary>
        /// Maps this Try to another type using the supplied function.
        /// </summary>
        /// <param name="func">A function converting this type to the desired type. If this function throws, the new Try will be constructed out of the thrown exception.</param>
        /// <typeparam name="TRes">The new type of the try.</typeparam>
        /// <returns>A new Try of the given type containing a value if this Try contains a value and the conversion function didn't throw, or the applicable exception if not.</returns>
        public Try<TRes> Select<TRes>(Func<T, TRes> func) => Match(
            val =>
            {
                try
                {
                    return new Try<TRes>(func(Value));
                }
                catch (Exception ex)
                {
                    return new Try<TRes>(ex);
                }
            },
            ex => ex
        );

        /// <summary>
        /// Maps this Try to another type using the supplied async function.
        /// </summary>
        /// <param name="func">A function converting this type to a task of the desired type. If this function throws, the new Try will be constructed out of the thrown exception.</param>
        /// <typeparam name="TRes">The new type of the try.</typeparam>
        /// <returns>A new Try of the given type containing a value if this Try contains a value and the conversion function didn't throw, or the applicable exception if not.</returns>
        public Task<Try<TRes>> SelectAsync<TRes>(Func<T, Task<TRes>> func) => Match(
            async val =>
            {
                try
                {
                    return new Try<TRes>(await func(Value));
                }
                catch (Exception ex)
                {
                    return new Try<TRes>(ex);
                }
            },
            ex => new Task<Try<TRes>>(() => throw ex)
        );

        public static implicit operator Try<T>(T val) => new Try<T>(val);

        public static implicit operator Try<T>(Exception ex) => new Try<T>(ex);

        public static implicit operator Optional<T>(Try<T> t) =>
            t.HasValue ? new Optional<T>(t.Value) : new Optional<T>();

        public static explicit operator T(Try<T> t) => t.Value;

        public static explicit operator Exception(Try<T> t) => t.Exception;

        public static implicit operator bool(Try<T> t) => t.HasValue;

        public bool Equals(Try<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasValue == other.HasValue &&
                   (HasValue && Equals(Value, other.Value) || !HasValue && Equals(Exception, other.Exception));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Try<T>) obj);
        }

        public override int GetHashCode()
        {
            return HasValue ? Value!.GetHashCode() : Exception!.GetHashCode();
        }

        public static bool operator ==(Try<T>? left, Try<T>? right)
        {
            return ReferenceEquals(left, right) || !ReferenceEquals(left, null) && Equals(left, right);
        }

        public static bool operator !=(Try<T>? left, Try<T>? right)
        {
            return !(left == right);
        }

        public static bool operator ==(Try<T>? left, T right)
        {
            return ReferenceEquals(right, null) && ReferenceEquals(left, null) ||
                   !ReferenceEquals(left, null) && left.HasValue && Equals(left.Value, right);
        }

        public static bool operator !=(Try<T>? left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, Try<T> right)
        {
            return right == left;
        }

        public static bool operator !=(T left, Try<T> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Try<T>? left, Exception right)
        {
            return ReferenceEquals(right, null) && ReferenceEquals(left, null) ||
                   !ReferenceEquals(left, null) && left.HasValue && Equals(left.Value, right);
        }

        public static bool operator !=(Try<T>? left, Exception right)
        {
            return !(left == right);
        }

        public static bool operator ==(Exception left, Try<T> right)
        {
            return right == left;
        }

        public static bool operator !=(Exception left, Try<T> right)
        {
            return !(left == right);
        }

        public static bool operator true(Try<T> r) => r.HasValue;

        public static bool operator false(Try<T> r) => !r.HasValue;

        public static Try<T> operator &(Try<T> t1, Try<T> t2) => !t1 ? t1 : t2;

        public static Try<T> operator |(Try<T> t1, Try<T> t2) => t1 ? t1 : t2;

        public override string ToString() => Match(val => val!.ToString(), ex => ex!.ToString())!;
    }

    /// <summary>
    /// Contains a value, or an exception detailing why said value is not present.
    /// </summary>
    /// <typeparam name="T">The underlying value.</typeparam>
    /// <typeparam name="TException">The type of the exception. This is Exception by default.</typeparam>
    public class Try<T, TException> : IEquatable<Try<T, TException>> where TException : Exception
    {
        private readonly T _value;
        private readonly TException _ex;
        private readonly string _stackTrace;
        public bool HasValue { get; }

        /// <summary>
        /// Constructs a Try out of the given value.
        /// </summary>
        /// <param name="val">The value.</param>
        public Try(T val)
        {
            (_value, _ex, HasValue) = (val, default!, true);
            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Constructs a Try out of the given exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public Try(TException ex)
        {
            (_value, _ex, HasValue) = (default!, ex, false);
            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public Try(Func<Either<T, TException>> func)
        {
            try
            {
                var val = func();
                if (val.HasLeft)
                {
                    (_value, _ex, HasValue) = (val.Left, default!, true);
                }
                else
                {
                    (_value, _ex, HasValue) = (default!, val.Right, false);
                }
            }
            catch (TException ex)
            {
                (_value, _ex, HasValue) = (default!, ex, false);
            }

            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Executes the given function, constructing the Try out of the return value, or the exception the function might throw.
        /// </summary>
        /// <param name="func">The function to execute.</param>
        public Try(Func<T> func)
        {
            try
            {
                var val = func();
                (_value, _ex, HasValue) = (val, default!, true);
            }
            catch (TException ex)
            {
                (_value, _ex, HasValue) = (default!, ex, false);
            }

            _stackTrace = Environment.StackTrace;
        }

        /// <summary>
        /// Copy constructor for Try[T, TException].
        /// </summary>
        /// <param name="other">The other Try.</param>
        public Try(Try<T, TException> other)
        {
            (_value, _ex, HasValue) = (other.HasValue ? other.Value : default,
                !other.HasValue ? other.Exception : default!, other.HasValue);
            _stackTrace = Environment.StackTrace;
        }

        public T Value
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }

                throw new InvalidOperationException(
                    $"This Try has an exception, not a value.\nStack trace of original Try: {_stackTrace}.", _ex);
            }
        }

        public T ValueOrThrow
        {
            get
            {
                if (HasValue)
                {
                    return _value;
                }

                throw _ex;
            }
        }

        public TException Exception
        {
            get
            {
                if (!HasValue)
                {
                    return _ex;
                }

                throw new InvalidOperationException($"This Try has a value, not an exception. Value: '{_value}'.");
            }
        }

        /// <summary>
        /// Executes one of two functions depending on if a value or an exception is present in this Try.
        /// </summary>
        /// <param name="val">The function to execute if a value is present.</param>
        /// <param name="ex">The function to execute if a value is not present.</param>
        /// <typeparam name="TRes">The value to return. Both functions must have the same return type.</typeparam>
        /// <returns>The return value of the function executed.</returns>
        public TRes Match<TRes>(Func<T, TRes> val, Func<TException, TRes> ex) => HasValue ? val(Value) : ex(Exception);

        /// <summary>
        /// Executes one of two functions depending on if a value or an exception is present in this Try.
        /// </summary>
        /// <param name="val">The function to execute if a value is present.</param>
        /// <param name="ex">The function to execute if a value is not present.</param>
        public void Match(Action<T> val, Action<TException> ex)
        {
            if (HasValue)
            {
                val(Value);
            }
            else
            {
                ex(Exception);
            }
        }

        /// <summary>
        /// Maps this Try to another type using the supplied function.
        /// </summary>
        /// <param name="func">A function converting this type to the desired type. If this function throws, the new Try will be constructed out of the thrown exception.</param>
        /// <typeparam name="TRes">The new type of the try.</typeparam>
        /// <returns>A new Try of the given type containing a value if this Try contains a value and the conversion function didn't throw, or the applicable exception if not.</returns>
        public Try<TRes, TException> Select<TRes>(Func<T, TRes> func) => Match(
            val =>
            {
                try
                {
                    return new Try<TRes, TException>(func(Value));
                }
                catch (TException ex)
                {
                    return new Try<TRes, TException>(ex);
                }
            },
            ex => ex
        );

        /// <summary>
        /// Maps this Try to another type using the supplied async function.
        /// </summary>
        /// <param name="func">A function converting this type to a task of the desired type. If this function throws, the new Try will be constructed out of the thrown exception.</param>
        /// <typeparam name="TRes">The new type of the try.</typeparam>
        /// <returns>A new Try of the given type containing a value if this Try contains a value and the conversion function didn't throw, or the applicable exception if not.</returns>
        public Task<Try<TRes, TException>> SelectAsync<TRes>(Func<T, Task<TRes>> func) => Match(
            async val =>
            {
                try
                {
                    return new Try<TRes, TException>(await func(Value));
                }
                catch (TException ex)
                {
                    return new Try<TRes, TException>(ex);
                }
            },
            ex => new Task<Try<TRes, TException>>(() => throw ex)
        );

        public static implicit operator Try<T, TException>(T val) => new Try<T, TException>(val);

        public static implicit operator Try<T, TException>(TException ex) => new Try<T, TException>(ex);

        public static implicit operator Optional<T>(Try<T, TException> t) =>
            t.HasValue ? new Optional<T>(t.Value) : new Optional<T>();

        public static explicit operator T(Try<T, TException> t) => t.Value;

        public static explicit operator TException(Try<T, TException> t) => t.Exception;

        public static implicit operator bool(Try<T, TException> t) => t.HasValue;

        public static implicit operator Try<T>(Try<T, TException> t) => t.Match(
            val => new Try<T>(val),
            ex => new Try<T>(ex)
        );

        public bool Equals(Try<T, TException> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HasValue == other.HasValue &&
                   (HasValue && Equals(Value, other.Value) || !HasValue && Equals(Exception, other.Exception));
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Try<T, TException>) obj);
        }

        public override int GetHashCode()
        {
            return HasValue ? Value!.GetHashCode() : Exception!.GetHashCode();
        }

        public static bool operator ==(Try<T, TException>? left, Try<T, TException>? right)
        {
            return ReferenceEquals(left, right) || !ReferenceEquals(left, null) && Equals(left, right);
        }

        public static bool operator !=(Try<T, TException>? left, Try<T, TException>? right)
        {
            return !(left == right);
        }

        public static bool operator ==(Try<T, TException>? left, T right)
        {
            return ReferenceEquals(right, null) && ReferenceEquals(left, null) ||
                   !ReferenceEquals(left, null) && left.HasValue && Equals(left.Value, right);
        }

        public static bool operator !=(Try<T, TException>? left, T right)
        {
            return !(left == right);
        }

        public static bool operator ==(T left, Try<T, TException> right)
        {
            return right == left;
        }

        public static bool operator !=(T left, Try<T, TException> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Try<T, TException>? left, TException right)
        {
            return ReferenceEquals(right, null) && ReferenceEquals(left, null) ||
                   !ReferenceEquals(left, null) && left.HasValue && Equals(left.Value, right);
        }

        public static bool operator !=(Try<T, TException>? left, TException right)
        {
            return !(left == right);
        }

        public static bool operator ==(TException left, Try<T, TException> right)
        {
            return right == left;
        }

        public static bool operator !=(TException left, Try<T, TException> right)
        {
            return !(left == right);
        }

        public static bool operator true(Try<T, TException> r) => r.HasValue;

        public static bool operator false(Try<T, TException> r) => !r.HasValue;

        public static Try<T, TException> operator &(Try<T, TException> t1, Try<T, TException> t2) => !t1 ? t1 : t2;

        public static Try<T, TException> operator |(Try<T, TException> t1, Try<T, TException> t2) => t1 ? t1 : t2;

        public override string ToString() => Match(val => val!.ToString(), ex => ex!.ToString())!;
    }
}