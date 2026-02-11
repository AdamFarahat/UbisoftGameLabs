class FlipFlop<T>
{
    private T obj1;
    private T obj2;
    private bool use1 = true;

    public T Front()
    {
        return use1 ? obj1 : obj2;
    }

    public T Back()
    {
        return use1 ? obj2 : obj1;
    }

    public void Toggle()
    {
        use1 = !use1;
    }
}
