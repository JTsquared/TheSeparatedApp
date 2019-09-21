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

        public void Test()
        {

        }

        public async Task<Uri> SaveToBlobStorage(IFormFile dependentImage)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lostchildimagestorage;AccountKey=LqoAeQUEq9I8+ba+O4TxsH0BgEvzgA073+XeFdFAG9z2z7bXCMkquaZTttOT44T1uboZPCDRnhLe6YUJA3lf4w==;EndpointSuffix=core.windows.net");
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("Images");

            var blob = await container.GetBlobReferenceFromServerAsync(dependentImage.FileName);
            await blob.UploadFromStreamAsync(dependentImage.OpenReadStream());

            return blob.Uri;

            //string imageName = Guid.NewGuid().ToString() + "_" + dependentImage.FileName;

            //CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);

            //blockBlob.Properties.ContentType = dependentImage.ContentType;
            //blockBlob.UploadFromStreamAsync(dependentImage.OpenReadStream());

        }
    }
}
