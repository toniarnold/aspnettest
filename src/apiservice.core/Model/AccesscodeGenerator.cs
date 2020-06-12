using System;
using System.Security.Cryptography;

namespace apiservice.Model
{
    public class AccesscodeGenerator
    {
        public static string New(int length)
        {
            using (var generator = RandomNumberGenerator.Create())
            {
                byte[] randoms = new byte[8];   // // ends with ’551’616 -> digits above slightly underrepresented
                generator.GetBytes(randoms);
                var number = BitConverter.ToUInt64(randoms);
                var range = Math.Pow(10, (ulong)length);            // 1000 for length 3
                var digits = number % range;                        //   12
                return (range + digits).ToString().Substring(1);    // 1012 -> "012"
            }
        }
    }
}