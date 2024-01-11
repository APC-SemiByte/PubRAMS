using System.Text.Json.Nodes;
using Microsoft.Identity.Abstractions;
using webapp.Models;
using webapp.Models.EntityManagers;

namespace webapp.Helpers;

/// <summary>
/// Helper class for dealing with MS Graph API requests.
/// </summary>
public class GraphHelper
{
    private enum AuthMode
    {
        Unrestricted,
        Student,
        Staff,
        Role
    }

    private AuthMode Mode { get; set; } = AuthMode.Unrestricted;
    private List<string>? Roles { get; set; }

    public GraphHelper StudentOnly()
    {
        Mode = AuthMode.Student;
        return this;
    }

    public GraphHelper StaffOnly()
    {
        Mode = AuthMode.Staff;
        return this;
    }

    public GraphHelper RolesOnly(List<string> roles)
    {
        Mode = AuthMode.Role;
        Roles = roles;
        return this;
    }

    /// <summary>
    /// Gets the current user as a <see cref="Student"/> or <see cref="Staff"/>. If the user is not in the DB, they will be added.
    /// </summary>
    /// <remarks>
    /// It is recommended to call this function in user-facing functions even if there is no need for the IUser.
    /// This is because it adds new users to the DB.
    /// </remarks>
    public async Task<IUser?> GetUser(IDownstreamApi graphApi, ILogger? logger = null)
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
            if (Mode is AuthMode.Staff || Mode is AuthMode.Role)
            {
                return null;
            }

            StudentManager studentManager = new();
            HandleFirstVisit(studentManager, user, logger);

            return studentManager.GetById(user["id"]!.ToString());
        }

        if (user["mail"]!.ToString().Contains("@apc.edu.ph"))
        {
            if (Mode is AuthMode.Student)
            {
                return null;
            }

            StaffManager staffManager = new();
            HandleFirstVisit(staffManager, user, logger);

            Staff? staff = staffManager.GetById(user["id"]!.ToString());
            if (staff == null || Mode is AuthMode.Unrestricted || Mode is AuthMode.Staff)
            {
                return staff;
            }

            List<Role> roles = staffManager.GetRoles(staff);
            bool valid = roles.Select(e => e.Name).Intersect(Roles!).Any();
            return valid ? staff : null;
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

