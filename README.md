# Localchat

Simple chat with a common room, implemented in C #.

The application is divided into 2 main parts with a graphical interface:
  - Server (displays all connected clients, displays server events, displays all messages in the chat room, can send messages to other clients)
  - Client (can only send messages and receive them from other clients)

### Requirements:
  - .NET Framework 4.7.1
  - Visual Studio 2019

### Technologies and libraries used:
  - WCF for networking
  - SQLite for storing messages
  - EntityFramework for working with a database
  - WPF for the graphical part of the server and client
  - MVVM Light to implement the MVVM approach
  - xUnit for testing
  - Moq for unit testing