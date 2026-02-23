using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Oda.HospitalManagement.Application.DTOs;
using Oda.HospitalManagement.Application.DTOs.Patient;
using Oda.HospitalManagement.Application.Interfaces;

namespace Oda.HospitalManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        private readonly IPatientService _patientService;

        [HttpGet]
        public async Task<Results<Ok<List<GetPatientDTO>>, BadRequest>> Get([FromQuery] FilteringParametersDTO paging, CancellationToken cancellationToken = default)
        {
            var result = await _patientService.FilterAsync(paging, cancellationToken);

            return result.Type != Application.ResultType.Success
                ? TypedResults.BadRequest()
                : TypedResults.Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<Results<Ok<GetPatientDTO>, BadRequest>> Get([FromRoute] Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _patientService.GetByIdAsync(id, cancellationToken);

            return result.Type != Application.ResultType.Success
                ? TypedResults.BadRequest()
                : TypedResults.Ok(result.Data);
        }

        [HttpPost]
        public async Task<Results<Created<GetPatientDTO>, Conflict<string>, BadRequest>> Post([FromBody] CreatePatientDTO dto, CancellationToken cancellationToken)
        {
            var result = await _patientService.AddAsync(dto, cancellationToken);

            if (result.Type == Application.ResultType.Success)
                return TypedResults.Created((string?)null, result.Data);
            else if (result.Type == Application.ResultType.Conflict)
                return TypedResults.Conflict(result.Message);

            return TypedResults.BadRequest();
        }

        [HttpPut]
        public async Task<Results<Ok<GetPatientDTO>, NotFound, BadRequest>> Put([FromBody] UpdatePatientDTO dto, CancellationToken cancellationToken)
        {
            var result = await _patientService.UpdateAsync(dto, cancellationToken);

            if (result.Type == Application.ResultType.NotFound)
                return TypedResults.NotFound();

            if (result.Type == Application.ResultType.Success)
                return TypedResults.Ok(result.Data);

            return TypedResults.BadRequest();
        }

        [HttpPost("{id}/discharge")]
        public async Task<Results<Ok, Ok<string>, NotFound<string>, BadRequest>> Discharge([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _patientService.DischargeAsync(id, cancellationToken);

            if (result.Type == Application.ResultType.Success)
                return TypedResults.Ok(result.Data);
            else if (result.Type == Application.ResultType.NotFound)
                return TypedResults.NotFound(result.Message);

            return TypedResults.BadRequest();
        }
    }
}
