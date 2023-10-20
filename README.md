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

   *Compensator* is for use in synchronous contexts and only supports synchronous executions, compensations, and tests.

   ```csharp
   // using Compensable;
   var compensator = new Compensator();
   ```

   *AsyncCompensator* is for use in asynchronous contexts and supports _both_ synchronous and asynchronous executions, compensations, and tests.  

   ```csharp
   // using Compensable;
   var asyncCompensator = new AsyncCompensator();
   ```


2. Execute the steps of your workflow in the context of the compensator.  

   * Upon successful completion of a step's execution, its defined compensation is added to the top of an internal stack.  If a *compensateAtTag* is specified the compensation will be inserted at the position of the tag.  See **Tags** for more details.
   
   * If an exception is thrown in any *execution*, *test*, or *items* enumeration, then the compensations in the stack are called in reverse order and the original exception is re-thrown.
   
   * If an exception is thrown when calling a compensation, then a `CompensationException` is thrown that contains **WhileExecuting** and **WhileCompensating** properties and whose inner exception is also the original WhileExecuting exception. See **CompensateAsync** for its alternate behavior.
   
   _Overloads are available for steps that do not require compensation, non-method-based tests, and for AsyncCompensator, calling both async and non-async target methods._

   * **Do / DoAsync** - executes a step that does not return a result.

     ```csharp
     compensator.Do(
         execution: () => step(),
         compensation: () => compensateStep(),
         compensateAtTag: null);
     ```
     
     ```csharp
     await asyncCompensator.DoAsync(
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **Get / GetAsync** - executes a step that returns a *result*.  
   
     Its compensation can optionally define a *_result* parameter that is equivalent to *result*, but will be unaffected if *result* is reassigned.  Be aware that if *result* is an object whose properties are reassigned, the *compensation* will be affected as well.
     
     ```csharp
     var result = compensator.Get(
         execution: () => step(),
         compensation: (_result) => compensateStep(_result),
         compensateAtTag: null);
     ```
     
     ```csharp
     var result = await asyncCompensator.GetAsync(
         execution: async () => await stepAsync(),
         compensation: async (_result) => await compensateStepAsync(_result),
         compensateAtTag: null);
     ```

   * **DoIf / DoIfAsync** - executes a step if its *test* evaluates to true.

     ```csharp
     compensator.DoIf(
         test: () => test(),
         execution: () => step(),
         compensation: () => compensateStep(),
         compensateAtTag: null);
     ```

     ```csharp
     await asyncCompensator.DoIfAsync(
         test: async () => await testAsync(),
         execution: async () => await stepAsync(),
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **Foreach / ForeachAsync** - executes a step per *item* in an IEnumerable&lt;T&gt;.  
   
     If the item enumerator or execution throws an exception, then remaining items are not executed.

     ```csharp
     // var items = new[] { "item1", "item2", "item3" };
     compensator.Foreach(
         items: items,
         execution: (item) => step(item),
         compensation: (item) => compensateStep(item),
         compensateAtTag: null);
     ```

     ```csharp
     // var items = new[] { "item1", "item2", "item3" };
     await asyncCompensator.ForeachAsync(
         items: items,
         execution: async (item) => await stepAsync(item),
         compensation: async (item) => await compensateStepAsync(item),
         compensateAtTag: null);
     ```

   * **AddCompensation / AddCompensationAsync** - defines a step that only provides compensation.
     
     ```csharp
     compensator.AddCompensation(
         compensation: () => compensateStep(),
         compensateAtTag: null);
     ```
     
     ```csharp
     await asyncCompensator.AddCompensationAsync(
         compensation: async () => await compensateStepAsync(),
         compensateAtTag: null);
     ```

   * **Commit / CommitAsync** - clears all defined compensations from the stack without calling them.
    
     ```csharp
     compensator.Commit();
     ```

     ```csharp
     await asyncCompensator.CommitAsync();
     ```

   * **Compensate / CompensateAsync** - invokes compensation directly.  
   
     If an exception occurs when calling a compensation, then a `CompensationException` is thrown that only contains a **WhileCompensating** value and whose inner exception is also the WhileCompensating exception.
     
     ```csharp
     compensator.Compensate();
     ```
     
     ```csharp
     await asyncCompensator.CompensateAsync();
     ```

   * **CreateTag / CreateTagAsync** - defines a "tagged" position in the stack.  See **Tags** for more details.

     ```csharp
     var tag = compensator.CreateTag();
     ```

     ```csharp
     var tag = await asyncCompensator.CreateTagAsync();
     ```

## Compensations

Compensations are types that a step's execution can return that encapsulate the compensation logic so that a calling compensator does not have to know the implemenation.

There are four types that apply to different scenarios. 

* **Compensation** - a synchrous compensation.
* **Compensation&lt;TResult&gt;** - a synchronous compensation with a Result.
* **AsyncCompensation** - an asynchronous compensation.
* **AsyncCompensation&lt;TResult&gt;** - an asynchronous compensation with a Result.

When used with a compensator, the returned compensation is added to the compensation stack, and in the case of _Get_ / _GetAsync_ steps, the applicable Result is returned. When used without a compensator, each compensation type defines a `Compensate` / `CompensateAsync` method that can be called to invoke the defined compensation, and in the case of a (Async)Compensation&lt;TResult&gt; a Result property contains the result.

When a step's internal logic does not need compensation in certain scenarios, each compensation also defines a static `Noop` method that can be used in place of a call to the constructor. 

_Disclaimer: this example is to demonstrate the use of Compensations and should not be considered a good example of a secure application._

   ```csharp
   internal enum AccountStatus { Active, Inactive };

   internal class Account
   {
       private readonly ISsoTokenRepository _ssoTokenRepository;

       public string Id { get; }

       public AccountStatus Status { get; private set; }

       public Compensation SetStatus(AccountStatus status)
       {
           // short-circuit status is already set
           if (Status == status)
               return Compensation.Noop;

           // capture compensation data
           var rollback = new { Status };

           // update status
           Status = status;

           // return compensation
           return new Compensation(() =>
           {
               Status = rollback.Status;
           });
       }

       public async Task<AsyncCompensation<string>> GenerateSsoTokenAsync()
       {
           // generate a token (pseudo random)
           var token = Guid.NewGuid().ToString("n");

           // store token
           await _ssoTokenRepository.CreateAsync(Id, token);

           // return token + compensation (in case an exception occurs and we need to delete token)
           return new AsyncCompensation<string>(
               result: token, 
               compensation: _ssoTokenRepository.DeleteToken);
       }
   }
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
   var tag = await asyncCompensator.CreateTagAsync();

   // step 1
   await asyncCompensator.DoAsync(
       async () => await step1Async(),
       compensation: async () => await compensateStep1Async());

   // step 2
   await asyncCompensator.DoAsync(
       async () => await step2Async(),
       compensation: async () => await compensateStep2Async(),
       compensateAtTag: tag);

   // compensate
   // compensateStep1Async will be called first, followed by compensateStep2Async.
   await asyncCompensator.CompensateAsync();
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
