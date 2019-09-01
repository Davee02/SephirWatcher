namespace DaHo.SephirWatcher.Web.Services.Abstraction
{
    public interface IStringCipher
    {
        string Encrypt(string text);

        string Decrypt(string text);
    }
}
