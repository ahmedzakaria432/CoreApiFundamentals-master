using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository campRepository;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this.campRepository = campRepository;
            this.mapper = mapper;
            this.linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await campRepository.GetTalksByMonikerAsync(moniker);

                if (!talks.Equals(null))
                {
                    return Ok(mapper.Map<TalkModel[]>(talks));
                }
                return NotFound("couldn't find talks for camp that you specefied");

            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong on the server ");
            }
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = campRepository.GetTalkByMonikerAsync(moniker, id);
                if (!talk.Equals(null))
                {
                    return Ok(mapper.Map<TalkModel>(talk));

                }
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "something went wrong");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel talkToAdd)
        {
            try
            {
                var campValiate = await campRepository.GetCampAsync(moniker);
                if (campValiate.Equals(null))
                {
                    return BadRequest("Camp not found");
                }
                var talk = mapper.Map<Talk>(talkToAdd);
                talk.Camp = campValiate;
                if (talkToAdd.Speaker == null) return BadRequest("enter speaker details");

                var speaker = await campRepository.GetSpeakerAsync(talkToAdd.Speaker.SpeakerId);
                if (speaker==null)
                {
                    return BadRequest("assosiated speaker not found");

                }


                talk.Speaker = speaker;

                 campRepository.Add(talkToAdd);
                if (await campRepository.SaveChangesAsync())
                {
                    var url = linkGenerator.GetPathByAction(HttpContext, "Get",
                     values: new {moniker,id=talk.TalkId }
                        );

                    return Created(url, mapper.Map<TalkModel>(talk));
                }
                return BadRequest("couldn't save changes");
                        
                        


            }
            catch (Exception)
            {

                
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");

            }
        }
        [HttpPut("id:int")]
        public async Task<ActionResult<TalkModel>> Put(string moniker,int id,TalkModel model) 
        {
            try
            {
                var talk = await campRepository.GetTalkByMonikerAsync(moniker, id,true);
                if (talk == null) { return NotFound("talk you want to update couldn't be found"); }
                mapper.Map(model, talk);

                if (model.Speaker!=null)
                {
                    var speaker = await campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (!speaker.Equals(null))
                        talk.Speaker = speaker;
                }
                if (await campRepository.SaveChangesAsync())
                { return Ok(mapper.Map<TalkModel>(talk)); }


                return BadRequest("failed to update database"); 
                
                




            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");


            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string moniker ,int id)
        {
            try
            {
                var talkToDelete =await campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talkToDelete == null)
                    return NotFound();
                campRepository.Delete(talkToDelete);
                if (await campRepository.SaveChangesAsync())
                {
                    return Ok("deleted successfully");
                }
                return BadRequest("failed to delete talk");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");

                throw;
            }
        }


    }
}
