using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Correspondence.Linq
{
    public static class CorrespondenceTaskExtensions
    {
        public static async Task<IEnumerable<A>> Where<A>(
            this Task<Result<A>> asyncSource,
            Func<A, bool> predicate)
            where A: CorrespondenceFact
        {
            return (await asyncSource).Where(predicate);
        }

        public static async Task<IEnumerable<A>> Where<A>(
            this Task<Result<A>> asyncSource,
            Func<A, Task<bool>> asyncPredicate)
            where A : CorrespondenceFact
        {
            return await (await asyncSource).Where(asyncPredicate);
        }

        public static async Task<IEnumerable<B>> Select<A, B>(
            this Task<Result<A>> asyncSource,
            Func<A, B> selector)
            where A: CorrespondenceFact
        {
            return (await asyncSource).Select(selector);
        }

        public static async Task<IEnumerable<B>> Select<A, B>(
            this Task<Result<A>> asyncSource,
            Func<A, Task<B>> asyncSelector)
            where A: CorrespondenceFact
        {
            return await (await asyncSource).Select(asyncSelector);
        }

        public static async Task<IEnumerable<A>> OrderBy<A, B>(
            this Task<Result<A>> asyncSource,
            Func<A, Task<B>> asyncKeySelector)
            where A : CorrespondenceFact
        {
            return await (await asyncSource).OrderBy(asyncKeySelector);
        }

        public static async Task<IEnumerable<A>> OrderBy<A, B>(
            this Task<Result<A>> asyncSource,
            Func<A, Task<Disputable<B>>> asyncKeySelector)
            where A : CorrespondenceFact
        {
            return await (await asyncSource).OrderBy(
                async a => (await asyncKeySelector(a)).Value);
        }

        public static async Task<A> FirstOrDefault<A>(
            this Task<Result<A>> asyncSource)
            where A: CorrespondenceFact
        {
            return (await asyncSource).FirstOrDefault();
        }

        public static async Task<A> FirstOrDefault<A>(
            this Task<Result<A>> asyncSource,
            Func<A, bool> predicate)
            where A : CorrespondenceFact
        {
            return (await asyncSource).FirstOrDefault(predicate);
        }

        public static async Task<A> LastOrDefault<A>(
            this Task<Result<A>> asyncSource)
            where A : CorrespondenceFact
        {
            return (await asyncSource).LastOrDefault();
        }

        public static async Task<A> LastOrDefault<A>(
            this Task<Result<A>> asyncSource,
            Func<A, bool> predicate)
            where A : CorrespondenceFact
        {
            return (await asyncSource).LastOrDefault(predicate);
        }
    }
}