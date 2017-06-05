using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace Finix.IPDC.API.Models
{
    public class BaseResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public List<object> Data { get; protected set; }

        private BaseResponse()
        {
            Data=new List<object>();
        }

        public static BaseResponse CreateWith(HttpStatusCode httpStatusCode=HttpStatusCode.OK)
        {
            var o = new BaseResponse();
            o.StatusCode = httpStatusCode;
            return o;
        }
    }

    public class TestModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public static void WriteToFile(TestModel model)
        {
            var path = @"D:\API\Data";
            Directory.CreateDirectory(path);
            var data = JsonConvert.SerializeObject(model);
            File.WriteAllText(Path.Combine(path, "Test_Model_" + DateTime.Now.Ticks), data);
        }
    }
}