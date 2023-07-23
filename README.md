# TL;DR 

This repo is a blazor site mimicking the [ChatGPT site](https://chat.openai.com/) site and using the [OpenAI API](https://platform.openai.com/overview) to create tags and topic for each chat session.

- The actual blazor app is the ChatGptBlazorApp in the repo
- The other functional project in the repo is the DevelopmentConsole which is mainly for testing out stuff in a console setting.

# Abotti - your personal bot assistant

This repository began as a Blazor practice and [OpenAI API](https://platform.openai.com/overview) research project, but it has evolved into something that I may actually find useful as a tool. 
In its current form, the Blazor UI (ChatGptBlazor) resembles the [ChatGPT site](https://chat.openai.com/) to some extent.

Basically the idea is the create a site like that and use OpenAI API to classify the chat sessions to see how well it can be used to consistently classify and organize the chats and then the bigger goal build a personal robot assistant from that. 

The way the code is structured in the repo was heavily influenced by "Uncle Bobs" book [Clean architecture](htps://www.goodreads.com/book/show/18043011-clean-architecture) which I was reading at the time of writing the code. As it is the repo code doesn't really achieve the "cleanliness" needed to be a good representation of that book. Oh well, this was just a "summer fun" project after all. 

The other book that influenced this is [Software engineering at Google](https://www.goodreads.com/book/show/48816586-software-engineering-at-google?from_search=true&from_srp=true&qid=y6KVD2v1sb&rank=1) which is why the OpenAI SDK is forked (in case you wonder).










