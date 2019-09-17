using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Models
{
    public class ImageService : IImageService
    {
        public int CompareDependentImages(IFormFile dependentImage, IEnumerable<IFormFile> queuedImages)
        {
            //TODO: Send images to facial recognition service to determine if there is a match
            int matchedImageIndex = -1;
            bool isMatchFound = false;

            for (int i = 0; i < queuedImages.Count(); i++)
            {
                //bool isMatchFound = FacialRecognitionService.CompareImages(dependentImage, queuedImages);
                if (isMatchFound)
                {
                    matchedImageIndex = i;
                    break;
                }
            }

            return matchedImageIndex;
        }
    }
}
