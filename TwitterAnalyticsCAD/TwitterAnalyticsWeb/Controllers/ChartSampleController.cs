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

namespace TwitterAnalyticsWeb.Controllers
{
    public class ChartSampleController: Controller
    {
        public ActionResult Index()
        {

            var transactionCount = new List<TransactionCount> {
                new TransactionCount { Timestamp=DateTime.Now.AddMinutes(2), Count=30},
                new TransactionCount { Timestamp=DateTime.Now.AddMinutes(4), Count=24},
                 new TransactionCount { Timestamp=DateTime.Now.AddMinutes(6), Count=12},
                  new TransactionCount { Timestamp=DateTime.Now.AddMinutes(8), Count=6},
                  new  TransactionCount { Timestamp=DateTime.Now.AddMinutes(10), Count=74},
                  new  TransactionCount { Timestamp=DateTime.Now.AddMinutes(12), Count=3}


            };

            var xDataMonths = transactionCount.Select(i => i.Timestamp.ToString()).ToArray();
            var yDataCounts = transactionCount.Select(i => new object[] { i.Count }).ToArray();
            var yDataCounts2 = transactionCount.Select(i => new object[] { (i.Count/2) }).ToArray();

            var mychart = new Highcharts("Charts")

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Line })
                .SetTitle(new Title { Text = "Incoming transaction per minute" })
                .SetSubtitle(new Subtitle { Text = "Accounting" })
                .SetXAxis(new XAxis { Categories = xDataMonths })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "No of Transactions" } })
                .SetTooltip(new Tooltip
                {
                    Enabled = true,
                    Formatter = "function() { return '<b>'+ this.point.name +'</b>: '+ this.percentage +' %'; }"                
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

                                new Series { Name="Hour",Data=new Data(yDataCounts) },
                                  new Series { Name="Div2Hour",Data=new Data(yDataCounts2) }

                            });

          
            
            return View(mychart);
        }

        public ActionResult FinalGraph()
        {
            AzureSQLConnMngr sqlConnMngr = AzureSQLConnMngr.GetInstance(true);
            var connnn = sqlConnMngr.GetSqlConnection();

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT top 10 * FROM TweetMentions order by time desc";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connnn;


            cmd.Connection.Open();
            reader = cmd.ExecuteReader();
            List<DateTime> timecollection = new List<DateTime>();
            List<int> tweetCountCollection = new List<int>();
            List<float> tweetAvgSentimentCollection = new List<float>();


            while (reader.Read())
            {
                DateTime timeStamp = DateTime.Parse(reader["time"].ToString());
                int count = int.Parse(reader["count"].ToString());
                float avgSentiment = float.Parse(reader["avg"].ToString());
                timecollection.Add(timeStamp);
                tweetCountCollection.Add(count);
                tweetAvgSentimentCollection.Add(avgSentiment);
            }
            cmd.Connection.Close();
                        
            var xDataMonths = timecollection.Select(i => i.TimeOfDay.ToString()).ToArray();
            var yDataCounts = tweetCountCollection.Select(i => new object[] { i.ToString() }).ToArray();
            var yDataAvgSentimentCounts = tweetAvgSentimentCollection.Select(i => new object[] { i.ToString() }).ToArray();

            var mychart = new Highcharts("Charts")

                .InitChart(new Chart { DefaultSeriesType = DotNet.Highcharts.Enums.ChartTypes.Line })
                .SetTitle(new Title { Text = "Tweets per 5 sec" })
                .SetSubtitle(new Subtitle { Text = "Overall Status" })
                .SetXAxis(new XAxis { Categories = xDataMonths })
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

            return View(mychart);
        }
    }
}