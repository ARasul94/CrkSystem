namespace Core.RequestQueue
{
    public interface IRequestQueue
    {
        RequestHandle<T> Enqueue<T>(IRequestCommand<T> _command);

        void CancelByOwner(object _owner);
        void CancelByTag(string _tag);
        void CancelByOwnerAndTag(object _owner, string _tag);
    }
}