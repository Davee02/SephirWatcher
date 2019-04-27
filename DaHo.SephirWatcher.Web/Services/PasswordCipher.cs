using DaHo.SephirWatcher.Web.Configuration;
using DaHo.SephirWatcher.Web.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace DaHo.SephirWatcher.Web.Services
{
    public class PasswordCipher : IPasswordCipher
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public PasswordCipher(IOptions<PasswordCipherOptions> optionsAccessor, IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
            Options = optionsAccessor.Value;
        }

        public PasswordCipherOptions Options { get; } //set only via Secret Manager


        public string Encrypt(string text)
        {
            var protector = _dataProtectionProvider.CreateProtector(Options.EncryptionKey);
            return protector.Protect(text);
        }

        public string Dencrypt(string text)
        {
            var protector = _dataProtectionProvider.CreateProtector(Options.EncryptionKey);
            return protector.Unprotect(text);
        }
    }
}
