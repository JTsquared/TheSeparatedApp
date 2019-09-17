using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public interface IImageService
    {
        int CompareDependentImages(IFormFile dependentImage, IEnumerable<IFormFile> queuedImages);
    }
}