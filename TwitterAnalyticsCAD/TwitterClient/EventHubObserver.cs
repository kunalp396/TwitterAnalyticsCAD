﻿//********************************************************* 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Configuration;
using TwitterAnalyticsCommon;

//source:https://docs.microsoft.com/en-us/azure/Event-Hubs/event-hubs-csharp-ephcs-getstarted
namespace TwitterClient
{
    public class EventHubObserver : IObserver<Payload>
    {
        private EventHubConfig _config;
        private EventHubClient _eventHubClient;
        public ILogger logger = LoggerFactory<ILogger>.Create(typeof(WebLogger));

        public EventHubObserver(EventHubConfig config)
        {
            try
            {
                _config = config;
                _eventHubClient = EventHubClient.CreateFromConnectionString(_config.ConnectionString, config.EventHubName);
                
            }
            catch (Exception ex)
            {
                logger.Log(ex.StackTrace,LOGLEVELS.ERROR);
            }

        }
        public void OnNext(Payload TwitterPayloadData)
        {
            try
            {

                var serialisedString = JsonConvert.SerializeObject(TwitterPayloadData);
                EventData data = new EventData(Encoding.UTF8.GetBytes(serialisedString)) { PartitionKey = TwitterPayloadData.Topic };
                _eventHubClient.Send(data);
                

            }
            catch (Exception ex)
            {
                logger.Log(ex.StackTrace, LOGLEVELS.ERROR);

            }

        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
           // logger.Log(error.StackTrace, LOGLEVELS.ERROR);
        }

    }
}
