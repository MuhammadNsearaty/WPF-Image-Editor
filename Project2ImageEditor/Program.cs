using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using RestSharp.Extensions;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Collections.Concurrent;

namespace Project2ImageEditor{
    public static class Comunicator
    {
        public static IRestResponse<AuthToken> SendLoginRequest(User user)
        {

            var request = new RestRequest("/api/login/", DataFormat.Json);
            request.AddParameter("username", user.Email);
            request.AddParameter("password", user.Password);
            var response = ImageEditor.client.Post<AuthToken>(request);
            return response;
        }

        public static IRestResponse SendSignupRequest(User user)
        {

            var request = new RestRequest("/api/profile/", DataFormat.Json);
            request.AddParameter("email", user.Email);
            request.AddParameter("name", user.Name);
            request.AddParameter("password", user.Password);
            var response = ImageEditor.client.Post(request);
            return response;
        }

        public static IRestResponse SendFeedItem(User user, int scale, string path)
        {
            var request = new RestRequest("/api/feed/", DataFormat.Json);
            request.AddParameter("scaling_factor", scale);
            request.AddFile("upload_image", path);
            var response = ImageEditor.client.Post(request);
            return response;
        }

        public static IRestResponse<List<FeedItem>> GetFeedItems(User user)
        {
            var request = new RestRequest("/api/feed/", DataFormat.Json);
            request.AddHeader("Authorization", user.AuthToken.GetHeaderValue());
            var response = ImageEditor.client.Get<List<FeedItem>>(request);
            return response;
        }

