using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Fl.Event.Handling.Extensions.Tests;

internal partial class ServiceCollectionExtensionsTests
{

    public class EventHandler(Func<Payload, EitherAsync<Error, Unit>> innerHandle) : IEventHandler<Payload>
    {
        private readonly Func<Payload, EitherAsync<Error, Unit>> _innerHandle = innerHandle;

        public EitherAsync<Error, Unit> HandleAsync(Payload evt) => _innerHandle(evt);
    }

    public class EventHandler2(Func<Payload, EitherAsync<Error, Unit>> innerHandle, Func<Payload2, EitherAsync<Error, Unit>> innerHandle2) : IEventHandler<Payload2>
    {
        private readonly Func<Payload, EitherAsync<Error, Unit>> _innerHandle = innerHandle;
        private readonly Func<Payload2, EitherAsync<Error, Unit>> _innerHandle2 = innerHandle2;

        public EitherAsync<Error, Unit> HandleAsync(Payload evt) => _innerHandle(evt);

        public EitherAsync<Error, Unit> HandleAsync(Payload2 evt) => _innerHandle2(evt);
    }

    public class EventHandler3(IEventProcessor<Payload2> processor, Func<Payload, EitherAsync<Error, Unit>> innerHandle) : IEventHandler<Payload>
    {
        private readonly Func<Payload, EitherAsync<Error, Unit>> _innerHandle = innerHandle;
        private readonly IEventProcessor<Payload2> _processor = processor;

        public EitherAsync<Error, Unit> HandleAsync(Payload evt) => _innerHandle(evt).Bind(_ => _processor.ProcessAsync(new Payload2(evt.Id + 1)));
    }

    public class GenericComponent(IEventProcessor<Payload> processor)
    {
        private readonly IEventProcessor<Payload> _processor = processor;

        public EitherAsync<Error, Unit> Execute(Payload evt) => _processor.ProcessAsync(evt);
    }

    [Test]
    public async Task Handlers_ShouldBeExecutedTheRightAmountOfTime()
    {
        var handle1 = 0;
        var handle2 = 0;
        var handle3 = 0;
        var func1 = new Func<Payload, EitherAsync<Error, Unit>>(evt => EitherAsync<Error, Unit>.Right(Unit.Default).Do(_ => handle1++).Do(_ => Console.WriteLine(evt)));
        var func2 = new Func<Payload2, EitherAsync<Error, Unit>>(evt => EitherAsync<Error, Unit>.Right(Unit.Default).Do(_ => handle2++).Do(_ => Console.WriteLine(evt)));

        var services = new ServiceCollection();
        services
            .AddSingleton(func1)
            .AddSingleton(func2)
            .AddEventHandling<Payload>(EventHandlingStrategy.Sequential, Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)))
            .AddEventHandling<Payload2>(EventHandlingStrategy.Parallel, Assembly.GetAssembly(typeof(ServiceCollectionExtensionsTests)))
            .AddSingleton<IEventHandler<Payload>>(new EventHandler(evt => EitherAsync<Error, Unit>.Right(Unit.Default).Do(_ => handle3++)))
            .AddSingleton<GenericComponent>();

        var provider = services.BuildServiceProvider();

        var component = provider.GetService<GenericComponent>();
        await component.Execute(new Payload(4));

        handle1.ShouldBe(2);
        handle2.ShouldBe(1);
        handle3.ShouldBe(1);
    }

    [TestCase(-1)]
    [TestCase(2)]
    [TestCase(3)]
    public void AddEventHandling_WhenStrategyIsNotValid_ShouldThrowException(int strategy)
    {
        var services = new ServiceCollection();
        Action act = () => services.AddEventHandling<Payload>((EventHandlingStrategy)strategy, Assembly.GetExecutingAssembly());
        act.ShouldThrow<ArgumentException>($"Invalid event handling strategy: '{strategy}'");
    }
}
