namespace PharmacyInventoryDispensingSystem.WebApi.RateLimiting
{
    public static class RateLimitPolicyNames
    {
        public const string AnonymousAuth = nameof(AnonymousAuth);
        public const string AuthenticatedAuth =
            nameof(AuthenticatedAuth);
        public const string PaginatedQuery =
            nameof(PaginatedQuery);
    }
}
