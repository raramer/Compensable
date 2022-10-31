namespace Compensable.Tests.Helpers;

public class DoIfHelper : DoHelper
{
    public Func<bool>? Test { get; }
    public Func<Task<bool>>? TestAsync { get; }

    private bool TestCalled { get; set; }

    private TestOptions TestOptions { get; }

    public DoIfHelper(string? label = null)
        : this(TestOptions.ShouldBeCalledAndReturnTrue,
            ExecutionOptions.ExpectToBeCalledAndSucceed,
            CompensationOptions.WouldSucceedButNotCalled, 
            label)
    {
    }

    public DoIfHelper(TestOptions testOptions, string? label = null)
        : this(testOptions,
            executionOptions: testOptions.ExpectTrueResult ? Options.ExecutionOptions.ExpectToBeCalledAndSucceed : Options.ExecutionOptions.WouldSucceedButNotCalled,
            compensationOptions: testOptions.ExpectTrueResult ? Options.CompensationOptions.ExpectToBeCalledAndSucceed : Options.CompensationOptions.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoIfHelper(ExecutionOptions executionOptions, string? label = null)
        : this(testOptions: executionOptions.IsNull ? Options.TestOptions.WouldSucceedButNotCalled : Options.TestOptions.ShouldBeCalledAndReturnTrue, 
            executionOptions,
            CompensationOptions.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoIfHelper(CompensationOptions compensationOptions, string? label = null)
        : this(TestOptions.ShouldBeCalledAndReturnTrue,
            ExecutionOptions.ExpectToBeCalledAndSucceed, 
            compensationOptions, 
            label)
    {
    }

    private DoIfHelper(TestOptions testOptions, ExecutionOptions executionOptions, CompensationOptions compensationOptions, string? label) 
        : base(executionOptions, compensationOptions, label)
    {
        TestOptions = testOptions;

        if (!TestOptions.IsNull)
        {
            Test = _Test;
            TestAsync = _TestAsync;
        }
    }

    public override void AssertHelper()
    {
        Assert.Equal(TestOptions.ExpectToBeCalled, TestCalled);
        base.AssertHelper();
    }

    private bool _Test()
    { 
        TestCalled = true;
        if (TestOptions.ThrowsException)
            throw new HelperTestException();
        return TestOptions.Result;
    }
    private async Task<bool> _TestAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        return _Test();
    }
}