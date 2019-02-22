using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    /// <summary>
    /// Class helper to create query using indexes in database. All methods are statics.
    /// Queries can be executed in 3 ways: Index Seek (fast), Index Scan (good), Full Scan (slow)
    /// </summary>
    public abstract class Query
    {
        internal Query()
        {
            
        }

        #region Static Methods

        /// <summary>
        /// Indicate when a query must execute in ascending order
        /// </summary>
        public const int Ascending = 1;

        /// <summary>
        /// Indicate when a query must execute in descending order
        /// </summary>
        public const int Descending = -1;

        /// <summary>
        /// Returns all documents using _id index order
        /// </summary>
        public static Query All(int order = Ascending)
        {
            return new QueryAll(order);
        }

        /// <summary>
        /// Returns all documents that value are equals to value (=)
        /// </summary>
        public static Query EQ(BsonValue value)
        {
            return new QueryEquals(value ?? BsonValue.Null);
        }

        /// <summary>
        /// Returns all documents that value are less than value (&lt;)
        /// </summary>
        public static Query LT(BsonValue value)
        {
            return new QueryLess(value ?? BsonValue.Null, false);
        }

        /// <summary>
        /// Returns all documents that value are less than or equals value (&lt;=)
        /// </summary>
        public static Query LTE(BsonValue value)
        {
            return new QueryLess(value ?? BsonValue.Null, true);
        }

        /// <summary>
        /// Returns all document that value are greater than value (&gt;)
        /// </summary>
        public static Query GT(BsonValue value)
        {
            return new QueryGreater(value ?? BsonValue.Null, false);
        }

        /// <summary>
        /// Returns all documents that value are greater than or equals value (&gt;=)
        /// </summary>
        public static Query GTE(BsonValue value)
        {
            return new QueryGreater(value ?? BsonValue.Null, true);
        }

        /// <summary>
        /// Returns all document that values are between "start" and "end" values (BETWEEN)
        /// </summary>
        public static Query Between(BsonValue start, BsonValue end, bool startEquals = true, bool endEquals = true)
        {
            return new QueryBetween(start ?? BsonValue.Null, end ?? BsonValue.Null, startEquals, endEquals);
        }

        /// <summary>
        /// Returns all documents that starts with value (LIKE)
        /// </summary>
        public static Query StartsWith(string value)
        {
            if (value.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(value));

            return new QueryStartsWith(value);
        }

        /// <summary>
        /// Returns all documents that contains value (CONTAINS)
        /// </summary>
        public static Query Contains(string value)
        {
            if (value.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(value));

            return new QueryContains(value);
        }

        /// <summary>
        /// Returns all documents that are not equals to value (not equals)
        /// </summary>
        public static Query Not(BsonValue value)
        {
            return new QueryNotEquals(value ?? BsonValue.Null);
        }

        /// <summary>
        /// Returns all documents that in query result (not result)
        /// </summary>
        public static Query Not(Query query, int order = Query.Ascending)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            return new QueryNot(query, order);
        }

        /// <summary>
        /// Returns all documents that has value in values list (IN)
        /// </summary>
        public static Query In(BsonArray value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return new QueryIn(value.RawValue);
        }

        /// <summary>
        /// Returns all documents that has value in values list (IN)
        /// </summary>
        public static Query In(params BsonValue[] values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            return new QueryIn(values);
        }

        /// <summary>
        /// Returns all documents that has value in values list (IN)
        /// </summary>
        public static Query In(IEnumerable<BsonValue> values)
        {
            if (values == null) throw new ArgumentNullException(nameof(values));

            return new QueryIn(values);
        }

        /// <summary>
        /// Apply a predicate function in an index result. Execute full index scan but it's faster then runs over deserialized document.
        /// </summary>
        public static Query Where(Func<BsonValue, bool> predicate, int order = Query.Ascending)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return new QueryWhere(predicate, order);
        }


        /// <summary>
        /// Returns documents that exists in ANY queries results (Union).
        /// </summary>
        public static Query Or(Query left, Query right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            return new QueryOr(left, right);
        }

        /// <summary>
        /// Returns document that exists in ANY queries results (Union).
        /// </summary>
        public static Query Or(params Query[] queries)
        {
            if (queries == null || queries.Length < 2) throw new ArgumentException("At least two Query should be passed");

            var left = queries[0];

            for (int i = 1; i < queries.Length; i++)
            {
                left = Or(left, queries[i]);
            }
            return left;
        }

        #endregion

        #region Executing Query

        /// <summary>
        /// Find witch index will be used and run Execute method
        /// </summary>
        internal virtual IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            // get index for this query
            var index = col.PK;

            // execute query to get all IndexNodes
            // do DistinctBy datablock to not duplicate same document in results
            return this.ExecuteIndex(indexer, index)
                .DistinctBy(x => x.DataBlock, null);
        }

        /// <summary>
        /// Abstract method that must be implement for index seek/scan - Returns IndexNodes that match with index
        /// </summary>
        internal abstract IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index);

        #endregion
    }
}