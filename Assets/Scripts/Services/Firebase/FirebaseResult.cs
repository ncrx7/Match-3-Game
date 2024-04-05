namespace Services.Firebase
{
    public class FirebaseResult<T>
    {
        public T Item;
        public bool Success;
        public string Cause;
    }
}