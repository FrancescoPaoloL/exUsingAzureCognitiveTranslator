using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TextTranslation {
    class Program {
        static string subscriptionKey = "<mykey>";
        static string endpoint = "https://<mytranslatorservice>.cognitiveservices.azure.com/";
        static string region = "<myregion>";
        
        static async Task Main(string[] args) {
            await OutputTranslatedResult();
        }

        static async Task OutputTranslatedResult() {
            // Set the text to be translated and the language
            string inputFile = "./input/DerProzess.txt";
            string text = await File.ReadAllTextAsync(inputFile);
            string toLanguage = "en";

            // Set the request URL and headers
            string url = $"{endpoint}/translator/text/v3.0/translate?to={toLanguage}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", region);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Create the request body
            var body = new object[] { new { Text = text } };
            var requestBody = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8, "application/json");
            request.Content = requestBody;

            // Send the translation request
            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Get the translated text from the response
                var responseBody = await response.Content.ReadAsStringAsync();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                string translation = result[0].translations[0].text;
                Console.WriteLine($"Translated text:\n {translation}");
            }
        }
    }
}
