using System;
using System.Collections.Generic;

namespace UltraLiteDB
{
    public partial class UltraLiteCollection
    {
        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public bool Upsert(BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return this.Upsert(new BsonDocument[] { document }) == 1;
        }

        /// <summary>
        /// Insert or Update all documents
        /// </summary>
        public int Upsert(IEnumerable<BsonDocument> documents)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            return _engine.Value.Upsert(_name, this.GetBsonDocs(documents), _autoId);
        }

        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public bool Upsert(BsonValue id, BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            // set document _id using id parameter
            document["_id"] = id;

            return _engine.Value.Upsert(_name, document);
        }
    }
}