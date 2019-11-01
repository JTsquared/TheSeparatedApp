using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LostChildApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;

namespace BusinessLayer.Models
{
    public class ImageService : IImageService
    {
        private CloudStorageAccount _cloudStorageAccount;
        private CloudBlobClient _cloudBlobClient;
        private CloudBlobContainer _cloudBlobContainer;
        private IFaceClient _faceClient;
        //TODO: move these constants into a configuration file or key vault
        const string FACE_API_SUBSCRIPTION_KEY = "8159e76c158f481da41dd022b3c5a287";
        const string FACE_API_URI = "https://separatedappfaceapi.cognitiveservices.azure.com";
        const double MATCH_CONFIDENCE_MIN = .50d;

        //// Set your environment variables with these with the names below. Close and reopen your project for changes to take effect.
        //string SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY");
        //string ENDPOINT = $"https://{Environment.GetEnvironmentVariable("FACE_REGION")}.api.cognitive.microsoft.com";
        //string ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT");
        //// The Snapshot example needs its own 2nd client, since it uses two different regions.
        //string TARGET_SUBSCRIPTION_KEY = Environment.GetEnvironmentVariable("FACE_SUBSCRIPTION_KEY2");
        //string TARGET_ENDPOINT = $"https://{Environment.GetEnvironmentVariable("FACE_REGION2")}.api.cognitive.microsoft.com";
        //string TARGET_ENDPOINT = Environment.GetEnvironmentVariable("FACE_ENDPOINT2");
        //// Grab your subscription ID, from any resource in Azure, from the Overview page (all resources have the same subscription ID). 
        //Guid AZURE_SUBSCRIPTION_ID = new Guid(Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID"));


        public ImageService()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=lostchildimagestorage;AccountKey=LqoAeQUEq9I8+ba+O4TxsH0BgEvzgA073+XeFdFAG9z2z7bXCMkquaZTttOT44T1uboZPCDRnhLe6YUJA3lf4w==;EndpointSuffix=core.windows.net");
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
            _cloudBlobContainer = _cloudBlobClient.GetContainerReference("images");
            _faceClient = new FaceClient(new ApiKeyServiceClientCredentials(FACE_API_SUBSCRIPTION_KEY));
            _faceClient.Endpoint = FACE_API_URI;
        }

        public ImageService(CloudStorageAccount cloudStorageAccount, CloudBlobClient cloudBlobClient, CloudBlobContainer cloudBlobContainer)
        {
            _cloudStorageAccount = cloudStorageAccount;
            _cloudBlobClient = cloudBlobClient;
            _cloudBlobContainer = cloudBlobContainer;
        }

        public List<byte[]> GetImagesFromStorage(string[] imageURIs)
        {
            List<byte[]> imageByteList = new List<byte[]>();

            for(int i = 0; i < imageURIs.Length; i++)
            {
                byte[] imageByte = GetImageFromStorage(imageURIs[i]);
                imageByteList.Add(imageByte);
            }

            return imageByteList;
        }

        public byte[] GetImageFromStorage(string imageURI)
        {
            //TODO: convert blob data from uri path to byte array
            MemoryStream memoryStream = new MemoryStream();

            CloudBlob blob = _cloudBlobContainer.GetBlobReference(imageURI);

            var imgStream = blob.OpenRead();
            BinaryReader binaryReader = new BinaryReader(imgStream);
            return binaryReader.ReadBytes((int)imgStream.Length);
        }

        public int CompareImagesFromURI(string dependentImageURI, string[] queuedImageURIs)
        {
            var dependentImage = GetImageFromStorage(dependentImageURI);
            var queuedImages = GetImagesFromStorage(queuedImageURIs);

            return CompareDependentImages(dependentImage, queuedImages);
        }

        public bool CompareDependentImages(byte[] imageByteArr1, byte[] imageByteArr2)
        {
            List<Guid> faceIDList1 = new List<Guid>();
            List<Guid> faceIDList2 = new List<Guid>();
            double matchConfidence = 0d;

            //make a call to Face - Detect for each image. This service call has to be made in order for the face api to generate a face ID for the image.
            IList<DetectedFace> detectedFaces1 = GetFaceID(imageByteArr1);
            IList<DetectedFace> detectedFaces2 = GetFaceID(imageByteArr2);

            //for all identified faces in each image add the faceID to the faceID to the list
            AddFaceIDsToList(faceIDList1, detectedFaces1);
            AddFaceIDsToList(faceIDList2, detectedFaces2);

            //if there is at least one identified face in each image make call to Face - Verify API
            if(faceIDList1.Count > 0 && faceIDList2.Count > 0)
            {
                //make another call to Face - Verify uri with both images to see if they are a match
                matchConfidence = GetMatchConfidence(faceIDList1, faceIDList2);
            }

            return matchConfidence > MATCH_CONFIDENCE_MIN ? true : false;
        }

        public void AddFaceIDsToList(List<Guid> faceIDList, IList<DetectedFace> detectedFace1)
        {
            if (detectedFace1 != null)
            {
                List<Guid> allFaceIDsInImage = detectedFace1.Select(x => x.FaceId.Value).ToList();

                if (allFaceIDsInImage.Count > 0) faceIDList.AddRange(allFaceIDsInImage);
            }
        }

