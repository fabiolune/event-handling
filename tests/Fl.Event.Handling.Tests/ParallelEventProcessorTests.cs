using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnitTesting;
using NSubstitute;
using Shouldly;
using Fl.Functional.Utils;

namespace Fl.Event.Handling.Tests;

public class ParallelEventProcessorTests
{

    [Test]
    public async Task ProcessAsync_WhenNoHandlers_ShouldReturnUnit()
    {
        var sut = new ParallelEventProcessor<TestPayload>([]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeRight();
    }

    [Test]
    public async Task ProcessAsync_WhenHandlerReturnsLeft_ShouldReturnError()
    {
        var handler = Substitute.For<IEventHandler<TestPayload>>();
        var error = Error.New("error");
        handler.HandleAsync(Arg.Any<TestPayload>()).Returns(error);

        var sut = new ParallelEventProcessor<TestPayload>([handler]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));
    }

    [Test]
    public async Task ProcessAsync_WhenHandlerReturnsRight_ShouldReturnUnit()
    {
        var handler = Substitute.For<IEventHandler<TestPayload>>();
        handler.HandleAsync(Arg.Any<TestPayload>()).Returns(Unit.Default);

        var sut = new ParallelEventProcessor<TestPayload>([handler]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeRight();
    }

    [Test]
    public async Task ProcessAsync_WhenMultipleHandlers_ShouldReturnUnit()
    {
        var handler1 = Substitute.For<IEventHandler<TestPayload>>();
        handler1.HandleAsync(Arg.Any<TestPayload>()).Returns(Unit.Default);

        var handler2 = Substitute.For<IEventHandler<TestPayload>>();
        handler2.HandleAsync(Arg.Any<TestPayload>()).Returns(Unit.Default);

        var sut = new ParallelEventProcessor<TestPayload>([handler1, handler2]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeRight();
    }

    [Test]
    public async Task ProcessAsync_WhenMultipleHandlersAndOneReturnsLeft_ShouldReturnError()
    {
        var count = 0;
        var handler1 = Substitute.For<IEventHandler<TestPayload>>();
        handler1.HandleAsync(Arg.Any<TestPayload>()).Returns(_ => Unit.Default.Tee(u => count++));

        var handler2 = Substitute.For<IEventHandler<TestPayload>>();
        var error = Error.New("error");
        handler2.HandleAsync(Arg.Any<TestPayload>()).Returns(error);

        var sut = new ParallelEventProcessor<TestPayload>([handler1, handler2]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));
        count.ShouldBe(1);
    }

    [Test]
    public async Task ProcessAsync_WhenMultipleHandlersAndAllReturnLeft_ShouldReturnError()
    {
        var handler1 = Substitute.For<IEventHandler<TestPayload>>();
        var error1 = Error.New("error1");
        handler1.HandleAsync(Arg.Any<TestPayload>()).Returns(error1);

        var handler2 = Substitute.For<IEventHandler<TestPayload>>();
        var error2 = Error.New("error2");
        handler2.HandleAsync(Arg.Any<TestPayload>()).Returns(error2);

        var sut = new ParallelEventProcessor<TestPayload>([handler1, handler2]);

        var result = await sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error1));
        await handler2.DidNotReceive().HandleAsync(default);
    }
}