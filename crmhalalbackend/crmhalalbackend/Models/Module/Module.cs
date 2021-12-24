namespace CRMHalalBackEnd.Models.Module
{
    public class Module
    {
        public int ModId { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }

        public override string ToString()
        {
            return $"{nameof(ModId)}: {ModId}, {nameof(Title)}: {Title}, {nameof(Status)}: {Status}";
        }
    }
}