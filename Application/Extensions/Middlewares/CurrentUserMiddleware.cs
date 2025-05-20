using System.IdentityModel.Tokens.Jwt;
using Abstractions.Services;
using BL.Services;

namespace Application.Extensions.Middlewares;

public class CurrentUserMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var token))
        {
            var parsedToken = ParseJwtToken(token.ToString().Replace("Bearer ", string.Empty));
            var userIdClaim = parsedToken.Claims.FirstOrDefault(x => x.Type == "UserId");

            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                currentUserService.SetCurrentUser(userId);
            }
            else
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            await _next(context);
        }
        else
        {
            await _next(context);
        }
    }

    private JwtSecurityToken ParseJwtToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ReadJwtToken(token);
    }
}