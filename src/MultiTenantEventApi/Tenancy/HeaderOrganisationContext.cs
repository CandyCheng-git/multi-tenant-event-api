namespace MultiTenantEventApi.Tenancy;

public sealed class HeaderOrganisationContext(IHttpContextAccessor accessor) : IOrganisationContext
{
    public Guid OrganisationId
    {
        get
        {
            var context = accessor.HttpContext ?? throw new InvalidOperationException("No active HTTP context.");
            if (!context.Request.Headers.TryGetValue("X-Organisation-Id", out var raw) ||
                !Guid.TryParse(raw.ToString(), out var id))
            {
                throw new InvalidOperationException("A valid X-Organisation-Id header is required.");
            }

            return id;
        }
    }
}
