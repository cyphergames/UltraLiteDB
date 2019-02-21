using System;
using System.Collections.Generic;

namespace LiteDB
{
    public sealed partial class LiteCollection
    {
        private string _name;
        private LazyLoad<LiteEngine> _engine;
        private BsonMapper _mapper;
        private Logger _log;
        private List<string> _includes;
        private QueryVisitor<BsonDocument> _visitor;
        private MemberMapper _id = null;
        private BsonType _autoId = BsonType.Null;

        /// <summary>
        /// Get collection name
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Returns visitor resolver query only for internals implementations
        /// </summary>
        internal QueryVisitor<BsonDocument> Visitor { get { return _visitor; } }

        public LiteCollection(string name, LazyLoad<LiteEngine> engine, BsonMapper mapper, Logger log)
        {
            _name = name ?? mapper.ResolveCollectionName(typeof(BsonDocument));
            _engine = engine;
            _mapper = mapper;
            _log = log;
            _visitor = new QueryVisitor<BsonDocument>(mapper);
            _includes = new List<string>();

            _autoId = BsonType.ObjectId;
        }
    }
}