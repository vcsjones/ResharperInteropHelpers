using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace InteropHelpers
{
    public class LibraryFactory
    {
        public Library LoadLibrary(string name)
        {
            if (Path.HasExtension(name))
            {
                name = Path.ChangeExtension(name, ".xml");
            }
            else
            {
                name += ".xml";
            }
            name = name.ToLower();
            var assem = GetType().Assembly;
            using (var stream = assem.GetManifestResourceStream("InteropHelpers.KnownImports." + name))
            {
                if (stream == null)
                {
                    return null;
                }
                stream.Position = 0;
                return new Library(XDocument.Load(stream));
            }
        }
    }

    public class Library
    {
        private readonly XDocument _document;

        public Library(XDocument document)
        {
            _document = document;
        }

        public string Name
        {
            get
            {
                return _document.Root.Attribute("name").Value;
            }
        }

        public IEnumerable<ILibraryExport> GetExports()
        {
            foreach (var exportElement in _document.Root.Elements("export"))
            {
                yield return new LibraryExport
                {
                    Name = exportElement.Attribute("name").Value,
                    Parameters = exportElement
                                    .Element("parameters")
                                    .Elements("parameter")
                                    .Select(d => new LibraryExportParameter
                                                     {
                                                         ClrType = d.Attribute("clrType").Value,
                                                         Name = d.Attribute("name").Value
                                                     }).ToArray()
                };
            }
        }

        public ILibraryExport GetExport(string exportName)
        {
            return GetExports().FirstOrDefault(exp => exp.Name == exportName);
        }

        private class LibraryExport : ILibraryExport
        {
            public string Name { get; set; }
            public ILibraryExportParameter[] Parameters { get; set; }
        }

        private class LibraryExportParameter : ILibraryExportParameter
        {
            public string Name { get; set; }
            public string ClrType { get; set; }
        }
    }

    public interface ILibraryExport
    {
        string Name { get; }
        ILibraryExportParameter[] Parameters { get; }
    }

    public interface ILibraryExportParameter
    {
        string Name { get; }
        string ClrType { get; }
    }
}
