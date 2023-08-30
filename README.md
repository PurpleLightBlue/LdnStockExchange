# London Stock API

My implementation of the API is something of a compromise given time restrictions, but I decided to progress with a Domain Driven Design approach which lends itself to SOLID optimisation as well where possible. I also tried to follow a clean architecture pattern with how I arranged my projects.

There are other ways which the same functionality could be achieved and if this were something that were to become a real stock exchange I would suggest a different approach which would be more complicated. These other approaches will be covered later. 

## Solution Structure
So given the above I determined my solution would have separate project for each of the clean architecture type layers as well as a test project. So we have:

*	StockAPI – which contains the top level WebAPI. This is where I anticipated trades would be posted to and calls to fetch stocks etc from some client app. 
*	StockAPI. Application – this is where I wanted to hold my business logic. In this simple example there is not a huge amount of business logic going on in addition to CRUD style pass through methods but what there is, is here. I opted to use Services relating to the domain models as often the logic was acting on collections of a domain model or made use of other resources that would not be appropriate in a domain model (I’m thinking of the method to insert a trade and update the average stock value).
*	StockAPI.Domain – Here I put my Domain models and all the interfaces that would be used in other parts of the system as per Clean Architecture principles. My Domain models are:
  -	Trade
  -	Stock
  -	Broker
I also placed in here a custom exception I created for use with my idempotency check I implemented. If felt like it should be hosted here. 
*	StockAPI.Infrastructure – here I defined the implementations of the Repository interfaces that can be found in the Domain layer. I used Dapper to connect to a local instance of SQL Server
*	StockAPI.Tests – Test project containing mostly unit tests and a form of integration test ( at least that’s how I thought of them as) for the repositories using a SQLite in memory database. 
I’ve also included a database creation script in the root of the git repository to create the tables from my local db. 
In terms of addon packages I used it included Dapper for ORM work, SQL server for the actual database, SQLite for the in memory test database. Xunit and moq for testing. I used Lamar as my IoC container as well.

## What I felt went well

I was please to achieve what felt like the right division of domain objects. The Broker is a bit of an extension to the spec as the Broker is only mentioned as an Id on a trade but I fleshed that out a little, the methods for that are not all there for full CRUD and management of the Brokers in the database. The same is somewhat true of the Stock domain model, I’ve not built out all of the actions for that domain model that you might typically expect. 

I thought it was a good idea to include an idempotency check on the trades. This wasn’t mentioned in the spec but seemed logical. It’s not unheard of for trades to come in twice by mistake or even, following some sort of outage or incident, some trades to be replayed as part of a recovery process. Therefore by have an idempotent id on the trade this can be checked and double processing prevented. 
Having the trade table record the trades but the overall average be held on the stock table felt right as well. In a way the trade table is heading towards a form of event sourcing as the trades are really events of a tickersymbol/stock lifespan.
I feel like I got a good level of unit test coverage in this project. Higher levels of the test pyramid were not really covered due to time, apart from the pseudo integration tests for the repositories. 
Last but not least I believe I have covered the explicit bit of functionality mentioned in the brief. 
We need to expose the current value of a stock, values for all the stocks on the market and values for a range of them (as a list of ticker symbols).  – The Stocks controller and therefore the swagger ui you can try has methods to fetch one Stock or all the ones in the database. 
For simplicity, consider the value of a stock as the average price of the stock through all transactions. – This is handled with the Trades Controller where the underlying trade service, as the trade is recorded, the average calculation is run again and the value of the relevant Stock updated. 

## What didn’t go so well/more improvements I’m conscious of. 

I did have this bundled with a Dockerfile but ran into the old problem of having issues with connecting to a local DB from within the container. I have encountered this before but its been a little while since I did battle with Docker. In the end for time sake I stripped that out. I guess it would have made sense to have the database containerised as well and the networking sorted between the two and got around the issue that way. 
Some clever integration tests could have been created that made use of a temporary container with a database purely for testing and loaded with some seed data. 
The domain models were simple and much of the logic on the services as well but I was a bit bothered by having the trade service reference the stock service. Really this should form part of a Use Case as per Domain Driven Design theory but with time and the relatively simple nature of the transaction I let it stay the way it is. But if I were to take this further that’s something I would change. 
The domain models are exposed by the API. Really there should be DTOs to ‘the outside world’ in at least the API layer to isolate the domain and prevent any data points from being exposed that shouldn’t be. Again time meant the domain models stayed. 

## Enhancements
This implementation of mine is very basic, but if you were thinking of taking this to the next level the code could be split in CQRS command and query stacks which would be hosted separately and could be scaled independently. CQRS is complex though and can be time consuming so I didn’t go that far. Also since the one database was mentioned in the brief, and a relational one at that, I figured CQRS wouldn’t deliver much value but would require much more time. One of the benefits of CQRS is having the database behind the commands and queries separate. Also if you were to consider moving to event sourcing as a pattern for the trades the write database would look very different to the read one, possibly not even a relation SQL database, something NoSQL like Cosmos or MongoDB. 
Performance could be improved via containerisation and microservices. There is a lot of code being lumped together in my implementation. Particularly if you were to move down the CQRS route then microservices for the command and query side of each domain object would be a good idea. Possibly break apart the Broker/Trades/Stock elements into their own microservices.
Security – there’s none! Again for times sake I left that but any real stock API would need to pay particular attention to security and vulnerability to attack. The lack of security extends to the database as well but I figured that is ok since this is a prototype. 
At the moment all the data storage and retrieval is in the db with a full round trip, some sort of caching would improve speed. 
The logging is pretty basic and monitoring non existent, both would need to be in place for any live deployment. 
If the containerisation used had worked then obviously there is the potential to have multiple instances, although the current naïve nature of the code wouldn’t suit that as it does not guard against race conditions or eventual consistency between instances.
The database has had no optimisation done to it so in a big high data production system then  database sharding, caching (e.g., Redis), and optimizing queries for improved performance should be considered to keep the performance sprightly. 

## To Run the code
Clone this repository, then use the database creation script in the root of this repo that is the folder DatabaseScript\CreateStockAPIDatabaseWithSampleData.sql. Use this to create the database on sql server instance and then update the appsettings.json file in the StockAPI project. There is no security at the db level (there should be!) for ease of getting something together. All being well you should be able to run the code locally and use the SwaggerUI to get data in and out of the system. 
