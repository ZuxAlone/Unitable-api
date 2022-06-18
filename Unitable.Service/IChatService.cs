using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.Dto.Request;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface IChatService
    {
        Task<ICollection<Chat>> Get();
        Task<Chat> Put(int ChatId, DtoChat request);
        Task<Chat> Delete(int ChatId);
        Task<Chat> Post(DtoChat request);
    }
}
