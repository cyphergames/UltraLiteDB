
namespace UltraLiteDB
{
    public sealed partial class UltraLiteCollection
    {
        private string _name;
        private LazyLoad<UltraLiteEngine> _engine;
        private Logger _log;


        private BsonType _autoId = BsonType.Null;

        /// <summary>
        /// Get collection name
        /// </summary>
        public string Name { get { return _name; } }


        public UltraLiteCollection(string name, LazyLoad<UltraLiteEngine> engine,  Logger log)
        {
            _name = name;
            _engine = engine;
            _log = log;

            _autoId = BsonType.ObjectId;
        }
    }
}