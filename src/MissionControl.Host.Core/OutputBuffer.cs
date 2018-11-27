using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MissionControl.Host.Core.Responses;

namespace MissionControl.Host.Core
{
    public class OutputBuffer
    {
        private readonly ILogger<OutputBuffer> _logger;

        private readonly ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource<CliResponse>>> _waitingRequests = new ConcurrentDictionary<string, ConcurrentQueue<TaskCompletionSource<CliResponse>>>();
        private readonly ConcurrentDictionary<string, ConcurrentQueue<CliResponse>> _waitingResponses = new ConcurrentDictionary<string, ConcurrentQueue<CliResponse>>();

        public OutputBuffer(ILogger<OutputBuffer> logger)
        {
            _logger = logger;
        }

        public void Send(CliResponse response)
        {
            // if there's a request waiting, set response to it
            // if no, enqueue for later when requests is made

            var requests = WaitingItemsByClientId(_waitingRequests, response.ClientId);

            if (requests.TryDequeue(out var request))
            {
                _logger.LogTrace($"Send {response.ClientId}: Request found, sending response");
                request.SetResult(response);
            }
            else
            {
                _logger.LogTrace($"Send {response.ClientId}: Request not found, queueing response");
                var responses = WaitingItemsByClientId(_waitingResponses, response.ClientId);
                responses.Enqueue(response);
            }
        }

        public async Task<CliResponse> GetNextResponse(string clientId)
        {
            // check if there's a response already waiting for this terminal
            // if not, enqueue it and return task

            var responses = WaitingItemsByClientId(_waitingResponses, clientId);
            if (responses.TryDequeue(out var response))
            {
                _logger.LogTrace($"GetNextResponse {clientId}: Response found, sending to request");
                return response;
            }

            var completionSource = new TaskCompletionSource<CliResponse>(TaskCreationOptions.RunContinuationsAsynchronously);

            var waitingRequests = WaitingItemsByClientId(_waitingRequests, clientId);
            waitingRequests.Enqueue(completionSource);

            // cancel the request after 5 seconds, let client retry again
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            cancellationTokenSource.Token.Register(() =>
            {
                _logger.LogTrace($"GetNextResponse {clientId}: Request timed out");
                completionSource.SetResult(new MultipartUpdateResponse(false, null, clientId));
            });

            _logger.LogTrace($"GetNextResponse {clientId}: Response not found, waiting for request");

            return await completionSource.Task;
        }

        private ConcurrentQueue<T> WaitingItemsByClientId<T>(ConcurrentDictionary<string, ConcurrentQueue<T>> list, string clientId) 
        {
            if (!list.TryGetValue(clientId, out var waitingRequests))
            {
                waitingRequests = new ConcurrentQueue<T>();
                list.TryAdd(clientId, waitingRequests);
            }

            return waitingRequests;
        }

    }
}