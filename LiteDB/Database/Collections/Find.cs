using System;
using System.Collections.Generic;
using System.Linq;


namespace LiteDB
{
    public partial class LiteCollection
    {
        #region Find

        /// <summary>
        /// Find documents inside a collection using Query object.
        /// </summary>
        public IEnumerable<BsonDocument> Find(Query query, int skip = 0, int limit = int.MaxValue)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var docs = _engine.Value.Find(_name, query, _includes.ToArray(), skip, limit);

            foreach(var doc in docs)
            {
                yield return doc;
            }
        }


        #endregion

        #region FindById + One + All

        /// <summary>
        /// Find a document using Document Id. Returns null if not found.
        /// </summary>
        public BsonDocument FindById(BsonValue id)
        {
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            return this.Find(Query.EQ("_id", id)).SingleOrDefault();
        }

        /// <summary>
        /// Find the first document using Query object. Returns null if not found. Must have index on query expression.
        /// </summary>
        public BsonDocument FindOne(Query query)
        {
            return this.Find(query).FirstOrDefault();
        }


        /// <summary>
        /// Returns all documents inside collection order by _id index.
        /// </summary>
        public IEnumerable<BsonDocument> FindAll()
        {
            return this.Find(Query.All());
        }

        #endregion
    }
}