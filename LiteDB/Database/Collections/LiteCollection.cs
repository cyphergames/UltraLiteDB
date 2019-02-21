using System;
using System.Collections.Generic;

namespace LiteDB
{
    public sealed partial class LiteCollection
    {
        private string _name;
        private LazyLoad<LiteEngine> _engine;
        private Logger _log;
        private List<string> _includes;

        private BsonType _autoId = BsonType.Null;

        /// <summary>
        /// Get collection name
        /// </summary>
        public string Name { get { return _name; } }


        public LiteCollection(string name, LazyLoad<LiteEngine> engine,  Logger log)
        {
            _name = name;
            _engine = engine;
            _log = log;
            _includes = new List<string>();

            _autoId = BsonType.ObjectId;
        }
    }
}