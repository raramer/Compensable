namespace Compensable.Tests.ReadmeExamples
{
    internal class Compensations
    {
        #pragma warning disable CS8618, CS0649
        interface ISsoTokenRepository { Task CreateAsync(string accountId, string token); Task DeleteToken(string token); }

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
        #pragma warning restore CS8618, CS0649
    }
}
