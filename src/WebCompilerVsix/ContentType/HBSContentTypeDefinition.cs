using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    /// <summary>
    /// Exports the HandleBars content type and file extension (.hbs)
    /// </summary>
    public class HBSContentTypeDefinition
    {
        public const string HBSContentType = "HandleBars";

        /// <summary>
        /// Exports the HandleBars content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(HBSContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition IHandleBarsContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(HBSContentType)]
        [FileExtension(".hbs")]
        public FileExtensionToContentTypeDefinition HandleBarsFileExtension { get; set; }
    }
}
