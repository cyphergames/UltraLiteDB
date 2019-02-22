using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    public partial class UltraLiteCollection
    {
        /// <summary>
        /// Update a document in this collection. Returns false if not found document in collection
        /// </summary>
        public bool Update(BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return _engine.Value.Update(_name, new BsonDocument[] { document }) > 0;
        }

        /// <summary>
        /// Update a document in this collection. Returns false if not found document in collection
        /// </summary>
        public bool Update(BsonValue id, BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            // set document _id using id parameter
            document["_id"] = id;

            return _engine.Value.Update(_name, new BsonDocument[] { document }) > 0;
        }

        /// <summary>
        /// Update all documents
        /// </summary>
        public int Update(IEnumerable<BsonDocument> documents)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            return _engine.Value.Update(_name, documents.Select(x => x));
        }
    }
}