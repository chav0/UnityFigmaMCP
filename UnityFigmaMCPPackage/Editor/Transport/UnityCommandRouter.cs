using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityFigmaMCP.Common;

namespace UnityFigmaMCP.Editor
{
    internal sealed class UnityCommandRouter : IDisposable
    {
        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private readonly HubConnection _hub;
        private readonly Action<string> _log;
        private readonly ComponentMappers _mappers = new();
        private readonly List<IDisposable> _subscriptions = new();

        public UnityCommandRouter(HubConnection hub, Action<string> log)
        {
            _hub = hub;
            _log = log;

            Register(new BuildPrefabCommandHandler());
            Register(new EditPrefabCommandHandler());
            Register(new GetHierarchyCommandHandler());
            Register(new GetPipelinesCommandHandler());
            Register(new BindAssetCommandHandler());
            Register(new ListAssetsCommandHandler());
            Register(new SavePrefabCommandHandler());
            Register(new SaveSpritesCommandHandler());
        }

        private void Register<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler)
            where TCommand : ICommand<TResult>
        {
            _subscriptions.Add(_hub.On<string, string>(typeof(TCommand).Name, payload => Dispatch(handler, payload)));
        }

        private Task<string> Dispatch<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler, string payload)
            where TCommand : ICommand<TResult>
        {
            var completion = new TaskCompletionSource<string>();

            EditorMainThreadQueue.Enqueue(() =>
            {
                try
                {
                    var settings = FigmaAutoLayoutSettings.GetOrCreate();
                    var context = new CommandContext(settings, _mappers);

                    var command = JsonConvert.DeserializeObject<TCommand>(payload)
                                  ?? throw new InvalidOperationException(
                                      $"Failed to deserialize {typeof(TCommand).Name}");

                    completion.TrySetResult(
                        JsonConvert.SerializeObject(handler.Handle(context, command), SerializerSettings));
                }
                catch (Exception exception)
                {
                    _log($"Error while handling {typeof(TCommand).Name}: {exception}");
                    completion.TrySetException(new Exception(exception.Message));
                }
            });

            return completion.Task;
        }

        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
                subscription?.Dispose();

            _subscriptions.Clear();
        }
    }
}
