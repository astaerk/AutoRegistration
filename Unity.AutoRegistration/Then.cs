namespace Unity.AutoRegistration
{
    public static class Then
    {
        public static IFluentRegistration Register()
        {
            return new Registration();
        }
    }
}