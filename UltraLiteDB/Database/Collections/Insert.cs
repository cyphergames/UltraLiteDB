using System;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    public partial class UltraLiteCollection
    {
        /// <summary>
        /// Insert a new entity to this collection. Document Id must be a new value in collection - Returns document Id
        /// </summary>
        public BsonValue Insert(BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var removed = this.RemoveDocId(document);

            var id = _engine.Value.Insert(_name, document, _autoId);

            return id;
        }

        /// <summary>
        /// Insert a new document to this collection using passed id value.
        /// </summary>
        public void Insert(BsonValue id, BsonDocument document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            document["_id"] = id;

            _engine.Value.Insert(_name, document);
        }

        /// <summary>
        /// Insert an array of new documents to this collection. Document Id must be a new value in collection. Can be set buffer size to commit at each N documents
        /// </summary>
        public int Insert(IEnumerable<BsonDocument> docs)
        {
            if (docs == null) throw new ArgumentNullException(nameof(docs));

            return _engine.Value.Insert(_name, this.GetBsonDocs(docs), _autoId);
        }

        /// <summary>
        /// Implements bulk insert documents in a collection. Usefull when need lots of documents.
        /// </summary>
        public int InsertBulk(IEnumerable<BsonDocument> docs, int batchSize = 5000)
        {
            if (docs == null) throw new ArgumentNullException(nameof(docs));

            return _engine.Value.InsertBulk(_name, this.GetBsonDocs(docs), batchSize, _autoId);
        }

        /// <summary>
        /// Implements bulk upsert of documents in a collection. Usefull when need lots of documents.
        /// </summary>
        public int UpsertBulk(IEnumerable<BsonDocument> docs, int batchSize = 5000)
        {
            if (docs == null) throw new ArgumentNullException(nameof(docs));

            return _engine.Value.UpsertBulk(_name, this.GetBsonDocs(docs), batchSize, _autoId);
        }

        /// <summary>
        /// Convert each T document in a BsonDocument, setting autoId for each one
        /// </summary>
        private IEnumerable<BsonDocument> GetBsonDocs(IEnumerable<BsonDocument> documents)
        {
            foreach (var document in documents)
            {
                var removed = this.RemoveDocId(document);

                yield return document;

            }
        }

        /// <summary>
        /// Remove document _id if contains a "empty" value (checks for autoId bson type)
        /// </summary>
        private bool RemoveDocId(BsonDocument doc)
        {
            if (doc.TryGetValue("_id", out var id)) 
            {
                // check if exists _autoId and current id is "empty"
                if ((_autoId == BsonType.ObjectId && (id.IsNull || id.AsObjectId == ObjectId.Empty)) ||
                    (_autoId == BsonType.Guid && id.AsGuid == Guid.Empty) ||
                    (_autoId == BsonType.DateTime && id.AsDateTime == DateTime.MinValue) ||
                    (_autoId == BsonType.Int32 && id.AsInt32 == 0) ||
                    (_autoId == BsonType.Int64 && id.AsInt64 == 0))
                {
                    // in this cases, remove _id and set new value after
                    doc.Remove("_id");
                    return true;
                }
            }

            return false;   
        }
    }
}