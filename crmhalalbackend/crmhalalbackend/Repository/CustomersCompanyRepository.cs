using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.CustomersCompany;
using CRMHalalBackEnd.Models.ExcelImport;
using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class CustomersCompanyRepository
    {
        private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<AllData> ReadDataFromExcel(string path)
        {
            List<AllData> allData = new List<AllData>();
            try
            {

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
                                if (item.Key == "Ad*" || item.Key == "Soyad*" || item.Key == "Telefon"
                                    || item.Key == "E-poçt" || item.Key == "Şirkət" || item.Key == "Doğum tarixi (dd/mm/yyyy)")
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
            }

            catch (Exception e)
            {
                Log.Error(e);
                throw;

            }
            return allData;
        }


        public string Insert(List<AllData> allDatas, string tenantId, int userId)
        {

            var finalData = ConverData(allDatas);
            foreach (var data in finalData)
            {
                try
                {

                    using (var con = new DbHandler())
                    {
                        var json = JsonConvert.SerializeObject(data);
                        con.ExecuteStoredProcedure("[CustCompanyInsert]", new[]
                        {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                    }


                }
                catch (Exception ex)
                {
                    Log.Warn("Could not CompanyInsert...");
                    Log.Error(ex);
                    throw;
                }
            }

            return " OK ";
        }
        public List<InsertCustomersData> ConverData(List<AllData> allDatas)
        {
            List<InsertCustomersData> customersData = new List<InsertCustomersData>();

            List<AllCustomerData> allBackData = new List<AllCustomerData>();

            foreach (var datas in allDatas)
            {
                AllCustomerData customerData = new AllCustomerData();
                foreach (var data in datas.Data)
                {

                    customerData.Name = data.Key == "Ad*" ? data.Value : customerData.Name;
                    customerData.Surname = data.Key == "Soyad*" ? data.Value : customerData.Surname;
                    customerData.Phone = data.Key == "Telefon" ? data.Value : customerData.Phone;
                    customerData.Email = data.Key == "E-poçt" ? data.Value : customerData.Email;
                    customerData.CompanyName = data.Key == "Şirkət" ? data.Value : customerData.CompanyName;
                    // customerData.BirthDay = data.Key == "Doğum tarixi (Məsələn: 03/04/2012)" ? DateTime.ParseExact(data.Value, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture) : customerData.BirthDay;

                    if (data.Key == "Doğum tarixi (dd/mm/yyyy)" && data.Value != null)
                    {
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        var splitedItem = data.Value.Split('/');
                        var finalItem = string.Empty;
                        for (int i = 0; i < splitedItem.Length; i++)
                        {
                            if (splitedItem[i].Length == 1)
                                splitedItem[i] = "0" + splitedItem[i];
                            var aaaa = splitedItem[i];



                            finalItem = i != 2 ? finalItem + splitedItem[i] + '/' : finalItem + splitedItem[i];

                        }

                        string test = string.Empty;
                        DateTime oDate;
                        if (finalItem.Contains(" "))
                        {
                            test = finalItem.Substring(0, finalItem.IndexOf(" "));
                            oDate = DateTime.ParseExact(test, "dd/MM/yyyy", provider);
                        }
                        else
                            oDate = DateTime.ParseExact(finalItem, "dd/MM/yyyy", provider);

                        customerData.BirthDay = oDate;
                    }
                    if (data.Key == "Doğum tarixi (dd/mm/yyyy)" && data.Value == null)
                        customerData.BirthDay = null;
                }
                allBackData.Add(customerData);
            }

            var groupedDatas = allBackData
                      .GroupBy(u => u.CompanyName)
                      .Select(grp => grp.ToList()).ToList()
                      ;


            foreach (var groupedData in groupedDatas)
            {
                List<Users> users = new List<Users>();
                InsertCustomersData insertData = new InsertCustomersData();
                foreach (var data in groupedData)
                {

                    insertData.CompanyName = data.CompanyName;

                    Users userData = new Users();

                    userData.Name = data.Name;
                    userData.Surname = data.Surname;
                    userData.Phone = data.Phone;
                    userData.Email = data.Email;
                    userData.BirthDay = data.BirthDay;
                    users.Add(userData);
                    insertData.Users = users;
                }

                customersData.Add(insertData);
            }





            return customersData;
        }


        public List<GetCustomers> AllCustomers(string tenantId, int userId)
        {
            const string sql =
                @"select (select C.NAME+' '+C.SURNAME UserName, C.PHONE Phone, C.EMAIL Email, C.BIRTH_DATE BirthDate, 
                    (select CC.NAME from NEW_CUST_COMPANY CC where CC.CUSTOMER_COMP_ID=C.CUSTOMER_COMPANY_ID) CompanyName 
                    from NEW_CUSTOMERS C where TENANT_ID=@tenantId and  C.IS_ACTIVE=1 and
                    EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId , '109'))  for json path)Json";

            List<GetCustomers> customers = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql, new[]
                        {
                            DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                            DbHandler.SetParameter("userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                        });

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                }
                customers = JsonConvert.DeserializeObject<List<GetCustomers>>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return customers;
        }
    }
}