using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    class ApiRest
    {
        private static ApiRest instance = null;
        private readonly HttpClient httpClient;
        private HttpResponseMessage httpResponse;

        private ApiRest(String uriApi, String authenticationString="")
        {
            httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("http://localhost/rest_mediatekdocuments/");

            Console.WriteLine(httpClient.BaseAddress);

            // prise en compte dans l'url de l'authentificaiton (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {


            if(instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        public JObject RecupDistant(string methode, string message)
        {
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            Console.WriteLine(httpClient.BaseAddress);
            Console.WriteLine(methode);
            Console.WriteLine(message);

            //Cas qui ne passait pas bien

            HttpContent content = null;

            // Vérifier si "message" contient un JSON (on suppose qu'il est après "{")
            int jsonStart = message.IndexOf('{');
            if (jsonStart != -1)
            {
                string jsonBody = message.Substring(jsonStart).Trim();
                message = message.Substring(0, jsonStart - 1).Trim(); // Supprime le JSON du message
                content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                Console.WriteLine("_________________________________");
                Console.WriteLine(content);
                Console.WriteLine(message);

            }

            // Envoi de la requête
            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    var request = new HttpRequestMessage(HttpMethod.Delete, message) { Content = content };
                    if (content != null)
                    {
                        request.Content = content;
                        request.Headers.TransferEncodingChunked = false; // Force l'inclusion du corps
                    }

                    httpResponse = httpClient.SendAsync(request).Result;

                    Console.WriteLine(httpResponse.Content);
                    Console.WriteLine(httpResponse.StatusCode);

                    break;
                default:
                    return new JObject();
            }

            Console.WriteLine(httpResponse.RequestMessage);

            return httpResponse.Content.ReadAsAsync<JObject>().Result;
        }


    }
}
