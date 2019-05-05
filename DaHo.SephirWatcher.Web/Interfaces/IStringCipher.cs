namespace DaHo.SephirWatcher.Web.Interfaces
{
    public interface IStringCipher
    {
        string Encrypt(string text);

        string Decrypt(string text);
    }
}
