using System;

namespace CRMHalalBackEnd.Models.Store
{
    public class SliderDto
    {
        public int ImageId { get; set; }
        public string FilePath { get; set; } = String.Empty;
        public string Url { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(ImageId)}: {ImageId}, {nameof(FilePath)}: {FilePath}, {nameof(Url)}: {Url}";
        }
    }
}