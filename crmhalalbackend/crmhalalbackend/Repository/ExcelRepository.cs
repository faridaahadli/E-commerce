using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Attribute;
using CRMHalalBackEnd.Models.ExcelImport;
using ExcelDataReader;
using ExcelDataReader.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class ExcelRepository
    {

        private static readonly log4net.ILog Log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly ProductRepository _productRepo = new ProductRepository();

        private static Random random = new Random();
        public int GetMeasureTypeIdByName(string name)
        {
            //AND EXISTS(SELECT* FROM dbo.GetEmployeePermission (@userId, @tenantId, '13' ) ) 
            //AND(SELECT dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, PR_CAT_ID)) = 1
            string sql =
                @"select PR_UNIT_ID AS Id from NEW_PR_UNIT_CODE where NAME=@name";
            int id = 0;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@name",SqlDbType.NVarChar,15,ParameterDirection.Input,name)              
                    });

                    if (reader.Read())
                    {
                        id = reader.GetInt("Id");
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return id;
        }

        public static string GetAttributeIdByValue(string value, Models.Product.Product productFormat)
        {
            string id = String.Empty;

            foreach (var item in productFormat.Attributes)
            {
                foreach (var pValue in item.Value)
                {


                    if (pValue == value)
                    {

                        id = item.Id;
                    }
                }

            }
            return id;
        }


        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static List<Models.Product.Product> ConvertData(List<AllDataFront> allDataFront)
        {

            ExcelRepository excelRepo = new ExcelRepository();
            List<Models.Product.Product> finalProduct = new List<Models.Product.Product>();
            int count = 0;
            foreach (var groupData in allDataFront)
            {

                foreach (var data in groupData.GroupData)
                {
                    Models.Product.Product product = new Models.Product.Product();

                    foreach (var finalData in data.Data)
                    {
                        if ((finalData.Key == "Hündürlük" || finalData.Key == "Çəki" || finalData.Key == "En" || finalData.Key == "Uzunluq") && finalData.Value == null)
                        {
                            //   Int32.Parse(finalData.Value)==0;
                            finalData.Value = "0";
                        }

                        product.Name = finalData.Key == "Məhsulun adı*" ? finalData.Value : product.Name;
                        product.GroupId = groupData.GroupId;
                        product.CategoryId = finalData.Key == "Kateqoriya Id*" ? Convert.ToInt32(finalData.Value) : product.CategoryId; //else hissesini duzgun yoxla
                        product.ManufacturerId = finalData.Key == "Brend*" ? finalData.Value : product.ManufacturerId;
                        product.MeasureTypeId = finalData.Key == "Ölçü vahidi*" ? excelRepo.GetMeasureTypeIdByName(finalData.Value) : product.MeasureTypeId;
                        product.Height = finalData.Key == "Hündürlük" ? Decimal.Parse(finalData.Value) : product.Height;
                        product.Weight = finalData.Key == "Çəki" ? Decimal.Parse(finalData.Value) : product.Weight;
                        product.Width = finalData.Key == "En" ? Decimal.Parse(finalData.Value) : product.Width;
                        product.Length = finalData.Key == "Uzunluq" ? Decimal.Parse(finalData.Value) : product.Length;
                        product.YoutubeLink = finalData.Key == "Youtube linki" ? finalData.Value : product.YoutubeLink;

                    }
                    finalProduct.Add(product);

                }
                foreach (var item in finalProduct)
                {
                    if (groupData.GroupId == item.GroupId)
                    {
                        int countShowMain = 0;
                        item.Attributes = groupData.Attributes;
                        foreach (var item2 in item.Attributes)
                        {
                            countShowMain++;
                            if (countShowMain == 1)
                                item2.ShowInName = true;
                            else
                                item2.ShowInName = false;


                            item2.Id = RandomString(20);
                            // item2.ShowInName = true;
                        }
                    }

                }


                foreach (var dataVariation in groupData.GroupData)
                {
                    Models.Product.Product productVariation = new Models.Product.Product();
                    foreach (var item in finalProduct)
                    {

                        if (groupData.GroupId == item.GroupId)
                        {
                            productVariation.Attributes = item.Attributes;
                        }

                    }


                    List<SelectedAttribute> selAttributes = new List<SelectedAttribute>();
                    foreach (var variation in dataVariation.Data)
                    {


                        if (variation.Key.Equals("Xüsusiyyət dəyəri N1") || variation.Key.Equals("Xüsusiyyət dəyəri N2") || variation.Key.Equals("Xüsusiyyət dəyəri N3") || variation.Key.Equals("Xüsusiyyət dəyəri N4") || variation.Key.Equals("Xüsusiyyət dəyəri N5") || variation.Key.Equals("Xüsusiyyət dəyəri N6") || variation.Key.Equals("Xüsusiyyət dəyəri N7") || variation.Key.Equals("Xüsusiyyət dəyəri N8") || variation.Key.Equals("Xüsusiyyət dəyəri N9") || variation.Key.Equals("Xüsusiyyət dəyəri N10") || variation.Key.Equals("Xüsusiyyət dəyəri N11") || variation.Key.Equals("Xüsusiyyət dəyəri N12") || variation.Key.Equals("Xüsusiyyət dəyəri N13") || variation.Key.Equals("Xüsusiyyət dəyəri N14") || variation.Key.Equals("Xüsusiyyət dəyəri N15") || variation.Key.Equals("Xüsusiyyət dəyəri N16") || variation.Key.Equals("Xüsusiyyət dəyəri N17") || variation.Key.Equals("Xüsusiyyət dəyəri N18") || variation.Key.Equals("Xüsusiyyət dəyəri N19") || variation.Key.Equals("Xüsusiyyət dəyəri N20"))
                        {
                            if (variation.Key.Equals("Xüsusiyyət dəyəri N1") && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N2" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N3" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N4" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N5" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N6" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N7" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N8" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N9" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N10" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N11" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N12" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N13" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N14" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N15" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N16" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N17" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N18" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N19" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }
                            if (variation.Key == "Xüsusiyyət dəyəri N20" && variation.Value != null)
                            {
                                selAttributes.Add(new SelectedAttribute
                                {
                                    Id = GetAttributeIdByValue(variation.Value, productVariation),
                                    Value = variation.Value
                                });
                            }

                        }
                    }

                    if (selAttributes != null)
                    {
                        productVariation.Variations.Add(new Models.Variation.Variation());

                        foreach (var item in productVariation.Variations)
                        {
                            for (int i = 0; i < finalProduct.Count; i++)
                            {

                                if (groupData.GroupId == finalProduct[i].GroupId)
                                {
                                    i = count;

                                    finalProduct[i].Variations.Add(new Models.Variation.Variation());
                                    foreach (var item4 in finalProduct[i].Variations)
                                    {
                                        item4.ShowOnMain = true;
                                        item4.SelectedAttributes = selAttributes;
                                        item4.ShowPrice = true;
                                        item4.Id = RandomString(7);
                                        dataVariation.Data.ForEach(x =>
                                        {
                                            if (x.Key == "Məhsul kodu" && x.Value != null)
                                            {
                                                item4.Sku = x.Value;
                                            }
                                            if (x.Key == "Açıqlama" && x.Value != null)
                                            {
                                                item4.Description = x.Value;
                                            }
                                            if (x.Key == "Qiymət" && x.Value != null)

                                            {
                                                item4.Price = Decimal.Parse(x.Value);
                                            }
                                            if (x.Key == "Anbardakı miqdarı" && x.Value != null)
                                            {
                                                item4.StockQuantity = Int32.Parse(x.Value);
                                            }
                                            if (x.Key == "Barkod" && x.Value != null)
                                            {
                                                item4.Barcode = x.Value;
                                            }
                                            if (x.Key == "Endirimli qiymət" && x.Value != null)
                                            {
                                                item4.Discount = Decimal.Parse(x.Value);
                                            }

                                        });
                                        count++;
                                        break;
                                    }
                                    break;
                                }

                            }
                        }
                    }
                }
            }


            List<Models.Product.Product> frontProduct = new List<Models.Product.Product>();

            var groupedCustomerList = finalProduct
                                      .GroupBy(u => u.GroupId)
                                      .Select(grp => grp.ToList())
                                      .ToList();

            foreach (var item in groupedCustomerList)
            {
                foreach (var item2 in item)
                {
                    frontProduct.Add(item2);
                    break;
                }
            }
            int num = 0;
            foreach (var item in groupedCustomerList)
            {
                List<Models.Variation.Variation> variations = new List<Models.Variation.Variation>();

                foreach (var item2 in item)
                {
                    foreach (var item3 in item2.Variations)
                    {
                        variations.Add(item3);
                    }

                }

                for (int i = 0; i < frontProduct.Count; i++)
                {
                    i = num;
                    frontProduct[i].Variations = variations;
                    num++;
                    break;
                }

            }


            return frontProduct;
        }

        public void InsertProducts(List<AllDataFront> allDataFront, int userId, string tenantId)
        {

            var dataFronts = ConvertData(allDataFront);
            foreach (var product in dataFronts)
            {
                if (product.Name == null)
                {
                    throw new Exception("Zəhmət olmazsa, məhsul adını daxil edin!");
                }
                if (product.CategoryId == 0)
                {
                    throw new Exception("Zəhmət olmazsa, kateqoriya id-i daxil edin!");
                }
                if (product.GroupId == null)
                {
                    throw new Exception("Zəhmət olmazsa, qrup id-i daxil edin!");
                }
                if (product.MeasureTypeId == 0)
                {
                    throw new Exception("Zəhmət olmazsa, ölçü vahidini daxil edin!");
                }
                if (product.ManufacturerId == null)
                {
                    throw new Exception("Zəhmət olmazsa, istehesalçını daxil edin!");
                }

                foreach (var item in product.Attributes)
                {
                    if (item.Name == null)
                    {
                        throw new Exception("Zəhmət olmazsa, xüsusiyyəti daxil edin!");
                    }
                    if (item.Value == null)
                    {
                        throw new Exception("Zəhmət olmazsa, xüsusiyyət dəyərini daxil edin!");
                    }
                }


                try
                {
                    using (var con = new DbHandler())
                    {
                        product.GroupId = _productRepo.GetGroupId(7, 8);
                        var json = JsonConvert.SerializeObject(product);
                        con.ExecuteStoredProcedure("[ProductVariationSave]", new[]
                        {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Could not ProductSave...");
                    Log.Error(ex);
                    throw;
                }
            }

        }


        public List<AllData> GetAllData(string path, int userid, string tenantId)
        {
            try
            {
                List<AllData> allData = new List<AllData>();
                List<Models.ExcelImport.Data> finalData = new List<Models.ExcelImport.Data>();
                using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        List<Models.ExcelImport.Data> data1 = new List<Models.ExcelImport.Data>();
                        while (reader.Read())
                        {
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                Models.ExcelImport.Data dataKey = new Models.ExcelImport.Data();
                                dataKey.Key = Convert.ToString(reader[i]);

                                data1.Add(dataKey);
                            }
                            finalData = data1;
                            foreach (var item in finalData)
                            {
                                if (item.Key == "Məhsulun adı*" || item.Key == "Qrup Nömrəsi*" || item.Key == "Kateqoriya Id*" || item.Key == "Brend*"
                                   || item.Key == "Endirimli qiymət" || item.Key == "Məhsul kodu" || item.Key == "Barkod" || item.Key == "Anbardakı miqdarı"
                                   || item.Key == "Ölçü vahidi*" || item.Key == "Hündürlük" || item.Key == "En" || item.Key == "Uzunluq"
                                   || item.Key == "Çəki" || item.Key == "Youtube linki" || item.Key == "Xüsusiyyət N1" || item.Key == "Xüsusiyyət dəyəri N1"
                                   || item.Key == "Xüsusiyyət N2" || item.Key == "Xüsusiyyət dəyəri N2" || item.Key == "Xüsusiyyət N3" || item.Key == "Xüsusiyyət dəyəri N3"
                                   || item.Key == "Xüsusiyyət N4" || item.Key == "Xüsusiyyət dəyəri N4" || item.Key == "Xüsusiyyət N5" || item.Key == "Xüsusiyyət dəyəri N5"
                                   || item.Key == "Xüsusiyyət N7" || item.Key == "Xüsusiyyət dəyəri N7" || item.Key == "Xüsusiyyət N8" || item.Key == "Xüsusiyyət dəyəri N8"
                                   || item.Key == "Xüsusiyyət N9" || item.Key == "Xüsusiyyət dəyəri N9" || item.Key == "Xüsusiyyət N10" || item.Key == "Xüsusiyyət dəyəri N10"
                                   || item.Key == "Xüsusiyyət N11" || item.Key == "Xüsusiyyət dəyəri N11" || item.Key == "Xüsusiyyət N12" || item.Key == "Xüsusiyyət dəyəri N12"
                                   || item.Key == "Xüsusiyyət N13" || item.Key == "Xüsusiyyət dəyəri N13" || item.Key == "Xüsusiyyət N14" || item.Key == "Xüsusiyyət dəyəri N14"
                                   || item.Key == "Xüsusiyyət N15" || item.Key == "Xüsusiyyət dəyəri N15" || item.Key == "Xüsusiyyət N16" || item.Key == "Xüsusiyyət dəyəri N16"
                                   || item.Key == "Xüsusiyyət N17" || item.Key == "Xüsusiyyət dəyəri N17" || item.Key == "Xüsusiyyət N18" || item.Key == "Xüsusiyyət dəyəri N18"
                                   || item.Key == "Xüsusiyyət N19" || item.Key == "Xüsusiyyət dəyəri N19" || item.Key == "Xüsusiyyət N20" || item.Key == "Xüsusiyyət dəyəri N20"
                                   || item.Key == "Xüsusiyyət N6" || item.Key == "Xüsusiyyət dəyəri N6" || item.Key == "Qiymət" || item.Key == "Açıqlama")
                                    continue;

                                else
                                    throw new Exception("Zəhmət olmazsa, Excel formatda dəyişiklik etməyin!");
                                

                            }
                            break;
                        }

                        while (reader.Read())
                        {

                            if (reader.Depth == 1)
                            {
                                List<Models.ExcelImport.Data> dataValue = new List<Models.ExcelImport.Data>();
                                dataValue = finalData;

                                for (int k = 0; k < reader.RowCount; k++)
                                {
                                    if (reader.Depth == k)
                                    {
                                        int count = 0;
                                        foreach (var item in dataValue)
                                        {
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                i = count;
                                                item.Value = reader.GetValue(i) != null ? reader.GetValue(i).ToString() : null;
                                                count++;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                var obj = new AllData();
                                obj.Data = dataValue;
                                allData.Add(obj);
                            }

                            if (reader.Depth != 1)
                            {

                                List<Models.ExcelImport.Data> forSecondList = new List<Models.ExcelImport.Data>();
                                foreach (var item in finalData)
                                {
                                    Models.ExcelImport.Data forSecond = new Models.ExcelImport.Data();
                                    forSecond.Key = item.Key;
                                    forSecondList.Add(forSecond);
                                }

                                for (int k = 0; k < reader.RowCount; k++)
                                {
                                    if (reader.Depth == k)
                                    {
                                        int count = 0;
                                        foreach (var item in forSecondList)
                                        {
                                            for (int i = 0; i < reader.FieldCount; i++)
                                            {
                                                i = count;
                                                item.Value = reader.GetValue(i) != null ? reader.GetValue(i).ToString() : null;
                                                count++;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                var obj = new AllData();
                                obj.Data = forSecondList;
                                allData.Add(obj);
                            }

                        }

                    }
                }
                return allData;
            }
            catch (Exception ex)
            {

                Log.Warn("Could not ProductSave...");
                Log.Error(ex);
                throw;
            }

        }

    }
}