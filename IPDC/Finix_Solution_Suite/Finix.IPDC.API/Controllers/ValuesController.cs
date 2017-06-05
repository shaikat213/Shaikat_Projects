using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Finix.IPDC.API.Helper;
using Finix.IPDC.API.Models;

namespace Finix.IPDC.API.Controllers
{
    public class SampleController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get()
        {
            var res = BaseResponse.CreateWith(HttpStatusCode.OK);
            res.Data.AddRange(new[]
            {
                new {Id = 1, Name = "This is a get message 1", Description = "This is Description 1"},
                new {Id = 2, Name = "This is a get message 2", Description = "This is Description 2"},
                new {Id = 3, Name = "This is a get message 3", Description = "This is Description 3"}
            }
                );

            return HttpResponseBuilder.BuildResponse(res);
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            var record = new TestModel
            {
                Id = id,
                Name = ("This is a get message" + id),
                Description = ("This is Description" + id)
            };
            var res = BaseResponse.CreateWith(HttpStatusCode.OK);
            res.Data.Add(record);
            return HttpResponseBuilder.BuildResponse(res);
        }

        // POST api/values
        public HttpResponseMessage Post(TestModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = DateTime.Now.Ticks;
                model.Description = "Model Is saved to file";
                TestModel.WriteToFile(model);
            }
            var res = BaseResponse.CreateWith(HttpStatusCode.Created);
            res.Data.Add(model);
            return HttpResponseBuilder.BuildResponse(res);
        }

        // PUT api/values/5
        public HttpResponseMessage Put(int id, TestModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = DateTime.Now.Ticks;
                model.Description = "Model Is saved to file";
                TestModel.WriteToFile(model);
            }
            var res = BaseResponse.CreateWith(HttpStatusCode.Created);
            res.Data.Add(model);
            return HttpResponseBuilder.BuildResponse(res);
        }

        // DELETE api/values/5
        public HttpResponseMessage Delete(int id)
        {
            var res = BaseResponse.CreateWith(HttpStatusCode.OK);
            res.Data.Add("Record Is Deleted");
            return HttpResponseBuilder.BuildResponse(res);
        }
    }
}
