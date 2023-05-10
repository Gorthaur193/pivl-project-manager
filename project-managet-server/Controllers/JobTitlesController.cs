﻿using Microsoft.AspNetCore.Mvc;
using project_managet_dblayer;
using project_managet_models;
using project_managet_models.Models;
using project_managet_server.Services;

namespace project_managet_server.Controllers
{
    /// <summary>
    /// Info about job titlees
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JobTitlesController : ControllerBase
    {
        private readonly EntityGateway _db = new();
        private Guid Token => Guid.Parse(Request.Headers["Token"] != string.Empty ?
                                    Request.Headers["Token"]! : Guid.Empty.ToString());

        /// <summary>
        /// Get all jobtitles
        /// </summary>
        [HttpGet]
        public IActionResult GetAll() => 
            Ok(new
            {
                status = "ok",
                jobtitles = _db.GetJobTitles()
            });

        /// <summary>
        /// Get job title by id. 
        /// </summary>
        [HttpGet] [Route("{id}")]
        public IActionResult GetById(Guid id)
        {
            var potentialTitle = _db.GetJobTitles(x => x.Id == id).FirstOrDefault();
            if (potentialTitle is not null)
                return Ok(new
                {
                    status = "ok",
                    jobtitles = potentialTitle
                });
            else
                return NotFound(new
                {
                    status = "fail",
                    message = $"There is no job title with {id} id"
                });
        }

        [HttpPost]
        public IActionResult PostJobTitle([FromBody] JobTitle jobTitle)
        {
            if (LocalAuthService.GetInstance().GetRole(Token) != Role.Admin)
                return Unauthorized(new
                {
                    status = "fail",
                    message = "You have no rights for that action."
                });

            _db.AddOrUpdate(jobTitle);
            return Ok(new
            {
                status = "ok",
                id = jobTitle.Id
            });            
        }
    }
}