﻿// GraphView
// 
// Copyright (c) 2015 Microsoft Corporation
// 
// All rights reserved. 
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphView;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace GraphView
{
    internal class MatchEdge
    {
        public MatchNode SourceNode { get; set; }
        public WColumnReferenceExpression EdgeColumn { get; set; }
        public string EdgeAlias { get; set; }
        public MatchNode SinkNode { get; set; }

        /// <summary>
        /// Schema Object of the node table/node view which the edge is bound to.
        /// It is an instance in the syntax tree.
        /// </summary>
        public WSchemaObjectName BindNodeTableObjName { get; set; }
        public double AverageDegree { get; set; }
        public IList<WBooleanExpression> Predicates { get; set; }

        public EdgeStatistics Statistics { get; set; }

        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public IList<Tuple<string, string>> IncludedEdgeNames { get; set; }
        public Dictionary<string, string> AttributeValueDict { get; set; }

        public override int GetHashCode()
        {
            return EdgeAlias.GetHashCode();
        }
        public bool IsPath
        {
            get { return !(MinLength == 1 && MaxLength == 1); }
        }

        public bool IsView
        {
            get { return IncludedEdgeNames != null && IncludedEdgeNames.Any(); }
        }
    }

    internal class MatchNode
    {
        public string NodeAlias { get; set; }
        public WSchemaObjectName NodeTableObjectName { get; set; }
        public IList<MatchEdge> Neighbors { get; set; }
        public double EstimatedRows { get; set; }
        public int TableRowCount { get; set; }
        /// <summary>
        /// True, if this node alias is defined in one of the parent query contexts;
        /// false, if the node alias is defined in the current query context.
        /// </summary>
        public bool External { get; set; }

        /// <summary>
        /// The density value of the GlobalNodeId Column of the corresponding node table.
        /// This value is used to estimate the join selectivity of A-->B. 
        /// </summary>
        public double GlobalNodeIdDensity { get;set; }

        public IList<WBooleanExpression> Predicates { get; set; }
        public HashSet<string> IncludedNodeNames { get; set; } 

        public string RefAlias
        {
            get { return NodeAlias + (External ? "Prime" : ""); }
        }

        public override int GetHashCode()
        {
            return NodeAlias.GetHashCode();
        }

        public bool IsView
        {
            get { return IncludedNodeNames != null && IncludedNodeNames.Any(); }
        }

    }

    internal class ConnectedComponent
    {
        public Dictionary<string, MatchNode> Nodes { get; set; }
        public Dictionary<string, MatchEdge> Edges { get; set; }
        public Dictionary<MatchNode, bool> IsTailNode { get; set; }


        public ConnectedComponent()
        {
            Nodes = new Dictionary<string, MatchNode>(StringComparer.OrdinalIgnoreCase);
            Edges = new Dictionary<string, MatchEdge>(StringComparer.OrdinalIgnoreCase);
            IsTailNode = new Dictionary<MatchNode, bool>();
        }
    }

    internal class MatchGraph
    {
        // Fully-connected components in the graph pattern 
        public IList<ConnectedComponent> ConnectedSubGraphs { get; set; }

        public bool ContainsNode(string key)
        {
            return ConnectedSubGraphs.Any(e => e.Nodes.ContainsKey(key) && !e.IsTailNode[e.Nodes[key]]);
        }

        public bool TryGetNode(string key, out MatchNode node)
        {
            foreach (var subGraph in ConnectedSubGraphs)
            {
                if (subGraph.Nodes.TryGetValue(key, out node))
                {
                    return true;
                }
            }
            node = null;
            return false;
        }

        public bool TryGetEdge(string key, out MatchEdge edge)
        {
            foreach (var subGraph in ConnectedSubGraphs)
            {
                if (subGraph.Edges.TryGetValue(key, out edge))
                {
                    return true;
                }
            }
            edge = null;
            return false;
        }

    }
}
