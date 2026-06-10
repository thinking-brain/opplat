// // CostTabsController has been replaced by Endpoints/SalesEndpoints.cs (Minimal API + MediatR).
// // This file is kept for reference only and is not mapped by the host.

// using Microsoft.AspNetCore.Mvc;
// using Opplat.Modules.Sales.Domain.Entities;
// using Opplat.Modules.Sales.Domain.Services;
// using Opplat.Microservices.Shared.Dtos;
// using Opplat.Shared.Services;

// namespace Opplat.Services.Sales.Api.Controllers;

// // [Authorize]
// // [Area("sales")]
// // [Route("[area]/[controller]/")]
// public class CostTabsController_Archived : ControllerBase
// {
//     private readonly ICostTabService _service;

//     public CostTabsController_Archived(ICostTabService service)
//     {
//         _service = service;
//     }

//     [HttpGet("{id}")]
//     public async Task<CostTab> Get(string id)
//     {
//         var result = await _service.Get(id);
//         if (result.Status == ServiceStatus.Ok)
//         {
//             return result.Value;
//         }
//         return new CostTab();
//     }

//     [HttpGet()]
//     public async Task<IEnumerable<CostTab>> List()
//     {
//         var result = await _service.List();
//         if (result.Status == ServiceStatus.Ok)
//         {
//             return result.List;
//         }
//         return new List<CostTab>();
//     }

//     [HttpPost]
//     public async Task<ResponseDto> Post([FromBody] CostTab costTab)
//     {
//         var user = User?.Identity?.Name;
//         var result = await _service.Create(costTab, user);
//         if (result.Status == ServiceStatus.Ok)
//         {
//             return new ResponseDto
//             {
//                 Status = true,
//                 Message = result.Message,
//                 Errors = new List<string>() { }
//             };
//         }
//         return new ResponseDto
//         {
//             Status = false,
//             Message = result.Message,
//             Errors = new List<string>() { result.Message }
//         };
//     }

//     [HttpPut]
//     public async Task<ResponseDto> Put([FromBody] CostTab costTab)
//     {
//         var user = User?.Identity?.Name;
//         var result = await _service.Update(costTab, user);
//         if (result.Status == ServiceStatus.Ok)
//         {
//             return new ResponseDto
//             {
//                 Status = true,
//                 Message = result.Message,
//                 Errors = new List<string>() { }
//             };
//         }
//         return new ResponseDto
//         {
//             Status = false,
//             Message = result.Message,
//             Errors = new List<string>() { result.Message }
//         };
//     }

//     [HttpDelete("{id}")]
//     public async Task<ResponseDto> Delete(string id)
//     {
//         var user = User?.Identity?.Name;
//         var result = await _service.Delete(id, user);
//         if (result.Status == ServiceStatus.Ok)
//         {
//             return new ResponseDto
//             {
//                 Status = true,
//                 Message = result.Message,
//                 Errors = new List<string>() { }
//             };
//         }
//         return new ResponseDto
//         {
//             Status = false,
//             Message = result.Message,
//             Errors = new List<string>() { result.Message }
//         };
//     }
// }

