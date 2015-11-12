using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EbookObjects.Models {
    partial class User {

        /// <summary>
        /// Hashes the password, either using the given salt, or creating a new salt.
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="salt">If null, a new salt is created, otherwise this is used</param>
        /// <param name="hash">The hashed password</param>
        private static void createPasswordHash(string password, ref string salt, out string hash) {
            var bsalt = salt == null ? new byte[8] : Convert.FromBase64String(salt);
            if (salt == null) {
                using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider()) {
                    // Fill the array with a random value.
                    rngCsp.GetBytes(bsalt);
                }
            }
            var k = new Rfc2898DeriveBytes(password, bsalt);
            if (salt == null) salt = Convert.ToBase64String(bsalt);
            hash = Convert.ToBase64String(k.GetBytes(16));
        }

        /// <summary>
        /// Checks whether the password matches, using the salt stored in the user record.
        /// </summary>
        /// <param name="testPassword"></param>
        /// <returns></returns>
        public bool CheckPassword(string testPassword) {
            string salt = Salt;
            string hash;
            createPasswordHash(testPassword, ref salt, out hash);
            return hash == Password;
        }

        /// <summary>
        /// Encrypts and writes the password into the record, saving the hash with a new salt.
        /// </summary>
        /// <param name="password"></param>
        public void SetPassword(string password) {
            string salt = null;
            string hash;
            createPasswordHash(password, ref salt, out hash);
            Password = hash;
            Salt = salt;
        }

    }
}
