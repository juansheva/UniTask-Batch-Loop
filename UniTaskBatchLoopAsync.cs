namespace Batch_Loop_Async.UniTask
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;


    public static class UniTaskBatchLoopAsync
    {
        #region Common Loop

        public static async UniTask UniTaskBatchLoop(int totaliterations, int batchsize, Action<int> process)
        {
            for (int i = 0; i < totaliterations; i += batchsize)
            {
                int end = Mathf.Min(i + batchsize, totaliterations);
                for (int j = i; j < end; j++)
                    process(j);
                await UniTask.Yield(); // Prevent Unity freezing
            }
        }

        public static async UniTask UniTaskBatchLoop<T>(T[] arraydata, int batchsize, Action<T> process) =>
            await UniTaskBatchLoop(arraydata.Length, batchsize, (i) => process(arraydata[i]));

        public static async UniTask UniTaskBatchLoopReverse(int totaliterations, int batchsize, Action<int> process)
        {
            for (int i = totaliterations; i > 0; i -= batchsize)
            {
                int start = Mathf.Max(i - batchsize, 0);
                for (int j = i - 1; j >= start; j--)
                    process(j);
                await UniTask.Yield(); // Prevent Unity freezing
            }
        }

        public static async UniTask UniTaskBatchLoopReverse<T>(T[] arraydata, int batchsize, Action<T> process) =>
            await UniTaskBatchLoopReverse(arraydata.Length, batchsize, (i) => process(arraydata[i]));

        #endregion

        #region Get All

        public static async UniTask<T[]> UniTaskBatchLoopGetAll<T>(int totaliterations, int batchsize,
            Func<int, T> process)
        {
            var results = new List<T>();

            for (int i = 0; i < totaliterations; i += batchsize)
            {
                int end = Mathf.Min(i + batchsize, totaliterations);
                for (int j = i; j < end; j++)
                {
                    T result = process(j);
                    if (result != null)
                        results.Add(result);
                }

                await UniTask.NextFrame(); // Prevent Unity freezing
            }

            return results.ToArray();
        }

        public static async UniTask<T[]> UniTaskBatchLoopGetAll<T>(T[] arraydata, int batchsize, Func<T, T> process) =>
            await UniTaskBatchLoopGetAll(arraydata.Length, batchsize, (i) => process(arraydata[i]));

        #endregion

        #region Get One

        public static async UniTask<T> UniTaskBatchLoopGetOne<T>(int totaliterations, int batchsize,
            Func<int, T> process, bool reverse = false)
        {
            if (!reverse)
            {
                for (int i = 0; i < totaliterations; i += batchsize)
                {
                    int end = Mathf.Min(i + batchsize, totaliterations);
                    for (int j = i; j < end; j++)
                    {
                        Debug.Log(j);
                        T result = process(j);
                        if (result != null)
                            return result;
                    }

                    await UniTask.NextFrame(); // Prevent Unity freezing
                }
            }
            else
            {
                for (int i = totaliterations; i > 0; i -= batchsize)
                {
                    int start = Mathf.Max(i - batchsize, 0);
                    for (int j = i - 1; j >= start; j--)
                    {
                        Debug.Log(j);
                        T result = process(j);
                        if (result != null)
                            return result;
                    }

                    await UniTask.Yield(); // Prevent Unity freezing
                }
            }

            return default;
        }

        public static async UniTask<T> UniTaskBatchLoopGetOneFirst<T>(int totaliterations, int batchsize,
            Func<int, T> process) => await UniTaskBatchLoopGetOne(totaliterations, batchsize, (j) => process(j));

        public static async UniTask<T>
            UniTaskBatchLoopGetOneFirst<T>(T[] arraydata, int batchsize, Func<T, T> process) =>
            await UniTaskBatchLoopGetOneFirst(arraydata.Length, batchsize, (i) => process(arraydata[i]));

        public static async UniTask<T> UniTaskBatchLoopGetOneLast<T>(int totaliterations, int batchsize,
            Func<int, T> process) => await UniTaskBatchLoopGetOne(totaliterations, batchsize, (j) => process(j), true);

        public static async UniTask<T>
            UniTaskBatchLoopGetOneLast<T>(T[] arraydata, int batchsize, Func<T, T> process) =>
            await UniTaskBatchLoopGetOneLast(arraydata.Length, batchsize, (i) => process(arraydata[i]));

        public static async UniTask<T> UniTaskBatchLoopGetOneEntire<T>(int totaliterations, int batchsize,
            Func<int, T> process)
        {
            T result = default(T);
            await UniTaskBatchLoop(totaliterations, batchsize, (j) => result = process(j));
            return result;
        }

        public static async UniTask<T> UniTaskBatchLoopGetOneEntire<T>(T[] arraydata, int batchsize,
            Func<T, T, T> process)
        {
            T result = default(T);
            await UniTaskBatchLoopGetOneEntire(arraydata.Length, batchsize, (i) =>
            {
                result = process(result, arraydata[i]);
                return result;
            });
            return result;
        }

        #endregion
    }
}