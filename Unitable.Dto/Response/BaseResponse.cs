using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unitable.Dto.Response
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public ICollection<string> Errors { get; set; }

        protected BaseResponse()
        {
            Errors = new List<string>();
        }
    }

    public class BaseResponseGeneric<TClass> : BaseResponse
    {
        public TClass Result { get; set; }
    }
}
