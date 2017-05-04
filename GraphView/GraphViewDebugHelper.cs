using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using static GraphView.GraphViewKeywords;

namespace GraphView
{
    public class GraphViewDebugHelper
    {
        private readonly GraphViewConnection _connection;

        public GraphViewDebugHelper(GraphViewConnection connection)
        {
            this._connection = connection;
        }
    }
}
