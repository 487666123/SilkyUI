namespace SilkyUI.Core;

public class SubscriptionType<T>
{
    public delegate void SubscriptionEventHandler(T newValue, T oldValue);

    public event SubscriptionEventHandler OnValueChanged;

    private void ValueChanged(T newValue, T oldValue)
    {
        OnValueChanged?.Invoke(newValue, oldValue);
    }

    private T _value;

    public T Value
    {
        get => _value;
        set
        {
            ValueChanged(value, _value);
            _value = value;
        }
    }
}