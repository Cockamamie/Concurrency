namespace ConcurrentStack;

public class ConcurrentStack<T>: IStack<T>
{
    private Node? head;
    private int count;
    
    public void Push(T item)
    {
        var newHead = new Node(item, head);
        while (Interlocked.CompareExchange(ref head, newHead, newHead.Next) != newHead.Next)
            newHead.Next = head;
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
        while (Interlocked.CompareExchange(ref head, currentHead.Next, currentHead) != currentHead)
        {
            currentHead = head;
            if (currentHead != null)
                continue;
            item = default!;
            return false;
        }

        Interlocked.Decrement(ref count);
        item = currentHead.Value;
        return true;
    }

    public int Count => count;
    
    private class Node
    {
        public Node? Next;
        public T Value;

        public Node(T value) => Value = value;
        
        public Node(T value, Node? next) : this(value) => Next = next;
    }
}