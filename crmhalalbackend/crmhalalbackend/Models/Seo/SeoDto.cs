namespace CRMHalalBackEnd.Models.Seo
{
    public class SeoDto
    {
        public int Id { get; set; }
        public string Seo { get; set; } = string.Empty;
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Seo)}: {Seo}";
        }
    }
}