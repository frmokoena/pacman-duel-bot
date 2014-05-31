using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacmanduelbot.models
{
    class ChoosePathNode : BuildPathNode
    {
        //choose path
        public bool _isLeaf = false;
        public int _score = 0;

        public ChoosePathNode _parent = null;
    }
}
