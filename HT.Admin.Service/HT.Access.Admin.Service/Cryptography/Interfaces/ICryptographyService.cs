namespace HT.Access.Admin.Service.Cryptography.Interfaces
{
    public interface ICryptographyService
    {
        byte[] Hash(string valueToHash);
    }
}
