namespace MPP_Client.Models
{
    public interface IIdentifiable<T>
    {
        T Id { get; set; }
    }
}