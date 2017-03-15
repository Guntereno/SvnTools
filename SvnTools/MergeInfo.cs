using System;
using System.Collections.Generic;
using System.Linq;

namespace SvnTools
{
    class MergeInfo
    {
        public MergeInfo(string property)
        {
            string[] lines = property.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Entries = lines.Select((line) => new MergeInfoEntry(line)).ToList();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Entries.Select((entry) => entry.ToString()));
        }

        public List<MergeInfoEntry> Entries;
    }

    struct MergeInfoEntry
    {
        public string Source;
        public List<MergeInfoRange> Ranges;

        public MergeInfoEntry(string propertyValue)
        {
            string[] components = propertyValue.Split(':');
            if (components.Length != 2)
            {
                throw new Exception("Unable to parse merge source: " + propertyValue);
            }

            Source = components[0];
            Ranges = components[1].Split(',').Select((str) =>
            {
                return new MergeInfoRange(str);
            }).ToList();
        }

        public override string ToString()
        {
            string changes = string.Join(",", Ranges.Select((change) => change.ToString()));
            return Source + ":" + changes;
        }
    }

    struct MergeInfoRange
    {
        public int From;
        public int To;

        public MergeInfoRange(string str)
        {
            From = 0;
            To = 0;

            bool result = false;

            if (str.Contains('-'))
            {
                string[] components = str.Split('-');

                result = (components.Length == 2);
                if (result)
                {
                    result = int.TryParse(components[0], out From);
                    if (result)
                    {
                        result = int.TryParse(components[1], out To);
                    }
                }
            }
            else
            {
                result = int.TryParse(str, out From);
                To = From;
            }

            if (!result)
            {
                throw new Exception("Failed to parse merge range '" + str + "'!");
            }
        }

        public override string ToString()
        {
            if (From == To)
            {
                return From.ToString();
            }
            else
            {
                return From.ToString() + "-" + To.ToString();
            }
        }
    }
}
