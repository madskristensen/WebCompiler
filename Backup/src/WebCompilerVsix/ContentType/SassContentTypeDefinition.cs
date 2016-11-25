﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace WebCompilerVsix
{
    /// <summary>
    /// Exports the Sass content type and file extension
    /// </summary>
    public class SassContentTypeDefinition
    {
        public const string SassContentType = "Sass";

        /// <summary>
        /// Exports the Sass content type
        /// </summary>
        [Export(typeof(ContentTypeDefinition))]
        [Name(SassContentType)]
        [BaseDefinition("code")]
        public ContentTypeDefinition ISassContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(SassContentType)]
        [FileExtension(".sass")]
        public FileExtensionToContentTypeDefinition SassFileExtension { get; set; }
    }
}
