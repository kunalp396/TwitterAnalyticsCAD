# TwitterAnalyticsCAD
Analysis of Live Twitter data and analysis of Sentiments.

  During login user will have to first register using his email ID, then user will be added into the database. The registered user will have access to the dashboard. While logging into the account users authentication will be verified against the database and permission will be given to the user to use the resource, if the user is not authenticate then the permission will be denied.
  
  Twitter API allows collecting real time data from the tweets which can be consumed by the client application. Twitter API requires developer account to allow any client application for consumption of its live stream data.  
  
  Real time data from the API is then forwarded for sentiment analysis. Sentiment analysis can be performed by Machine Learning API available such as provide by Sentiment140, a twitter sentiment analysis tool. Processes records are provided with polarity values like 0 for negative, 2 for neutral, 4 for positive and sent to Azure Stream Analytics. 
  
  Azure Stream Analytics job consists of Input and Output modules which can be configured as per the needs to get and forward data in compatible format to other system components. We will use JSON as input to Stream Analytics and SQL Server/Blog storage as output. It allows us to filter and query our data from storage. Power BI or Web App can be used to develop intuitive user interface for user to get results. 
  
  Azure Web Application would act as the primary interface for user to view current live update and sentiment analysis of twitter data. Dashboard can provide deep insights about user’s interest and sentiments about services or product.
  Please read Twitter_Real_Time_Data_Analysis_CAD_GitHub.doc for more architecture and implementation details.

