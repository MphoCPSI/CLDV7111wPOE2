namespace KhumaloCraft.BusinessAPI;

public class TokenMiddleware
{
  private readonly RequestDelegate _next;

  public TokenMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task Invoke(HttpContext context)
  {
    var token = context.Request.Cookies["AuthToken"];
    if (!string.IsNullOrEmpty(token))
    {
      context.Request.Headers["Authorization"] = $"Bearer {token}";
    }

    await _next(context);
  }
}
