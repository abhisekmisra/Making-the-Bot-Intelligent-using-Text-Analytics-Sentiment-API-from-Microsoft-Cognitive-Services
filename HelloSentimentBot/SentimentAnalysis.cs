using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.IO;

namespace HelloSentimentBot
{
    public class SentimentAnalysis
    {
        //Uri to Text Analytics sentiment API 
        const string uriSentiment = @"https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        public static async Task<string> DoSentimentAnalysis(string messageSentByUser)
        {
            HttpClient client = new HttpClient();
            //Build header - This takes in subscription key. Get the subscription key for TextAnalytics Api from your Azure subscription.
            //Provide your subscription key here.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "[Your Subscription key from Azure portal]");
            //Build the request body. This should pass on the message sent by end user. We can pass on multiple texts and id parameter 
            //helps in identifying its corresponding text.
            string requestBody = @"{
                                    ""documents"": [
                                                     {
                                                         ""id"": ""message"",
                                                         ""text"": """+messageSentByUser+@"""
                                                     }
                                                   ]
                                   }";
            byte[] byteData = Encoding.UTF8.GetBytes(requestBody);
            string responseText = string.Empty;
            HttpResponseMessage response;
            using (var content = new ByteArrayContent(byteData))
            {
                //Add content type to header of the content
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //Send Post request
                response = await client.PostAsync(uriSentiment, content);
                //Read the json response text
                responseText = await response.Content.ReadAsStringAsync();
            }
            //Fetching the response sentiment score. The text is analyzed and we get a score between 0 and 1. 1 being positive and 0 being negative.
            //It is highly recommended to use Json object deserializers and not to do string string operations I have done below. 
            Double sentimentScore = Double.Parse(((responseText.Split(new char[] { ',' }))[0].Split(new char[] { ':' }))[2]);
            string responseMessage = string.Empty;
            //Do an analysis of the score and suggest response message that will be sent back to the user. 
            //The conditions should be made more granular to get more accurate response.
            if (sentimentScore <= 1 && sentimentScore > 0.9)
            {
                responseMessage = "Great!!! Thanks for an awesome feedback";
            }
            else if (sentimentScore <= 0.9 && sentimentScore > 0.4)
            {
                responseMessage = "Your feedback is recoreded. Corrective action will be taken!!! We will try to serve you better next time";
            }
            else
            {
                responseMessage = "Sorry to learn that you did not have a good experience. We will definitely serve you better";
            }
           
            return responseMessage;
        }

    }
}