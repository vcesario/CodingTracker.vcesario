namespace vcesario.CodingTracker;

public class CodingGoal
{
    private uint m_Value;
    private DateOnly m_DueDate;

    public uint Value => m_Value;
    public DateOnly DueDate => m_DueDate;

    public CodingGoal(int value, DateTime due_date)
    {
        m_Value = (uint)value;
        m_DueDate = DateOnly.FromDateTime(due_date);
    }
}