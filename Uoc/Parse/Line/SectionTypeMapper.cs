using System.Collections.Generic;

namespace Uoc.Parse.Line
{
    internal static class SectionTypeMapper
    {
        private const string SECTION_NAME_PROPERTIES = "PROPERTIES";
        private const string SECTION_NAME_NOTEDEFS = "NOTESDEFS";
        private const string SECTION_NAME_CHART = "CHART";

        private static readonly Dictionary<string, SectionType> sectionMap = new()
        {
            { SECTION_NAME_PROPERTIES, SectionType.Properties },
            { SECTION_NAME_NOTEDEFS, SectionType.NotesDefs },
            { SECTION_NAME_CHART, SectionType.Chart }
        };

        public static SectionType GetSectionType(string sectionName)
        {
            if (sectionMap.TryGetValue(sectionName, out var sectionType))
            {
                return sectionType;
            }
            return SectionType.None;
        }
    }
}
