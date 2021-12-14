using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    public class StylusContentTypeDefinition
    {
        public const string StylusContentType = "Stylus";

        /// <summary>
        /// Exports the Stylus content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(StylusContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition IStylusContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(StylusContentType)]
        [FileExtension(".stylus")]
        public FileExtensionToContentTypeDefinition StylusFileExtension { get; set; }
    }
}
