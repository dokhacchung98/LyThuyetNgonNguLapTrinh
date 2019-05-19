using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Entity
{
    public abstract class BaseMouseHandler
    {
        List<BaseMouseHandler> _handlerList;
        delegate bool MouseEventDelegate(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e);
        /// <summary>
        /// Initializes a new instance of the BaseMouseHandler class.
        /// </summary>
        public BaseMouseHandler()
        {
            _handlerList = new List<BaseMouseHandler>();
        }


        public void InitiateTrackMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e)
        {
            List<BaseMouseHandler> list = new List<BaseMouseHandler>(50);
            List<object> senderList = new List<object>(50);
            list.Add(this);
            senderList.Add(sender);
            while (list.Count > 0)
            {
                var currentObj = list[0];
                var currentSender = senderList[0];
                if (!sourceChain.Contains(currentObj))
                {
                    if (currentObj.TrackMouse(currentSender, sourceChain, e))
                    {
                        list.AddRange(currentObj._handlerList);
                        for (int i = 0; i < currentObj._handlerList.Count; i++)
                        {
                            senderList.Add(currentObj);
                        }
                    }
                    sourceChain.Add(currentObj);
                    if (currentSender is BaseMouseHandler)
                    {
                        var mouseHandler = currentSender as BaseMouseHandler;
                        if (mouseHandler._handlerList.IndexOf(currentObj)
                            == mouseHandler._handlerList.Count - 1)
                            if (mouseHandler.lateFunc != null)
                            {
                                mouseHandler.lateFunc();
                                mouseHandler.lateFunc = null;
                            }
                    }
                }
                list.RemoveAt(0);
                senderList.RemoveAt(0);
            }
        }
        public delegate void voidFunction();
        voidFunction lateFunc;

        public void ExecuteAfter(voidFunction func)
        {
            lateFunc = lateFunc == null ? func : lateFunc + func;
        }

        public void InitiateHandleMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e)
        {
            List<BaseMouseHandler> list = new List<BaseMouseHandler>(50);
            List<object> senderList = new List<object>(50);
            list.Add(this);
            senderList.Add(sender);
            while (list.Count > 0)
            {
                var currentObj = list[0];
                var currentSender = senderList[0];
                if (!sourceChain.Contains(currentObj))
                {
                    if (currentObj.HandleMouseEvent(currentSender, sourceChain, e))
                    {
                        list.AddRange(currentObj._handlerList);
                        for (int i = 0; i < currentObj._handlerList.Count; i++)
                        {
                            senderList.Add(currentObj);
                        }
                    }
                    sourceChain.Add(currentObj);
                    if (currentSender is BaseMouseHandler)
                    {
                        var mouseHandler = currentSender as BaseMouseHandler;
                        if (mouseHandler._handlerList.IndexOf(currentObj)
                            == mouseHandler._handlerList.Count - 1)
                            if (mouseHandler.lateFunc != null)
                            {
                                mouseHandler.lateFunc();
                                mouseHandler.lateFunc = null;
                            }

                    }
                }
                list.RemoveAt(0);
                senderList.RemoveAt(0);
            }
        }
        public abstract bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e);
        public abstract bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e);


        public void AddHandler(BaseMouseHandler mouseHandler)
        {
            _handlerList.Add(mouseHandler);
        }
        public void RemoveHandler(BaseMouseHandler mouseHandler)
        {
            _handlerList.Remove(mouseHandler);
        }
        public bool InCollection(BaseMouseHandler mouseHandler)
        {
            return _handlerList.Contains(mouseHandler);
        }
        public bool Sastisfy(Predicate<BaseMouseHandler> pred)
        {
            return _handlerList.Exists(pred);
        }
        public BaseMouseHandler Find(Predicate<BaseMouseHandler> pred)
        {
            return _handlerList.Find(pred);
        }

    }
    struct SourceMouseHandler
    {
        public BaseMouseHandler originalSender;
        public BaseMouseHandler sender;

    }
}
