using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public interface IImageService
    {
        int CompareDependentImages(IFormFile dependentImage, IFormFile[] queuedImages);

        bool CompareDependentImages(IFormFile image1, IFormFile image2);

        int CompareImagesFromURI(string dependentImageURI, string[] queuedImageURIs);

        Task<String> SaveImageToStorage(IFormFile dependentImage);

        IFormFile[] GetImagesFromStorage(string[] imageLocation);

        IFormFile GetImageFromStorage(string imageLocation);
    }
}