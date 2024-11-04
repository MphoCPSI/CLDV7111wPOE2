using System.Security.Claims;
using KhumaloCraft.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KhumaloCraft.Web.Pages
{
    public class NotificationsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificationsModel> _logger;

        public NotificationsModel(IHttpClientFactory httpClientFactory, ILogger<NotificationsModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("BusinessAPI");
            _logger = logger;
        }

        public List<NotificationDTO> Notifications { get; private set; }

        public async Task OnGetAsync()
        {
            await LoadNotificationsAsync();
        }

        // Method to load all notifications
        private async Task LoadNotificationsAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _logger.LogWarning("User ID not found in claims.");
                    Notifications = new List<NotificationDTO>();
                    return;
                }

                Notifications = await _httpClient.GetFromJsonAsync<List<NotificationDTO>>($"api/notifications/user/{userId}")
                                ?? new List<NotificationDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications.");
                Notifications = new List<NotificationDTO>();
            }
        }

        // Method to load only unread notifications
        public async Task<IActionResult> OnGetUnreadAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    _logger.LogWarning("User ID not found in claims.");
                    Notifications = new List<NotificationDTO>();
                    return Page();
                }

                Notifications = await _httpClient.GetFromJsonAsync<List<NotificationDTO>>($"api/notifications/user/{userId}/unread")
                                ?? new List<NotificationDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching unread notifications.");
                Notifications = new List<NotificationDTO>();
            }

            return Page();
        }

        // Method to mark a notification as read
        public async Task<IActionResult> OnPostMarkAsReadAsync(int notificationId)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"api/notifications/{notificationId}/mark-read", null);
                if (response.IsSuccessStatusCode)
                {
                    await LoadNotificationsAsync(); // Refresh the list after marking as read
                }
                else
                {
                    _logger.LogWarning("Failed to mark notification as read.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read.");
            }

            return RedirectToPage();
        }
    }
}
