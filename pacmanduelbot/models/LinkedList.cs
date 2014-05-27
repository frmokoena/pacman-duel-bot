using System.Drawing;

namespace pacmanduelbot.models
{
    public class LinkedList
    {
        public class Node
        {
            public Point _position { get; set; }
            public int _g { get; set; }
            public int _h { get; set; }
            public int _f { get; set; }
            public Node _parent { get; set; }
            public Node _next = null;
        }

        private Node _head = null;

        public bool isEmpty()
        {
            return _head == null;
        }

        public Node First { get { return _head; } }
        public Node Last
        {
            get
            {
                Node _current = _head;
                if (_current == null)
                    return null;
                while (_current._next != null)
                    _current = _current._next;
                return _current;

            }
        }

        public void Append(Point position, int g, int h, int f, Node parent)
        {
            Node _node = new Node { _position = position, _g = g, _h = h, _f = f, _parent = parent };
            if (!isEmpty())
                Last._next = _node;
            else
                _head = _node;

        }

        public void Insert(Node _node)
        {

            if (!isEmpty())
                Last._next = _node;
            else
                _head = _node;

        }

        public void Delete(Node _node)
        {
            if (_node == _head)
            {
                _head = _node._next;
                _node._next = null;
            }
            else
            {
                Node _current = _head;
                while (_current._next != null)
                {
                    if (_current._next == _node)
                    {
                        _current._next = _node._next;
                        _node._next = null;
                        break;
                    }
                    _current = _current._next;
                }
            }
        }

        public bool contains(Node _node)
        {
            bool _found = false;
            if (_node._position.X == _head._position.X
                && _node._position.Y == _head._position.Y)
            {
                _found = true;
            }
            else
            {
                Node _current = _head;
                while (_current._next != null)
                {
                    if (_current._next._position.X == _node._position.X
                        && _current._next._position.Y == _node._position.Y)
                    {
                        _found = true;
                        break;
                    }
                    _current = _current._next;
                }
            }
            return _found;
        }
    }
}
