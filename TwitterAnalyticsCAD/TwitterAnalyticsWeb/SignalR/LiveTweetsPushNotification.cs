using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Timers;
using TwitterAnalyticsDBL.BusinessObjects;
using TwitterClient;
using System.Configuration;
using System.Linq;
using System.Reactive.Linq;
using TwitterAnalyticsCommon;

namespace TwitterAnalyticsWeb
{
    [HubName("liveTweetsPushNotification")]
    public class LiveTweetsPushNotification : Hub
    {
        static Timer _timer;
        static long SentTweetId;
        public static string keywords;
        public static DateTime tweetEndTime;
        public static string tweetTimeZone;
        public ILogger logger = LoggerFactory<ILogger>.Create(typeof(WebLogger));


        public void SendTweets(string message)
        {
            if (_timer == null)
            {
                _timer = new Timer(1000);
                _timer.Elapsed += _timer_Elapsed;
                _timer.Start();
            }
        }

        public void AnalyzeData(string UserId)
        {
            if (!string.IsNullOrWhiteSpace(UserId))
            {
                
                //Configure Twitter OAuth
                var oauthToken = ConfigurationManager.AppSettings["oauth_token"];
                var oauthTokenSecret = ConfigurationManager.AppSettings["oauth_token_secret"];
                var oauthCustomerKey = ConfigurationManager.AppSettings["oauth_consumer_key"];
                var oauthConsumerSecret = ConfigurationManager.AppSettings["oauth_consumer_secret"];

                //Configure EventHub
                var config = new EventHubConfig();
                config.ConnectionString = ConfigurationManager.AppSettings["EventHubConnectionString"];
                config.EventHubName = ConfigurationManager.AppSettings["EventHubName"];
                var myEventHubObserver = new EventHubObserver(config);


                var datum = Tweet.StreamStatuses(new TwitterConfig(oauthToken, oauthTokenSecret, oauthCustomerKey, oauthConsumerSecret,
                    keywords)).Select(tweet => Sentiment.ComputeScore(tweet, keywords)).
                    Where(tweet => tweet.Topic != "Unknown"
                    && (tweetTimeZone.ToUpper().Contains(tweet.TimeZone.ToUpper()) || string.Compare(tweetTimeZone, "All") == 0)
                    && DateTime.Compare(DateTime.Now, tweetEndTime) <= 0).
                    Select(tweet => new Payload
                    {
                        CreatedAt = tweet.CreatedAt,
                        UserId = UserId,
                        Topic = tweet.Topic,
                        SentimentScore = tweet.SentimentScore,
                        PlaceTimeZone = tweet.TimeZone,
                        TweetText = tweet.Text,
                        Retweeted = tweet.Retweeted,
                        RetweetCount = tweet.RetweetCount
                    });

                datum.ToObservable().Subscribe(myEventHubObserver);
            }
        }

        public void SetAnalyzeParameters(string topic, string duration, string timeZone)
        {
            keywords = topic;

            tweetEndTime = GetTweetEndTime(duration);
            tweetTimeZone = timeZone;
        }

        private DateTime GetTweetEndTime(string duration)
        {
            if (string.Compare(duration, "15 min") == 0)
            {
                return DateTime.Now.AddMinutes(15);
            }
            else if (string.Compare(duration, "30 min") == 0)
            {
                return DateTime.Now.AddMinutes(30);

            }
            else if (string.Compare(duration, "1 hr") == 0)
            {
                return DateTime.Now.AddHours(1);

            }
            else if (string.Compare(duration, "1 week") == 0)
            {
                return DateTime.Now.AddDays(7);

            }
            else
            {
                return DateTime.Now.AddMinutes(15);
            }

        }       


        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try {

                IList<BOTweetCount> latestTweets = BOTweetCount.TweetCountGetTweetsGreatorThanId(SentTweetId);
                  
                //Keep records of total no of tweets received for each topic.
                foreach (BOTweetCount tweet in latestTweets)
                {
                    //SignalR Code to send Tweet to All Client
                    Clients.All.sendTweetsToPage(((DateTime)tweet.CreatedAt).TimeOfDay.ToString("hh:mm:ss.ff"), tweet.TweetText);
                                        
                }

                //Keep an eye on latest Tweet push to dashboard
                if (latestTweets.Count > 0)
                {
                    SentTweetId = (long)latestTweets[latestTweets.Count - 1].Id;
                }
            }
            catch (Exception ex)
            {
                logger.Log(ex.StackTrace,LOGLEVELS.ERROR);
            }
            
            }
    }
}