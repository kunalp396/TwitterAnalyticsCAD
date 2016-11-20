using System.Collections.Generic;
using System.Web.Mvc;
using TwitterAnalyticsWeb.Models;
using System.Linq;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using System;
using TwitterAnalyticsCommon;
using System.Data.SqlClient;
using System.Data;
using TwitterAnalyticsDBL.BusinessObjects;

namespace TwitterAnalyticsWeb.Controllers
{
    public class ChartSampleController: Controller
    {
        public ActionResult FinalGraph()
        {
            IList<BOTweetMentions> listLatestTenTweetMentions = BOTweetMentions.TweetMentionsLatestTen();

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
            Highcharts mychart = lineChartSentiment(xDataMonths, yDataCounts, yDataAvgSentimentCounts);

            return PartialView(mychart);
        }

        private static Highcharts lineChartSentiment(string[] xDataMonths, object[] yDataCounts, object[] yDataAvgSentimentCounts)
        {
            return new Highcharts("Charts")

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Line })
                .SetTitle(new Title { Text = "Tweets per 5 sec" })
                .SetSubtitle(new Subtitle { Text = "Overall Status" })
                .SetXAxis(new XAxis { Categories = xDataMonths, Labels = new XAxisLabels { Rotation = -90 } })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Count" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = @"function(){return '<b>'+this.series.name+'</b></br>'+this.x+': '+this.y; }"
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
    }
}