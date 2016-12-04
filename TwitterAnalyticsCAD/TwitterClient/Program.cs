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
                

                var datum = Tweet.StreamStatuses(new TwitterConfig(oauthToken, oauthTokenSecret, oauthCustomerKey, oauthConsumerSecret,
                    keywords)).Select(tweet => Sentiment.ComputeScore(tweet, keywords)).Where(tweet=>tweet.Topic!="Unknown").Select(tweet => new Payload { CreatedAt = tweet.CreatedAt, Topic = tweet.Topic,
                        SentimentScore = tweet.SentimentScore, PlaceTimeZone = tweet.TimeZone,
                        TweetText = tweet.Text,Retweeted=tweet.Retweeted, RetweetCount=tweet.RetweetCount});

                datum.ToObservable().Subscribe(myEventHubObserver);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"F:\www.txt", ex.Message.ToString()+" kkk "+ex.InnerException.ToString()+" sdfsd "+ex.StackTrace.ToString());
               // ex.Data.ToObservable<string>().Subscribe(logObserver);
            }

        }
    }
}
