using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace SpecFlowTests
{
    [Binding]
    public class PublicApiSteps
    {
        private const string PublicApiBaseUrl = "http://localhost/Management/PublicApi/";

        private string _authorizationToken = string.Empty;
        private string _addedServiceId = string.Empty;

        #region Login

        [Given(@"I login with username '(.*)' and password '(.*)'")]
        public void GivenILoginWithUsernameAndPassword(string username, string password)
        {
            var task = Post<Dictionary<string, string>>("Authentication/Login", new
            {
                Username = username,
                Password = password
            });
            task.Wait();
            var response = task.Result;

            _authorizationToken = response["authorizationToken"];
        }

        [Then(@"I should receive an authorization token")]
        public void ThenIShouldReceiveAnAuthorizationToken()
        {
            Assert.AreNotEqual(string.Empty, _authorizationToken);
        }

        #endregion

        #region Services

        [Given(@"I add a new service with name '(.*)'")]
        public void GivenIAddANewServiceWithName(string serviceName)
        {
            var task = Post<string>("Service", new
            {
                Name = serviceName
            });
            task.Wait();

            var serviceId = task.Result;
            _addedServiceId = serviceId;
        }

        [Then(@"A service with the name '(.*)' should exist")]
        public void ThenAServiceWithTheNameShouldExist(string name)
        {
            var task = Get<Dictionary<string, object>>($"Service/{_addedServiceId}");
            task.Wait();

            var response = task.Result;
            Assert.AreEqual(name, response["name"]);
            Assert.AreNotEqual(string.Empty, response["id"]);
            Assert.AreNotEqual(string.Empty, response["updateId"]);
        }

        [AfterScenario("AfterScenarioDeleteService")]
        public void AfterScenarioDeleteService()
        {
            var task = Delete<string>($"Service/{_addedServiceId}");
            task.Wait();
        }

        #endregion

        #region HTTP Get, Post, Delete

        public async Task<T> Get<T>(string path)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Auth-Token", _authorizationToken);

                var responseMessage = await client.GetAsync(PublicApiBaseUrl + path);

                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }

                throw new Exception($"Request failed - {(int)responseMessage.StatusCode} {responseMessage.StatusCode} - {response}");
            }
        }

        public async Task<T> Post<T>(string path, object data)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Auth-Token", _authorizationToken);

                var json = JsonConvert.SerializeObject(data);

                var responseMessage = await client.PostAsync(PublicApiBaseUrl + path, new StringContent(json, Encoding.UTF8, "application/json"));

                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }

                throw new Exception($"Request failed - {(int)responseMessage.StatusCode} {responseMessage.StatusCode} - {response}");
            }
        }

        public async Task<T> Delete<T>(string path)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Auth-Token", _authorizationToken);

                var responseMessage = await client.DeleteAsync(PublicApiBaseUrl + path);

                var response = await responseMessage.Content.ReadAsStringAsync();

                if (responseMessage.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }

                throw new Exception($"Request failed - {(int)responseMessage.StatusCode} {responseMessage.StatusCode} - {response}");
            }
        }

        #endregion

    }


}
