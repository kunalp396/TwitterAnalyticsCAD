using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using TwitterAnalyticsDBL.BusinessObjects;
using System;
using TwitterAnalyticsWeb.Models;
using Microsoft.AspNet.Identity;
using static TwitterClient.Sentiment;
using TwitterAnalyticsCommon;
using System.Globalization;
using Newtonsoft.Json;

namespace TwitterAnalyticsWeb.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        public ILogger logger = LoggerFactory<ILogger>.Create(typeof(WebLogger));
        JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        List<DataPoint> dummylist = new List<DataPoint>();

        public DashBoardController()
        {
            dummylist.Add(new DataPoint(0, 0));

        }

        [HttpGet]
        public ViewResult FilterCriteria()
        {
            var timeZoneList = BOTimeZone.TimeZoneCollection().OrderBy(tz => tz.TimeZoneDisplayName).
                Select(tz => new SelectListItem { Value = tz.TimeZoneDisplayName, Text = tz.TimeZoneDisplayName }).ToList();

            timeZoneList.Insert(0,new SelectListItem() { Text = "All", Value = "All" });
            FilterCriteria filterCriteria = new FilterCriteria();

            filterCriteria.TimeZone = timeZoneList;

            filterCriteria.Duration = new[]{
                                new SelectListItem { Text="15 min",Value= "15 min" },

                new SelectListItem { Text="30 min",Value= "30 min" },
                new SelectListItem { Text = "1 hr", Value = "1 hr" },
                new SelectListItem { Text = "1 Week", Value = "1 Week" }

            }.ToList();

            return View(filterCriteria);
        }


        public ActionResult FilterCriteria(FilterCriteria filterCriteria)
        {
            try
            {
                throw new Exception();
                if (string.IsNullOrWhiteSpace(filterCriteria.Topics))
                {
                    var timeZoneList = BOTimeZone.TimeZoneCollection().OrderBy(tz => tz.TimeZoneDisplayName).
               Select(tz => new SelectListItem { Value = tz.TimeZoneDisplayName, Text = tz.TimeZoneDisplayName }).ToList();

                    timeZoneList.Insert(0, new SelectListItem() { Text = "All", Value = "All" });


                    filterCriteria.TimeZone = timeZoneList;

                    filterCriteria.Duration = new[]{
                new SelectListItem { Text="15 min",Value= "15 min" },
                new SelectListItem { Text="30 min",Value= "30 min" },
                new SelectListItem { Text = "1 hr", Value = "1 hr" },
                new SelectListItem { Text = "1 week", Value = "1 week" }

            }.ToList();



                    return View(filterCriteria);
                }

            }
            catch (Exception ex)
            {
                logger.Log(ex.StackTrace, LOGLEVELS.ERROR);
                //error = ex.StackTrace;
            }

            //Clearing existing data for current User for fresh analysis
            BOTweetMentions.DeleteAll(User.Identity.GetUserId());
            BOTweetCount.DeleteAll(User.Identity.GetUserId());

            return RedirectToAction("Index", "Home", new { UserId = Url.Encode(User.Identity.GetUserId()) });
        }


        public ActionResult FinalGraph()
        {
            try
            {
                IList<BOTweetMentions> listLatestTenTweetMentions = BOTweetMentions.TweetMentionsLatest(User.Identity.GetUserId());

                

                List<DataPoint> tweetCountSeries = new List<DataPoint>();
                List<DataPoint> avgSentimentSeries = new List<DataPoint>();

                foreach (BOTweetMentions tweetMention in listLatestTenTweetMentions)
                {
                    
                    double floattime = float.Parse(tweetMention.Time.ToString("mm.ss", CultureInfo.InvariantCulture));
                    floattime = Math.Round(floattime, 2);
                    floattime = Math.Ceiling(floattime * 20) / 20;

                    double xvalue = floattime;
                    double yvalue_tweetCount = tweetMention.Count.Value;
                    double yvalue_avgSentiment = tweetMention.Avg.Value;

                    tweetCountSeries.Add(new DataPoint(xvalue,yvalue_tweetCount));
                    avgSentimentSeries.Add(new DataPoint(xvalue, yvalue_avgSentiment));
                }

             
                ViewBag.TweetCountSeries = JsonConvert.SerializeObject(tweetCountSeries, _jsonSetting);
                ViewBag.AvgSentimentSeries = JsonConvert.SerializeObject(avgSentimentSeries, _jsonSetting); 

                return PartialView();
            }
            catch (Exception ex)
            {
                logger.Log(ex.StackTrace, LOGLEVELS.ERROR);
                ViewBag.TweetCountSeries = JsonConvert.SerializeObject(dummylist, _jsonSetting);
                ViewBag.AvgSentimentSeries = JsonConvert.SerializeObject(dummylist, _jsonSetting);

                return PartialView();
            }
        }

        public ActionResult LiveTweets()
        {
            return PartialView();
        }

        public ActionResult OverAllTweets()
        {
            try
            {
                IList<BOTweetMentions> tweetMentionTopicList= BOTweetMentions.TweetMentionsDistinctTopics(User.Identity.GetUserId());
                List<string> distinctTopicList = tweetMentionTopicList.Select(data => data.Topic).ToList();

                List<DataPoint> tweetSpeeder = new List<DataPoint>();

                foreach (var topic in distinctTopicList)
                {
                    int speed = 0;
                    speed=BOTweetCount.TweetSpeed(User.Identity.GetUserId(),topic);

                    tweetSpeeder.Add(new DataPoint(speed,topic));
                }


               

                ViewBag.TopicSpeeder = JsonConvert.SerializeObject(tweetSpeeder, _jsonSetting);

                return PartialView();
            }
            catch (Exception ex)
            {
                //error = ex.StackTrace;
                logger.Log(ex.StackTrace, LOGLEVELS.ERROR);
                ViewBag.TopicSpeeder = JsonConvert.SerializeObject(dummylist, _jsonSetting);

                return PartialView();
            }


        }


        public ActionResult OverAllSentimentsCount()
        {
            List<DataPoint> dummylist = new List<DataPoint>();
            dummylist.Add(new DataPoint(0, 0));
            try
            {
                IList<BOTweetCount> listLatestTweetCounts = BOTweetCount.TweetCountCollection(User.Identity.GetUserId());
                Dictionary<string, Dictionary<string, int>> allSentimentsbyTopic = new Dictionary<string, Dictionary<string, int>>();

                foreach (BOTweetCount tweetCount in listLatestTweetCounts)
                {
                    if (!allSentimentsbyTopic.ContainsKey(tweetCount.Topic.ToUpper()))
                    {
                        allSentimentsbyTopic.Add(tweetCount.Topic.ToUpper(),
                            new Dictionary<string, int>() { { Enum.GetName(typeof(SentimentScore), SentimentScore.Negative).ToUpper(), 0},
                                                        { Enum.GetName(typeof(SentimentScore), SentimentScore.Positive).ToUpper(), 0},
                                                        { Enum.GetName(typeof(SentimentScore), SentimentScore.Neutral).ToUpper(), 0}});
                    }
                    else
                    {
                        allSentimentsbyTopic[tweetCount.Topic.ToUpper()][Enum.GetName(typeof(SentimentScore), tweetCount.SentimentScore).ToUpper()] += 1;

                    }
                }


                var topics = allSentimentsbyTopic.Keys.ToArray();

                List<Series> yAxisData = new List<Series>();

                List<DataPoint> negativeSentimentbyTopic = new List<DataPoint>();
                List<DataPoint> positiveSentimentbyTopic = new List<DataPoint>();
                List<DataPoint> neutralSentimentbyTopic = new List<DataPoint>();


                string[] sentimentScoreCounter = new string[] { "NEGATIVE", "POSITIVE", "NEUTRAL" };
                foreach (string item in sentimentScoreCounter)
                {
                    //List<object> data = new List<object>();
                    foreach (string key in allSentimentsbyTopic.Keys)
                    {
                        //data.Add(allSentimentsbyTopic[key][item]);
                        if (string.Compare(item, "NEGATIVE", true) == 0)
                        {
                            negativeSentimentbyTopic.Add(new DataPoint(allSentimentsbyTopic[key][item], key));
                        }
                        else if (string.Compare(item, "POSITIVE", true) == 0)
                        {
                            positiveSentimentbyTopic.Add(new DataPoint(allSentimentsbyTopic[key][item], key));
                        }
                        else if (string.Compare(item, "NEUTRAL", true) == 0)
                        {
                            neutralSentimentbyTopic.Add(new DataPoint(allSentimentsbyTopic[key][item], key));
                        }

                    }

                   
                }

                ViewBag.NegativeSentimentbyTopic = JsonConvert.SerializeObject(negativeSentimentbyTopic, _jsonSetting);
                ViewBag.PositiveSentimentbyTopic = JsonConvert.SerializeObject(positiveSentimentbyTopic, _jsonSetting);
                ViewBag.NeutralSentimentbyTopic = JsonConvert.SerializeObject(neutralSentimentbyTopic, _jsonSetting);
               

                return PartialView();
            }
            catch (Exception ex)
            {
                //error = ex.StackTrace;
                logger.Log(ex.StackTrace, LOGLEVELS.ERROR);
                ViewBag.NegativeSentimentbyTopic = JsonConvert.SerializeObject(dummylist, _jsonSetting);
                ViewBag.PositiveSentimentbyTopic = JsonConvert.SerializeObject(dummylist, _jsonSetting);
                ViewBag.NeutralSentimentbyTopic = JsonConvert.SerializeObject(dummylist, _jsonSetting);
                return PartialView();

            }


        }
        


    }
}