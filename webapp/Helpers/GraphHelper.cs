using System.Data;
using System.Text.Json.Nodes;
using Microsoft.Identity.Abstractions;
using webapp.Models;
using webapp.Models.EntityManagers;

namespace webapp.Helpers;

/// <summary>
/// Static helper class for dealing with MS Graph API requests.
/// </summary>
public static class GraphHelper
{
    /// <summary>
    /// Gets the current user as a <see cref="Student"/> or <see cref="Staff"/>. If the user is not in the DB, they will be added.
    /// </summary>
    /// <remarks>
    /// It is recommended to call this function in user-facing functions even if there is no need for the IUser.
    /// This is because it adds new users to the DB.
    /// </remarks>
    public static async Task<IUser?> GetUser(IDownstreamApi graphApi, ILogger? logger = null)
    {
        using HttpResponseMessage response = await graphApi
            .CallApiForUserAsync("GraphApi")
            .ConfigureAwait(false);
        string apiResult = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new HttpRequestException(
                $"Invalid status code in the HttpResponseMessage: {response.StatusCode}: {apiResult}"
            );
        }

        JsonNode? user = JsonNode.Parse(apiResult);
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

    private static void HandleFirstVisit(
        StudentManager manager,
        JsonNode user,
        ILogger? logger = null
    )
    {
        if (!manager.DbContains(user["id"]!.ToString()))
        {
            logger?.LogInformation($"User `{user["mail"]}` is new, adding to database.");
            Student newUser =
                new()
                {
                    Id = user["id"]!.ToString(),
                    FirstName = user["givenName"]!.ToString(),
                    LastName = user["surname"]!.ToString(),
                    Email = user["mail"]!.ToString(),
                };

            manager.Add(newUser);
        }
    }

    private static void HandleFirstVisit(
        StaffManager manager,
        JsonNode user,
        ILogger? logger = null
    )
    {
        if (!manager.DbContains(user["id"]!.ToString()))
        {
            logger?.LogInformation($"User `{user["mail"]}` is new, adding to database.");
            Staff newUser =
                new()
                {
                    Id = user["id"]!.ToString(),
                    FirstName = user["givenName"]!.ToString(),
                    LastName = user["surname"]!.ToString(),
                    Email = user["mail"]!.ToString()
                };

            manager.Add(newUser);

            RoleManager roleManager = new();
            roleManager.AssignDefaultRole(newUser);
        }
    }
}

