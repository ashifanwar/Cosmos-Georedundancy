using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using APIManagement.Azure.CosmosDb;
using APIManagement.Contract;
using APIManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace APIManagement.Controllers
{
    /// <summary>
    /// Controller.
    /// </summary>
    [Produces("application/json")]
    [Route("api/Personal")]
    public class PersonalController : Controller
    {
        /// <summary>
        /// Version of the controller
        /// </summary>
        public const string APIVERSION = "1.0";
        private List<PersonalInfo> personalInfoCollection;
        private readonly IRepository<PersonalInfo> _repository;
        /// <summary>
        /// Constructor
        /// </summary>
        public PersonalController(IRepository<PersonalInfo> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets all the personal info.
        /// </summary>
        /// <returns>collection of all personal info.</returns>
        [HttpGet]
        [SwaggerOperation("GetallPersonalInfo")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public IActionResult GetPersonalInfo()
        { 
            if(personalInfoCollection != null || personalInfoCollection.Count > 0)
            {
                return NotFound();
            }
            return Ok(personalInfoCollection);
        }

        /// <summary>
        ///  Creates a new personal info
        /// </summary>
        /// <param name="personalInfo"></param>
        /// <returns>201 created response.</returns>
        [HttpPost]
        [Route("CreateEast")]
        [SwaggerOperation("CreatePersonalInfo")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEast([FromBody] PersonalInfo personalInfo)
        {
            if(personalInfo == null)
            {
                return BadRequest("Personal Info is missing");
            }
            personalInfo.Id = Guid.NewGuid().ToString();
            personalInfo.SequenceNumber = 0;
            await _repository.CreateItemOnEastAsync(personalInfo);
            return Created("/create", personalInfo);
        }


        [HttpPost]
        [Route("Create")]
        [SwaggerOperation("CreatePersonalInfo")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] PersonalInfo personalInfo)
        {
            if (personalInfo == null)
            {
                return BadRequest("Personal Info is missing");
            }
            personalInfo.Id = Guid.NewGuid().ToString();
            personalInfo.SequenceNumber = 0;
            await _repository.CreateItemAsync(personalInfo);
            return Created("/create", personalInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personalInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateWest")]
        [SwaggerOperation("CreatePersonalInfo")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostWest([FromBody] PersonalInfo personalInfo)
        {
            if (personalInfo == null)
            {
                return BadRequest("Personal Info is missing");
            }
            personalInfo.Id = Guid.NewGuid().ToString();
            personalInfo.SequenceNumber = 0;
            await _repository.CreateItemOnWestAsync(personalInfo);
            return Created("/create", personalInfo);
        }

        /// <summary>
        /// Get specific personal info
        /// </summary>
        /// <param name="id">id of the personal info</param>
        /// <returns>personal info</returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPersonalInfoById([FromRoute] string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id is required");
            }
            var personalInfo = await _repository.GetItemByIdAsync(id);
            if(personalInfo == null)
            {
                return NotFound();
            }
            return Ok(personalInfo);
        }

        /// <summary>
        /// Get specific personal info
        /// </summary>
        /// <param name="id">id of the personal info</param>
        /// <returns>personal info</returns>
        [HttpGet]
        [Route("East/{id}")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPersonalInfoByIdEast([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id is required");
            }
            var personalInfo = await _repository.GetItemByIdEastAsync(id);
            if (personalInfo == null)
            {
                return NotFound();
            }
            return Ok(personalInfo);
        }

        /// <summary>
        /// Get specific personal info
        /// </summary>
        /// <param name="id">id of the personal info</param>
        /// <returns>personal info</returns>
        [HttpGet]
        [Route("West/{id}")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPersonalInfoByIdWest([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("id is required");
            }
            var personalInfo = await _repository.GetItemByIdWestAsync(id);
            if (personalInfo == null)
            {
                return NotFound();
            }
            return Ok(personalInfo);
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] PersonalInfo item)
        {
            return Ok();
        }
    }
}