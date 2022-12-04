namespace ConcurrentStack;

public class ConcurrentStack<T>: IStack<T>
{
    private Node? head;
    private int count;
    
    public void Push(T item)
    {
        var newHead = new Node(item, head);
        Interlocked.Exchange(ref head, newHead);
        Interlocked.Increment(ref count);
    }

    public bool TryPop(out T item)
    {
        var currentHead = head;
        
        if (currentHead is null)
        {
            item = default!;
            return false;
        }

        item = currentHead.Value;
        Interlocked.Exchange(ref head, currentHead.Next);
        Interlocked.Decrement(ref count);
        return true;
    }

    public int Count => count;
    
    private class Node
    {
        public Node? Next { get; set; }
        public T Value { get; set; }

        public Node(T value) => Value = value;
        
        public Node(T value, Node? next) : this(value) => Next = next;
    }
}