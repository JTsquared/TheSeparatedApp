using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public interface IImageService
    {
        bool CompareDependentImages(byte[] imageByteArr1, byte[] imageByteArr2);

        int CompareImagesFromURI(string dependentImageURI, string[] queuedImageURIs);

        string SaveImageToStorage(IFormFile dependentImage);

        List<byte[]> GetImagesFromStorage(string[] imageLocation);

        byte[] GetImageFromStorage(string imageLocation);

        int RemoveImagesFromStorage(params string[] imageURLs);
    }
}