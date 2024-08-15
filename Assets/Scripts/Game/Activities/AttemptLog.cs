
public class AttemptLog 
{
	public System.Guid adventurerId;
	public int attemptCount = 0;

	public bool wasLastAttemptSuccess = true;
	// other info - timestamps?

	public AttemptLog(System.Guid id, bool isSuccess)
	{
		adventurerId = id;
		LogAttempt(isSuccess);
	}

	public void LogAttempt(bool isSuccess)
	{
		wasLastAttemptSuccess = isSuccess;
		attemptCount++;
	}
}