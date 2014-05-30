using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacmanduelbot.models
{
    class PathNode
    {
        public Point _position { get; set; }

        //build path
        public int _g { get; set; }
        public int _h { get; set; }
        public int _f { get; set; }

        //choose path
        public bool isLeaf = false;
        public int _score = 0;

        //both        
        public PathNode _parent = null;
    }
}
