﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers.Filters;
using WebApi.Models.Interfaces;
using WebApi.Models.Schemas;

namespace WebApi.Controllers
{
    [UseApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Route("AllReviews")]
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            if (ModelState.IsValid)
            {
                var result = await _reviewService.GetAllAsync();
                if (result != null)
                    return Ok(result);
                else
                    return NotFound("No reviews found");
            }

            return BadRequest("Something went wrong, try again!");
        }

        [Route("GetByProductId")]
        [HttpGet]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            if (ModelState.IsValid)
            {
                var result = await _reviewService.GetByProductId(productId);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound("No reviews found");
            }

            return BadRequest("Something went wrong, try again!");
        }

        [Route("AddReview")]
        [HttpPost]
       // [Authorize]
        public async Task<IActionResult> AddReview(ReviewSchema schema)
        {
            if (ModelState.IsValid)
            {
                var userName = HttpContext.User.Identity!.Name;
                if (userName != null)
                {
                    var result = await _reviewService.CreateAsync(schema, userName);
                    if (result)
                        return Created("", null);
                }
            }

            return BadRequest("Something went wrong, try again!");
        }
    }
}
