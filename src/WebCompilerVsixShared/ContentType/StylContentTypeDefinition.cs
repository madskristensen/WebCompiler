using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    public class StylContentTypeDefinition
    {
        public const string StylContentType = "Stylus";

        /// <summary>
        /// Exports the Stylus content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(StylContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition IStylusContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(StylContentType)]
        [FileExtension(".styl")]
        public FileExtensionToContentTypeDefinition StylusFileExtension { get; set; }
    }
}
