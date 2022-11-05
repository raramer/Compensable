namespace Compensable.Tests.ReadmeTests;

internal class PracticalExample
{
    private IAccountServiceRepository? _accountServiceRepository = null;
    private IEmailPlatform? _emailPlatform = null;

    #pragma warning disable CS8602
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
    #pragma warning restore CS8602

    private interface IAccountServiceRepository
    {
        void Create(Guid accountId, string serviceName, string platformId);

        void Delete(Guid accountId, string serviceName);
    }

    private interface IEmailPlatform
    {
        Task<string> CreateAdminMailboxAsync(string serviceId, string adminMailboxAddress, string adminMailboxPassword, int adminMailboxDiskQuotaMb);

        Task CreateAliasAsync(string serviceId, string mailboxAddress, string aliasAddress);

        Task<string> CreateServiceAsync(string domainName, int mailboxQuota, int diskQuotaMb);

        Task DeleteServiceAsync(string serviceId);

        Task SetCatchallAsync(string serviceId, string adminEmailAddress);
    }
}