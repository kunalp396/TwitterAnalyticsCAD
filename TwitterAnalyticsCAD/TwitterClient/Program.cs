//********************************************************* 
// 
//    Copyright (c) Microsoft. All rights reserved. 
//    This code is licensed under the Microsoft Public License. 
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF 
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY 
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR 
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT. 
// 
//*********************************************************

using System;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TwitterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

=======
using System.Linq;
using System.Configuration;
using System.Data;
using System.Reactive.Linq;

namespace TwitterClient
{
    public class Program
    {

        public static void Main(string[] args)
        {
            //LogObserver logObserver = new LogObserver();

            try
            {
                
>>>>>>> ff7e99ee4882a9597d76dc297032c1e9c3ef0f53
                //Configure Twitter OAuth
                var oauthToken = ConfigurationManager.AppSettings["oauth_token"];
                var oauthTokenSecret = ConfigurationManager.AppSettings["oauth_token_secret"];
                var oauthCustomerKey = ConfigurationManager.AppSettings["oauth_consumer_key"];
                var oauthConsumerSecret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
                var keywords = ConfigurationManager.AppSettings["twitter_keywords"];

                //Configure EventHub
                var config = new EventHubConfig();
                config.ConnectionString = ConfigurationManager.AppSettings["EventHubConnectionString"];
                config.EventHubName = ConfigurationManager.AppSettings["EventHubName"];
                var myEventHubObserver = new EventHubObserver(config);
<<<<<<< HEAD

                var datum = Tweet.StreamStatuses(new TwitterConfig(oauthToken, oauthTokenSecret, oauthCustomerKey, oauthConsumerSecret,
                    keywords)).Select(tweet => Sentiment.ComputeScore(tweet, keywords)).Select(tweet => new Payload { CreatedAt = tweet.CreatedAt, Topic = tweet.Topic, SentimentScore = tweet.SentimentScore, PlaceTimeZone = tweet.TimeZone, TweetText= tweet.Text,Retweeted=tweet.Retweeted, RetweetCount=tweet.RetweetCount});
=======
                

                var datum = Tweet.StreamStatuses(new TwitterConfig(oauthToken, oauthTokenSecret, oauthCustomerKey, oauthConsumerSecret,
                    keywords)).Select(tweet => Sentiment.ComputeScore(tweet, keywords)).Where(tweet=>tweet.Topic!="Unknown").Select(tweet => new Payload { CreatedAt = tweet.CreatedAt, Topic = tweet.Topic,
                        SentimentScore = tweet.SentimentScore, PlaceTimeZone = tweet.TimeZone,
                        TweetText = tweet.Text,Retweeted=tweet.Retweeted, RetweetCount=tweet.RetweetCount});
>>>>>>> ff7e99ee4882a9597d76dc297032c1e9c3ef0f53

                datum.ToObservable().Subscribe(myEventHubObserver);
            }
            catch (Exception ex)
            {
<<<<<<< HEAD


=======
                System.IO.File.AppendAllText(@"F:\www.txt", ex.Message.ToString()+" kkk "+ex.InnerException.ToString()+" sdfsd "+ex.StackTrace.ToString());
               // ex.Data.ToObservable<string>().Subscribe(logObserver);
>>>>>>> ff7e99ee4882a9597d76dc297032c1e9c3ef0f53
            }

        }
    }
}
