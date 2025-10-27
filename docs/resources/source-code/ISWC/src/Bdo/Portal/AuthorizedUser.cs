namespace SpanishPoint.Azure.Iswc.Bdo.Portal
{
    public class AuthorizedUser
    {
        public AuthorizedUser(string userId, string agencyId, string isMangeRoles)
        {
            UserId = userId;
            AgencyId = agencyId;
            bool.TryParse(isMangeRoles, out IsMangeRoles);
        }

        public readonly string UserId;
        public readonly string AgencyId;
        public readonly bool IsMangeRoles;
    }
}
