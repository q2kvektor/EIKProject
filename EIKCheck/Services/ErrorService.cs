using Microsoft.Net.Http.Headers;

namespace EIKCheck.Services;

//Add error messages here
public enum Errors : int
{
    Unknown_Error_Occured = 0,
    Invalid_EIK = 1,
    EIK_Too_Short = 2,
    Company_Not_Found = 3,
    No_File_Found = 4
}

public interface IErrorService
{
    //return error message based on int value
    string ReturnError(int value = 0);
}

public class ErrorService : IErrorService
{
    public string ReturnError(int value)
    {
        Errors error = (Errors)value;
        string errorResult = error.ToString().Replace("_", " ");
        return errorResult;
    }
}
