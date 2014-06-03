﻿using System.Drawing;

namespace pacmanduelbot.models
{
    class Node
    {
        public Point _position { get; set; }

        //build path
        public int _g { get; set; }
        public int _h { get; set; }
        public int _f { get; set; }

        //choose path
        public bool _isLeaf { get; set; }
        public int _score { get; set; }

        //both        
        public Node _parent { get; set; }
    }
}
