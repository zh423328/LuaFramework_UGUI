/******************************************************
 * file: AsyncOperationMgr.cs
 * date: 2016/10/10
 *
 * author:郑辉
 *
 * TODO: 对AsyncOperation异步管理封装
*******************************************************/
//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LuaFramework
{
    public class AsyncOperationMgr: Manager
    {
        public interface IAsyncOperationItem
        {
            void Release();
            AsyncOperation GetOperation();
            void Process();
            /// <summary>
            /// 是否删除
            /// </summary>
            /// <returns></returns>
            bool IsDelete();
            void SetDelete(bool bDelete);
        }

        public class AsyncOperationItem<T, U> : IAsyncOperationItem where T : AsyncOperation
        {
            internal T opt = null;
            internal System.Action<T> onProcess = null;
            private bool bDelete = false;


            public void Release()
            {
            }
            public AsyncOperation GetOperation()
            {
                return opt;
            }

            public bool IsDelete()
            {
                return bDelete;
            }

            public void SetDelete(bool delete)
            {
                bDelete = delete;
            }
            public U UserData
            {
                get;
                set;
            }
            public static AsyncOperationItem<T, U> Create()
            {
                AsyncOperationItem<T, U> ret1 = new AsyncOperationItem<T, U>();
                return ret1;
            }
            private void Reset()
            {
                opt = null;
                onProcess = null;
                UserData = default(U);
            }
            public void Process()
            {
                if((opt == null) || (onProcess == null))
                    return;

                onProcess(opt);
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(mDic.Count == 0)
                return;

            Dictionary<AsyncOperation, TimerInfo>.Enumerator iter = mDic.GetEnumerator();

            while(iter.MoveNext())
            {
                if(iter.Current.Value != null)
                {
                    if(iter.Current.Value.target != null)
                    {
                        OnTimerEvent(iter.Current.Value, Time.deltaTime);
                    }
                }
            }

            if(mDeleteList.Count > 0)
            {
                for(int i = 0; i < mDeleteList.Count; ++i)
                {
                    IAsyncOperationItem item = mDeleteList[i];
                    RemoveAsyncOperation(item);
                }

                mDeleteList.Clear();
            }
        }

        #region public function

        public AsyncOperationItem<T, U> FindItem<T, U>(T opt) where T : AsyncOperation
        {
            if(opt == null)
                return null;

            TimerInfo time;

            if(mDic.TryGetValue(opt, out time))
            {
                return GetAsyncOptionTimerOprItem<T, U>(time);
            }
            else
                return null;
        }

        public AsyncOperationItem<T, U> AddAsyncOperation<T, U>(T opt, System.Action<T> onProcess) where T : AsyncOperation
        {
            TimerInfo time;

            if(mDic.TryGetValue(opt, out time))
            {
                if(time == null)
                    return null;

                AsyncOperationItem<T, U> old = time.target as AsyncOperationItem<T, U>;

                if(old == null)
                    return null;

                if(onProcess != null)
                    old.onProcess += onProcess;

                return old;
            }

            AsyncOperationItem<T, U> item = new AsyncOperationItem<T, U>();
            item.opt = opt;
            time = new TimerInfo("AsyncOperationItem", item);
            item.onProcess = onProcess;
            mDic.Add(opt, time);
            return item;
        }

        public void Clear()
        {
            Dictionary<AsyncOperation, TimerInfo>.Enumerator iter = mDic.GetEnumerator();

            while(iter.MoveNext())
            {
                if(iter.Current.Value != null)
                {
                    if(iter.Current.Value.target != null)
                    {
                        IAsyncOperationItem item = iter.Current.Value.target as IAsyncOperationItem;

                        if(item != null)
                            item.Release();
                    }
                }
            }

            iter.Dispose();
            mDic.Clear();
        }


        public void AddDeleteList(IAsyncOperationItem item)
        {
            if(item == null)
                return;

            item.SetDelete(true);
        }

        public void AddDeleteList(AsyncOperation opt)
        {
            TimerInfo time;

            if(mDic.TryGetValue(opt, out time) && (time != null))
            {
                IAsyncOperationItem item = (IAsyncOperationItem)time.target;

                if(item == null)
                    return;

                item.SetDelete(true);
            }
        }


        public AsyncOperation GetAsyncOptionTimerOpr(TimerInfo obj)
        {
            if(obj == null)
                return null;

            IAsyncOperationItem item = (IAsyncOperationItem)obj.target;

            if(item == null)
                return null;

            AsyncOperation opt = item.GetOperation();
            return opt;
        }

        public AsyncOperationItem<T, U> GetAsyncOptionTimerOprItem<T, U>(TimerInfo obj) where T : AsyncOperation
        {
            if(obj == null)
                return null;

            IAsyncOperationItem item = (IAsyncOperationItem)obj.target;

            if(item == null)
                return null;

            return item as AsyncOperationItem<T, U>;
        }

        #endregion public function

        #region protected function

        protected void RemoveAsyncOperation(IAsyncOperationItem item)
        {
            if(item == null)
                return;

            AsyncOperation opt = item.GetOperation();
            RemoveAsyncOperation(opt);
            item.Release();
        }

        protected void RemoveAsyncOperation(AsyncOperation opt)
        {
            if(opt == null)
                return;

            TimerInfo time;

            if(mDic.TryGetValue(opt, out time) && (time != null))
            {
                mDic.Remove(opt);
            }
        }

        protected void OnTimerEvent(TimerInfo obj, float timer)
        {
            IAsyncOperationItem item = (IAsyncOperationItem)obj.target;

            if(item.IsDelete())
            {
                mDeleteList.Add(item);
                return;
            }

            AsyncOperation opt = item.GetOperation();
            item.Process();

            if(opt.isDone)
            {
                mDeleteList.Add(item);
            }

            //RemoveAsyncOperation(item);
        }
        #endregion protected function

        protected Dictionary<AsyncOperation, TimerInfo> mDic = new Dictionary<AsyncOperation, TimerInfo>();
        protected List<IAsyncOperationItem> mDeleteList = new List<IAsyncOperationItem>();
    }

}
