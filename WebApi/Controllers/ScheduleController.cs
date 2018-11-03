using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Business;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        [HttpGet]
        [Route("List")]
        public IActionResult List([FromQuery] string username)
        {
            return Ok(
                new {
                    schedules = new ScheduleBusiness().ListSchedule(username)
                });
        }

        [HttpPost]
        [Route("Upsert")]
        public IActionResult Upsert([FromBody] Schedule schedule)
        {   
            var modelSchedule = new Business.Models.Schedule(){
                _id = schedule._id,
                Username = schedule.Username,
                Contacts = schedule.Contacts,
                ChatMessage = schedule.ChatMessage,
                PathImages = schedule.PathImages,
                WillSendDate = schedule.WillSendDate
            };
            return Ok(
                new {
                    Result = new ScheduleBusiness().UpsertSchedule(modelSchedule)
                });       
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete([FromQuery] string id)
        {
            return Ok(new ScheduleBusiness().DeleteSchedule(id));
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromBody] Base64 image)
        {
            return Ok(
                new {
                    ImagePath = await new ScheduleBusiness().SaveBase64ToLocalAsync(image.Base64Image)
                });    
        }

        [HttpGet]
        [Route("Photo")]
        public async Task<IActionResult> Photo([FromQuery] string id)
        {
            return Ok(
                new {
                    ImageBase64 = await new ScheduleBusiness().GetBase64Async(id)
                }
            );
        }

        [HttpGet]
        [Route("GetContacts")]
        public async Task<IActionResult> GetContactsAsync([FromQuery] string userid)
        {
            return Ok(
                await new ScheduleBusiness().GetContactsAsync(userid)
            );
        }
    }
}
