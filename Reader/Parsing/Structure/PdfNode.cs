using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Core;

namespace Mio.Reader.Parsing.Structure
{
    public class PdfNode : Node
    {
        public List<TextNode>? TextNodes { get; set; } = [];
        public ImageNode? ImageNode { get; set; }
        public bool IsImage { get {
                return ImageNode != null;
            } 
        }
        //All measurements are in points
        public PdfRectangle BoundingBox { get; set; }
        public double FontSize { get; set; }
        public bool IsFurigana { get; set; }
        /// <summary>
        /// When a line of speech is split between two display lines in the PDF, it may be the case that a word is split.
        /// This properties indicated that the first TextNode of the next PdfNode is the same as the last TextNode of this one.
        /// As such, they must be highlighted together, among other things.
        /// </summary>
        public bool IsLastNodeShared { get; set; }
        /// <summary>
        /// When a line of speech is split between two display lines in the PDF, it may be the case that a word is split.
        /// This properties indicated that the first TextNode of this PdfNode is the same as the last TextNode of the previous one.
        /// As such, they must be highlighted together, among other things.
        /// </summary>
        public bool IsFirstNodeShared { get; set; }
        /// <summary>
        /// Only used before parsing the lines for the purposes of determining the two properties above.
        /// </summary>
        public bool PossibleSharing { get; set; }
        /// <summary>
        /// The first index of Character in the first TextNode that belongs to this PdfNode, in case of the first TextNode being shared.
        /// </summary>
        public int FirstFrom { get; set; }
        /// <summary>
        /// The last index of Character in the last TextNode that belongs to this PdfNode, in case of the last TextNode being shared.
        /// </summary>
        public int LastUntil{ get; set; }
    }
}
