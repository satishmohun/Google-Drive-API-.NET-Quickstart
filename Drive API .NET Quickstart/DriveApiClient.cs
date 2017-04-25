using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drive_API.NET_Quickstart
{
    public class DriveApiClient
    {
        private static string credentialPath = string.Empty;
        private static string folderId = string.Empty;
        private static string basePath = string.Empty;

        private readonly string DownloadEndPoint = "https://www.googleapis.com/drive/v3/files/{0}?alt=media";

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        private static string ApplicationName = "Drive API .NET Quickstart";

        public DriveApiClient()
        {
            ReadConfiguration();
        }

        private void ReadConfiguration()
        {
            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["Credential"]))
                credentialPath = ConfigurationManager.AppSettings["Credential"];

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["FolderId"]))
                folderId = ConfigurationManager.AppSettings["FolderId"];

            if (!String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["BasePath"]))
                basePath = ConfigurationManager.AppSettings["BasePath"];
        }

        public void Run()
        {
            UserCredential credential;

            using (var stream =
                new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/drive-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            FilesResource.ListRequest listRequest = service.Files.List();
            // Read from a folder.
            listRequest.Q = string.Format("'{0}' in parents", folderId);
            listRequest.PageSize = 10;
            // Add fields that you need.
            listRequest.Fields = "nextPageToken, files(id, name, webContentLink, webViewLink)";

            // List files.
            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
                .Files;
            Console.WriteLine("Files:");
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    Console.WriteLine("{0} ({1}) {2} {3}", file.Name, file.Id, file.WebContentLink, file.WebViewLink);

                    // Download file.
                    DownloadFile(file, credential);
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            Console.Read();
        }

        private void DownloadFile(Google.Apis.Drive.v3.Data.File file, UserCredential credential)
        {
            string filePath = Path.Combine(basePath, file.Name);
            string downloadLink = string.Format(DownloadEndPoint, file.Id);

            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization, string.Format("Bearer {0}", credential.Token.AccessToken));
                client.DownloadFile(downloadLink, filePath);
            }
        }
    }
}
