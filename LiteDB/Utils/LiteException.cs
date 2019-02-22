using System;


namespace LiteDB
{
    /// <summary>
    /// The main exception for LiteDB
    /// </summary>
    public class LiteException : Exception
    {
        #region Errors code

        public const int FILE_NOT_FOUND = 101;
        public const int INVALID_DATABASE = 103;
        public const int INVALID_DATABASE_VERSION = 104;
        public const int FILE_SIZE_EXCEEDED = 105;
        public const int COLLECTION_LIMIT_EXCEEDED = 106;
        public const int INDEX_DROP_IP = 108;
        public const int INDEX_LIMIT_EXCEEDED = 109;
        public const int INDEX_DUPLICATE_KEY = 110;
        public const int INDEX_KEY_TOO_LONG = 111;
        public const int INDEX_NOT_FOUND = 112;
        public const int LOCK_TIMEOUT = 120;
        public const int INVALID_COMMAND = 121;
        public const int ALREADY_EXISTS_COLLECTION_NAME = 122;
        public const int DATABASE_WRONG_PASSWORD = 123;
        public const int SYNTAX_ERROR = 127;

        public const int INVALID_FORMAT = 200;
        public const int INVALID_DATA_TYPE = 204;


        #endregion

        #region Ctor

        public int ErrorCode { get; private set; }
        public string Line { get; private set; }
        public int Position { get; private set; }

        public LiteException(string message)
            : base(message)
        {
        }

        internal LiteException(int code, string message, params object[] args)
            : base(string.Format(message, args))
        {
            this.ErrorCode = code;
        }

        #endregion

        #region Method Errors

        internal static LiteException FileNotFound(string fileId)
        {
            return new LiteException(FILE_NOT_FOUND, "File '{0}' not found.", fileId);
        }

        internal static LiteException InvalidDatabase()
        {
            return new LiteException(INVALID_DATABASE, "Datafile is not a LiteDB database.");
        }

        internal static LiteException InvalidDatabaseVersion(int version)
        {
            return new LiteException(INVALID_DATABASE_VERSION, "Invalid database version: {0}", version);
        }

        internal static LiteException FileSizeExceeded(long limit)
        {
            return new LiteException(FILE_SIZE_EXCEEDED, "Database size exceeds limit of {0}.", StorageUnitHelper.FormatFileSize(limit));
        }

        internal static LiteException CollectionLimitExceeded(int limit)
        {
            return new LiteException(COLLECTION_LIMIT_EXCEEDED, "This database exceeded the maximum limit of collection names size: {0} bytes", limit);
        }

        internal static LiteException IndexLimitExceeded(string collection)
        {
            return new LiteException(INDEX_LIMIT_EXCEEDED, "Collection '{0}' exceeded the maximum limit of indices: {1}", collection, CollectionIndex.INDEX_PER_COLLECTION);
        }

        internal static LiteException IndexDuplicateKey(string field, BsonValue key)
        {
            return new LiteException(INDEX_DUPLICATE_KEY, "Cannot insert duplicate key in unique index '{0}'. The duplicate value is '{1}'.", field, key);
        }

        internal static LiteException IndexKeyTooLong()
        {
            return new LiteException(INDEX_KEY_TOO_LONG, "Index key must be less than {0} bytes.", IndexService.MAX_INDEX_LENGTH);
        }

        internal static LiteException LockTimeout(TimeSpan ts)
        {
            return new LiteException(LOCK_TIMEOUT, "Timeout. Database is locked for more than {0}.", ts.ToString());
        }

        internal static LiteException AlreadyExistsCollectionName(string newName)
        {
            return new LiteException(ALREADY_EXISTS_COLLECTION_NAME, "New collection name '{0}' already exists.", newName);
        }

        internal static LiteException DatabaseWrongPassword()
        {
            return new LiteException(DATABASE_WRONG_PASSWORD, "Invalid database password.");
        }

        internal static LiteException InvalidFormat(string field)
        {
            return new LiteException(INVALID_FORMAT, "Invalid format: {0}", field);
        }

        internal static LiteException InvalidDataType(string field, BsonValue value)
        {
            return new LiteException(INVALID_DATA_TYPE, "Invalid BSON data type '{0}' on field '{1}'.", value.Type, field);
        }

        internal static LiteException SyntaxError(StringScanner s, string message = "Unexpected token")
        {
            return new LiteException(SYNTAX_ERROR, message)
            {
                Line = s.Source,
                Position = s.Index
            };
        }

        #endregion
    }
}