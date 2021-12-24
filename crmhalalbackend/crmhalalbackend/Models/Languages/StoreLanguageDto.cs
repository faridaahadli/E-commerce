namespace CRMHalalBackEnd.Models.Languages
{
    public class StoreLanguageDto
    {
        public int StoreLanguageId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string CodeTwo { get; set; }
        public string CodeThree { get; set; }
        public bool IsDefault { get; set; }
        public int Number { get; set; }

    }
}