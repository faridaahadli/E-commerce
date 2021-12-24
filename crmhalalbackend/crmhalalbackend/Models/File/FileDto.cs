using System;

namespace CRMHalalBackEnd.Models.File
{
    public class FileDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = String.Empty;
        public string FilePath { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(FilePath)}: {FilePath}";
        }
    }
}