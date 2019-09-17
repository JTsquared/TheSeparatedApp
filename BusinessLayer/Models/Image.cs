using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace LostChildApp.Models
{
    public class Image
    {
        public int ImgID { get; set; }
        public string Title { get; set; }
        public string ImgURL { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
