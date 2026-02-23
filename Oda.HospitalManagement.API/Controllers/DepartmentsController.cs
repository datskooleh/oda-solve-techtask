using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Oda.HospitalManagement.Application;
using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Department;
using Oda.HospitalManagement.Application.Interfaces;

namespace Oda.HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<Results<Ok<List<GetDepartmentDTO>>, BadRequest>> Get([FromQuery] FilteringParametersDTO paging, CancellationToken cancellationToken = default)
        {
            var result = await _departmentService.FilterAsync(paging, cancellationToken);

            return result.Type != Application.ResultType.Success
                ? TypedResults.BadRequest()
                : TypedResults.Ok(result.Data);
        }

        [HttpGet("{id}/patients")]
        public async Task<Results<BadRequest, Ok<List<GetDepartmentPatientDTO>>>> Get([FromRoute] Guid id, [FromQuery] DateOnly? submissionDate, [FromQuery] FilteringParametersDTO paging, CancellationToken cancellationToken = default)
        {
            var result = await _departmentService.GetOccupationAsync(id, submissionDate, paging, cancellationToken);

            return result.Type != Application.ResultType.Success
                ? TypedResults.BadRequest()
                : TypedResults.Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IResult> Get([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _departmentService.GetByIdAsync(id, cancellationToken);

            if (result.Type == Application.ResultType.NotFound)
                return TypedResults.NotFound();
            else if (result.Type == Application.ResultType.Success)
                return TypedResults.Ok(result.Data);

            return TypedResults.BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _departmentService.DeleteAsync(id, cancellationToken);

            if (result.Type != Application.ResultType.Success)
                return TypedResults.BadRequest(result.Message);

            return TypedResults.Ok();
        }

        [HttpPost]
        public async Task<Results<Created<GetDepartmentDTO>, BadRequest<string>, Conflict<string>>> Post([FromBody] CreateDepartmentDTO dto, CancellationToken cancellationToken)
        {
            var result = await _departmentService.AddDepartmentAsync(dto, cancellationToken);

            if (result.Type == Application.ResultType.Success)
                return TypedResults.Created((string?)null, result.Data);
            else if (result.Type == Application.ResultType.Conflict)
                return TypedResults.Conflict(result.Message);

            return TypedResults.BadRequest(result.Message);
        }

        [HttpPut]
        public async Task<Results<Ok<GetDepartmentDTO>, Conflict<string>, NotFound, BadRequest>> Put([FromBody] UpdateDepartmentDTO dto, CancellationToken cancellationToken)
        {
            var result = await _departmentService.UpdateAsync(dto, cancellationToken);

            if (result.Type == Application.ResultType.NotFound)
                return TypedResults.NotFound();
            else if (result.Type == Application.ResultType.Conflict)
                return TypedResults.Conflict(result.Message);

            if (result.Type == Application.ResultType.Success)
                return TypedResults.Ok(result.Data);

            return TypedResults.BadRequest();
        }

        [HttpPost("admissions")]
        public async Task<Results<Ok<string>, Conflict<string>, NotFound, BadRequest<string>>> Admit([FromBody] AdmitPatientDTO dto, CancellationToken cancellationToken = default)
        {
            var result = await _departmentService.AssignAsync(dto, cancellationToken);

            if (result.Type == ResultType.NotFound)
                return TypedResults.NotFound();
            else if(result.Type == ResultType.Conflict)
                return TypedResults.Conflict(result.Message);
            if (result.Type == ResultType.Success)
                return TypedResults.Ok(result.Data);

            return TypedResults.BadRequest(result.Message);
        }
    }
}
