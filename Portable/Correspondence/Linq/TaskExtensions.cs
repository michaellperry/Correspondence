using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Correspondence.Linq
{
    public static class TaskExtensions
    {
        public static Task<B> AndThen<A,B>(
            this Task<A> asyncFirst,
            Func<A, B> second)
        {
            return asyncFirst.ContinueWith(ta => second(ta.Result));
        }

        public static async Task<B> AndThen<A,B>(
            this Task<A> asyncFirst,
            Func<A, Task<B>> asyncSecond)
        {
            return await await asyncFirst.ContinueWith(ta => asyncSecond(ta.Result));
        }

        public static async Task<IEnumerable<A>> Where<A>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, bool> predicate)
        {
            return (await asyncSource).Where(predicate);
        }

        public static async Task<IEnumerable<A>> Where<A>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, Task<bool>> asyncPredicate)
        {
            return await (await asyncSource).Where(asyncPredicate);
        }

        public static async Task<IEnumerable<A>> Where<A>(
            this IEnumerable<A> source,
            Func<A, Task<bool>> asyncPredicate)
        {
            var filtered = await source.Select(async a =>
            {
                bool condition = await asyncPredicate(a);
                return new { a, condition };
            });

            return filtered
                .Where(p => p.condition)
                .Select(p => p.a);
        }

        public static async Task<IEnumerable<B>> Select<A, B>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, B> selector)
        {
            return (await asyncSource).Select(selector);
        }

        public static async Task<IEnumerable<B>> Select<A, B>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, Task<B>> asyncSelector)
        {
            return await (await asyncSource).Select(asyncSelector);
        }

        public static async Task<IEnumerable<B>> Select<A, B>(
            this IEnumerable<A> source,
            Func<A, Task<B>> asyncSelector)
        {
            return await Task.WhenAll(Enumerable.Select(source, asyncSelector));
        }

        public static async Task<IEnumerable<A>> OrderBy<A, B>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, Task<B>> asyncKeySelector)
        {
            return await (await asyncSource).OrderBy(asyncKeySelector);
        }

        public static async Task<IEnumerable<A>> OrderBy<A, B>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, B> keySelector)
        {
            return (await asyncSource).OrderBy(keySelector);
        }

        public static async Task<IEnumerable<A>> OrderBy<A,B>(
            this IEnumerable<A> source,
            Func<A, Task<B>> asyncKeySelector)
        {
            var indexed = await source.Select(async a =>
            {
                var b = await asyncKeySelector(a);
                return new { a, b };
            });
            return indexed
                .OrderBy(p => p.b)
                .Select(p => p.a);
        }

        public static async Task<A> FirstOrDefault<A>(
            this Task<IEnumerable<A>> asyncSource)
        {
            return (await asyncSource).FirstOrDefault();
        }

        public static async Task<A> FirstOrDefault<A>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, bool> predicate)
        {
            return (await asyncSource).FirstOrDefault(predicate);
        }

        public static async Task<A> LastOrDefault<A>(
            this Task<IEnumerable<A>> asyncSource)
        {
            return (await asyncSource).LastOrDefault();
        }

        public static async Task<A> LastOrDefault<A>(
            this Task<IEnumerable<A>> asyncSource,
            Func<A, bool> predicate)
        {
            return (await asyncSource).LastOrDefault(predicate);
        }
    }
}
