using Microsoft.AspNetCore.Components.Forms;

namespace SuggestionAppUI.Components;

public class MyInputRadioGroup<TValue>
    : InputRadioGroup<TValue>
{
    private string name;
    private string fieldClass;

    protected override void OnParametersSet()
    {
        var fieldClass = EditContext?.FieldCssClass(FieldIdentifier) ?? string.Empty;
        if(fieldClass != this.fieldClass || Name != name)
        {
            this.fieldClass = fieldClass;
            name = Name;
            base.OnParametersSet();
        }
    }
}