namespace Compensable;

public class CompensatorStatusException(CompensatorStatus status) : Exception($"Compensator status is {status}");