namespace Compensable
{
    partial class Compensator
    {
        public void Commit()
            => Execute(_compensationStack.Clear);
    }
}
