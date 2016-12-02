using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using TwitterAnalyticsDBL.BusinessObjects;
using TwitterAnalyticsCommon;
using System;
using TwitterAnalyticsWeb.Models;
using System.Diagnostics;

namespace TwitterAnalyticsWeb.Controllers
{

    public class DashBoardController : Controller
    {
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

                System.IO.File.AppendAllText(@"F:\wwwwww.txt", ex.Message + " sssssssssssss " + ex.StackTrace.ToString());
            }

            return RedirectToAction("Index", "Home");
        }


        public ActionResult FinalGraph()
        {
            IList<BOTweetMentions> listLatestTenTweetMentions = BOTweetMentions.TweetMentionsLatest();

            List<string> timecollection = new List<string>();
            List<object> tweetCountCollection = new List<object>();
            List<object> tweetAvgSentimentCollection = new List<object>();

            foreach (BOTweetMentions tweetMention in listLatestTenTweetMentions)
            {
                timecollection.Add(tweetMention.Time.TimeOfDay.ToString());
                tweetCountCollection.Add(tweetMention.Count.Value);
                tweetAvgSentimentCollection.Add(tweetMention.Avg.Value);
            }

            var xDataMonths = timecollection.ToArray();
            var yDataCounts = tweetCountCollection.ToArray();
            var yDataAvgSentimentCounts = tweetAvgSentimentCollection.ToArray();
            Highcharts mychart = lineChartSentiment("FinalDashBoard", xDataMonths, yDataCounts, yDataAvgSentimentCounts);

            return PartialView(mychart);
        }

        public ActionResult LiveTweets()
        {
            return PartialView();
        }

        public ActionResult OverAllTweets()
        {
            try
            {
                IList<BOTweetCount> listLatestTenTweetCount = BOTweetCount.TweetCountCollection();

                Dictionary<string, int> allTweetsCountbyTopic = new Dictionary<string, int>();
                Dictionary<string, Dictionary<string, int>> allSentimentsbyTopic = new Dictionary<string, Dictionary<string, int>>();

                foreach (BOTweetCount tweetCount in listLatestTenTweetCount)
                {
                    if (!allTweetsCountbyTopic.ContainsKey(tweetCount.Topic.ToUpper()))
                    {
                        allTweetsCountbyTopic.Add(tweetCount.Topic.ToUpper(), 0);
                    }
                    else
                    {
                        allTweetsCountbyTopic[tweetCount.Topic.ToUpper()] += 1;
                    }



                };

                var xDataMonths = allTweetsCountbyTopic.Keys.ToArray();
                var yDataCounts = allTweetsCountbyTopic.Values.Select(count => (object)count).ToArray();

                Highcharts overAllSentimentChart = barChartTweetsByTopic("OverAllTweets", xDataMonths, yDataCounts);

                return PartialView(overAllSentimentChart);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"F:\temp1.txt", ex.StackTrace);
                return null;

            }


        }


        public ActionResult OverAllSentimentsCount()
        {
            try
            {
                IList<BOTweetCount> listLatestTenTweetCount = BOTweetCount.TweetCountCollection();
                Dictionary<string, Dictionary<string, int>> allSentimentsbyTopic = new Dictionary<string, Dictionary<string, int>>();

                foreach (BOTweetCount tweetCount in listLatestTenTweetCount)
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

                Highcharts overAllSentimentChart = barChartSentimentsByCount("OverAllSentimentsCount", topics, allSentimentsbyTopic);

                return PartialView(overAllSentimentChart);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText(@"F:\temp2.txt", ex.StackTrace);
                return null;

            }


        }


        private static Highcharts lineChartSentiment(string chartName, string[] xDataMonths, object[] yDataCounts, object[] yDataAvgSentimentCounts)
        {
            return new Highcharts(chartName)

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Line })
                .SetTitle(new Title { Text = "Tweets per 5 sec" })
                .SetSubtitle(new Subtitle { Text = "Overall Status" })
                .SetXAxis(new XAxis { Categories = xDataMonths, Labels = new XAxisLabels { Rotation = -90 } })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Count" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = @"function(){return '<b> '+this.series.name+' </b></br>'+this.x+' : '+this.y; }"
                })
                            .SetPlotOptions(new PlotOptions
                            {
                                Line = new PlotOptionsLine
                                {
                                    DataLabels = new PlotOptionsLineDataLabels
                                    {
                                        Enabled = true
                                    },
                                    EnableMouseTracking = false
                                }

                            })
                            .SetSeries(new[]
                            {

                                new Series { Name="Total Tweets",Data=new Data(yDataCounts) },
                                new Series { Name="Avg Sentiment",Data=new Data(yDataAvgSentimentCounts) }

                            });
        }


        private static Highcharts barChartTweetsByTopic(string chartName, string[] xAxisPlot, object[] yAxisData)
        {
            return new Highcharts(chartName)

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Column })
                .SetTitle(new Title { Text = "" })
                .SetSubtitle(new Subtitle { Text = "Total no. of Tweets" })
                .SetXAxis(new XAxis { Categories = xAxisPlot, Labels = new XAxisLabels { Rotation = -90 } })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Tweets Count" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = @"function(){return '<b> '+this.series.name+'</b> </br> '+this.x+' : '+this.y; }"
                })
                            .SetPlotOptions(new PlotOptions
                            {
                                Line = new PlotOptionsLine
                                {
                                    DataLabels = new PlotOptionsLineDataLabels
                                    {
                                        Enabled = true
                                    },
                                    EnableMouseTracking = false
                                }

                            })
                            .SetSeries(new[]
                            {

                                new Series { Name=" Topics ",Data=new Data(yAxisData) }
                               // new Series { Name="Skype",Data=new Data(yAxisData2) }

                            });
        }

        private static Highcharts barChartSentimentsByCount(string chartName, string[] xAxisPlot, Dictionary<string, Dictionary<string, int>> overAllSentimentChart)
        {
            List<Series> yAxisData = new List<Series>();

            string[] sentimentScoreCounter = new string[] { "NEGATIVE", "POSITIVE", "NEUTRAL" };
            foreach (string item in sentimentScoreCounter)
            {
                List<object> data = new List<object>();
                foreach (string key in overAllSentimentChart.Keys)
                {
                    data.Add(overAllSentimentChart[key][item]);
                }

                yAxisData.Add(new Series()
                {
                    Name = item,
                    Data = new Data(data.ToArray())
                });
            }





            return new Highcharts(chartName)

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Column })
                .SetTitle(new Title { Text = "" })
                .SetSubtitle(new Subtitle { Text = "Total no. of Sentiments" })
                .SetXAxis(new XAxis { Categories = xAxisPlot })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Tweets Count" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = @"function(){return '<b> '+this.series.name+'</b> </br> '+this.x+' : '+this.y; }"
                })
                            .SetPlotOptions(new PlotOptions
                            {
                                Line = new PlotOptionsLine
                                {
                                    DataLabels = new PlotOptionsLineDataLabels
                                    {
                                        Enabled = true
                                    },
                                    EnableMouseTracking = false
                                }

                            })
                            .SetSeries(


                               yAxisData.ToArray()
                            // new Series { Name="Skype",Data=new Data(yAxisData2) }

                            );
        }

    }
}