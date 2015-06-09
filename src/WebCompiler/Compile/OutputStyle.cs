namespace WebCompiler
{
    /// <summary>
    /// Represent all type of output style.
    /// </summary>
    /// <remarks>
    /// Documentation based on: 
    ///    http://sass-lang.com/documentation/file.SASS_REFERENCE.html#output_style 
    /// </remarks>
    public enum OutputStyle
    {
        /// <summary>
        /// Nested style is the default Sass style, because it reflects the structure of the CSS 
        /// styles and the HTML document they are styling.
        /// </summary>
        Nested = 0,

        /// <summary>
        /// Expanded is a more typical human-made CSS style, with each property and rule taking up 
        /// one line.
        /// </summary>
        Expanded = 1,

        /// <summary>
        /// Compact style takes up less space than Nested or Expanded
        /// </summary>
        /// <remarks>
        /// It also draws the focus more to the selectors than to their properties. Each CSS rule
        /// takes up only one line, with every property defined on that line. Nested rules are 
        /// placed next to each other with no newline, while separate groups of rules have newlines 
        /// between them.
        /// </remarks>
        Compact = 2,

        /// <summary>
        /// Compressed style takes up the minimum amount of space possible, having no whitespace 
        /// except that necessary to separate selectors and a newline at the end of the file.
        /// </summary>
        Compressed = 3,

        /// <summary>
        /// Echo
        /// </summary>
        Echo = 4
    }
}