        public async Task<double> GetMatchConfidenceAsync(List<Guid> faceIDList1, List<Guid> faceIDList2)
        {
            double matchConfidence = 0d;
         
            foreach(Guid firstFaceID in faceIDList1)
            {
                foreach(Guid secondFaceID in faceIDList2)
                {
                    matchConfidence = await GetMatchConfidenceAsync(firstFaceID, secondFaceID);
                    if (matchConfidence > MATCH_CONFIDENCE_MIN) return matchConfidence;
                }
            }

            return matchConfidence;
        }

        public async Task<double> GetMatchConfidenceAsync(Guid faceID1, Guid faceID2)
        {
            //Make call to Azure Face Verify API and get match confidence from response.
            var verifyResponse = await _faceClient.Face.VerifyFaceToFaceAsync(faceID1, faceID2);
            return verifyResponse.Confidence;
        }

        public double GetMatchConfidence(List<Guid> faceIDlist1, List<Guid> faceIDlist2)
        {
            var response = GetMatchConfidenceAsync(faceIDlist1, faceIDlist2);
            return response.Result;
        }

        public double GetMatchConfidence(Guid faceID1, Guid faceID2)
        {
            _faceClient.Endpoint = FACE_API_URI + "/verify";
            var response = GetMatchConfidenceAsync(faceID1, faceID2);
            return response.Result;
        }

        public async Task<IList<DetectedFace>> GetFaceIDAsync(string imageURI)
        {
            IList<FaceAttributeType> faceAttributes = new FaceAttributeType[]
            {
                FaceAttributeType.Gender, FaceAttributeType.Age,
                FaceAttributeType.Smile, FaceAttributeType.Emotion,
                FaceAttributeType.Glasses, FaceAttributeType.Hair,
                FaceAttributeType.HeadPose, FaceAttributeType.FacialHair,
                FaceAttributeType.Makeup, FaceAttributeType.Occlusion,
                FaceAttributeType.Noise, FaceAttributeType.Blur,
                FaceAttributeType.Accessories, FaceAttributeType.Exposure
            };

            //_faceClient.Endpoint = "https://separatedappfaceapi.cognitiveservices.azure.com";

            CloudBlob blob = _cloudBlobContainer.GetBlobReference(imageURI);

            var response = await _faceClient.Face.DetectWithUrlAsync(blob.Uri.ToString());
            return response;
        }

        public async Task<IList<DetectedFace>> GetFaceIDAsync(byte[] imageByteArr)
        {
            IList<FaceAttributeType> faceAttributes = new FaceAttributeType[]
            {
                FaceAttributeType.Gender, FaceAttributeType.Age,
                FaceAttributeType.Smile, FaceAttributeType.Emotion,
                FaceAttributeType.Glasses, FaceAttributeType.Hair,
                FaceAttributeType.HeadPose, FaceAttributeType.FacialHair,
                FaceAttributeType.Makeup, FaceAttributeType.Occlusion,
                FaceAttributeType.Noise, FaceAttributeType.Blur,
                FaceAttributeType.Accessories, FaceAttributeType.Exposure
            };

            //_faceClient.Endpoint = "https://separatedappfaceapi.cognitiveservices.azure.com";

            var response = await _faceClient.Face.DetectWithStreamAsync(new MemoryStream(imageByteArr), true, false, faceAttributes);
            return response;
        }

        public IList<DetectedFace> GetFaceID(byte[] imageByteArr)
        {
            var response = GetFaceIDAsync(imageByteArr);
            return response.Result;
        }

        public int CompareDependentImages(byte[] reportImgByteArr, List<byte[]> storedImgsByteArrList)
        {
            //TODO: Send images to facial recognition service to determine if there is a match
            int matchedImageIndex = -1;
            bool isMatchFound = false;

            for (int i = 0; i < storedImgsByteArrList.Count(); i++)
            {
                //bool isMatchFound = FacialRecognitionService.CompareImages(dependentImage, queuedImages);
                isMatchFound = CompareDependentImages(reportImgByteArr, storedImgsByteArrList[i]);
                if (isMatchFound)
                {
                    matchedImageIndex = i;
                    break;
                }
            }

            return matchedImageIndex;
        }

        public string SaveImageToStorage(IFormFile dependentImage)
        {
            var response = SaveImageToStorageAsync(dependentImage);
            return response.Result;
        }

        public async Task<String> SaveImageToStorageAsync(IFormFile dependentImage)
        {
            string imageName = Guid.NewGuid().ToString() + "_" + dependentImage.FileName;

            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(imageName);
            blockBlob.Properties.ContentType = dependentImage.ContentType;
            await blockBlob.UploadFromStreamAsync(dependentImage.OpenReadStream());

            return await Task.FromResult(blockBlob.Uri.Segments[2]);
        }

        public int RemoveImagesFromStorage(params string[] imageURLs)
        {
            throw new NotImplementedException();
        }
    }
}
