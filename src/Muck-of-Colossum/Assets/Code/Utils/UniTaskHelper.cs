using System;
using Cysharp.Threading.Tasks;

namespace Code.Utils
{
    public static class UniTaskHelper {
        public static Action<T> Action<T>(Func<T, UniTask> asyncAction) => 
            (t1) => asyncAction(t1).Forget();
        public static Action<T1, T2> Action<T1, T2>(Func<T1, T2, UniTask> asyncAction) => 
            (t1, t2) => asyncAction(t1, t2).Forget();
        public static Action<T1, T2, T3> Action<T1, T2, T3>(Func<T1, T2, T3, UniTask> asyncAction) => 
            (t1, t2, t3) => asyncAction(t1, t2, t3).Forget();
        
        public static Action<T> Action<T>(Func<T, UniTaskVoid> asyncAction) => 
            (t1) => asyncAction(t1).Forget();
        public static Action<T1, T2> Action<T1, T2>(Func<T1, T2, UniTaskVoid> asyncAction) => 
            (t1, t2) => asyncAction(t1, t2).Forget();
        public static Action<T1, T2, T3> Action<T1, T2, T3>(Func<T1, T2, T3, UniTaskVoid> asyncAction) => 
            (t1, t2, t3) => asyncAction(t1, t2, t3).Forget();
    }
}