        public static FileStream DownLoadOriginalImage(User user, FeedItem feedItem)
        {
            RestClient restClient = new RestClient(feedItem.originalImageURL);
            var request = new RestRequest("#");
            request.AddHeader("Authorization", user.AuthToken.GetHeaderValue());
            var response = restClient.Head(request);
            if (response.IsSuccessful)
            {
                request = new RestRequest("#");
                request.AddHeader("Authorization", user.AuthToken.GetHeaderValue());
                var fileBytes = restClient.DownloadData(request);
                Directory.CreateDirectory(@"..\..\temp");
                var filePath = Path.Combine(@"..\..\temp\", new Uri(feedItem.originalImageURL).Segments.Last());
                Console.WriteLine("SR filepath" + filePath);
                File.WriteAllBytes(filePath, fileBytes);
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            else
                throw new Exception(response.StatusCode.ToString());
        }

        public static FileStream DownLoadEnhancedImage(User user, FeedItem feedItem)
        {
            var request = new RestRequest("api/feed/check/");
            request.AddHeader("Authorization", user.AuthToken.GetHeaderValue());
            request.AddQueryParameter("task_id", feedItem.ID.ToString());
            var response = ImageEditor.client.Get(request);
            if (response.IsSuccessful)
            {
                JsonObject json = JsonConvert.DeserializeObject<JsonObject>(ImageEditor.client.Get(request).Content);
                if ((bool)json["state"])
                {
                    var restClient = new RestClient(json["url"].ToString());
                    request = new RestRequest("#");
                    request.AddHeader("Authorization", user.AuthToken.GetHeaderValue());
                    var fileBytes = restClient.DownloadData(request);
                    Directory.CreateDirectory(@"..\..\temp");
                    var filePath = Path.Combine(@"..\..\temp\", "SR" + new Uri(json["url"].ToString()).Segments.Last());
                    Console.WriteLine("SR filepath" + filePath);
                    File.WriteAllBytes(filePath, fileBytes);
                    return new FileStream(filePath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    return null;
                }
            }
            else
                throw new Exception(response.StatusCode.ToString());
        }
    }

    public class AuthToken
    {
        public string token { set; get; }

        public override string ToString()
        {
            return "token: " + token;
        }

        public string GetHeaderValue()
        {
            return "Token " + token;
        }
    }

    public class User
    {
        public long Id { set; get; }
        public string Email { set; get; }
        public string Name { set; get; }
        public string Password { set; get; }
        public AuthToken AuthToken { set; get; }

        public override string ToString()
        {
            if (AuthToken == null)
                return "username: " + Name + "\n" + "password: " + Password;
            return "ID: " + Id + "\nEmail: " + Email + "\nusername: " + Name + "\npassword: " + Password + "\n" + AuthToken.ToString();

        }

        public void Login()
        {
            var response = Comunicator.SendLoginRequest(this);
            if (response.IsSuccessful)
            {
                AuthToken = response.Data;
                var request1 = new RestRequest("/api/profile/", DataFormat.Json);
                request1.AddQueryParameter("search", Email);
                List<JsonObject> list = JsonConvert.DeserializeObject<List<JsonObject>>(ImageEditor.client.Get(request1).Content);
                Name = (string)list[0]["name"];
                Id = (long)list[0]["id"];
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public void Signup()
        {
            var response = Comunicator.SendSignupRequest(this);
            if (response.IsSuccessful)
            {
                Login();
            }
            throw new Exception(response.StatusCode.ToString());
        }
    }

    public class FeedItem
    {
        [JsonProperty("id")]
        public long ID { set; get; }
        [JsonProperty("user_profile")]
        public long UserProfileID { set; get; }
        [JsonProperty("upload_image")]
        public string originalImageURL { set; get; }
        public DateTime CreatedOn { set; get; }
        public int ScalingFactor { set; get; }
        public bool FinishedProcessing { set; get; }

        public override string ToString()
        {
            return "ID: " + ID + "\nUserProfileID: " + UserProfileID + "\nCreatedOn: " + CreatedOn.ToString() +
                "\nScalingFactor: " + ScalingFactor + "\nFinishedProcessing: " + FinishedProcessing + "\nGet Original Image @ " + originalImageURL;
        }

        public Bitmap GetOriginalImage()
        {
            return null;
        }
    }

    public class ImageEditor
    {
        public static readonly RestClient client = new RestClient("http://127.0.0.1:8000");

        static ImageEditor()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            RestSharp.Serializers.NewtonsoftJson.RestClientExtensions.UseNewtonsoftJson(client, new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = contractResolver });
        }

    }

    abstract public class DrawKit
    {
        abstract public void OnMouseClick();
        abstract public void OnMouseMove();
    }

    abstract public class Pen : DrawKit
    {

    }

    abstract public class ShapePen : Pen
    {

    }

    public class LinePen : Pen
    {
        public override void OnMouseClick()
        {
            throw new NotImplementedException();
        }

        public override void OnMouseMove()
        {
            throw new NotImplementedException();
        }
    }

    public class RecanglePen : ShapePen
    {
        public override void OnMouseClick()
        {
            throw new NotImplementedException();
        }

        public override void OnMouseMove()
        {
            throw new NotImplementedException();
        }
    }

    public class CirclePen : ShapePen
    {
        public override void OnMouseClick()
        {
            throw new NotImplementedException();
        }

        public override void OnMouseMove()
        {
            throw new NotImplementedException();
        }
    }

    public class SelectTool : RecanglePen
    {

    }

    public class ImageProcessor
    {
        static void PerformSR(int scale, string input, string output)
        {
            var psi = new ProcessStartInfo
            {
                FileName = @"..\..\venv\Scripts\python.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                RedirectStandardError = true,
            };
            var script = @"..\..\EDSR\main.py";
            psi.Arguments = $"{script} --scale {scale} --input {input} --output {output}";

            var process = Process.Start(psi);

            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
            while (!process.StandardError.EndOfStream)
            {
                var error = process.StandardError.ReadLine();
                Console.Error.WriteLine(error);
            }
        }

        static System.Drawing.Bitmap PerformSR(int scale, System.Drawing.Bitmap bmp)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "tmp");
            Directory.CreateDirectory(path);

            string enhanced_path = Path.Combine(path, $"tmp_image_x{scale}.png");
            path = Path.Combine(path, "tmp_image.png");

            bmp.Save(path, ImageFormat.Png);

            PerformSR(scale, path, enhanced_path);

            return new Bitmap(Image.FromFile(enhanced_path));
        }

    }

    class Program {

        static void Main1(string[] args) {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(System.Drawing.Image.FromFile(@"..\..\Eh8bK2lXYAAPwSx.png"));
            {
                User user = new User { Email = "adnankattan9@gmail.com", Password = "password" };
                try {
                    Console.WriteLine("---------------------------");
                    user.Login();
                    Console.WriteLine(user);
                    //Console.WriteLine(user);
                    List<FeedItem> feedItems = Comunicator.GetFeedItems(user).Data;
                    foreach (FeedItem item in feedItems) {
                        Console.WriteLine(item);
                        Console.WriteLine("------------------------------------------------");
                    }

                    //Console.WriteLine(user.AuthToken);
                    //DownLoadOriginalImage(user, feedItems[0]);
                    //DownLoadEnhancedImage(user, feedItems[0]);
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            Console.ReadKey();
        }

        

    }
}
