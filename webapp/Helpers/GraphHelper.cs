using Microsoft.Identity.Abstractions;
using System.Text.Json.Nodes;
using webapp.Models.EntityManagers;
using webapp.Models;

namespace webapp.Helpers;

/// <summary>
/// Static helper class for dealing with MS Graph API requests.
/// </summary>
public static class GraphHelper
{
    /// <summary>
    /// Gets the current user as an <see cref="IUser"/> from the DB. If the user is not in the DB, they will be added.
    /// </summary>
    /// <remarks>
    /// It is recommended to call this function in user-facing functions even if there is no need for the IUser.
    /// This is because it adds new users to the DB.
    /// </remarks>
    public static async Task<IUser?> GetUser(IDownstreamApi graphApi, ILogger? logger = null)
    {
        using var response = await graphApi.CallApiForUserAsync("GraphApi").ConfigureAwait(false);
        var apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {apiResult}");
        }

        var user = JsonNode.Parse(apiResult);
        if (user!["mail"]!.ToString().Contains("@student.apc.edu.ph"))
        {
            StudentManager studentManager = new();
            HandleFirstVisit(studentManager, user, logger);

            return studentManager.GetById(user["id"]!.ToString());
        }

        if (user["mail"]!.ToString().Contains("@apc.edu.ph"))
        {
            StaffManager staffManager = new();
            HandleFirstVisit(staffManager, user, logger);

            return staffManager.GetById(user["id"]!.ToString());
        }

        return null;
    }

    private static void HandleFirstVisit<T>(IUserManager<T> manager, JsonNode user, ILogger? logger = null) where T : IUser, new()
    {
        if (!manager.DbContains(user["id"]!.ToString()))
        {
            logger?.LogInformation($"User `{user["mail"]!.ToString()}` is new, adding to database.");
            T newStaff = new()
            {
                Id = user["id"]!.ToString(),
                FirstName = user["givenName"]!.ToString(),
                LastName = user["surname"]!.ToString(),
                Email = user["mail"]!.ToString(),
            };

            manager.Add(newStaff);
        }
    }
}
