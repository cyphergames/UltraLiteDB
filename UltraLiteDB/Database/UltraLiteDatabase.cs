using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UltraLiteDB
{
    /// <summary>
    /// The LiteDB database. Used for create a LiteDB instance and use all storage resources. It's the database connection
    /// </summary>
    public partial class UltraLiteDatabase : IDisposable
    {
        #region Properties

        private LazyLoad<UltraLiteEngine> _engine = null;
        private Logger _log = null;
        private ConnectionString _connectionString = null;

        /// <summary>
        /// Get logger class instance
        /// </summary>
        public Logger Log { get { return _log; } }

        /// <summary>
        /// Get current database engine instance. Engine is lower data layer that works with BsonDocuments only (no mapper, no LINQ)
        /// </summary>
        public UltraLiteEngine Engine { get { return _engine.Value; } }

        #endregion

        #region Ctor

        /// <summary>
        /// Starts LiteDB database using a connection string for file system database
        /// </summary>
        public UltraLiteDatabase(string connectionString, Logger log = null)
            : this(new ConnectionString(connectionString), log)
        {
        }

        /// <summary>
        /// Starts LiteDB database using a connection string for file system database
        /// </summary>
        public UltraLiteDatabase(ConnectionString connectionString, Logger log = null)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
            _log = log ?? new Logger();
            _log.Level = log?.Level ?? _connectionString.Log;

            var options = new FileOptions
            {
#if HAVE_SYNC_OVER_ASYNC
                Async = _connectionString.Async,
#endif
#if HAVE_FLUSH_DISK
                Flush = _connectionString.Flush,
#endif
                InitialSize = _connectionString.InitialSize,
                LimitSize = _connectionString.LimitSize,
                Journal = _connectionString.Journal,
            };

            _engine = new LazyLoad<UltraLiteEngine>(() => new UltraLiteEngine(new FileDiskService(_connectionString.Filename, options), _connectionString.Password, _connectionString.Timeout, _connectionString.CacheSize, _log, _connectionString.UtcDate));
        }

        /// <summary>
        /// Starts LiteDB database using a Stream disk
        /// </summary>
        public UltraLiteDatabase(Stream stream, string password = null, bool disposeStream = false)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            _log = new Logger();

            _engine = new LazyLoad<UltraLiteEngine>(() => new UltraLiteEngine(new StreamDiskService(stream, disposeStream), password: password, log: _log));
        }

        /// <summary>
        /// Starts LiteDB database using a custom IDiskService with all parameters available
        /// </summary>
        /// <param name="diskService">Custom implementation of persist data layer</param>
        /// <param name="password">Password to encrypt you datafile</param>
        /// <param name="timeout">Locker timeout for concurrent access</param>
        /// <param name="cacheSize">Max memory pages used before flush data in Journal file (when available)</param>
        /// <param name="log">Custom log implementation</param>
        public UltraLiteDatabase(IDiskService diskService, string password = null, TimeSpan? timeout = null, int cacheSize = 5000, Logger log = null)
        {
            if (diskService == null) throw new ArgumentNullException(nameof(diskService));

            _log = log ?? new Logger();

            _engine = new LazyLoad<UltraLiteEngine>(() => new UltraLiteEngine(diskService, password: password, timeout: timeout, cacheSize: cacheSize, log: _log ));
        }

        #endregion

        #region Collections


        /// <summary>
        /// Get a collection using a generic BsonDocument. If collection does not exits, create a new one.
        /// </summary>
        /// <param name="name">Collection name (case insensitive)</param>
        public UltraLiteCollection GetCollection(string name)
        {
            if (name.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(name));

            return new UltraLiteCollection(name, _engine, _log);
        }

        #endregion


        #region Shortcut

        /// <summary>
        /// Get all collections name inside this database.
        /// </summary>
        public IEnumerable<string> GetCollectionNames()
        {
            return _engine.Value.GetCollectionNames();
        }

        /// <summary>
        /// Checks if a collection exists on database. Collection name is case insensitive
        /// </summary>
        public bool CollectionExists(string name)
        {
            if (name.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(name));

            return _engine.Value.GetCollectionNames().Contains(name, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Drop a collection and all data + indexes
        /// </summary>
        public bool DropCollection(string name)
        {
            if (name.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(name));

            return _engine.Value.DropCollection(name);
        }

        /// <summary>
        /// Rename a collection. Returns false if oldName does not exists or newName already exists
        /// </summary>
        public bool RenameCollection(string oldName, string newName)
        {
            if (oldName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(oldName));
            if (newName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(newName));

            return _engine.Value.RenameCollection(oldName, newName);
        }

        #endregion

        #region Shrink

        /// <summary>
        /// Reduce disk size re-arranging unused spaces.
        /// </summary>
        public long Shrink()
        {
            return this.Shrink(_connectionString?.Password);
        }

        /// <summary>
        /// Reduce disk size re-arranging unused space. Can change password. If a temporary disk was not provided, use MemoryStream temp disk
        /// </summary>
        public long Shrink(string password)
        {
            // if has connection string, use same path
            if (_connectionString != null)
            {
                // get temp file ("-temp" suffix)
                var tempFile = FileHelper.GetTempFile(_connectionString.Filename);
                var reduced = 0L;

                try
                {
                    // get temp disk based on temp file
                    var tempDisk = new FileDiskService(tempFile, new FileOptions { Journal = false });

                    reduced = _engine.Value.Shrink(password, tempDisk);
                }
                finally
                {
                    // delete temp file
                    File.Delete(tempFile);
                }

                return reduced;
            }
            else
            {
                return _engine.Value.Shrink(password);
            }
        }

        #endregion

        public void Dispose()
        {
            if (_engine.IsValueCreated) _engine.Value.Dispose();
        }
    }
}
