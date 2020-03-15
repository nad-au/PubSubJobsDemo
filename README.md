# PubSubJobsDemo
A collection of apps to demonstrate a distributed approach to orchestrating long-running jobs using **ServiceStack** &amp; **Redis**

## Getting Started
Make sure `Redis` is installed and running. The easiest is to use `Docker` and run the following:

```
docker run  -d -p 6379:6379 redis
```
Build solution and then start `Subscriber1` and `Subscriber2` which run on ports `8001` & `8002`. Finally, start `Publisher`, which will dispatch some sample `JobRequest` messages to the Redis MQ, which will be processed by any of the subscribers.
 