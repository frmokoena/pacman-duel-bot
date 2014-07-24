using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace pacmanduelbot.models
{
    class PathFinderNode
    {
        public Point _position { get; set; }

        //build path
        public int _g_score { get; set; }
        public int _h_score { get; set; }
        public int _f_score { get; set; }

        //choose path
        public bool _isLeaf { get; set; }
        public int _score { get; set; }

        //both        
        public PathFinderNode _parent { get; set; }
    }
}