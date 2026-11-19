using Microsoft.AspNetCore.Http;

namespace KickZone.Services
{
    public static class SessionHelper
    {
        public const string UserIdKey = "UserId";
        public const string UsernameKey = "Username";
        public const string RoleKey = "Role";

        public static int? GetUserId(this HttpContext context) => context.Session.GetInt32(UserIdKey);
        public static string? GetUsername(this HttpContext context) => context.Session.GetString(UsernameKey);
        public static string? GetRole(this HttpContext context) => context.Session.GetString(RoleKey);

        public static bool IsLoggedIn(this HttpContext context) => context.GetUserId() != null;
        public static bool IsAdmin(this HttpContext context) => context.GetRole() == "admin";

        public static void SetUser(this HttpContext context, int id, string username, string role)
        {
            context.Session.SetInt32(UserIdKey, id);
            context.Session.SetString(UsernameKey, username);
            context.Session.SetString(RoleKey, role);
        }

        public static void ClearUser(this HttpContext context) => context.Session.Clear();
    }
}
