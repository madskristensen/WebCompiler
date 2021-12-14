using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    /// <summary>
    /// Exports the HandleBars content type and file extension (.handlebars)
    /// </summary>
    public class HandlebarsContentTypeDefinition
    {
        public const string HandleBarsContentType = "HandleBars";

        /// <summary>
        /// Exports the HandleBars content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(HandleBarsContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition IHandleBarsContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(HandleBarsContentType)]
        [FileExtension(".handlebars")]
        public FileExtensionToContentTypeDefinition HandleBarsFileExtension { get; set; }
    }
}
