﻿using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/Camps")]
    [ApiVersion("2.0")]
  
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public Camps2Controller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<IActionResult> Get(bool IncludeTalks = false)
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync(IncludeTalks);

                var ret = mapper.Map<List<CampModel>>(camps);
                return Ok( new {
                    count=ret.Count,
                result=ret
                
                } );
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "database failure");
            }

        }

        public async Task<ActionResult<List<CampModel>>> Get11()
        {
            try
            {
                var camps = await campRepository.GetAllCampsAsync(includeTalks:true);


                return mapper.Map<List<CampModel>>(camps);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "database failure");
            }

        }


        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> GetCamp(string moniker)
        {
            try
            {
                var camp = await campRepository.GetCampAsync(moniker);
                if (camp.Equals(null))
                {
                    return NotFound();
                }
                return mapper.Map<CampModel>(camp);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "database failure");

            }

        }
        [HttpGet("Search")]
        public async Task<ActionResult<List<CampModel>>> SearchByDate(DateTime TheDate, bool IncludeTalks = false)
        {
            try
            {
                var result = await campRepository.GetAllCampsByEventDate(TheDate, IncludeTalks);
                if (!result.Any())
                {
                    return NotFound();

                }

                return mapper.Map<List<CampModel>>(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "database failure");

            }

        }
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post([FromBody] CampModel campmodel)
        {
            try
            {
                if (await campRepository.GetCampAsync(campmodel.Moniker) != null)
                    return BadRequest("moniker is already exist");

                var Location = linkGenerator.GetPathByAction
                    ("GetCamp", "Camps", new { moniker = campmodel.Moniker });

                if (String.IsNullOrEmpty(Location))
                    return BadRequest("couldn't use current moniker");

                var camp = mapper.Map<Camp>(campmodel);
                campRepository.Add(camp);

                if (await campRepository.SaveChangesAsync())
                    return Created(Location, mapper.Map<CampModel>(camp));


            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "database failure");


            }
            return BadRequest();
        }
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel UpdatedModel)
        {
            try
            {
                var Old = await campRepository.GetCampAsync(moniker);
                if (Old.Equals(null)) { return NotFound($"Couldnt find camp with moniker with{moniker}"); }
                mapper.Map<Camp>(UpdatedModel);
                if (await campRepository.SaveChangesAsync())
                {
                    return mapper.Map<Camp, CampModel>(Old);
                }

                return BadRequest();
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "something went wrong it may be database failure");

            }
        }
        [HttpDelete("{moniker}")]
        public async Task<ActionResult> Delete(string moniker)
        {
            try
            {
                var CampToDelete = await campRepository.GetCampAsync(moniker);
                if (CampToDelete.Equals(null))
                {
                    return NotFound("the camp you want to delete wasnot found");


                }
                campRepository.Delete(CampToDelete);
                if (await campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
                return BadRequest("the server couldn't save the changes");
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong");

            }


        }

    }
}