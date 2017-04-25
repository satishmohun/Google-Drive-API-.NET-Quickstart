using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Drive_API.NET_Quickstart
{
    class Program
    {

        static void Main(string[] args)
        {
            var DriveApiClient = new DriveApiClient();
            DriveApiClient.Run();
        }
    }
}
