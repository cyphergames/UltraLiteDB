using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LiteDB
{
    public partial class LiteCollection
    {
        /// <summary>
        /// Run an include action in each document returned by Find(), FindById(), FindOne() and All() methods to load DbRef documents
        /// Returns a new Collection with this action included
        /// </summary>
        public LiteCollection Include(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return Include(new string[] { path });
        }

        /// <summary>
        /// Run an include action in each document returned by Find(), FindById(), FindOne() and All() methods to load DbRef documents
        /// Returns a new Collection with this action included
        /// </summary>
        /// <param name="paths">Property paths to include.</param>
        public LiteCollection Include(string[] paths)
        {
            if (paths == null)
            {
                throw new ArgumentNullException(nameof(paths));
            }

            // cloning this collection and adding this include
            var newcol = new LiteCollection(_name, _engine, _log);

            newcol._includes.AddRange(_includes);

            // add all paths that are not null nor empty due to previous check
            newcol._includes.AddRange(paths.Where(x => !String.IsNullOrEmpty(x)));

            return newcol;
        }
   
    }
}