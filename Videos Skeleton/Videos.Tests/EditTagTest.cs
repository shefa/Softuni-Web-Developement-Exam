namespace Videos.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    //using Videos.Tests.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

   
    [TestClass]
    public class EditTagTest
    {

        [TestMethod]
        public void EditExistingChannel_ShouldReturn200OK_Modify()
        {
            // Arrange -> create a new channel
            TestingEngine.CleanDatabase();
            var channelName = "channel" + DateTime.Now.Ticks;
            var httpPostResponse = this.CreateChannelHttpPost(channelName);
            Assert.AreEqual(HttpStatusCode.Created, httpPostResponse.StatusCode);
            var postedChannel = httpPostResponse.Content.ReadAsAsync<ChannelModel>().Result;

            // Act -> edit the above created channel
            var channelNewName = "Edited " + channelName;
            var httpPutResponse = this.EditChannelHttpPut(postedChannel.Id, channelNewName);

            // Assert -> the PUT result is 200 OK
            Assert.AreEqual(HttpStatusCode.OK, httpPutResponse.StatusCode);

            // Assert the service holds the modified channel
            var httpGetResponse = TestingEngine.HttpClient.GetAsync("/api/channels").Result;
            var channelsFromService = httpGetResponse.Content.ReadAsAsync<List<ChannelModel>>().Result;
            Assert.AreEqual(HttpStatusCode.OK, httpGetResponse.StatusCode);
            Assert.AreEqual(1, channelsFromService.Count);
            Assert.AreEqual(postedChannel.Id, channelsFromService.First().Id);
            Assert.AreEqual(channelNewName, channelsFromService.First().Name);
        }

        private HttpResponseMessage CreateChannelHttpPost(string channelName)
        {
            var postContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", channelName)
            });
            var httpPostResponse = TestingEngine.HttpClient.PostAsync(
                "/api/channels", postContent).Result;
            return httpPostResponse;
        }

        private HttpResponseMessage EditChannelHttpPut(int id, string channelName)
        {
            var putContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("name", channelName)
            });
            var httpPutResponse = TestingEngine.HttpClient.PutAsync(
                "/api/channels/" + id, putContent).Result;
            return httpPutResponse;
        }
    }
}
