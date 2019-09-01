using DaHo.SephirWatcher.Web.Configuration;
using DaHo.SephirWatcher.Web.Services.Abstraction;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DaHo.SephirWatcher.Web.Services
{
    public class StringCipher : IStringCipher
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public StringCipher(IOptions<PasswordCipherOptions> optionsAccessor, IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            Options = optionsAccessor.Value;
        }

        public PasswordCipherOptions Options { get; } //set only via Secret Manager


        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var protector = _dataProtectionProvider.CreateProtector(Options.EncryptionKey);
            return protector.Protect(text);
        }

        public string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var protector = _dataProtectionProvider.CreateProtector(Options.EncryptionKey);
            return protector.Unprotect(text);
        }
    }
}
