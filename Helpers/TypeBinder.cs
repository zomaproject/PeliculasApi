using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace PeliculasApi.Helpers;

public class TypeBinder<T>: IModelBinder
{
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        try
        {
            var deserializedValue = JsonConvert.DeserializeObject<T>(valueProviderResult.FirstValue!);
            bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            
            
            
        }
        catch
        {
            bindingContext.ModelState.TryAddModelError(modelName, "Invalid type");
        }

        return Task.CompletedTask;
    }
}