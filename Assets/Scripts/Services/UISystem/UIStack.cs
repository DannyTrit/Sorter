using System.Collections.Generic;

namespace Sorter.Services.UI
{
    public class UIStack
    {
        private readonly Stack<UIViewBase> _views = new Stack<UIViewBase>();

        public int Count => _views.Count;

        public void Add(UIViewBase view) => _views.Push(view);

        public void Remove(UIViewBase view)
        {
            var temp = new Stack<UIViewBase>();
            while (_views.Count > 0 && _views.Peek() != view) 
                temp.Push(_views.Pop());
            
            if (_views.Count > 0)
                _views.Pop();
            
            while (temp.Count != 0)
                _views.Push(temp.Pop());
        }

        public void CloseTop()
        {
            if (_views.Count > 0)
                _views.Pop().Close();
        }
        
        public void CloseAll()
        {
            var count = Count;
            while (count > 0)
            {
                CloseTop();
                count--;
            }
        }
    }
}