using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace pacmanduelbot.models
{
    class PathNode
    {
        private PathNode parent = null;

        private Point position;
        private int _g_score, _h_score, _f_score, _score;
        
        public void InsertChild(PathNode newChild)
        {
            newChild.parent = this;
        }

        public PathNode Parent
        {
            get
            {
                return parent;
            }
        }

        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
       
        public int GScore
        {
            get
            {
                return _g_score;
            }
            set
            {
                _g_score = value;
            }
        }

        public int HScore
        {
            get
            {
                return _h_score;
            }
            set
            {
                _h_score = value;
            }
        }

        public int FScore
        {
            get
            {
                return _f_score;
            }
            set
            {
                _f_score = value;
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
            }
        }
    }
}