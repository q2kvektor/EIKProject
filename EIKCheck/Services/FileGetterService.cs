using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EIKCheck.Services;

//add more in the future if needed
public enum FileType : int
{
    articles_of_association = 451,    
}


public interface IFileGetterService
{
    public bool FileExists { get; set; }

    public string DownloadURL { get; set; } 

    bool CheckForFile(string url, FileType type);
    byte[] GetFile();
}

public class FileGetterService : IFileGetterService
{
    private readonly IHttpClientService _service;
    private readonly IVerficationService _verficationService;
    public bool FileExists { get; set; } = false;

    public string DownloadURL { get; set; } = "";
    public FileGetterService(IHttpClientService service, IVerficationService verificaservice)
    {
        _service = service;
        _verficationService = verificaservice;
    }
   
    public bool CheckForFile(string url, FileType type)
    {  
        string result = _service.GetHTML(url);
        var fields = ExtraAllSubstring(result, "groupID\":", "}");
        int fieldType = (int) type;
        
        //add different documents here, some might require different logic
        if(fieldType == 451)
        {
            var field = fields.Where(x => x.Contains($"{fieldType.ToString()},")).First();
            this.DownloadURL = ExtraSubstring(field, "DocumentAccess/", "' ");            
        }      

        if(this.DownloadURL != "error" && _verficationService.statusCode == 100) this.FileExists = true;       

        return this.FileExists;
    }

    public byte[] GetFile()
    {
        if(this.FileExists && !string.IsNullOrWhiteSpace(this.DownloadURL))
        {
            var bytes = _service.GetBytes($"https://portal.registryagency.bg/CR/api/Documents/{this.DownloadURL}");

            return bytes;
        }

        return new byte[] { };
    }

    public string ExtraSubstring(string str, string from, string to)
    {
        string result = "";

        try
        {
            int pFrom = str.IndexOf(from) + from.Length;
            int pTo = str.LastIndexOf(to);

            result = str.Substring(pFrom, pTo - pFrom);
        }
        catch (Exception ex)
        {
            result = "error";
            _verficationService.statusCode = 0;
        }

        //OK code
        _verficationService.statusCode = 100;
        return result;
    }

    public List<string> ExtraAllSubstring(string str, string from, string to)
    {
        string text = str.ToLower();
        string start = from.ToLower();
        string end = to.ToLower();
        List<string> result = new List<string>();
        int index_start = 0, index_end = 0;
        bool exit = false;

        while (!exit)
        {
            index_start = text.IndexOf(start);
            if (index_start != -1)
            {
                str = str.Substring(index_start + start.Length);
                text = text.Substring(index_start + start.Length);
            }

            index_end = text.IndexOf(end);
            if (index_start != -1 && index_end != -1)
            {
                result.Add(str.Substring(0, index_end).Trim());
                str = str.Substring(index_end + end.Length);
                text = text.Substring(index_end + end.Length);

            }
            else
                exit = true;
        }
        
        return result;        
    }
}
