namespace CRMHalalBackEnd.Models.File
{
    public class File
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
        public string OriginalFileName { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Path)}: {Path}, {nameof(FileName)}: {FileName}, {nameof(Extension)}: {Extension}, {nameof(FileType)}: {FileType}, {nameof(MimeType)}: {MimeType}, {nameof(Size)}: {Size}, {nameof(OriginalFileName)}: {OriginalFileName}";
        }
    }
}