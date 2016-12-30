﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinRangeOp: GremlinTranslationOperator
    {
        public long Low { get; set; }
        public long High { get; set; }
        public GremlinRangeOp(long low, long high)
        {
            Low = low;
            High = high;
        }
        public override GremlinToSqlContext GetContext()
        {
            GremlinToSqlContext inputContext = GetInputContext();
            throw new NotImplementedException();

            return inputContext;
        }
    }
}
