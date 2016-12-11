using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TwitterClient;
using static TwitterClient.Sentiment;

namespace TwitterAnalyticsWeb.Tests.Tests
{
    [TestClass]
    public class SentimentAnalysisTests
    {
        [TestMethod]
        public void SentimentAnalysis_Positive_Sentiment()
        {
           string textToAnalyse = @"That was..Interesting. I mean I love Google photos .. but yes.. interesting, especially when Japan is mostly iPhone. https://t.co/0WsJnGzUyy";
           SentimentScore score= Analyze(textToAnalyse);
           Assert.AreEqual(SentimentScore.Positive, score);

        }

        [TestMethod]
        public void SentimentAnalysis_Negative_Sentiment()
        {
            string textToAnalyse = @"@talk2GLOBE Why can't I use my postpaid line as a billing method for Google Play?";
            SentimentScore score = Analyze(textToAnalyse);
            Assert.AreEqual(SentimentScore.Negative, score);

        }

        [TestMethod]
        public void SentimentAnalysis_Neutral_Sentiment()
        {
            string textToAnalyse = @"Maven Wave Partners is looking for: Google Deployment Lead https://t.co/LJPPQL6pkV #job";
            SentimentScore score = Analyze(textToAnalyse);
            Assert.AreEqual(SentimentScore.Neutral, score);

        }


        [TestMethod]
        public void SentimentAnalysis_DetermineTopics()
        {
            string textToAnalyse = @"Maven Wave Partners is looking for: Google Deployment Lead https://t.co/LJPPQL6pkV #job";
            string searchTopics = "Microsoft,google";

            string topicResult = DetermineTopic(textToAnalyse,searchTopics);
            Assert.AreNotEqual("Unknown", topicResult);

        }
    }
}
