using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using EIKCheck.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Metadata;

namespace EIKCheck.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IVerficationService _service;
        private readonly IFileGetterService _fileGetterService;
        public IndexModel(IVerficationService service, IFileGetterService fileService)
        {
            _service = service;
            _fileGetterService = fileService;
        }      

        [BindProperty]        
        public string EIKValue { get; set; } = "";

        public void OnGet()
        {
            
        }

        public void OnPost()
        {
            //can be made in a partial view if used in different pages
            if(!_service.IsEIKValid(EIKValue))
            {
                return;
            }
            
            string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            string url = $"https://portal.registryagency.bg/CR/api/Deeds/{EIKValue}?entryDate={date}T20%3A59%3A59.999Z&loadFieldsFromAllLegalForms=false";

            //pass the filetype you desire
            bool fileExists = _fileGetterService.CheckForFile(url, FileType.articles_of_association);

            //Download the file bytes if a file exists 
            if (fileExists)
            {
                var byteStream = _fileGetterService.GetFile();

                Response.Headers.Add("content-disposition", $"attachment; filename={EIKValue}.pdf");
                Response.Body.WriteAsync(byteStream, 0, byteStream.Length);                
            }
            else
            {
                _service.statusCode = 4;
                return;
            }            
        }
    }
}