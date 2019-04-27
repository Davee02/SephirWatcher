namespace DaHo.SephirWatcher.Web.Interfaces
{
    public interface IPasswordCipher
    {
        string Encrypt(string text);

        string Dencrypt(string text);
    }
}
