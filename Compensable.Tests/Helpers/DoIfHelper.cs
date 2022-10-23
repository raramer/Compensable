namespace Compensable.Tests.Helpers;

public class DoIfHelper : DoHelper
{
    public Func<Task<bool>>? TestAsync { get; }

    private bool TestCalled { get; set; }

    private Test.Options TestOptions { get; }

    public DoIfHelper(string? label = null)
        : this(Test.ShouldBeCalledAndReturnTrue, 
            Execution.ExpectToBeCalledAndSucceed, 
            Compensation.WouldSucceedButNotCalled, 
            label)
    {
    }

    public DoIfHelper(Test.Options testOptions, string? label = null)
        : this(testOptions,
            executionOptions: testOptions.ExpectTrueResult ? Execution.ExpectToBeCalledAndSucceed : Execution.WouldSucceedButNotCalled,
            compensationOptions: testOptions.ExpectTrueResult ? Compensation.ExpectToBeCalledAndSucceed : Compensation.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoIfHelper(Execution.Options executionOptions, string? label = null)
        : this(testOptions: executionOptions.IsNull ? Test.WouldSucceedButNotCalled : Test.ShouldBeCalledAndReturnTrue, 
            executionOptions, 
            Compensation.WouldSucceedButNotCalled,
            label)
    {
    }

    public DoIfHelper(Compensation.Options compensationOptions, string? label = null)
        : this(Test.ShouldBeCalledAndReturnTrue, 
            Execution.ExpectToBeCalledAndSucceed, 
            compensationOptions, 
            label)
    {
    }

    private DoIfHelper(Test.Options testOptions, Execution.Options executionOptions, Compensation.Options compensationOptions, string? label) 
        : base(executionOptions, compensationOptions, label)
    {
        TestOptions = testOptions;

        TestAsync = TestOptions.IsNull ? null : _TestAsync;
    }

    public override void AssertHelper()
    {
        Assert.Equal(TestOptions.ExpectToBeCalled, TestCalled);
        base.AssertHelper();
    }

    private async Task<bool> _TestAsync()
    {
        await Task.Delay(1).ConfigureAwait(false);
        TestCalled = true;
        if (TestOptions.ThrowsException)
            throw new HelperTestException();
        return TestOptions.Result;
    }


}