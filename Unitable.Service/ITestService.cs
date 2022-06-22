using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitable.Dto.Request;
using Unitable.Dto.Response;
using Unitable.Entities;

namespace Unitable.Service
{
    public interface ITestService
    {
        Task<List<Test>> Get();
        Task<BaseResponseGeneric<Test>> Post(DtoTest request);
        Task<Test> Delete(int TestId);
        Task<BaseResponseGeneric<Test>> Put(int TestId, DtoTest request);
        /*Task<List<Boolean>> TestResultado(List<Boolean> request);*/
        /*Task<List<Test>> GetTestById(int testId);*/
    }
}
