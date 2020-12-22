using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/security")]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService _hashService;
        public SecurityController(IDataProtectionProvider protectionProvider,
            HashService hashService)
        {
            _protector = protectionProvider.CreateProtector("value_secret_and_unique");
            _hashService = hashService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Name Surname";
            string encryptedText = _protector.Protect(plainText);
            string decryptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        /// <summary>
        /// This method creates data protector for encryption that expires data after some time.
        /// It is a way to limit encryption possibilities by time.
        /// </summary>
        /// <returns></returns>
        [HttpGet("TimeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var protecterTimeBound = _protector.ToTimeLimitedDataProtector();

            string plainText = "Name Surname";
            string encryptedText = protecterTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));
            await Task.Delay(6000);
            string decryptedText = protecterTimeBound.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decryptedText });
        }

        [HttpGet("hash")]
        public IActionResult GetHash()
        {
            var plainText = "Name Surname";
            var hashResult1 = _hashService.Hash(plainText);
            var hashResult2 = _hashService.Hash(plainText);
            return Ok(new { plainText, hashResult1, hashResult2 });
        }
    }
}
