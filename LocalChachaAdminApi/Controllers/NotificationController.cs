using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LocalChachaAdminApi.Core.Interfaces;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LocalChachaAdminApi.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [AllowAnonymous]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly IMapper mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            this.notificationService = notificationService;
            this.mapper = mapper;
        }

        [HttpPost("sendemail")]
        public async Task<ActionResult> SendEmail(EmailModel emailModel)
        {
            try
            {
                var email = mapper.Map<EmailModel, Email>(emailModel);
                await notificationService.SendEmail(email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("sendsms")]
        public async Task<ActionResult> SendSms()
        {
            try
            {
                await notificationService.SendSms();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
