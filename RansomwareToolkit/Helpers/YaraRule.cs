using System;

namespace RansomwareToolkit.Helpers
{
    public class YaraRule
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string MalwareFamily { get; set; }
        public string Strings { get; set; }
        public string Condition { get; set; }

        public string ToYaraFormat()
        {
            return $@"rule {Name} {{
    meta:
        description = ""{Description}""
        author = ""{Author}""
        malware_family = ""{MalwareFamily}""
    strings:
{Strings}
    condition:
        {Condition}
}}";
        }
    }
}
