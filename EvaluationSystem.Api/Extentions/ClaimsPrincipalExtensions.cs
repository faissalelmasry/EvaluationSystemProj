using EvaluationSystem.Domain.Exceptions;
using System.Security.Claims;

namespace EvaluationSystem.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var reviewerIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(reviewerIdClaim) || !int.TryParse(reviewerIdClaim, out int userId))
            {
                throw new UnauthorizedException("Invalid user token or user ID could not be parsed.");
            }

            return userId;
        }
    }
}