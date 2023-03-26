# UrlShortener

Provide some details about your application 

## How to start?
### Docker Compose 
#### Prerequisites
 - Docker installed(v20.10.0 or higher)
 - Docker compose is present
 - .NET SDK (v6.0 or higher)
#### Setup and run using Docker Compose
1. Clone the repository to your local machine
2. Move into the solution directory
3. Build and run the project using docker compose
```
    docker compose build
    docker compose up
    
    OR
    
    docker-compose build
    docker-compose up
```
4. Using default files, docker should expose client app on http://localhost:3000 and api on http://localhost:8080 
5. To access swagger endpoint go to http://localhost:8080/api/swagger

### Local environment
#### Prerequisites
 - Node installed(v18.15.0)
 - npm installed(v9.5.0)
 - .NET SDK (v6.0 or higher)

#### Setup and run using local environment using IDE
 - Open solution file in VS/Rider
 - Run application using UrlShortener.WebApplication profile
#### Setup and run using local environment from console
 - Go to ./src/UrlShortener.WebApplication
 - Restore, build and run the project using launch profile
```
    dotnet restore
    dotnet build
    dotnet run --launch-profile UrlShortener.WebApplication
```
 - Once the project has started go to http://localhost:5000
 - After the proxy starts you'll be redirected to the Client Application

## Key assumptions
1. URL shouldn't be preserved forever - if they're not accessed for some amount of time then they should be removed
2. Using InMemoryStorage should be only done in local dev environment

## Future Ideas
1. There might be an issue with "phantom reads" if more than one client is currently using API - that's due to the use of Redis and SortedSets - I wanted to sort the results by how ofter they're accessed, so at each time the URL is accessed, score in sorted set is increased for that url - I thought about adding second database to just serve user the list
2. API versioning
3. Enable HTTPS communication between ClientApp and API


## Task Description 
>Build a URL shortening service like TinyURL. This service will provide short aliases redirecting to long URLs.
### Why do we use Url shortening?
URL shortening is used to create shorter aliases for long URLs. We call these shortened aliases “short links.” Users are redirected to the original URL when they hit these short links. Short links save a lot of space when displayed, printed, messaged, or tweeted. Additionally, users are less likely to mistype shorter URLs.

For example, if we shorten the following URL: `https://www.some-website.io/realy/long-url-with-some-random-string/m2ygV4E81AR`

We would get something like this: `https://short.url/xer23`

URL shortening is used to optimize links across devices, track individual links to analyze audience, measure ad campaigns’ performance, or hide affiliated original URLs.

### URL shortening application should have:
 - A page where a new URL can be entered and a shortened link is generated. You can use Home page.
 - A page that will show a list of all the shortened URL’s.
### Functional Requirements:
- Given a URL, our service should generate a shorter and unique alias of it. This is called a short link. This link should be short enough to be easily copied and pasted into applications.
- When users access a short link, our service should redirect them to the original link.
- Application should store logs information about requests.
### Non-Functional Requirements:
- URL redirection should happen in real-time with minimal latency.
- Please add small project description to Readme.md file.
### During implementation please pay attention to:
- Application is runnable out of box. If some setup is needed please provide details on ReadMe file.
- Project structure and code smells.
- Design Principles and application testability.
- Main focus should be on backend functionality, not UI.
- Input parameter validation.
- Please, don't use any library or service that implements the core functionality of this test.
### Other recommendation:
- You may change UI technology to any other you are most familiar with.
- You can use InMemory data storage.
- You can use the Internet.
# May the force be with you {username}!