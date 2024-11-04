using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using KhumaloCraft.Shared.DTOs;
using System.Security.Claims;

namespace KhumaloCraft.Pages.Auth
{
  public class LoginModel : PageModel
  {
    private readonly HttpClient _httpClient;
    private readonly ILogger<LoginModel> _logger;

    // Bind the LoginDTO to handle form data from Razor Page
    [BindProperty]
    public LoginDTO LoginDTO { get; set; }

    public string ErrorMessage { get; set; }

    public LoginModel(IHttpClientFactory httpClientFactory, ILogger<LoginModel> logger)
    {
      _httpClient = httpClientFactory.CreateClient("BusinessAPI");
      _logger = logger;
    }

    public IActionResult OnGet()
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToPage("/Index");
      }

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      // Prepare login data
      var loginData = new
      {
        Email = LoginDTO.Email,
        Password = LoginDTO.Password
      };

      // Send login request to the API
      var jsonContent = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
      var response = await _httpClient.PostAsync("api/auth/login", jsonContent);

      if (response.IsSuccessStatusCode)
      {
        // Parse the token response
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponseDTO>(jsonResponse, new JsonSerializerOptions
        {
          PropertyNameCaseInsensitive = true
        });

        // Store the JWT token in cookies
        HttpContext.Response.Cookies.Append("AuthToken", tokenResponse.Token, new CookieOptions
        {
          HttpOnly = true, // Prevent client-side access to the cookie for security
          Expires = DateTimeOffset.UtcNow.AddHours(1) // Token expires in 1 hour
        });

        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(tokenResponse.Token);
        var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        Response.Cookies.Append("CartId", userId, new CookieOptions
        {
          Expires = DateTimeOffset.UtcNow.AddDays(30),
          HttpOnly = true,
          IsEssential = true
        });

        var linkCartData = new CartLinkDTO
        {
          cartId = userId,
          userId = userId
        };

        var cartLinkData = new StringContent(JsonSerializer.Serialize(linkCartData), Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("api/cart/link", cartLinkData);
        // Redirect to a protected page or homepage after login
        return RedirectToPage("/Index");
      }
      else
      {
        // Log the error and display a message to the user
        ErrorMessage = "Invalid email or password.";
        _logger.LogWarning("Failed login attempt for user: {Email}", LoginDTO.Email);
        return Page();
      }
    }
  }
}
