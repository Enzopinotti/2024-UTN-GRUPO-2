using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Mapping
{
    public static class RegistrationMapper
    {
        public static User ToUser(UserForRegistrationDto source)
        {
            return new User
            {
                FirstName = source.FirstName,
                LastName = source.LastName,
                Email = source.Email,
                UserName = source.Email
            };
        }
    }
}
