using BCrypt.Net;

namespace helloworld.Services
{
    public class SecurityService
    {
        /// <summary>
        /// Hashes a password using BCrypt.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <returns>The hashed password.</returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a password against a hash.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <param name="hash">The hash to verify against.</param>
        /// <returns>True if the password matches the hash, false otherwise.</returns>
        public bool VerifyPassword(string password, string hash)
        {
            try 
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // In case existing passwords are plain text, fall back to simple comparison (Migration only)
                // WARNING: Remove this in production after migration
                return password == hash;
            }
        }
    }
}
