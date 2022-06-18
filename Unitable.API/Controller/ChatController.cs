using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unitable.DataAccess;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;
using Unitable.Service;

namespace Unitable.API.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly UnitableDbContext _context;

        public ChatController(IChatService chatService, UnitableDbContext context)
        {
            _chatService = chatService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BaseResponseGeneric<ICollection<Chat>>>> Get()
        {
            var response = new BaseResponseGeneric<ICollection<Chat>>();
            try
            {
                response.Result = await _chatService.Get();
                response.Success = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(DtoChat request)
        {
            var entity = await _chatService.Post(request);

            HttpContext.Response.Headers.Add("location", $"/api/Tema/{entity.Id}");

            return Ok(entity);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int ChatId)
        {
            var entity = await _chatService.Delete(ChatId);
            if (entity == null)
                return NotFound("Chat #" + ChatId + " Not Found");
            return Ok(entity);
        }

        [HttpPut("{ChatId:int}")]
        public async Task<ActionResult> Put(int ChatId, DtoChat request)
        {
            var ChatFromDb = await _chatService.Put(ChatId, request);
            if (ChatFromDb != null)
            {
                HttpContext.Response.Headers.Add("location", $"/api/Chat/{ChatFromDb.Id}");
                return Ok(new { Id = ChatId });
            }
            else
            {
                return NotFound("Chat #" + ChatId + " Not Found");
            }
        }
    }
}
