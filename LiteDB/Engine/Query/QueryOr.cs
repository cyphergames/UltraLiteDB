using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDB
{
    internal class QueryOr : Query
    {
        private Query _left;
        private Query _right;

        public QueryOr(Query left, Query right)
            : base()
        {
            _left = left;
            _right = right;
        }

        internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            var left = _left.Run(col, indexer);
            var right = _right.Run(col, indexer);

            return left.Union(right, new IndexNodeComparer());
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("({0} or {1})", _left, _right);
        }
    }
}