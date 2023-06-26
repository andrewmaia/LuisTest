
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Intent;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace LuisTest
{
    class Program
    {
        static string predictionKey = "4b5549c816e6487baa0da98a269f1871";
        static string predictionEndpoint = "https://luisipcorp.cognitiveservices.azure.com/";
        static string appId = "b3ebd374-5a15-45dc-a04a-eaf2f628b93e";
        static async Task Main(string[] args)
        {
            while(true)
            {
                Console.WriteLine("Bot: Diga algo");
                Console.Write("Você:");
                string texto=Console.ReadLine();
                Prediction p = ObterIntencao(texto);
                Responder(p);
            }
            
        }

        private static Prediction ObterIntencao(string texto)
        {
            using (LUISRuntimeClient luisClient = ObterLuisClient())
            {
                var predictionRequest = new PredictionRequest(texto);
                var p = luisClient.Prediction.GetSlotPredictionAsync(
                        Guid.Parse(appId),
                        slotName: "staging",
                        predictionRequest,
                        verbose: true
                    ).Result;
                return p.Prediction;
            }
        }

        private static LUISRuntimeClient ObterLuisClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(predictionKey);
            var luisClient = new LUISRuntimeClient(credentials, new DelegatingHandler[] { });
            luisClient.Endpoint = predictionEndpoint;
            return luisClient;
        }

        private static void Responder(Prediction p)
        {
            string resposta = null;
            switch (p.Intents.FirstOrDefault().Key)
            {
                case "VerValorFatura":
                    resposta = ResponderFatura(p.Entities);
                    break;
                case "Cumprimentar":
                    resposta = "Oi em que posso te ajudar?";
                    break;
                default:
                    resposta = "Não entendi o que você quer!";
                    break;
            }

            Console.WriteLine(resposta);
            Console.WriteLine();
            Console.WriteLine();
        }

        private static string ResponderFatura(IDictionary<string,object> entidades)
        {
            try
            {


                var j = new JArray(entidades["datetimeV2"]);
                string anoMes = ((JObject)((JArray)((JObject)((JArray)j[0])[0])["values"])[0])["timex"].ToString();
                DateTime d = new DateTime(int.Parse(anoMes.Substring(0, 4)),int.Parse(anoMes.Substring(5, 2)), 1);
                return $"Sua fatura referente a {anoMes} é R$ 100,00";
            }
            catch
            {
                return "Não consegui entender de qual mês e ano você quer sua fatura";
            }
        }
    }
}
/*
 * if (entity.Type == "builtin.datetimeV2.daterange")
{
    foreach (var vals in entity.Resolution.Values)
    {
        if (((Newtonsoft.Json.Linq.JArray)vals).First.SelectToken("type").ToString() == "daterange")
        {
            start = (DateTime)((Newtonsoft.Json.Linq.JArray)vals).First.SelectToken("start");
            end = (DateTime)((Newtonsoft.Json.Linq.JArray)vals).First.SelectToken("end");
        }
    }
}
*/