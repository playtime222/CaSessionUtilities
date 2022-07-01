using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

public static class AuthorizedControllerEx
{
    public static string GetUserId(this ControllerBase thiz) => ((ClaimsIdentity)thiz.User.Identity).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
}