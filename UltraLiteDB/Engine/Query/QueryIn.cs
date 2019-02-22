using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    internal class QueryIn : Query
    {
        private IEnumerable<BsonValue> _values;

        public QueryIn(IEnumerable<BsonValue> values)
            : base()
        {
            _values = values ?? Enumerable.Empty<BsonValue>();
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            foreach (var value in _values.Distinct())
            {
                foreach (var node in Query.EQ(value).ExecuteIndex(indexer, index))
                {
                    yield return node;
                }
            }
        }


    }
}