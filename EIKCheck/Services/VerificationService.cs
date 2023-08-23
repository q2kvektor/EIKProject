namespace EIKCheck.Services;

public interface IVerficationService
{
    public int statusCode { get; set; }
    bool IsEIKValid(string eik);
}

public class VerificationService : IVerficationService
{
    public int statusCode { get; set; } = 0;

    public bool IsEIKValid(string eik)
    {
        if (eik == null || !IsDigitsOnly(eik))
        {
            this.statusCode = 1;
            return false;
        }

        if (eik.Length != 9) 
        { 
            this.statusCode = 2; 
            return false;
        }        
        
        return true;
    }

    public bool IsDigitsOnly(string str)
    {
        foreach (char c in str)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }
}
