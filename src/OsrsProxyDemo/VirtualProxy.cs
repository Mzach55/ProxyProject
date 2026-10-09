namespace OsrsProxyDemo;

public sealed class VirtualProxy : IItemModel
{
    private readonly Func<IItemModel> _modelFactory;
    private IItemModel? _realModel;

    public VirtualProxy(Func<IItemModel> modelFactory)
    {
        ArgumentNullException.ThrowIfNull(modelFactory);
        _modelFactory = modelFactory;
    }

    public void Display()
    {
        // The expensive object does not exist until the first display request.
        if (_realModel is null)
        {
            _realModel = _modelFactory()
                ?? throw new InvalidOperationException("The model factory returned null.");
        }

        // Later requests reuse the same object.
        _realModel.Display();
    }
}
