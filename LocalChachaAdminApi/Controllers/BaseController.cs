using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalChachaAdminApi.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction]
        public new OkResult Ok()
        {
            return new OkResult();
        }

        [NonAction]
        public OkObjectResult Ok<T>(T obj)
        {
            return new OkObjectResult(obj);
        }

        [NonAction]
        public ObjectResult Accepted<T>(T obj)
        {
            ObjectResult objectResult = new ObjectResult(obj)
            {
                StatusCode = StatusCodes.Status202Accepted
            };
            return objectResult;
        }

        [NonAction]
        public ObjectResult NotAcceptable<T>(T obj)
        {
            ObjectResult objectResult = new ObjectResult(obj)
            {
                StatusCode = StatusCodes.Status406NotAcceptable
            };
            return objectResult;
        }

        [NonAction]
        public ObjectResult NotAcceptable(string errorMessage)
        {
            ObjectResult objectResult = new ObjectResult(errorMessage)
            {
                StatusCode = StatusCodes.Status406NotAcceptable
            };
            return objectResult;
        }

        [NonAction]
        public ObjectResult NotFound<T>(T obj)
        {
            return new NotFoundObjectResult(obj);
        }

        [NonAction]
        public ObjectResult NotImplemented<T>(T obj)
        {
            ObjectResult objectResult = new ObjectResult(obj)
            {
                StatusCode = StatusCodes.Status501NotImplemented
            };
            return objectResult;
        }

        [NonAction]
        public ObjectResult BadRequest<T>(T obj)
        {
            return new BadRequestObjectResult(obj);
        }

        [NonAction]
        public ObjectResult Unauthorized<T>(T obj)
        {
            return new UnauthorizedObjectResult(obj);
        }
    }
}
