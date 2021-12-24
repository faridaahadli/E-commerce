namespace CRMHalalBackEnd.Models.Faq
{
    public class FaqDto
    {
        public int FaqId { get; set; }

        public int ModId { get; set; }

        public string Question { get; set; } = string.Empty;
        public string Question2 { get; set; } = string.Empty;
        public string Question3 { get; set; } = string.Empty;
        public string Question4 { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Answer2 { get; set; } = string.Empty;
        public string Answer3 { get; set; } = string.Empty;
        public string Answer4 { get; set; } = string.Empty;
        public string ModTitle { get; set; }
        public string ModTitle2 { get; set; }
        public string ModTitle3 { get; set; }
        public string ModTitle4 { get; set; }
        public override string ToString()
        {
            return $"{nameof(FaqId)}: {FaqId}, {nameof(ModId)}: {ModId}, {nameof(Question)}: {Question}, {nameof(Answer)}: {Answer},{nameof(ModTitle)}: {ModTitle}";
        }
    }
}