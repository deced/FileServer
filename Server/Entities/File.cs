using System.Collections.Generic;

namespace Server.Entities
{
    public class File
    {
        public string Name { get; set; }
        public FileType FileType { get; set; }
        public string BackPath { get; set; }
    }

    public enum FileType
    {
        File,
        Directory,
        Backwards
    }
}