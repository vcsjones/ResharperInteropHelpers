using System;
using System.Collections.Generic;
using System.IO;
using InteropHelpers.KnownImports;

namespace InteropHelpers
{
    public class LibraryFactory
    {
        private static readonly Dictionary<string, Library> _cache = new Dictionary<string, Library>();
        public Library LoadLibrary(string name)
        {
            if (Path.HasExtension(name))
            {
                name = Path.GetFileNameWithoutExtension(name);
            }
            name = name.ToLower();
            if (!_cache.ContainsKey(name))
            {
                switch (name)
                {
                    case "kernel32":
                        var library = new Kernel32();
                        library.Intialize();
                        _cache.Add(name, library);
                        break;
                    default:
                        _cache.Add(name, null);
                        break;
                }
            }
            return _cache[name];
        }
    }
}
