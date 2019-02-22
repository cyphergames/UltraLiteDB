using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    internal class QueryLess : Query
    {
        private BsonValue _value;
        private bool _equals;

        public BsonValue Value { get { return _value; } }
        public bool IsEquals { get { return _equals; } }

        public QueryLess(BsonValue value, bool equals)
            : base()
        {
            _value = value;
            _equals = equals;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            foreach (var node in indexer.FindAll(index, Query.Ascending))
            {
                // compares only with are same type
                if (node.Key.Type == _value.Type || (node.Key.IsNumber && _value.IsNumber))
                {
                    var diff = node.Key.CompareTo(_value);

                    if (diff == 1 || (!_equals && diff == 0)) break;

                    if (node.IsHeadTail(index)) yield break;

                    yield return node;
                }
            }
        }


    }
}