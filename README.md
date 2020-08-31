# Financial Chat Challenge

## Installation

1. Clone the repo
2. Run `UPDATE-DATABASE` in the Visual Studio Package Manager to apply migrations
3. Install [RabbitMQ](https://www.rabbitmq.com/). I used it inside [Docker](https://www.docker.com/) through the following command: `docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
`
4. Run the app

## Tests

For the RabbitMQTests to run properly, RabbitMQ has to be running. The ProduceConsumeTest produces and consumes a message in the RabbitMQ queue.

## About the implementation

About the optional item "Handle messages that are not understood or any exceptions raised within the bot", I made an OnMessageRejected event in the StockBot class that can be listened to. For now it only debugs to the console.

I also didn't manage to implement:
- Scrolling the page down automatically to view most recent messages
- Reloading the view on every client as soon as a new message is received. Currently only the client posting messages gets its view refreshed so the other clients have to F5 until new messages arrive
