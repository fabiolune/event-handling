using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnitTesting;
using NSubstitute;
using Serilog;
using Shouldly;

namespace Fl.Event.Handling.Tests;

internal class LoggingEventProcessorTests
{
    private LoggingEventProcessor<TestPayload> _sut;
    private ILogger _mockLogger;
    private IEventProcessor<TestPayload> _mockProcessor;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = Substitute.For<ILogger>();
        _mockProcessor = Substitute.For<IEventProcessor<TestPayload>>();
        _sut = new LoggingEventProcessor<TestPayload>(_mockProcessor, _mockLogger);
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsLeft_ShouldReturnErrorAndLog()
    {
        var error = Error.New("some message");
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(error);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));

        _mockLogger
            .Received(1)
            .Error("{Component} raised an error with {Message}", (object)"IEventProcessor", (object)"some message");
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsLeftWithException_ShouldReturnErrorAndLog()
    {
        var exception = new Exception("some exception");
        var error = Error.New(exception);
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(error);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));

        _mockLogger
            .Received(1)
            .Error(exception, "{Component} raised an error with {Message}", (object)"IEventProcessor", (object)"some exception");
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsRight_ShouldReturnUnit()
    {
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(Unit.Default);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeRight();

        _mockLogger
            .DidNotReceiveWithAnyArgs()
            .Error(default);
    }
}
