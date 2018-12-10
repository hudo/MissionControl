# Sequence diagrams

Copy/paste to https://www.websequencediagrams.com/ to render

```
title Multipart responses
Term->+Server: request
Server->+ConHost: command
ConHost->+Handler: exec
ConHost->Server: multipart response
Server->-Term: swich to multipart
Handler->ConHost: yield response
Term->+Server: get update 
ConHost->Server: yeld response
Server->-Term: response (done:false)
Handler->-ConHost: yield response
Term->+Server: get update 
ConHost->Server: yield response
Server->-Term: response (done:true)
```