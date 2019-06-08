using System.Collections.Generic;

public class BoundedQueue<T> : Queue<T>
{
    public int Limit { get; set; }

    public BoundedQueue(int limit)
    {
        Limit = limit;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        while (base.Count > Limit)
        {
            base.Dequeue();
        }
    }

}
