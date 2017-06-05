using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Finix.IPDC.API.Models;
using Newtonsoft.Json;

namespace Finix.IPDC.API.Helper
{
    public class HttpResponseBuilder
    {

        public static HttpResponseMessage BuildResponse(BaseResponse responseDto)
        {
            return new HttpResponseMessage(responseDto.StatusCode)
            {

                Content = new StringContent(
                    JsonConvert.SerializeObject(responseDto,
                        new JsonSerializerSettings
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Ignore
                        }
                        )
                    )
            };
        }
    }
}