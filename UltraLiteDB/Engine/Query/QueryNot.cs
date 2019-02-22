using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    /// <summary>
    /// Not is an Index Scan operation
    /// </summary>
    internal class QueryNot : Query
    {
        private Query _query;
        private int _order;

        public QueryNot(Query query, int order)
            : base()
        {
            _query = query;
            _order = order;
        }

        internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            // run base query
            var result = _query.Run(col, indexer);

            // if is by index, resolve here
            var all = new QueryAll(_order).Run(col, indexer);

            return all.Except(result, new IndexNodeComparer());
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return string.Format("!({0})", _query);
        }
    }
}