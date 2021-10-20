using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatAppWSServer.PrivateServices
{
    public class RandomService
    {
        private Random _random;
        public RandomService()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        public string SecureRandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            var result = new StringBuilder();
            
            byte[] uintBuffer = new byte[sizeof(uint)];

            while (length-- > 0)
            {
                randomNumberGenerator.GetBytes(uintBuffer);
                var num = BitConverter.ToUInt32(uintBuffer, 0);
                result.Append(valid[(int)(num % (uint)valid.Length)]);
            }

            return result.ToString();
        }
    }
}