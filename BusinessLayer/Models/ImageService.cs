using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace BusinessLayer.Models
{
    public class ImageService : IImageService
    {
        public IFormFile[] GetImagesFromStorage(string[] imageURIs)
        {
            //TODO: convert blob data from uri path to IFormFile
            return new IFormFile[0];
        }

        public IFormFile GetImageFromStorage(string imageURI)
        {
            //TODO: convert blob data from uri path to IFormFile
            IFormFile image = null;
            return image;
        }

        public int CompareImagesFromURI(string dependentImageURI, string[] queuedImageURIs)
        {
            var dependentImage = GetImageFromStorage(dependentImageURI);
            var queuedImages = GetImagesFromStorage(queuedImageURIs);

            return CompareDependentImages(dependentImage, queuedImages);
        }

        public bool CompareDependentImages(IFormFile image1, IFormFile image2)
        {
            //TODO: make call to azure facial recognition service then evalute the percentage to determine if match is found
            return false;
        }

        public int CompareDependentImages(IFormFile dependentImage, IFormFile[] queuedImages)
        {
            //TODO: Send images to facial recognition service to determine if there is a match
            int matchedImageIndex = -1;
            bool isMatchFound = false;

            for (int i = 0; i < queuedImages.Count(); i++)
            {
                //bool isMatchFound = FacialRecognitionService.CompareImages(dependentImage, queuedImages);
                isMatchFound = CompareDependentImages(dependentImage, queuedImages[i]);
                if (isMatchFound)
                {
                    matchedImageIndex = i;
                    break;
                }
            }

            return matchedImageIndex;
        }

        public async Task<String> SaveImageToStorage(IFormFile dependentImage)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lostchildimagestorage;AccountKey=LqoAeQUEq9I8+ba+O4TxsH0BgEvzgA073+XeFdFAG9z2z7bXCMkquaZTttOT44T1uboZPCDRnhLe6YUJA3lf4w==;EndpointSuffix=core.windows.net");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            string imageName = Guid.NewGuid().ToString() + "_" + dependentImage.FileName;

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);
            blockBlob.Properties.ContentType = dependentImage.ContentType;
            await blockBlob.UploadFromStreamAsync(dependentImage.OpenReadStream());

            return await Task.FromResult(blockBlob.Uri.ToString());

        }
    }
}
