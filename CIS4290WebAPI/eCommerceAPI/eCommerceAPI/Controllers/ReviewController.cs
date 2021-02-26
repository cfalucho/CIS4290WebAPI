using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eCommerceAPI.Entities;
using eCommerceAPI.Models;
using eCommerceAPI.Services;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eCommerceAPI.Controllers
{
    [Route("api/review")]
    public class ReviewController : Controller
    {
        IGenericEFRepository _rep;

        public ReviewController(IGenericEFRepository rep)
        {
            _rep = rep;
        }
        // GET: api/review
        [HttpGet]
        public IActionResult Get()
        {
            var items = _rep.Get<Review>();
            var DTOs = Mapper.Map<IEnumerable<ReviewDTO>>(items);
            return Ok(DTOs);
        }

        // GET api/review/:reviewId:
        [HttpGet("{reviewId}", Name = "GetGenericReview")]
        public IActionResult Get(int reviewId)
        {
            var reviews = _rep.Get<Review>().Where(p =>
            p.ReviewID.Equals(reviewId));

            var DTOs = Mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return Ok(DTOs);
        }
    
        // POST api/review
        [HttpPost]
        public IActionResult Post([FromBody]ReviewDTO DTO)
        {
            if (DTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var itemToCreate = Mapper.Map<Review>(DTO);

            _rep.Add(itemToCreate);

            if (!_rep.Save()) return StatusCode(500,
                 "A problem occured while handling your request.");
            var createdDTO = Mapper.Map<ReviewDTO>(itemToCreate);

            return CreatedAtRoute("GetGenericReview",
                new { reviewId = createdDTO.ReviewID }, createdDTO);
        }

        // PUT api/review/:reviewId:
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]ReviewUpdateDTO DTO)
        {
            if (DTO == null) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _rep.Get<Review>(id);
            if (entity == null) return NotFound();

            Mapper.Map(DTO, entity);

            if (!_rep.Save()) return StatusCode(500,
                "A Problem Happend while handling your request.");
            return NoContent();
        }

        // DELETE api/review/:reviewId:
        [HttpDelete("{reviewId}")]
        public IActionResult Delete(int reviewId)
        {
            if (!_rep.Exists<Review>(reviewId)) return NotFound();

            var entityToDelete = _rep.Get<Review>(reviewId);

            _rep.Delete(entityToDelete);

            if (!_rep.Save()) return StatusCode(500,
                 "A problem occured while handling your request.");

            return NoContent();

        }
    }
}
