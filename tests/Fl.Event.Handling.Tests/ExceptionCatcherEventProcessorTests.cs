using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;

namespace Fl.Event.Handling.Tests;

internal class ExceptionCatcherEventProcessorTests
{
    private ExceptionCatcherEventProcessor<TestPayload> _sut;
    private IEventProcessor<TestPayload> _mockProcessor;

    [SetUp]
    public void SetUp()
    {
        _mockProcessor = Substitute.For<IEventProcessor<TestPayload>>();
        _sut = new ExceptionCatcherEventProcessor<TestPayload>(_mockProcessor);
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorThrowsException_ShouldReturnError()
    {
        var exception = new Exception("some exception");
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Throws(exception);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.IsLeft.ShouldBeTrue();
        result.IfLeft(e => e.Message.ShouldBe(exception.Message));
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsLeft_ShouldReturnError()
    {
        var error = Error.New("some message");
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(error);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsLeftWithException_ShouldReturnError()
    {
        var exception = new Exception("some exception");
        var error = Error.New(exception);
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(error);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeLeft(e => e.ShouldBe(error));
    }

    [Test]
    public async Task ProcessAsync_WhenProcessorReturnsRight_ShouldReturnUnit()
    {
        _mockProcessor.ProcessAsync(Arg.Any<TestPayload>()).Returns(Unit.Default);

        var result = await _sut.ProcessAsync(new TestPayload());

        result.ShouldBeRight();
    }
}
