using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Enums;
using Server.Repositories;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Microsoft.Extensions.ML;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly ReviewService _reviewService;
        private readonly UserService _userService;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _model;

        public ReviewController(ReviewService reviewService, UnitOfWork unitOfWork, PredictionEnginePool<ModelInput, ModelOutput> model,
            UserService userService)
        {
            _reviewService = reviewService;
            _model = model;
            _userService = userService;
        }

        [Authorize]
        [HttpPost("AddReview")]
        public async Task AddReviewAsync([FromBody]ReviewDto reviewDto)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                Console.WriteLine("No hay usuario");
                return;
            }

            await _reviewService.AddReview(reviewDto, user.Id);
        }

        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _userService.GetUserFromDbByStringId(idString);
        }

    }
}