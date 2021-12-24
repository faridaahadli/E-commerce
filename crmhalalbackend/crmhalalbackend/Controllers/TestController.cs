using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Helpers;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ApproveStore(int id)
        {
            string sql = @"SELECT
	                        S.[DOMAIN],
	                        C.[TEXT] 
                        FROM
	                        [dbo].[NEW_STORE] S
	                        INNER JOIN NEW_CONTACT C ON C.CONTACT_ID = S.DEFAULT_EMAIL_ID 
                        WHERE
	                        S.STORE_ID = @id 
	                        AND S.IS_ACTIVE =1";
            string sqlUpdate = @"update NEW_STORE set IS_ALLOWED = 1 where STORE_ID = @id and IS_ACTIVE=1 ";
            string domain = "";
            string storeEmail = "";
            string k;
            object res = new object();


            try
            {
              

                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@id", SqlDbType.Int, 10, ParameterDirection.Input, id)
                    });

                    if (reader.Read())
                    {
                        domain = reader["DOMAIN"].ToString();
                        storeEmail = reader["TEXT"].ToString();
                    }
                }

                if (domain.Contains(".note.az"))
                {
                    domain = domain.Substring(0, domain.LastIndexOf(".note.az", StringComparison.CurrentCultureIgnoreCase));
                    k = await CreateSubDomain.CreateDomain(domain);
                    res = JsonConvert.DeserializeObject(k);
                }
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            try
            {
                string htmlConfirm = HtmlFileSend.HtmlFileSender("~/HtmlFiles/Magaza tesdiq/index.html");
                using (var con = new DbHandler())
                {
                    con.ExecuteSql(sqlUpdate, new[]
                    {
                        DbHandler.SetParameter("@id", SqlDbType.Int, 10, ParameterDirection.Input, id)
                    });

                    await EmailSend.SendEmailAsync(storeEmail, "Note- Mağaza qeydiyyatı", htmlConfirm );
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Ok(res);
        }

        [HttpGet]
        [AllowAnonymous]
        public  IHttpActionResult GetCompanyAndStoreData(bool? isStore)
        {
            string sql = $@"SELECT
	                        C.NAME CompanyName,
	                        ( SELECT CA.[TEXT] FROM NEW_CONTACT CA WHERE CA.COMPANY_ID = C.COMPANY_ID AND CA.CONTACT_TYPE_ID = 2 AND IS_ACTIVE = 1 ) CompanyPhone,
	                        S.STORE_ID StoreId,
	                        S.NAME Name,
	                        ( SELECT CA.[TEXT] FROM NEW_CONTACT CA WHERE CA.CONTACT_ID = S.DEFAULT_PHONE_ID ) StorePhone,
	                        U.FIRST_NAME + ' ' + U.LAST_NAME UserName,
	                        U.EMAIL UserEmail,
	                        ( SELECT CA.[TEXT] FROM NEW_CONTACT CA WHERE CA.USER_ID = U.USER_ID AND CA.IS_ACTIVE = 1 ) UserPhone,
	                        ( CASE WHEN s.STORE_ID IS NOT NULL THEN CONVERT ( BIT, 1 ) ELSE CONVERT ( BIT, 0 ) END ) Is_Store,
	                        S.IS_ALLOWED IsAllowed,
	                        S.DOMAIN [Domain] 
                        FROM
	                        NEW_COMPANY C
	                        LEFT JOIN NEW_STORE S ON S.TENANT_ID = ( SELECT T.TENANT_ID FROM NEW_TENANT T WHERE T.COMPANY_ID = C.COMPANY_ID )
	                        LEFT JOIN NEW_USER U ON U.USER_ID = C.USER_ID 
                        WHERE
	                        C.IS_ACTIVE = 1 
	                        AND U.IS_ACTIVE = 1
                            {(isStore.HasValue? (isStore.Value? "AND S.STORE_ID IS NOT NULL": "AND S.STORE_ID IS NULL "):"") }
                        ORDER BY
	                        C.COMPANY_ID DESC";




            return Ok();
        }



    }
}
