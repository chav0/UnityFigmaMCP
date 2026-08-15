using System;

namespace UnityFigmaMCP.Common
{
    [Flags]
    public enum FigmaObjectType
    {
        NONE = 0,
        DOCUMENT = 1 << 0,
        CANVAS = 1 << 1,
        FRAME = 1 << 2,
        RECTANGLE = 1 << 3,
        LINE = 1 << 4,
        REGULAR_POLYGON = 1 << 5,
        VECTOR = 1 << 6,
        STAR = 1 << 7,
        ELLIPSE = 1 << 8,
        TEXT = 1 << 9,
        IMAGE = 1 << 10,
        GROUP = 1 << 11, 
        INSTANCE = 1 << 12, 
        COMPONENT = 1 << 13,
        COMPONENT_SET = 1 << 14,
        BOOLEAN_OPERATION = 1 << 15,
        SECTION = 1 << 16,
        SHAPE_WITH_TEXT = 1 << 17,
        SLICE = 1 << 18,
        STICKY = 1 << 19,
        CONNECTOR = 1 << 20,
        WIDGET = 1 << 21,
        EMBED = 1 << 22,
        LINK_UNFURL = 1 << 23,
        MEDIA = 1 << 24,
        HIGHLIGHT = 1 << 25,
        WASHI_TAPE = 1 << 26,
        
        GRAPHIC = FRAME | RECTANGLE | LINE | REGULAR_POLYGON | VECTOR | STAR | ELLIPSE | IMAGE | GROUP | INSTANCE | COMPONENT,
        EXPORTABLE = FRAME | COMPONENT | INSTANCE | COMPONENT_SET | GROUP,
    }
}