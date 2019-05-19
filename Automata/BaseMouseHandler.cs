using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Automata
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

        /// <summary>
        /// khởi động việc theo dõi sự kiện chuột
        /// </summary>
        /// <param name="sender">nguồn tạo ra sự kiện</param>
        /// <param name="sourceChain">chuỗi đối tượng chịu ảnh hưởng</param>
        /// <param name="e">sự kiện chuột</param>                
        public void InitiateTrackMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e)
        {
            List<BaseMouseHandler> list = new List<BaseMouseHandler>(Config.MAX_HANDLERS);
            List<object> senderList = new List<object>(Config.MAX_HANDLERS);
            list.Add(this);
            senderList.Add(sender);
            while (list.Count > 0)
            {
                var currentObj = list[0];
                var currentSender = senderList[0];
                if(!sourceChain.Contains(currentObj))
                {
                    /*nếu đối tượng hiện tại chịu ảnh hưởng
                     * cập nhật thêm danh sách các đối tượng nó kết nối tới
                     */
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

        /// <summary>
        /// bắt đầu xử lý sự kiện chuột. Lúc này chuột đang di chuyển
        /// </summary>
        /// <param name="sender">Nguồn gửi sự kiện</param>
        /// <param name="sourceChain">chuỗi đối tượng bị ảnh hưởng</param>
        /// <param name="e">sự kiện chuột</param>
        public void InitiateHandleMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e)
        {
            List<BaseMouseHandler> list = new List<BaseMouseHandler>(Config.MAX_HANDLERS);
            List<object> senderList = new List<object>(Config.MAX_HANDLERS);
            list.Add(this);
            senderList.Add(sender);
            while (list.Count > 0)
            {
                var currentObj = list[0];
                var currentSender = senderList[0];
                if (!sourceChain.Contains(currentObj))
                {
                    /*nếu đối tượng hiện tại chịu ảnh hưởng
                     * cập nhật thêm danh sách các đối tượng nó kết nối tới
                     */
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
                        if(mouseHandler._handlerList.IndexOf(currentObj) 
                            == mouseHandler._handlerList.Count - 1)
                            if(mouseHandler.lateFunc != null)
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

        // theo dõi sự kiện xảy ra với chuột
        public abstract bool TrackMouse(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e);
        // xử lý sự kiện xảy ra với chuột
        public abstract bool HandleMouseEvent(object sender, List<BaseMouseHandler> sourceChain, MouseEventArgs e);

        /// <summary>
        /// thêm kết nối với đối tượng
        /// </summary>
        /// <param name="mouseHandler"></param>
        public void AddHandler(BaseMouseHandler mouseHandler)
        {
        	_handlerList.Add(mouseHandler);
        }

        /// <summary>
        /// gỡ bỏ một kết nối với đối tượng
        /// </summary>
        /// <param name="mouseHandler"></param>
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

    /*
	    struct SourceMouseHandler
	    {
	        public BaseMouseHandler originalSender;
	        public BaseMouseHandler sender;
	
	    }
     */
}