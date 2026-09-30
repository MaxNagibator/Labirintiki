using Microsoft.AspNetCore.Components;

namespace Labirint.Web.Services.Dialogs;

public sealed class DialogService
{
    private readonly List<DialogInstance> _instances = [];

    public event Action? Changed;

    public IReadOnlyList<DialogInstance> Instances => [.. _instances];

    public Task<DialogResult> ShowAsync<TDialog>(object owner, string title, DialogParameters? parameters = null, DialogOptions? options = null)
        where TDialog : IComponent
    {
        ArgumentNullException.ThrowIfNull(owner);

        DialogInstance instance = new(this, owner, typeof(TDialog), title, parameters ?? [], options ?? new DialogOptions());

        _instances.Add(instance);
        Changed?.Invoke();

        return instance.Result;
    }

    public void CancelOwnedBy(object owner)
    {
        foreach (var instance in _instances.Where(instance => ReferenceEquals(instance.Owner, owner)).ToList())
        {
            instance.Cancel();
        }
    }

    internal void Remove(DialogInstance instance)
    {
        if (_instances.Remove(instance))
        {
            Changed?.Invoke();
        }
    }
}
