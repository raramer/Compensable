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
1. Define a compensator.

   ```csharp
   // using Compensable;
   var compensator = new AsyncCompensator();
   ```

2. Execute the steps of your workflow in the context of the compensator.  

   * Upon successful completion of a step's execution, its defined compensation is added to the top of an internal stack.  If a *compensateAtTag* is specified the compensation will be inserted at the position of the tag.  See **Tags** for more details.
   
   * If an exception is thrown in any *execution*, *test*, or *items* enumeration, then the compensations in the stack are called in reverse order and the original exception is re-thrown.
   
   * If an exception is thrown when calling a compensation, then a `CompensationException` is thrown that contains **WhileExecuting** and **WhileCompensating** properties and whose inner exception is also the original WhileExecuting exception. See **CompensateAsync** for its alternate behavior.
   
   _Overloads are available for calling async and non-async target methods, non-method tests, and steps that do not require compensation._

   * **DoAsync** - executes a step that does not return a result.
     
     ```csharp
     await compensator.DoAsync(
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **GetAsync** - executes a step that returns a *result*.  
   
     Its compensation can optionally define a *_result* parameter that is equivalent to *result*, but will be unaffected if *result* is reassigned.  Be aware that if *result* is an object whose properties are reassigned, the *compensation* will be affected as well.
     
     ```csharp
     var result = await compensator.GetAsync(
         execution: async () => await stepAsync(),
         compensation: async (_result) => await compensateStepAsync(_result),
         compensateAtTag: null);
     ```

   * **DoIfAsync** - executes a step if its *test* evaluates to true.

     ```csharp
     await compensator.DoIfAsync(
         test: async () => await testAsync(),
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **ForeachAsync** - executes a step per *item* in an IEnumerable&lt;T&gt;.  
   
     If the item enumerator or execution throws an exception, then remaining items are not executed.

     ```csharp
     // var items = new[] { "item1", "item2", "item3" };
     await compensator.ForeachAsync(
         items: items,
         execution: async (item) => await stepAsync(item),
         compensation: async (item) => await compensateStepAsync(item),
         compensateAtTag: null);
     ```

   * **AddCompensationAsync** - defines a step that only provides compensation.
     
     ```csharp
     await compensator.AddCompensationAsync(
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **CommitAsync** - clears all defined compensations from the stack without calling them.
    
     ```csharp
     await compensator.CommitAsync();
     ```

   * **CompensateAsync** - invokes compensation directly.  
   
     If an exception occurs when calling a compensation, then a `CompensationException` is thrown that only contains a **WhileCompensating** value and whose inner exception is also the WhileCompensating exception.
     
     ```csharp
     await compensator.CompensateAsync();
     ```

   * **CreateTagAsync** - defines a "tagged" position in the stack.  See **Tags** for more details.

     ```csharp
     var tag = await compensator.CreateTagAsync();
     ```

## Status

The compensator exposes a **Status** property that can be used to inspect its current internal state.  

If the Status is anything other than *Executing*, it cannot be used for executing additional steps.  Attempts to do so will result in a `CompensatorStatusException` being thrown.

   * **Executing** - the compensator is idle or executing a step.

   * **FailedToExecute** - a step's execution has failed, but compensation has not yet started.

   * **Compensating** - the compensator is compensating steps in the compensation stack.

   * **Compensated** - the compensator has successfully compensated all steps.

   * **FailedToCompensate** - the compensator failed to compensate a step in the stack.
  
## Tags

Tags define a position in the stack. 

* They do nothing unless a step defines a *compensation* that should *compensateAtTag*.  When that happens the compensation is pushed into the stack at the position of the tag instead of the top of the stack.  
 
* More than one step can reference the same tag, and their compensations will be called in reverse order when compensating.  

* If a tag does not exist in the stack, a `TagNotFoundException` will be thrown.

_Tags should be used sparingly!_

   ```csharp
   // create tag
   var tag = await compensator.CreateTagAsync();

   // step 1
   await compensator.DoAsync(
       async () => await step1Async(),
       compensation: async () => await compensateStep1Async());

   // step 2
   await compensator.DoAsync(
       async () => await step2Async(),
       compensation: async () => await compensateStep2Async(),
       compensateAtTag: tag);

   // compensate
   // compensateStep1Async will be called first, followed by compensateStep2Async.
   await compensator.CompensateAsync();
   ```

## Practical Example
This example creates an email service on an async email platform and stores a reference to it in a non-async account service repository.  A tag is used alter the compensation stack to delete the entry from the local account service repository last.

   ```csharp
   public async Task CreateEmailAsync(
       Guid accountId,
       string domainName,
       int mailboxQuota,
       int diskQuotaMb,
       string adminMailboxAddress,
       string adminMailboxPassword,
       int adminMailboxDiskQuotaMb,
       bool adminMailboxIsCatchall,
       string[] adminAliasAddresses)
   {
       // define a new compensator
       var compensator = new AsyncCompensator();

       // define a tag
       var deleteFromRepositoryTag = await compensator.CreateTagAsync();

       // create email service on the email platform
       // on compensate delete the email service and any mailboxes
       var emailServiceId = await compensator.GetAsync(
           async () =>
           {
               return await _emailPlatform.CreateServiceAsync(
                   domainName,
                   mailboxQuota,
                   diskQuotaMb);
           },
           compensation: async (_emailServiceId) =>
           {
               await _emailPlatform.DeleteServiceAsync(_emailServiceId);
           });

       // add an email service to the account service repository
       // on compensate remove the email service from the repository at the deleteFromRepositoryTag
       await compensator.DoAsync(
           () =>
           {
               _accountServiceRepository.Create(accountId, "Email", platformId: emailServiceId);
           },
           compensation: () =>
           {
               _accountServiceRepository.Delete(accountId, "Email");
           },
           compensateAtTag: deleteFromRepositoryTag);

       // create admin mailbox
       // no compensation necessary
       await compensator.DoAsync(
           async () =>
           {
               await _emailPlatform.CreateAdminMailboxAsync(emailServiceId,
                   adminMailboxAddress,
                   adminMailboxPassword,
                   adminMailboxDiskQuotaMb);
           });

       // optionally set admin mailbox as the catchall
       // no compensation necessary
       await compensator.DoIfAsync(adminMailboxIsCatchall,
           async () =>
           {
               await _emailPlatform.SetCatchallAsync(emailServiceId, adminMailboxAddress);
           });

       // create aliases
       // no compensation necessary
       await compensator.ForeachAsync(adminAliasAddresses,
           async (adminAliasAddress) =>
           {
               await _emailPlatform.CreateAliasAsync(
                   emailServiceId,
                   adminMailboxAddress,
                   adminAliasAddress);
           });
   }
   ```

## Thread-safety

In order to ensure a predictable order of operations, the compensator is intended to be single-threaded.  However, it is thread-safe through a set of execution, compensation, and status locks.
