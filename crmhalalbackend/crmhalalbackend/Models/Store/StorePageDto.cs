using System.Collections.Generic;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.Languages;
using CRMHalalBackEnd.Models.SpecialOffer;
using CRMHalalBackEnd.Models.Variation;

namespace CRMHalalBackEnd.Models.Store
{
    public class StorePageDto
    {
        public StoreResponse Store { get; set; } = new StoreResponse();
        public List<AllCategoryForSelect> Category { get; set; } = new List<AllCategoryForSelect>();
        public List<SliderDto> Slider { get; set; } = new List<SliderDto>();
        public List<SpecialOfferDto> DailySpecialOffer { get; set; } = new List<SpecialOfferDto>();
        public bool IsFollowing { get; set; }
        public List<SpecialOfferDto> SpecialOffer { get; set; } = new List<SpecialOfferDto>();
        public List<Promotion.PromotionResponse> Promotions { get; set; }
        public List<VariationMainPageDto> ProductMainPages { get; set; } = new List<VariationMainPageDto>();

        public override string ToString()
        {
            return $"{nameof(Store)}: {Store}, {nameof(Category)}: {Category}, {nameof(Slider)}: {Slider}, {nameof(DailySpecialOffer)}: {DailySpecialOffer}, {nameof(SpecialOffer)}: {SpecialOffer}, {nameof(Promotions)}: {Promotions}, {nameof(ProductMainPages)}: {ProductMainPages}";
        }
    }
}