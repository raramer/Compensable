# Compensable

A library to facilitate the compensating (rolling back) of a series of successfully completed workflow steps when an exception occurs.

## Where do I get it

Compensable can be installed using the Nuget package manager 

```
PM> Install-Package Compensable
```

or the dotnet CLI.

```
dotnet add package Compensable
```

## How to get started
1. Define a compensator

   ```csharp
   var compensator = new Compensator();
   ```

2. Execute the steps of your workflow in the context of the compensator.  

   * Upon successful completion of a step, its compensation (if defined) is added to an internal compensation stack.  
   
   * If an exception is thrown in any execution, (DoIfAsync) test, or (ForeachAsync) enumeration, then the compensations in the stack are called in reverse order (except in the case of tags) and the original exception is thrown.  
   
   * If an exception occurs when calling a compensation, then a `CompensationException` is thrown that contains `WhileExecuting` and `WhileCompensating` properties and whose inner exception is also the original WhileExecuting exception.  (See below for alternate behavior when `CompensateAsync` is called.)
   
   _Overloads are available for async and non-async methods, and methods that do not require compensation._

   * `DoAsync` - defines a step that does not return a response.
     
     ```csharp
     await compensator.DoAsync(
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * `DoIfAsync` - defines a step peforms a `test`.  If the result of that test is true, then the execution and compensation are defined.
     
     ```csharp
     await compensator.DoIfAsync(test: async () => await testAsync(),
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * `GetAsync` - executes a step that returns a `result`.  The compensation method of GetAsync can optionally define a `_result` parameter that is equivalent to `result`, but will be unaffected if `result` is reassigned.
     
     ```csharp
     var result = await compensator.DoIfAsync(async () => await testAsync(),
         execution: async () => await stepAsync(),
         compensation: async (_result) => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * `ForeachAsync` - executes a step per item in an IEnumerable<T>.  If the item enumerator or execution throws an exception then remaining items are not executed.

     ```csharp
     //var items = new[] { "item1", "item2", item3" };
     await compensator.ForeachAsync(items,
         execution: async (item) => await stepAsync(item),
         compensation: async (item) => await compensateStepAsync(item),
         compensateAtTag: null);
     ```

   * `AddCompensationAsync` - defines a step that only provides compensation.  This is useful if you use more than one compensator and/or as an alternative to using tags.
     
     ```csharp
     await compensator.AddCompensationAsync(async () => await testAsync(),
         execution: async () => await stepAsync(),
         compensation: async (_result) => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * `CommitAsync` - clears all defined compensations from the stack without calling them, and the Status will remain Executing.
    
     ```csharp
     await compensator.CommitAsync();
     ```

   * `CompensateAsync` - invokes compensation directly.  
   
     If an exception occurs when calling a compensation, then a `CompensationException` is thrown that only contains a `WhileCompensating` value and whose inner exception is also the WhileCompensating exception.
     
     ```csharp
     await compensator.CompensateAsync();
     ```

   * `CreateTagAsync` - defines a tagged position in the compensation stack with an optional label.

     ```csharp
     var tag = await compensator.CreateTagAsync();
     ```

## Status

The compensator exposes a `Status` property that can be used to inspect its current internal state.  

If the Status is anything other than `Executing`, it cannot be used for executing additional steps.  Attempts to do so will result in a `CompensatorStatusException` being thrown.

   * `Executing` - the compensator is idle or executing a step.

   * `FailedToExecute` - a step's execution has failed, but compensation has not yet started.

   * `Compensating` - the compensator is compensating steps in the compensation stack.

   * `Compensated` - the compensator has successfully compensated all steps.

   * `FailedToCompensate` - the compensator failed to compensate a step in the stack.
  
## Tags

Tags define a position in the compensation stack with an optional label. They do nothing unless a step defines a `compensation` that should `compensateAtTag`.  When that happens the compensation is pushed into the compensation stack at the position of the tag.  More than one step can reference the same tag, but they are added in order that they are called.  

_Tags should be used sparingly!_

   ```csharp
   var tag = await compensator.CreateTagAsync();

   await compensator.DoAsync(
       async () => await step1Async(),
       compensation: async () => await compensateStep1Async());

   await compensator.DoAsync(
       async () => await step2Async(),
       compensation: async () => await compensateStep2Async()
       compensateAtTag: tag);

   // on exception, compensateStep1Async will be called first, followed by compensateStep2Async.
   await compensator.DoAsync(
       async () => await step3Async());
```

## Multi-threading

The compensator is intended to be single-threaded in order to better guarantee an order of operations, however it is intended to be thread safe.

## Practical Example
This example creates an email service on an async remote platform and stores a reference to it in a non-async local account service repository.  A tag is used alter the compensation stack to delete the entry from the local repository last.  If the compensation call to delete the service from the remote platform fails, the `emailService.Id` will still be available in the local repository for manual cleanup.

   ```csharp
   public async Task CreateEmailAsync(Guid accountId, string domainName, int mailboxQuota, int diskQuotaMb, 
        string adminMailboxAddress, string adminMailboxPassword, int adminMailboxDiskQuotaMb, bool adminMailboxIsCatchall)
   {
       // define a new compensator
       var compensator = new Compensator();

       // (optional) define a tag to that on compensate the local account service repository entry will be deleted last
       var deleteFromRepositoryTag = await compensator.CreateTagAsync();

       // create email service on the remote platform
       // on compensate delete the email service and any mailboxes
       var emailService = await compensator.GetAsync(
           async () => await _emailPlatform.CreateService(domainName, mailboxQuota, diskQuota),
           compensation: async (_emailService) => await _emailPlatform.DeleteService(_emailService.Id));

       // add an email service to the local account service repository
       // on compensate remove the email service from the local account service repository at the deleteFromRepositoryTag
       await compensator.DoAsync(
           () => _accountServiceRepository.Create(accountId, "Email", platformId: service.Id),
           compensation: () => _accountServiceRepository.Delete(accountId, "Email"),
           compensateAtTag: deleteFromRepositoryTag);

       // create admin mailbox
       // no compensation necessary
       await compensator.DoAsync(
           async () => await _emailPlatform.CreateAdminAsync(service.Id, adminEmailAddress, adminPassword, adminDiskQuotaMb));

       // optionally set admin mailbox as the catchall
       // no compensation necessary
       await compensator.DoIfAsync(adminIsCatchall,
           async () => await _emailPlatform.SetCatchall(service.Id, adminEmailAddress));
   }
   ```
